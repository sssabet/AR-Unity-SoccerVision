using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.Simulation.SimulationUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Helper class for <see cref="SimulationImageTrackingSubsystem"/> which simulates a device's discovery of
    /// tracked images in an environment and fires corresponding events.
    /// </summary>
    class SimulationTrackedImageDiscoverer
    {
        int m_TrackingUpdateIntervalMilliseconds = 100;
        int m_PreviousLibraryCount;
        SimulationRuntimeImageLibrary m_SimulationRuntimeImageLibrary;
        TrackedImageDiscoveryStrategy m_ImageDiscoveryStrategy;
        bool m_Initialized;
        bool m_IsRunning;
        CancellationTokenSource m_UpdateCancellationTokenSource;

        readonly List<SimulatedTrackedImage> m_ImagesInScene = new();
        readonly List<TrackingState> m_TrackingStatesOfImages = new();
        readonly List<XRReferenceImage?> m_ReferenceImagesForImages = new();

        public SimulationRuntimeImageLibrary simulationRuntimeImageLibrary
        {
            set => m_SimulationRuntimeImageLibrary = value;
        }

        /// <summary>
        /// Invoked during initialization, after all <see cref="SimulatedTrackedImage"/> components in the
        /// simulation environment have been loaded. Returns the total number of <see cref="SimulatedTrackedImage"/>
        /// components in the simulation environment.
        /// </summary>
        public event Action<int> imagesInitialized;

        /// <summary>
        /// Invoked when a tracked image is discovered.
        /// </summary>
        public event Action<XRTrackedImage> imageAdded;

        /// <summary>
        /// Invoked when a tracked image is updated.
        /// </summary>
        public event Action<XRTrackedImage> imageUpdated;

        /// <summary>
        /// Invoked when a tracked image is removed.
        /// </summary>
        public event Action<TrackableId> imageRemoved;

        /// <summary>
        /// Starts actively trying to discover tracked images in the environment.
        /// </summary>
        public void Start()
        {
            if (!m_Initialized)
            {
                BaseSimulationSceneManager.environmentSetupFinished += Initialize;
                return;
            }

            BeginUpdateLoop();
        }

        public void Stop()
        {
            BaseSimulationSceneManager.environmentSetupFinished -= Initialize;

            if (!m_IsRunning)
                return;

            m_UpdateCancellationTokenSource?.Cancel();

            for (var i = 0; i < m_ImagesInScene.Count; i++)
            {
                if (m_TrackingStatesOfImages[i] != TrackingState.None)
                    SubsystemRemoveImageAtIndex(i);
            }

            m_IsRunning = false;
        }

        async void BeginUpdateLoop()
        {
            m_IsRunning = true;
            using (m_UpdateCancellationTokenSource = new CancellationTokenSource())
            {
                var updateTask = UpdateTracking(m_UpdateCancellationTokenSource.Token);
                await RunWithoutCancellationExceptions(updateTask);
            }
        }

        public void Restart()
        {
            if(!m_Initialized)
                return;

            if(m_IsRunning)
                Stop();

            for (var i = 0; i < m_TrackingStatesOfImages.Count; i++)
            {
                m_TrackingStatesOfImages[i] = TrackingState.None;
            }

            BeginUpdateLoopAfterDelay();
        }

        /// <summary>
        /// Necessary to allow the ARManager-side to catch up to the removed trackables,
        /// otherwise we could end up with a duplicate guid entry and an exception will be thrown.
        /// </summary>
        async void BeginUpdateLoopAfterDelay()
        {
            await RunWithoutCancellationExceptions(Task.Delay(m_TrackingUpdateIntervalMilliseconds,
                CancellationToken.None));

            BeginUpdateLoop();
        }

        void Initialize()
        {
            if (m_IsRunning)
                Stop();

            m_ImagesInScene.Clear();
            m_TrackingStatesOfImages.Clear();
            m_ReferenceImagesForImages.Clear();
            m_PreviousLibraryCount = 0;

            var origin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();

            if (origin == null)
                throw new NullReferenceException($"{nameof(SimulationImageTrackingSubsystem)} requires that the current scene contains an {nameof(XROrigin)}, but none was found.");

            var camera = origin.Camera;
            if (camera == null)
                throw new NullReferenceException($"{nameof(SimulationImageTrackingSubsystem)} requires a valid {nameof(XROrigin)} Camera, but none was set.");

            var simScene = SimulationSessionSubsystem.simulationSceneManager.environmentScene;

            if (!simScene.IsValid())
                throw new InvalidOperationException("The scene loaded for simulation is not valid.");

            var simPhysicsScene = simScene.GetPhysicsScene();

            if (!simPhysicsScene.IsValid())
                throw new InvalidOperationException("The physics scene loaded for simulation is not valid.");

            m_ImageDiscoveryStrategy = new TrackedImageDiscoveryStrategy(camera, simPhysicsScene);
            var trackingUpdateIntervalMilliseconds =
                (int)(XRSimulationRuntimeSettings.Instance.trackedImageDiscoveryParams.trackingUpdateInterval * 1000);

            if (trackingUpdateIntervalMilliseconds > 0)
                m_TrackingUpdateIntervalMilliseconds = trackingUpdateIntervalMilliseconds;

            m_PreviousLibraryCount = m_SimulationRuntimeImageLibrary?.count ?? 0;

            foreach (var rootObject in simScene.GetRootGameObjects())
            {
                foreach (var image in rootObject.GetComponentsInChildren<SimulatedTrackedImage>())
                {
                    m_ImagesInScene.Add(image);
                    m_TrackingStatesOfImages.Add(TrackingState.None);
                    m_ReferenceImagesForImages.Add(m_SimulationRuntimeImageLibrary?.GetReferenceImageWithTexture(image.texture));
                }
            }

            imagesInitialized?.Invoke(m_ImagesInScene.Count);
            m_Initialized = true;
            BeginUpdateLoop();
        }

        async Task UpdateTracking(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!m_Initialized)
                {
                    await RunWithoutCancellationExceptions(Task.Delay(m_TrackingUpdateIntervalMilliseconds, cancellationToken));
                    continue;
                }

                using (new ScopedProfiler("XRSimulationImageTracking"))
                {
                    var hasLibrary = m_SimulationRuntimeImageLibrary != null;
                    var currentLibraryCount = hasLibrary ? m_SimulationRuntimeImageLibrary.count : 0;
                    var hasLibraryChanged = hasLibrary && currentLibraryCount > m_PreviousLibraryCount;

                    for (var i = 0; i < m_ImagesInScene.Count; i++)
                    {
                        var image = m_ImagesInScene[i];
                        var prevTrackingState = m_TrackingStatesOfImages[i];
                        var newTrackingState = m_ImageDiscoveryStrategy.ComputeTrackingState(image);

                        if (!m_ReferenceImagesForImages[i].HasValue && hasLibraryChanged)
                        {
                            m_ReferenceImagesForImages[i] = m_SimulationRuntimeImageLibrary.GetReferenceImageWithTexture(image.texture);
                        }

                        if (prevTrackingState is TrackingState.None && newTrackingState is TrackingState.Tracking)
                            SubsystemAddImageAtIndex(i);
                        else if (prevTrackingState is TrackingState.Tracking || newTrackingState is TrackingState.Tracking)
                            SubsystemUpdateImageAtIndex(i, newTrackingState);
                    }

                    m_PreviousLibraryCount = currentLibraryCount;
                }

                await RunWithoutCancellationExceptions(Task.Delay(m_TrackingUpdateIntervalMilliseconds, cancellationToken));
            }
        }

        void SubsystemAddImageAtIndex(int imageIndex)
        {
            var image = m_ImagesInScene[imageIndex];
            m_TrackingStatesOfImages[imageIndex] = TrackingState.Tracking;
            imageAdded?.Invoke(CreateXRImage(image, TrackingState.Tracking, m_ReferenceImagesForImages[imageIndex]));
        }

        void SubsystemUpdateImageAtIndex(int imageIndex, TrackingState trackingState)
        {
            var image = m_ImagesInScene[imageIndex];
            m_TrackingStatesOfImages[imageIndex] = trackingState;
            imageUpdated?.Invoke(CreateXRImage(image, trackingState, m_ReferenceImagesForImages[imageIndex]));
        }

        void SubsystemRemoveImageAtIndex(int imageIndex)
        {
            var image = m_ImagesInScene[imageIndex];
            m_TrackingStatesOfImages[imageIndex] = TrackingState.None;
            imageRemoved?.Invoke(image.trackableId);
        }

        static XRTrackedImage CreateXRImage(SimulatedTrackedImage image, TrackingState trackingState, XRReferenceImage? referenceImage)
        {
            return new XRTrackedImage(
                trackableId: image.trackableId,
                sourceImageId: referenceImage?.guid ?? image.fallbackSourceImageId,
                pose: image.transform.GetWorldPose(),
                size: image.size,
                trackingState: trackingState,
                nativePtr: IntPtr.Zero);
        }
    }
}
