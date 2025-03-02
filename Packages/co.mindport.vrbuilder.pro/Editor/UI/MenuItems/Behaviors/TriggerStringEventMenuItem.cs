using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class TriggerStringEventMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Trigger Event (string)";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new TriggerGenericEventBehavior<string>();
        }
    }
}
