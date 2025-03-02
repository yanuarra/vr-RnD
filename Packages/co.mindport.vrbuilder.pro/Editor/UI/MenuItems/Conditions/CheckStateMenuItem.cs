using VRBuilder.Core.Conditions;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Conditions;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Conditions
{
    /// <inheritdoc />
    public class CheckStateMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Check State";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new CheckStateCondition();
        }
    }
}
