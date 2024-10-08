using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// This subsystem provides information regarding the detection of planes (that is, flat surfaces) in the real-world
    /// environment.
    /// </summary>
    /// <remarks>
    /// This is a base class with an abstract provider type to be implemented by provider plug-in packages.
    /// This class itself does not implement plane detection.
    /// </remarks>
    public class XRPlaneSubsystem
        : TrackingSubsystem<BoundedPlane, XRPlaneSubsystem, XRPlaneSubsystemDescriptor, XRPlaneSubsystem.Provider>
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        ValidationUtility<BoundedPlane> m_ValidationUtility = new();
#endif

        /// <summary>
        /// Do not invoke this constructor directly.
        /// </summary>
        /// <remarks>
        /// If you are implementing your own custom subsystem [Lifecycle management](xref:xr-plug-in-management-provider#lifecycle-management),
        /// use the [SubsystemManager](xref:UnityEngine.SubsystemManager)
        /// to enumerate the available <see cref="XRPlaneSubsystemDescriptor"/>s, then call
        /// <see cref="ISubsystemDescriptor.Create">XRPlaneSubsystemDescriptor.Create()</see> on the desired descriptor.
        /// </remarks>
        public XRPlaneSubsystem() { }

        /// <summary>
        /// Get or set the requested <see cref="PlaneDetectionMode"/> to enable different modes of detection.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown if <see cref="requestedPlaneDetectionMode"/> is set to
        /// something other than <see cref="PlaneDetectionMode.None"/> when plane detection is not supported.</exception>
        public PlaneDetectionMode requestedPlaneDetectionMode
        {
            get => provider.requestedPlaneDetectionMode;
            set => provider.requestedPlaneDetectionMode = value;
        }

        /// <summary>
        /// Get the current <see cref="PlaneDetectionMode"/> in use by the provider.
        /// </summary>
        public PlaneDetectionMode currentPlaneDetectionMode => provider.currentPlaneDetectionMode;

        /// <summary>
        /// Gets a <see cref="TrackableChanges{T}"/> struct containing any changes to detected planes since the last
        /// time you called this method. You are responsible to <see cref="TrackableChanges{T}.Dispose"/> the returned
        /// <c>TrackableChanges</c> instance.
        /// </summary>
        /// <param name="allocator">An <c>Allocator</c> to use for the returned <c>NativeArray</c>s.</param>
        /// <returns>The <see cref="TrackableChanges{T}"/>.</returns>
        /// <remarks>
        /// The <see cref="TrackableChanges{T}"/> struct returned by this method contains separate
        /// <see cref="NativeArray{T}"/> objects for the added, updated, and removed planes. These arrays are created
        /// using the <see cref="Allocator"/> type specified by <paramref name="allocator"/>.
        /// </remarks>
        public override TrackableChanges<BoundedPlane> GetChanges(Allocator allocator)
        {
            var changes = provider.GetChanges(BoundedPlane.defaultValue, allocator);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
#endif
            return changes;
        }

        /// <summary>
        /// Gets the boundary vertices of a given <see cref="BoundedPlane"/> based on its <see cref="BoundedPlane.trackableId"/>.
        /// </summary>
        /// <param name="trackableId">The <see cref="TrackableId"/> of a <see cref="BoundedPlane"/>.</param>
        /// <param name="allocator">An <see cref="Allocator"/> to use if <paramref name="boundary"/> needs to be created.
        ///   <see cref="Allocator.Temp"/> is not supported. Use <see cref="Allocator.TempJob"/> if you need temporary memory.</param>
        /// <param name="boundary">The boundary vertices will be stored here. If the given array is the same length as
        ///   the number of vertices, it is overwritten with the new data. Otherwise, it is disposed and recreated with
        ///   the correct length.</param>
        public void GetBoundary(
            TrackableId trackableId,
            Allocator allocator,
            ref NativeArray<Vector2> boundary)
        {
            switch (allocator)
            {
                case Allocator.Temp:
                    throw new InvalidOperationException("Allocator.Temp is not supported. Use Allocator.TempJob if you wish to use a temporary allocator.");
                case Allocator.None:
                    throw new InvalidOperationException("Allocator.None is not a valid allocator.");
                default:
                    provider.GetBoundary(trackableId, allocator, ref boundary);
                    break;
            }
        }

        /// <summary>
        /// Creates or resizes the <paramref name="array"/> if necessary. If <paramref name="array"/> has been allocated
        /// and its length is equal to <paramref name="length"/>, then this method does nothing. If its length is
        /// different, then it is first disposed before being assigned to a new <c>NativeArray</c>.
        /// </summary>
        /// <param name="length">The length that <paramref name="array"/> will have after this method returns.</param>
        /// <param name="allocator">If allocation is necessary, this allocator will be used to create the new <c>NativeArray</c>.</param>
        /// <param name="array">The array to create or resize.</param>
        /// <typeparam name="T">The type of elements held by the <paramref name="array"/>.</typeparam>
        protected static void CreateOrResizeNativeArrayIfNecessary<T>(
            int length,
            Allocator allocator,
            ref NativeArray<T> array) where T : struct
        {
            if (array.IsCreated)
            {
                if (array.Length != length)
                {
                    array.Dispose();
                    array = new NativeArray<T>(length, allocator);
                }
            }
            else
            {
                array = new NativeArray<T>(length, allocator);
            }
        }

        /// <summary>
        /// The provider API for <see cref="XRPlaneSubsystem"/>-derived classes to implement.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRPlaneSubsystem>
        {
            /// <summary>
            /// Creates or resizes the <paramref name="array"/> if necessary. If <paramref name="array"/>
            /// has been allocated and its length is equal to <paramref name="length"/>, then this method
            /// does nothing. If its length is different, then it is first disposed before being assigned
            /// to a new <c>NativeArray</c>.
            /// </summary>
            /// <param name="length">The length that <paramref name="array"/> will have after this method returns.</param>
            /// <param name="allocator">If allocation is necessary, this allocator will be used to create the new <c>NativeArray</c>.</param>
            /// <param name="array">The array to create or resize.</param>
            /// <typeparam name="T">The type of elements held by the <paramref name="array"/>.</typeparam>
            protected static void CreateOrResizeNativeArrayIfNecessary<T>(
                int length,
                Allocator allocator,
                ref NativeArray<T> array) where T : struct
            {
                if (array.IsCreated)
                {
                    if (array.Length != length)
                    {
                        array.Dispose();
                        array = new NativeArray<T>(length, allocator);
                    }
                }
                else
                {
                    array = new NativeArray<T>(length, allocator);
                }
            }

            /// <summary>
            /// Gets the boundary vertices of a given <see cref="BoundedPlane"/> based on its <see cref="BoundedPlane.trackableId"/>.
            /// </summary>
            /// <param name="trackableId">The <see cref="TrackableId"/> of a <see cref="BoundedPlane"/>.</param>
            /// <param name="allocator">An <see cref="Allocator"/> to use if <paramref name="boundary"/> needs to be created.
            ///   <see cref="Allocator.Temp"/> is not supported. Use <see cref="Allocator.TempJob"/> if you need temporary memory.</param>
            /// <param name="boundary">The boundary vertices will be stored here. If the given array is the same length as
            ///   the number of vertices, it is overwritten with the new data. Otherwise, it is disposed and recreated with
            ///   the correct length.</param>
            /// <remarks>
            /// See <see cref="CreateOrResizeNativeArrayIfNecessary{T}"/> for a helper method implementation.
            /// </remarks>
            public virtual void GetBoundary(
                TrackableId trackableId,
                Allocator allocator,
                ref NativeArray<Vector2> boundary)
            {
                throw new NotSupportedException("Boundary vertices are not supported.");
            }

            /// <summary>
            /// Gets a <see cref="TrackableChanges{T}"/> struct containing any changes to detected planes since the last
            /// time you called this method. You are responsible to <see cref="TrackableChanges{T}.Dispose"/> the returned
            /// <c>TrackableChanges</c> instance.
            /// </summary>
            /// <param name="defaultPlane">The default plane. You should use this to initialize the returned
            ///   <see cref="TrackableChanges{T}"/> instance by passing it to the constructor
            ///   <see cref="TrackableChanges{T}(int,int,int,Allocator,T)"/>.
            /// </param>
            /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
            /// <returns>The changes to planes since the last call to this method.</returns>
            public abstract TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator);

            /// <summary>
            /// Get or set the requested <see cref="PlaneDetectionMode"/>.
            /// </summary>
            public virtual PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => PlaneDetectionMode.None;
                set
                {
                    if (value != PlaneDetectionMode.None)
                    {
                        throw new NotSupportedException("Plane detection is not supported.");
                    }
                }
            }

            /// <summary>
            /// Get the current plane detection mode in use by the provider.
            /// </summary>
            public virtual PlaneDetectionMode currentPlaneDetectionMode => PlaneDetectionMode.None;
        }
    }
}
