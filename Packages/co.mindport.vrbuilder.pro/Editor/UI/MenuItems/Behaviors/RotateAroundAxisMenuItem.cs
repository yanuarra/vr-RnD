using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class RotateAroundAxisMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Rotate Around Axis";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new RotateAroundAxisBehavior();
        }
    }
}
