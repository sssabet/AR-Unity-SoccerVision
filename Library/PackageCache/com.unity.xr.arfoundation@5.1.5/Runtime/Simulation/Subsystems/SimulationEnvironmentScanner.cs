using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Scanner that scans the simulation environment using raycasts and provides a list of points and their normals.
    /// </summary>
    class SimulationEnvironmentScanner : IDisposable
    {
        static SimulationEnvironmentScanner s_Instance;
        
        // Local method use only -- created here to reduce garbage collection. Collections must be cleared before use.
        // Reference type collections must also be cleared after use
        static readonly List<MeshRenderer> s_EnvironmentMeshes = new();

        readonly List<SubsystemWithProvider> m_TrackingSubsystems = new();

        EnvironmentScanParams m_EnvironmentScanParams;

        PhysicsScene m_PhysicsScene;
        Transform m_SimulationCameraTransform;
        Camera m_Camera;
        float m_CameraScale;
        Pose m_CameraPose;
        Pose m_PreviousCameraPose;
        int m_PointCount;
        float m_LastScanTime;
        bool m_Initialized;
        bool m_Running;
        GameObject m_EnvironmentRoot;
        bool m_MeshCollidersRequested;
        bool m_MeshCollidersCreated;

        NativeArray<Vector3> m_Normals;
        NativeArray<Vector3> m_Points;

        public float lastScanTime => m_LastScanTime;

        SimulationEnvironmentScanner()
        {
            m_EnvironmentScanParams = XRSimulationRuntimeSettings.Instance.environmentScanParams;
            m_Initialized = false;
        }

        public static SimulationEnvironmentScanner GetOrCreate()
        {
            // Written this way to make it easier to add a breakpoint when new instance is created
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (s_Instance == null)
                s_Instance = new SimulationEnvironmentScanner();

            return s_Instance;
        }

        public void Initialize(SimulationCamera simulationCamera, PhysicsScene physicsScene, GameObject environmentRoot)
        {
            if (!physicsScene.IsValid())
                throw new InvalidOperationException("The physics scene loaded for simulation is not valid.");

            m_PhysicsScene = physicsScene;
            m_EnvironmentRoot = environmentRoot;

            m_SimulationCameraTransform = simulationCamera.transform;
            m_Camera = simulationCamera.GetComponent<Camera>();

            m_PreviousCameraPose = m_SimulationCameraTransform.GetWorldPose();

            m_Normals = new NativeArray<Vector3>(m_EnvironmentScanParams.raysPerCast, Allocator.Persistent);
            m_Points = new NativeArray<Vector3>(m_EnvironmentScanParams.raysPerCast, Allocator.Persistent);

            if (m_MeshCollidersRequested)
            {
                m_MeshCollidersRequested = false;
                CreateMeshColliders();
            }

            m_Initialized = true;
        }

        public void Start()
        {
            if (!m_Initialized)
                throw new InvalidOperationException($"Attempting to start uninitialized {GetType().Name} Scanner");

            m_Running = true;
        }

        public void Stop()
        {
            m_Running = false;
        }

        public void Dispose()
        {
            if (m_Normals.IsCreated)
                m_Normals.Dispose();

            if (m_Points.IsCreated)
                m_Points.Dispose();

            m_PointCount = 0;
            m_Initialized = false;

            s_EnvironmentMeshes.Clear();
            m_MeshCollidersRequested = false;

            m_TrackingSubsystems.Clear();

            s_Instance = null;
        }

        public void Update()
        {
            if (!m_Running ||
                m_TrackingSubsystems.Count <= 0 ||
                !ShouldRescan())
                return;

            m_PreviousCameraPose = m_CameraPose;

            using (new ScopedProfiler("ScanSimulationEnvironment"))
                PerformEnvironmentScan();
        }

        public NativeArray<Vector3> GetPoints(Allocator allocator)
        {
            var points = new NativeArray<Vector3>(m_PointCount, allocator);

            if (m_Initialized)
                NativeArray<Vector3>.Copy(m_Points, points, m_PointCount);

            return points;
        }

        public NativeArray<Vector3> GetNormals(Allocator allocator)
        {
            var normals = new NativeArray<Vector3>(m_PointCount, allocator);

            if (m_Initialized)
                NativeArray<Vector3>.Copy(m_Normals, normals, m_PointCount);

            return normals;
        }

        void PerformEnvironmentScan()
        {
            var raycastParams = new RaycastParams(m_SimulationCameraTransform, m_Camera, m_EnvironmentScanParams, m_CameraScale);

#if UNITY_2022_1_OR_NEWER
            var raycastHits = PerformRaycastBatch(raycastParams);
#else // UNITY_2022_1_OR_NEWER
            var raycastHits = PerformRaycast(raycastParams);
#endif // UNITY_2022_1_OR_NEWER
            try
            {
                NativeArrayUtils.EnsureCapacity(ref m_Normals, m_EnvironmentScanParams.raysPerCast, Allocator.Persistent,
                    NativeArrayOptions.UninitializedMemory);
                NativeArrayUtils.EnsureCapacity(ref m_Points, m_EnvironmentScanParams.raysPerCast, Allocator.Persistent,
                    NativeArrayOptions.UninitializedMemory);

                m_PointCount = 0;
                for (var i = 0; i < m_EnvironmentScanParams.raysPerCast; i++)
                {
                    var hit = raycastHits[i];
                    if (hit.collider == null || hit.distance < raycastParams.scaledMinimumDistance)
                        continue;

                    m_Normals[m_PointCount] = hit.normal;
                    m_Points[m_PointCount] = hit.point;

                    m_PointCount++;
                }
            }
            finally
            {
                if (raycastHits.IsCreated)
                    raycastHits.Dispose();

                m_LastScanTime = Time.timeSinceLevelLoad;
            }
        }

#if UNITY_2022_1_OR_NEWER
        NativeArray<RaycastHit> PerformRaycastBatch(RaycastParams raycastParams)
        {
            var raycasts = new NativeArray<RaycastCommand>(m_EnvironmentScanParams.raysPerCast, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var raycastHits = new NativeArray<RaycastHit>(m_EnvironmentScanParams.raysPerCast, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            try
            {
                var raycastGeneratorJob = new RaycastGeneratorJob
                {
                    seed = (uint)Random.Range(1, uint.MaxValue),
                    physicsScene = m_PhysicsScene,
                    cameraPosition = raycastParams.cameraPosition,
                    cameraRotation = raycastParams.cameraRotation,
                    halfFov = raycastParams.halfFov,
                    horizontalFov = raycastParams.horizontalFov,
                    scaledMaximumHitDistance = raycastParams.scaledMaximumHitDistance,
                    raysOut = raycasts
                };

                var raycastGeneratorHandle = raycastGeneratorJob.Schedule(m_EnvironmentScanParams.raysPerCast, 32);
                raycastGeneratorHandle.Complete();

                var raycastHandle = RaycastCommand.ScheduleBatch(raycasts, raycastHits, 32, raycastGeneratorHandle);
                raycastHandle.Complete();
            }
            finally
            {
                raycasts.Dispose();
            }

            return raycastHits;
        }
#else // UNITY_2022_1_OR_NEWER
        NativeArray<RaycastHit> PerformRaycast(RaycastParams raycastParams)
        {
            var directions = new NativeArray<Vector3>(m_EnvironmentScanParams.raysPerCast, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var raycastHits = new NativeArray<RaycastHit>(m_EnvironmentScanParams.raysPerCast, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            try
            {
                var directionGeneratorJob = new DirectionGeneratorJob
                {
                    seed = (uint)Random.Range(1, uint.MaxValue),
                    cameraRotation = raycastParams.cameraRotation,
                    halfFov = raycastParams.halfFov,
                    horizontalFov = raycastParams.horizontalFov,
                    directions = directions
                };

                var raycastGeneratorHandle = directionGeneratorJob.Schedule(m_EnvironmentScanParams.raysPerCast, 32);
                raycastGeneratorHandle.Complete();

                for (var i = 0; i < m_EnvironmentScanParams.raysPerCast; i++)
                {
                    m_PhysicsScene.Raycast(raycastParams.cameraPosition, directions[i], out var hit, raycastParams.scaledMaximumHitDistance);
                    raycastHits[i] = hit;
                }
            }
            finally
            {
                directions.Dispose();
            }

            return raycastHits;
        }
#endif // UNITY_2022_1_OR_NEWER

        bool ShouldRescan()
        {
            if (Time.timeSinceLevelLoad - m_LastScanTime < m_EnvironmentScanParams.minimumRescanTime)
                return false;

            var cameraTransform = m_SimulationCameraTransform;
            m_CameraPose = cameraTransform.GetWorldPose();
            m_CameraScale = cameraTransform.lossyScale.x;

            return Vector3.Distance(m_PreviousCameraPose.position, m_CameraPose.position) > m_EnvironmentScanParams.deltaCameraDistanceToRescan * m_CameraScale ||
                   Quaternion.Angle(m_PreviousCameraPose.rotation, m_CameraPose.rotation) > m_EnvironmentScanParams.deltaCameraAngleToRescan;
        }

        public void RegisterSubsystem<TSubsystem>(TSubsystem subsystem)
            where TSubsystem : SubsystemWithProvider, new()
        {
            if (m_TrackingSubsystems.Contains(subsystem))
                return;

            m_TrackingSubsystems.Add(subsystem);
            EnsureMeshColliders();
        }

        public void UnregisterSubsystem<TSubsystem>(TSubsystem subsystem)
            where TSubsystem : SubsystemWithProvider, new()
        {
            m_TrackingSubsystems.Remove(subsystem);
        }

        public void EnsureMeshColliders()
        {
            if (!m_Initialized)
            {
                m_MeshCollidersRequested = true;
                return;
            }

            if (!m_MeshCollidersCreated)
                CreateMeshColliders();
        }

        void CreateMeshColliders()
        {
            // k_EnvironmentMeshes is cleared by GetComponentsInChildren
            m_EnvironmentRoot.GetComponentsInChildren(s_EnvironmentMeshes);
            foreach (var mesh in s_EnvironmentMeshes)
            {
                if (mesh.GetComponent<SimulatedBoundedPlane>())
                    continue;

                if (mesh.GetComponent<Collider>() != null)
                    continue;

                var meshObject = mesh.gameObject;
                meshObject.hideFlags = HideFlags.DontSave;
                meshObject.AddComponent<MeshCollider>();
            }

            s_EnvironmentMeshes.Clear();

            m_MeshCollidersCreated = true;
        }

        readonly struct RaycastParams
        {
            public Vector3 cameraPosition { get; }
            public Quaternion cameraRotation { get; }

            public float halfFov { get; }
            public float horizontalFov { get; }

            public float scaledMinimumDistance { get; }
            public float scaledMaximumHitDistance { get; }

            public RaycastParams(Transform transform, Camera camera, EnvironmentScanParams environmentScanParams, float cameraScale)
            {
                cameraPosition = transform.position;
                cameraRotation = transform.rotation;
                halfFov = camera.fieldOfView * 0.5f;
                horizontalFov = camera.GetHorizontalFieldOfView();

                scaledMinimumDistance = environmentScanParams.minimumHitDistance * cameraScale;
                scaledMaximumHitDistance = environmentScanParams.maximumHitDistance * cameraScale;
            }
        }

#if UNITY_2022_1_OR_NEWER
        struct RaycastGeneratorJob : IJobParallelFor
        {
            [Unity.Collections.ReadOnly]
            public uint seed;

            [Unity.Collections.ReadOnly]
            public PhysicsScene physicsScene;

            [Unity.Collections.ReadOnly]
            public Vector3 cameraPosition;

            [Unity.Collections.ReadOnly]
            public Quaternion cameraRotation;

            [Unity.Collections.ReadOnly]
            public float halfFov;

            [Unity.Collections.ReadOnly]
            public float horizontalFov;

            [Unity.Collections.ReadOnly]
            public float scaledMaximumHitDistance;

            [WriteOnly]
            public NativeArray<RaycastCommand> raysOut;

            public void Execute(int index)
            {
                var random = Unity.Mathematics.Random.CreateFromIndex((uint)index + seed);
                var x = random.NextFloat(-halfFov, halfFov);
                var y = random.NextFloat(-horizontalFov, horizontalFov);
                var direction = cameraRotation * Quaternion.Euler(x, y, 0) * Vector3.forward;

#if UNITY_2022_2_OR_NEWER
                raysOut[index] = new RaycastCommand(physicsScene, cameraPosition, direction, QueryParameters.Default, scaledMaximumHitDistance);
#else
                raysOut[index] = new RaycastCommand(physicsScene, cameraPosition, direction, scaledMaximumHitDistance);
#endif
            }
        }
#else // UNITY_2022_1_OR_NEWER
        struct DirectionGeneratorJob : IJobParallelFor
        {
            [Unity.Collections.ReadOnly]
            public uint seed;

            [Unity.Collections.ReadOnly]
            public Quaternion cameraRotation;

            [Unity.Collections.ReadOnly]
            public float halfFov;

            [Unity.Collections.ReadOnly]
            public float horizontalFov;

            [WriteOnly]
            public NativeArray<Vector3> directions;

            public void Execute(int index)
            {
                var random = Unity.Mathematics.Random.CreateFromIndex((uint)index + seed);
                var x = random.NextFloat(-halfFov, halfFov);
                var y = random.NextFloat(-horizontalFov, horizontalFov);

                directions[index] = cameraRotation * Quaternion.Euler(x, y, 0) * Vector3.forward;
            }
        }
#endif // UNITY_2022_1_OR_NEWER
    }
}
