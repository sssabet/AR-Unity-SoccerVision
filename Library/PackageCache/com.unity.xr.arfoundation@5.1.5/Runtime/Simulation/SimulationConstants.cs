namespace UnityEngine.XR.Simulation
{
    static class SimulationConstants
    {
        public static readonly int[] reservedLayers = { 0, 1, 2, 3, 4, 5, 6, 7, 31 };

        public const int firstValidLayer = 1;
        public const int allLayerCount = 32;
        public const int validLayerCount = allLayerCount - firstValidLayer;

        public const float sixtyFps = 1.0f / 60;
        public const float oneHundredTwentyFps = 1.0f / 120;

        public const string userSettingsPath = "Assets/XR/UserSimulationSettings";
        public const string runtimeSettingsPath = "Assets/XR";
    }
}
