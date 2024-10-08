#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all environment probes managed by the input <see cref="AREnvironmentProbeManager"/>.
    /// </summary>
    [UnitTitle("Get Environment Probes")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.EnvironmentProbes)]
    [TypeIcon(typeof(List<AREnvironmentProbe>))]
    public sealed class GetEnvironmentProbesUnit : GetTrackablesUnit<
        AREnvironmentProbeManager,
        XREnvironmentProbeSubsystem,
        XREnvironmentProbeSubsystemDescriptor,
        XREnvironmentProbeSubsystem.Provider,
        XREnvironmentProbe,
        AREnvironmentProbe>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
