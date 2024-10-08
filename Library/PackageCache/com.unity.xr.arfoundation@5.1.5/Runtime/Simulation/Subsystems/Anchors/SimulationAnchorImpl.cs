using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    class SimulationAnchorImpl
    {
        const int k_DefaultCapacity = 4;
        const int k_DefaultUpdateCapacity = 16;

        readonly List<(TrackableId id, AttachedAnchorData data)> m_DeferredAttachedAnchorDataUpdateBuffer = new();
        readonly List<(TrackableId id, SimulatedAnchorData data)> m_DeferredSimulatedAnchorDataUpdateBuffer = new();
        readonly List<TrackableId> m_DeferredRemovedBuffer = new();

        readonly Dictionary<TrackableId, XRAnchor> m_Added = new(k_DefaultCapacity);
        readonly Dictionary<TrackableId, XRAnchor> m_Updated = new(k_DefaultUpdateCapacity);
        readonly HashSet<TrackableId> m_Removed = new(k_DefaultCapacity);

        readonly Dictionary<TrackableId, PoseAnchorData> m_PoseAnchorDataLookup = new(k_DefaultCapacity);
        readonly Dictionary<TrackableId, AttachedAnchorData> m_AttachedAnchorDataLookup = new(k_DefaultCapacity);
        readonly Dictionary<TrackableId, SimulatedAnchorData> m_SimulatedAnchorDataLookup = new(k_DefaultCapacity);

        SimulationPlaneSubsystem m_PlaneSubsystem;
        XROrigin m_Origin;

        Guid m_SessionId;
        bool m_IsEnvironmentSceneInitialized;
        bool m_IsStarted;

        public bool isReady => m_IsEnvironmentSceneInitialized && m_IsStarted;

        public bool hasAnyChanges => m_Added.Count > 0 || m_Updated.Count > 0 || m_Removed.Count > 0;

        public IReadOnlyCollection<XRAnchor> added => m_Added.Values;

        public IReadOnlyCollection<XRAnchor> updated => m_Updated.Values;

        public IReadOnlyCollection<TrackableId> removed => m_Removed;

        public void Start()
        {
            if (m_IsStarted)
                return;

            m_SessionId = Guid.Empty;
            if (SubsystemsUtility.TryGetLoadedSubsystem<XRSessionSubsystem, SimulationSessionSubsystem>(out var sessionSubsystem))
                m_SessionId = sessionSubsystem.sessionId;

            SubsystemsUtility.TryGetLoadedSubsystem<XRPlaneSubsystem, SimulationPlaneSubsystem>(out m_PlaneSubsystem);
            
            m_Origin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
            if(m_Origin == null || m_PlaneSubsystem == null || sessionSubsystem == null)
            {
                Debug.LogWarning("SimulationAnchorImpl could not be started because the XROrigin, SimulationPlaneSubsystem, or SimulationSessionSubsystem could not be found.");
                return;
            }

            BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentSetupFinished;
            BaseSimulationSceneManager.environmentTeardownStarted += OnEnvironmentTeardownStarted;

            m_IsStarted = true;
        }

        public void Stop()
        {
            if (!m_IsStarted)
                return;

            BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentSetupFinished;
            BaseSimulationSceneManager.environmentTeardownStarted -= OnEnvironmentTeardownStarted;

            m_IsEnvironmentSceneInitialized = false;

            RemoveAllAnchorsFrom(m_AttachedAnchorDataLookup);
            RemoveAllAnchorsFrom(m_PoseAnchorDataLookup);
            RemoveAllAnchorsFrom(m_SimulatedAnchorDataLookup);
            ClearChangeBuffers();

            m_IsStarted = false;
        }

        public void Update()
        {
            if(!m_IsStarted)
                return;

            using (new ScopedProfiler("XRSimulationAnchorUpdate"))
            {
                UpdateAttachedAnchors();
                UpdateSimulatedAnchors();
                ApplyDeferredUpdates();
                ApplyDeferredRemovals();
            }
        }

        void UpdateAttachedAnchors()
        {
            if (m_PlaneSubsystem == null)
                return;

            var attachedCount = m_AttachedAnchorDataLookup.Count;
            if (attachedCount == 0)
                return;

            var planesLookup = m_PlaneSubsystem.GetPlanesReadOnly();
            if (planesLookup.Count > 0)
            {
                foreach (var attachedAnchorData in m_AttachedAnchorDataLookup.Values)
                {
                    var anchor = attachedAnchorData.anchor;

                    var plane = planesLookup.GetValueOrDefault(attachedAnchorData.attachedToId);
                    if (plane.trackableId == TrackableId.invalidId)
                    {
                        m_DeferredRemovedBuffer.Add(anchor.trackableId);
                        continue;
                    }

                    var state = plane.trackingState;
                    if (anchor.trackingState == state)
                        continue;

                    var offset = PoseUtils.CalculateOffset(plane.pose, anchor.pose);
                    if (offset != attachedAnchorData.offsetFromAttached)
                        UpdatePose(ref anchor, plane.pose.WithOffset(attachedAnchorData.offsetFromAttached));

                    UpdateTrackingState(ref anchor, state);
                    m_DeferredAttachedAnchorDataUpdateBuffer.Add((anchor.trackableId, new AttachedAnchorData(anchor, attachedAnchorData.attachedToId, default)));
                    MarkUpdatedAnchor(anchor);
                }
            }
            else
            {
                RemoveAllAnchorsFrom(m_AttachedAnchorDataLookup);
            }
        }

        void UpdateSimulatedAnchors()
        {
            foreach (var simulatedAnchorData in m_SimulatedAnchorDataLookup.Values)
            {
                var xrAnchor = simulatedAnchorData.anchor;
                var simAnchor = simulatedAnchorData.simulatedAnchor;

                if (simAnchor == null)
                {
                    m_DeferredRemovedBuffer.Add(xrAnchor.trackableId);
                    continue;
                }

                var transform = simAnchor.transform;
                if (!transform.hasChanged)
                    continue;

                var worldPose = transform.GetWorldPose();
                var sessionPose = m_Origin.transform.InverseTransformPose(worldPose);

                UpdatePose(ref xrAnchor, sessionPose);
                m_DeferredSimulatedAnchorDataUpdateBuffer.Add((xrAnchor.trackableId, new SimulatedAnchorData(xrAnchor, simAnchor)));
                MarkUpdatedAnchor(xrAnchor);

                transform.hasChanged = false;
            }
        }

        static void UpdateTrackingState(ref XRAnchor anchor, TrackingState trackingState) => anchor =
            new XRAnchor(anchor.trackableId, anchor.pose, trackingState, anchor.nativePtr, anchor.sessionId);

        static void UpdatePose(ref XRAnchor anchor, Pose pose) => anchor =
            new XRAnchor(anchor.trackableId, pose, anchor.trackingState, anchor.nativePtr, anchor.sessionId);

        void MarkUpdatedAnchor(in XRAnchor anchor) => m_Updated[anchor.trackableId] = anchor;

        void ApplyDeferredUpdates()
        {
            ApplyDeferredDictionaryUpdates(m_AttachedAnchorDataLookup, m_DeferredAttachedAnchorDataUpdateBuffer);
            ApplyDeferredDictionaryUpdates(m_SimulatedAnchorDataLookup, m_DeferredSimulatedAnchorDataUpdateBuffer);
        }

        // cannot modify dictionary while iterating collection.
        // so reuse modification buffers and apply them with this function.
        static void ApplyDeferredDictionaryUpdates<T>(
            IDictionary<TrackableId, T> lookupToUpdate,
            List<(TrackableId id, T data)> modifiedEntries) where T : IAnchorData
        {
            foreach (var entry in modifiedEntries)
            {
                lookupToUpdate[entry.id] = entry.data;
            }

            modifiedEntries.Clear();
        }

        void ApplyDeferredRemovals()
        {
            foreach (var id in m_DeferredRemovedBuffer)
            {
                TryRemoveAnchor(id);
            }

            m_DeferredRemovedBuffer.Clear();
        }

        void OnEnvironmentSetupFinished()
        {
            if (string.IsNullOrEmpty(BaseSimulationSceneManager.activeSceneName))
            {
                m_IsEnvironmentSceneInitialized = false;
                return;
            }

            var sceneManager = SimulationSessionSubsystem.simulationSceneManager;
            var scene = sceneManager.environmentScene;

            m_IsEnvironmentSceneInitialized = scene.IsValid() && scene.isLoaded;
            if (!m_IsEnvironmentSceneInitialized)
                return;

            foreach (var simulatedAnchor in SimulatedAnchor.instances)
            {
                var simulatedWorldPose = simulatedAnchor.transform.GetWorldPose();
                var sessionPose = m_Origin.transform.InverseTransformPose(simulatedWorldPose);

                CreateAndAddAnchor(sessionPose, TrackingState.Tracking, out var anchor);

                m_SimulatedAnchorDataLookup.Add(anchor.trackableId, new SimulatedAnchorData(anchor, simulatedAnchor));
                simulatedAnchor.transform.hasChanged = false;
            }
        }

        void OnEnvironmentTeardownStarted() => ClearEnvironmentSimulatedAnchors();

        public void Reset()
        {
            if (!m_IsStarted)
                return;

            RemoveAllAnchorsFrom(m_AttachedAnchorDataLookup);
            RemoveAllAnchorsFrom(m_PoseAnchorDataLookup);
            ClearEnvironmentSimulatedAnchors();

            OnEnvironmentSetupFinished();
        }

        void ClearEnvironmentSimulatedAnchors()
        {
            if (!m_IsEnvironmentSceneInitialized)
                return;

            RemoveAllAnchorsFrom(m_SimulatedAnchorDataLookup);

            m_IsEnvironmentSceneInitialized = false;
        }

        public void ClearChangeBuffers()
        {
            m_Added.Clear();
            m_Updated.Clear();
            m_Removed.Clear();
        }

        public void AddPoseAnchor(Pose pose, out XRAnchor anchor)
        {
            CreateAndAddAnchor(pose, TrackingState.Tracking, out anchor);
            m_PoseAnchorDataLookup.Add(anchor.trackableId, anchor);
        }

        public bool TryAttachAnchor(TrackableId attachedToId, Pose pose, out XRAnchor anchor)
        {
            anchor = XRAnchor.defaultValue;

            if (m_PlaneSubsystem == null)
            {
                Debug.LogWarning(
                    $"No plane subsystem available. Cannot attach anchor to trackable id {attachedToId}.");

                return false;
            }

            if (attachedToId == TrackableId.invalidId)
            {
                Debug.LogWarning($"Cannot attach anchor to invalid trackable id {attachedToId}.");
                return false;
            }

            var planesLookup = m_PlaneSubsystem.GetPlanesReadOnly();
            if (planesLookup == null)
            {
                Debug.LogError($"Unable to lookup planes. Cannot attach anchor to trackable id {attachedToId}.");
                return false;
            }

            // ARAnchorManager only supports attaching to ARPlanes, so we currently follow suit
            var plane = planesLookup.GetValueOrDefault(attachedToId);
            if (plane.trackableId == TrackableId.invalidId)
            {
                Debug.LogError($"Unable to get plane with id {attachedToId}.");
                return false;
            }

            CreateAndAddAnchor(pose, plane.trackingState, out anchor);

            m_AttachedAnchorDataLookup.Add(
                anchor.trackableId,
                new AttachedAnchorData(anchor, attachedToId, PoseUtils.CalculateOffset(plane.pose, pose)));

            return true;
        }

        void CreateAndAddAnchor(Pose pose, TrackingState trackingState, out XRAnchor anchor)
        {
            var id = SimulationUtils.GenerateTrackableId();
            anchor = new XRAnchor(id, pose, trackingState, IntPtr.Zero, m_SessionId);
            m_Added.Add(anchor.trackableId, anchor);
        }

        public bool TryRemoveAnchor(TrackableId anchorId)
        {
            if (TryRemoveAnchorFrom(m_AttachedAnchorDataLookup, anchorId)
                || TryRemoveAnchorFrom(m_PoseAnchorDataLookup, anchorId))
                return true;

            m_SimulatedAnchorDataLookup.TryGetValue(anchorId, out var simulatedAnchorData);
            if (!TryRemoveAnchorFrom(m_SimulatedAnchorDataLookup, anchorId))
                return false;

            UnityObjectUtils.Destroy(simulatedAnchorData.simulatedAnchor);
            return true;
        }

        bool TryRemoveAnchorFrom<TAnchorData>(Dictionary<TrackableId, TAnchorData> anchorDataLookup, in TrackableId id)
            where TAnchorData : struct, IAnchorData
        {
            if (!anchorDataLookup.TryGetValue(id, out var anchorData))
                return false;

            MarkAsRemoved(anchorData.GetAnchor());
            anchorDataLookup.Remove(id);

            return true;
        }

        void MarkAsRemoved(in XRAnchor anchor)
        {
            var id = anchor.trackableId;

            m_Removed.Add(id);
            m_Added.Remove(id);
            m_Updated.Remove(id);
        }

        void RemoveAllAnchorsFrom<TAnchorData>(Dictionary<TrackableId, TAnchorData> anchorDataLookup)
            where TAnchorData : struct, IAnchorData
        {
            if (anchorDataLookup.Count == 0)
                return;

            foreach (var anchorData in anchorDataLookup.Values)
            {
                MarkAsRemoved(anchorData.GetAnchor());
            }

            anchorDataLookup.Clear();
        }

        interface IAnchorData
        {
            public XRAnchor GetAnchor();
        }

        readonly struct PoseAnchorData : IAnchorData
        {
            public readonly XRAnchor anchor;

            PoseAnchorData(in XRAnchor anchor) => this.anchor = anchor;

            public XRAnchor GetAnchor() => anchor;

            public static implicit operator PoseAnchorData(in XRAnchor anchor) => new(anchor);

            public static implicit operator XRAnchor(in PoseAnchorData anchorData) => anchorData.anchor;
        }

        struct AttachedAnchorData : IAnchorData
        {
            public XRAnchor anchor;
            public readonly TrackableId attachedToId;
            public readonly Pose offsetFromAttached;

            public AttachedAnchorData(in XRAnchor anchor, in TrackableId attachedToId, Pose offsetFromAttached)
            {
                this.anchor = anchor;
                this.attachedToId = attachedToId;
                this.offsetFromAttached = offsetFromAttached;
            }

            public XRAnchor GetAnchor() => anchor;
        }

        struct SimulatedAnchorData : IAnchorData
        {
            public XRAnchor anchor;
            public readonly SimulatedAnchor simulatedAnchor;

            public SimulatedAnchorData(in XRAnchor anchor, SimulatedAnchor simulatedAnchor)
            {
                this.anchor = anchor;
                this.simulatedAnchor = simulatedAnchor;
            }

            public XRAnchor GetAnchor() => anchor;
        }
    } 
}
