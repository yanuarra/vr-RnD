using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class MathOperationMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Math Operation";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new MathOperationBehavior();
        }
    }
}
