using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;
using static UnityEngine.XR.Simulation.SimulationUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of <see cref="XRMeshSubsystem"/>.
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    class SimulationMeshSubsystem : IDisposable, ISimulationSessionResetHandler
    {
        internal const string k_SubsystemId = "XRSimulation-Meshing";

        readonly Dictionary<TrackableId, IDisposable[]> m_NativeArrays = new();
        readonly List<SimulationMesh> m_Meshes = new();
        readonly List<CombineInstance> k_CombineInstances = new();
        readonly Dictionary<string, List<MeshFilter>> k_MeshesByClassification = new();
        readonly List<MeshFilter> k_UnclassifiedMeshFilters = new();

        XRMeshSubsystem m_Subsystem;

        internal static XRMeshSubsystem GetActiveSubsystemInstance()
        {
            XRMeshSubsystem activeSubsystem = null;

            // Query the currently active loader for the created subsystem, if one exists.
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                var loader = XRGeneralSettings.Instance.Manager.activeLoader;
                if (loader != null)
                    activeSubsystem = loader.GetLoadedSubsystem<XRMeshSubsystem>();
                if (activeSubsystem != null && activeSubsystem.subsystemDescriptor.id != k_SubsystemId)
                    activeSubsystem = null;
            }

            return activeSubsystem;
        }

        /// <summary>
        /// Sets up the subsystem that adds, updates and removes meshes via communication with the native mesh provider.
        /// </summary>
        public void Start()
        {
#if UNITY_EDITOR
            SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif

            BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentReady;
            SimulationSessionSubsystem.s_SimulationSessionReset += OnSimulationSessionReset;

            m_Subsystem ??= GetActiveSubsystemInstance();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            SimulationSessionSubsystem.s_SimulationSessionReset -= OnSimulationSessionReset;
            BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentReady;

            RemoveMeshes();

            foreach (var nativeArrays in m_NativeArrays.Values)
            {
                Array.ForEach(nativeArrays, x => x.Dispose());
            }

            m_NativeArrays.Clear();
        }

        public void OnSimulationSessionReset() => OnEnvironmentReady();

        void AddOrUpdateMesh(SimulationMesh mesh)
        {
            var vertices = new NativeArray<Vector3>(mesh.mesh.vertices, Allocator.Persistent);

            // Will be zero length if no normals present
            var normals = new NativeArray<Vector3>(mesh.mesh.normals, Allocator.Persistent);
            IDisposable indicesDisposable;

            if (mesh.mesh.indexFormat == IndexFormat.UInt16)
            {
                List<ushort> indicesList = new List<ushort>();
                mesh.mesh.GetIndices(indicesList, 0);
                var indices = new NativeArray<ushort>(indicesList.ToArray(), Allocator.Persistent);

                unsafe
                {
                    AddMesh(mesh.trackableId.subId1,
                        mesh.trackableId.subId2,
                        vertices.Length,
                        NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(vertices),
                        normals.Length > 0 ? NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(normals) : null,
                        indices.Length,
                        NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(indices),
                        true
                    );
                }

                indicesDisposable = indices;
            }
            else
            {
                var indices = new NativeArray<int>(mesh.mesh.GetIndices(0), Allocator.Persistent);
                unsafe
                {
                    AddMesh(mesh.trackableId.subId1,
                        mesh.trackableId.subId2,
                        vertices.Length,
                        NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(vertices),
                        normals.Length > 0 ? NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(normals) : null,
                        indices.Length,
                        NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(indices),
                        false
                    );
                }

                indicesDisposable = indices;
            }

            if (m_NativeArrays.TryGetValue(mesh.trackableId, out var nativeArrays))
                Array.ForEach(nativeArrays, x => x.Dispose());

            m_NativeArrays[mesh.trackableId] = new[] {vertices, normals, indicesDisposable};
        }

        void OnEnvironmentReady()
        {
            if (SimulationSessionSubsystem.simulationSceneManager == null ||
                SimulationSessionSubsystem.simulationSceneManager.simulationEnvironment == null)
                return;

            var environmentRoot = SimulationSessionSubsystem.simulationSceneManager.simulationEnvironment.gameObject;

            RemoveMeshes();

            // Mesh data can only be accessed in the player loop if the mesh has read/write enabled, but it can
            // always be accessed from Editor code. So in Editor play mode we combine meshes in a delayCall.
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                EditorApplication.delayCall += () =>
                {
                    var activeSubsystem = GetActiveSubsystemInstance();
                    if (activeSubsystem is { running: true })
                        AddMeshes(environmentRoot);
                };
            }
            else
                AddMeshes(environmentRoot);
