using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    internal static class SubsystemsUtility
    {
        internal static bool TryGetLoadedSubsystem<TSubsystemBase, TSubsystemConcrete>(out TSubsystemConcrete subsystem)
            where TSubsystemBase : SubsystemWithProvider
            where TSubsystemConcrete : TSubsystemBase
        {
            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null)
            {
                subsystem = null;
                return false;
            }
            
            var loader = XRGeneralSettings.Instance.Manager.activeLoader;
            var asBaseSubsystem = loader != null ? loader.GetLoadedSubsystem<TSubsystemBase>() : null;
            subsystem = asBaseSubsystem as TSubsystemConcrete;
            return subsystem != null;
        }
    }
}
