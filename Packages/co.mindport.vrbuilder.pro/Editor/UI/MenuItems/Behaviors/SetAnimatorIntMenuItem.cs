using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class SetAnimatorIntMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Set Animator Integer";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new SetAnimatorIntParameterBehavior();
        }
    }
}