#else
            AddMeshes(environmentRoot);
#endif
        }

        void AddMeshes(GameObject environmentRoot)
        {
            m_Meshes.Clear();
            k_MeshesByClassification.Clear();
            k_UnclassifiedMeshFilters.Clear();

            var meshFilters = new List<MeshFilter>();
            environmentRoot.GetComponentsInChildren(meshFilters);

            foreach (var meshFilter in meshFilters)
            {
                var classification = meshFilter.GetComponentInParent<SimulatedMeshClassification>();
                if (classification == null || string.IsNullOrEmpty(classification.classificationType))
                    k_UnclassifiedMeshFilters.Add(meshFilter);
                else
                {
                    var classificationType = classification.classificationType;
                    if (!k_MeshesByClassification.TryGetValue(classificationType, out var classifiedMeshFilters))
                    {
                        classifiedMeshFilters = new List<MeshFilter>();
                        k_MeshesByClassification[classificationType] = classifiedMeshFilters;
                    }

                    classifiedMeshFilters.Add(meshFilter);
                }
            }

            var environmentWorldToLocal = environmentRoot.transform.worldToLocalMatrix;
            AddMeshesWithClassification(k_UnclassifiedMeshFilters, environmentWorldToLocal, "");
            foreach (var kvp in k_MeshesByClassification)
            {
                var classification = kvp.Key;
                var meshFiltersByClassification = kvp.Value;
                AddMeshesWithClassification(meshFiltersByClassification, environmentWorldToLocal, classification);
            }
        }

        void AddMeshesWithClassification(List<MeshFilter> meshFilters, Matrix4x4 environmentWorldToLocal, string classification)
        {
            const IndexFormat meshIndexFormat = IndexFormat.UInt32;
            const uint vertexCap = uint.MaxValue;
            long combineVertexCount = 0;
            k_CombineInstances.Clear();

            for (var i = 0; i < meshFilters.Count; i++)
            {
                var meshFilter = meshFilters[i];
                var mesh = meshFilter.mesh;
                var subMeshCount = mesh.subMeshCount;
                var combineTransform = environmentWorldToLocal * meshFilter.transform.localToWorldMatrix;
                for (var subIndex = 0; subIndex < subMeshCount; subIndex++)
                {
                    var subVertexCount = mesh.GetSubMesh(subIndex).vertexCount;
                    var newVertexCount = combineVertexCount + subVertexCount;
                    if (newVertexCount > vertexCap || newVertexCount < combineVertexCount) // Check for overflow
                    {
                        var combinedMesh = CreateMeshFromCombineList(k_CombineInstances, meshIndexFormat);
                        AddSimulationMesh(combinedMesh, classification);
                        k_CombineInstances.Clear();
                        newVertexCount = subVertexCount;
                    }

                    combineVertexCount = newVertexCount;
                    k_CombineInstances.Add(new CombineInstance
                    {
                        mesh = mesh,
                        subMeshIndex = subIndex,
                        transform = combineTransform
                    });
                }
            }

            if (k_CombineInstances.Count > 0)
                AddSimulationMesh(CreateMeshFromCombineList(k_CombineInstances, meshIndexFormat), classification);
        }

        static Mesh CreateMeshFromCombineList(List<CombineInstance> combineInstancesList, IndexFormat meshIndexFormat)
        {
            var mesh = new Mesh
            {
                indexFormat = meshIndexFormat
            };

            mesh.CombineMeshes(combineInstancesList.ToArray());
            return mesh;
        }

        void AddSimulationMesh(Mesh mesh, string classification)
        {
            var simMesh = new SimulationMesh
           (
                GenerateTrackableId(),
                classification,
                Pose.identity,
                mesh
           );

            m_Meshes.Add(simMesh);
            AddOrUpdateMesh(simMesh);
        }

        void RemoveMeshes()
        {
            foreach (var mesh in m_Meshes)
            {
                if (m_Subsystem != null && m_Subsystem.running)
                    RemoveMesh(mesh.trackableId.subId1, mesh.trackableId.subId2);

                if (!m_NativeArrays.TryGetValue(mesh.trackableId, out var nativeArrays))
                    continue;

                Array.ForEach(nativeArrays, x => x.Dispose());
                m_NativeArrays.Remove(mesh.trackableId);
            }

            m_Meshes.Clear();
        }

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_AddOrUpdateMesh")]
        static extern unsafe void AddMesh(ulong id1, ulong id2, int numVertices, void* vertices, void* normals, int numTriangles, void* indices, bool shortIndices);

        [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_RemoveMesh")]
        static extern void RemoveMesh(ulong id1, ulong id2);
    }
}
