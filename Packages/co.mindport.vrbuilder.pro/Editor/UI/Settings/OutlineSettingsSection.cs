using VRBuilder.Core.Editor.UI.ProjectSettings;
using VRBuilder.Pro.Settings;
using VRBuilder.Pro.Utils.QuickOutline;

namespace VRBuilder.Pro.Editor.UI.Settings
{
    /// <summary>
    /// Settings section for <see cref="OutlineRenderer"/> settings.
    /// </summary>
    public class OutlineSettingsSection : ComponentSettingsSection<OutlineRenderer, OutlineSettings>
    {
        /// <inheritdoc/>
        public override string Title => "Outlines";

        /// <inheritdoc/>
        public override string Description => "Specify global settings for all outline renderers in the project. These settings can be overridden locally.";

        /// <inheritdoc/>
        public override int Priority => 2000;
    }
}