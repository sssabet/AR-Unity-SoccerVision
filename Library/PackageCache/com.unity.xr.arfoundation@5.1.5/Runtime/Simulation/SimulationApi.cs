using UnityEngine.XR.Management;

namespace UnityEngine.XR.Simulation
{
    static class Api
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        public static bool supported => true;
#else
        public static bool supported => false;
#endif

        public static bool loaderPresent => FindLoader() != null;

        static SimulationLoader FindLoader()
        {
            var instance = XRGeneralSettings.Instance;
            if (instance == null)
                return null;

            var manager = instance.Manager;
            if (manager == null || manager.activeLoaders == null)
                return null;

            foreach (var loader in manager.activeLoaders)
            {
                if (loader is SimulationLoader simulationLoader)
                    return simulationLoader;
            }

            return null;
        }
    }
}
