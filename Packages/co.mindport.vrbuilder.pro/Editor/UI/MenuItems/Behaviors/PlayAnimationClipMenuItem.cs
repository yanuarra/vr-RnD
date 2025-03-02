using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class PlayAnimationClipMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Play Animation Clip";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new PlayAnimationClipBehavior();
        }
    }
}
