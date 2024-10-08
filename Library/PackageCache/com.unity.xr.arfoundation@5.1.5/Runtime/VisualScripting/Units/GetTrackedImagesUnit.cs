#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all tracked images managed by the input <see cref="ARTrackedImageManager"/>.
    /// </summary>
    [UnitTitle("Get Tracked Images")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.ImageTracking)]
    [TypeIcon(typeof(List<ARTrackedImage>))]
    public sealed class GetTrackedImagesUnit : GetTrackablesUnit<
        ARTrackedImageManager,
        XRImageTrackingSubsystem,
        XRImageTrackingSubsystemDescriptor,
        XRImageTrackingSubsystem.Provider,
        XRTrackedImage,
        ARTrackedImage>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
