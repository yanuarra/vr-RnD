using VRBuilder.Core.Conditions;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Conditions
{
    /// <inheritdoc />
    public class CompareTextMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Compare Text";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new CompareValuesCondition<string>();
        }
    }
}
