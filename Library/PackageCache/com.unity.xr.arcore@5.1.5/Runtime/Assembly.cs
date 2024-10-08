using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.XR.ARCore.Editor")]
[assembly: InternalsVisibleTo("Unity.XR.ARCore.Editor.Tests")]
#endif
