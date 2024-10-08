using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.XR.ARFoundation")]
[assembly: InternalsVisibleTo("Unity.XR.ARSubsystems")]
[assembly: InternalsVisibleTo("Unity.XR.Simulation")]
[assembly: InternalsVisibleTo("Unity.XR.ARFoundation.Runtime.Tests")]
[assembly: InternalsVisibleTo("Unity.XR.Simulation.Runtime.Tests")]

#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.XR.ARFoundation.Editor")]
[assembly: InternalsVisibleTo("Unity.XR.ARFoundation.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.XR.ARSubsystems.Editor")]
[assembly: InternalsVisibleTo("Unity.XR.ARSubsystems.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.XR.Simulation.Editor")]
[assembly: InternalsVisibleTo("Unity.XR.ARFoundation.InternalUtils.Editor.Tests")]
#endif
