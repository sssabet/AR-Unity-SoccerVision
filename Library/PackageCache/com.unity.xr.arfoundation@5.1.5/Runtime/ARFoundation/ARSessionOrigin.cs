using System;
#if USE_LEGACY_INPUT_HELPERS
using UnityEngine.SpatialTracking;
#endif
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// An <c>ARSessionOrigin</c> is the parent for an AR setup. It contains a <c>Camera</c> and
    /// any <c>GameObject</c>s created from detected features, such as planes or point clouds.
    /// </summary>
    /// <remarks>
    /// Session space vs. Unity space
    ///
    /// Because an AR device will be used to drive the <c>Camera</c>'s position and rotation,
    /// you cannot directly place the <c>Camera</c> at an arbitrary position in the Unity scene.
    /// Instead, you should position the <c>ARSessionOrigin</c>. This makes the <c>Camera</c>
    /// (and any detected features) relative to that as a result.
    ///
    /// It is important to keep the <c>Camera</c> and detected features in the same space relative to
    /// each other (otherwise, detected features like planes won't appear in the correct place relative
    /// to the <c>Camera</c>). The space relative to the AR device's starting position is called
    /// "session space" or "device space". For example, when the AR session begins, the device might
    /// report its position as (0, 0, 0). Detected features, such as planes, will be reported relative
    /// to this starting position. The purpose of the <c>ARSessionOrigin</c> is to convert the session space
    /// to Unity world space.
    ///
    /// To facilitate this, the <c>ARSessionOrigin</c> creates a new <c>GameObject</c> called "Trackables"
    /// as a sibling of its <c>Camera</c>. This should be the parent <c>GameObject</c> for all
    /// detected features.
    ///
    /// At runtime, a typical scene graph might look like this:
    /// - AR Session Origin
    ///     - Camera
    ///     - Trackables
    ///         - Detected plane 1
    ///         - Detected plane 2
    ///         - Point cloud
    ///         - etc...
    ///
    /// You can access the "trackables" <c>GameObject</c> with <see cref="trackablesParent"/>.
    ///
    /// Note that the <c>localPosition</c> and <c>localRotation</c> of detected trackables
    /// remain in real-world meters relative to the AR device's starting position and rotation.
    ///
    /// Scale
    ///
    /// If you want to scale the content rendered by the <c>ARSessionOrigin</c> you should apply
    /// the scale to the <c>ARSessionOrigin</c>'s transform. This is preferrable to scaling
    /// the content directly, which can have undesirable side effects. Physics and NavMeshes,
    /// for example, do not perform well when scaled very small.
    /// </remarks>
    [DisallowMultipleComponent]
    [Obsolete("ARSessionOrigin has been deprecated. Use XROrigin instead.")]
    [HelpURL(typeof(ARSessionOrigin))]
    public class ARSessionOrigin : XROrigin
    {
        /// <summary>
        /// (Deprecated) The <c>Camera</c> to associate with the AR device. It must be a child of this <c>ARSessionOrigin</c>.
        /// </summary>
        [Obsolete("camera has been deprecated. Use Camera instead.")]
#if UNITY_EDITOR
        public new Camera camera
#else
        public Camera camera
#endif
        {
            get => Camera;
            set => Camera = value;
        }

        /// <summary>
        /// (Deprecated) The parent <c>Transform</c> for all "trackables" (for example, planes and feature points).
        /// </summary>
        [Obsolete("trackablesParent has been deprecated. Use TrackablesParent instead.")]
        public Transform trackablesParent => TrackablesParent;

    }
}
