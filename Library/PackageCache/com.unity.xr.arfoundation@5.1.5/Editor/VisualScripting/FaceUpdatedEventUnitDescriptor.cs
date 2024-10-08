#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="FaceUpdatedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(FaceUpdatedEventUnit))]
    public sealed class FaceUpdatedEventUnitDescriptor : UnitDescriptor<FaceUpdatedEventUnit>
    {
        static readonly string k_UnitDescription =
            $"Triggers when the {nameof(ARFace)} component on the input GameObject is updated.\n" +
            $"{nameof(ARFace)}s can be updated every frame if there is an enabled {nameof(ARFaceManager)} in the scene.";

        static readonly string k_TargetPortDescription =
            $"Target GameObject must have an enabled {nameof(ARFace)} component, otherwise this node throws an exception.";
        
        static readonly string k_FaceOutPortDescription =
            $"The updated {nameof(ARFace)}. You can connect this to an Expose node to access its contents.";

        /// <inheritdoc/>
        public FaceUpdatedEventUnitDescriptor(FaceUpdatedEventUnit target) : base(target)
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
                case nameof(target.faceOut):
                    portDescription.summary = k_FaceOutPortDescription;
                    break;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
