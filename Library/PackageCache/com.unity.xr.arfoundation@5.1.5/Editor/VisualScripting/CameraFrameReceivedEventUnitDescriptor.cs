#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="CameraFrameReceivedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(CameraFrameReceivedEventUnit))]
    public sealed class CameraFrameReceivedEventUnitDescriptor : UnitDescriptor<CameraFrameReceivedEventUnit>
    {
        const string k_UnitDescription = "Triggers every frame when Unity receives a new image from the device camera.";

        static readonly string k_TargetPortDescription =
            $"Target GameObject should have an enabled {nameof(ARCameraManager)} component.\n" +
            $" If you do not connect this port, this node searches for an enabled {nameof(ARCameraManager)} component" +
            " instead, and throws an exception if none is found.";
        
        static readonly string k_FrameEventArgsPortDescription =
            $"The output {nameof(ARCameraFrameEventArgs)}. You can connect this to an Expose node to access its contents.";

        /// <inheritdoc/>
        public CameraFrameReceivedEventUnitDescriptor(CameraFrameReceivedEventUnit target)
            : base(target)
        {
        }

        /// <summary>
        /// Render a summary under the unit title in the Graph Inspector.
        /// </summary>
        /// <returns>The summary.</returns>
        protected override string DefinedSummary() => k_UnitDescription;

        /// <summary>
        /// Render summaries of each port in the Graph Inspector.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="portDescription">The port description to write to.</param>
        protected override void DefinedPort(IUnitPort port, UnitPortDescription portDescription)
        {
            base.DefinedPort(port, portDescription);

            switch (port.key)
            {
                case nameof(target.target):
                    portDescription.summary = k_TargetPortDescription;
                    break;
                case nameof(target.frameEventArgs):
                    portDescription.summary = k_FrameEventArgsPortDescription;
                    break;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
