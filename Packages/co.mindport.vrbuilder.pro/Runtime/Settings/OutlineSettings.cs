using UnityEngine;
using VRBuilder.Core.Settings;
using VRBuilder.Pro.Utils.QuickOutline;

namespace VRBuilder.Pro.Settings
{
    /// <summary>
    /// Settings scriptable object for <see cref="OutlineRenderer"/> settings.
    /// </summary>
    [CreateAssetMenu(fileName = "OutlineSettings", menuName = "VR Builder/OutlineSettings", order = 2)]
    public class OutlineSettings : ComponentSettings<OutlineRenderer, OutlineSettings>
    {
        /// <summary>
        /// The outline mode used by all outline renderers in the project.
        /// </summary>
        [SerializeField]
        [Tooltip("The outline mode used by all outline renderers in the project.")]
        private OutlineRenderer.Mode outlineMode;

        /// <summary>
        /// Default color of the outline. This is overridden by the Outline Objects behavior.
        /// </summary>
        [SerializeField]
        [Tooltip("Default color of the outline. This is overridden by the Outline Objects behavior.")]
        private Color outlineColor = Color.white;

        /// <summary>
        /// Width of the drawn outline.
        /// </summary>
        [SerializeField]
        [Tooltip("Width of the drawn outline.")]
        private float outlineWidth = 2;

        public override void ApplySettings(OutlineRenderer target)
        {
            target.OutlineMode = outlineMode;
            target.OutlineColor = outlineColor;
            target.OutlineWidth = outlineWidth;
        }
    }
}