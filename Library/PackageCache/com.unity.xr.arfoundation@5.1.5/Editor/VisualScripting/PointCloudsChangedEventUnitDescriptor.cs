#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="PointCloudsChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(PointCloudsChangedEventUnit))]
    public sealed class PointCloudsChangedEventUnitDescriptor : TrackablesChangedEventUnitDescriptor<
        ARPointCloudManager,
        XRPointCloudSubsystem,
        XRPointCloudSubsystemDescriptor,
        XRPointCloudSubsystem.Provider,
        XRPointCloud,
        ARPointCloud,
        ARPointCloudChangedEventArgs,
        ARPointCloudManagerListener>
    {
        /// <inheritdoc/>
        public PointCloudsChangedEventUnitDescriptor(PointCloudsChangedEventUnit target) : base(target)
        {
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
