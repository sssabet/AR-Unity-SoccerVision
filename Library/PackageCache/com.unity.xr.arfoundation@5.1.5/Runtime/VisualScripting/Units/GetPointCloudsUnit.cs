#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all point clouds managed by the input <see cref="ARPointCloudManager"/>.
    /// </summary>
    [UnitTitle("Get Point Clouds")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.PointClouds)]
    [TypeIcon(typeof(List<ARPointCloud>))]
    public sealed class GetPointCloudsUnit : GetTrackablesUnit<
        ARPointCloudManager,
        XRPointCloudSubsystem,
        XRPointCloudSubsystemDescriptor,
        XRPointCloudSubsystem.Provider,
        XRPointCloud,
        ARPointCloud>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
