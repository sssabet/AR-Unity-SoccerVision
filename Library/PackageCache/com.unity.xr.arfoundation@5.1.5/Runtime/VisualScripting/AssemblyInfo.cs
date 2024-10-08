#if UNITY_EDITOR
using System.Runtime.CompilerServices;
#endif
#if VISUALSCRIPTING_1_8_OR_NEWER
using UnityEngine.Scripting;
#endif

#if UNITY_EDITOR
[assembly: InternalsVisibleTo("Unity.XR.ARFoundation.VisualScripting.Editor")]
#endif
#if VISUALSCRIPTING_1_8_OR_NEWER
[assembly: AlwaysLinkAssembly, Preserve]
#endif
