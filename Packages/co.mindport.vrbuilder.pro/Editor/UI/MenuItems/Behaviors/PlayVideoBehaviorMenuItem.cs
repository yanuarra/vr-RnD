using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <summary>
    /// Menu item for <see cref="PlayVideoBehavior"/>.
    /// </summary>
    public class PlayVideoBehaviorMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc/>        
        public override string DisplayedName => "Guidance/Play Video";

        /// <inheritdoc/>        
        public override IBehavior GetNewItem()
        {
            return new PlayVideoBehavior();
        }
    }
}