using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.Pro.Behaviors;

namespace VRBuilder.Pro.Editor.UI.MenuItems.Behaviors
{
    /// <inheritdoc />
    public class PointAtObjectsMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Guidance/Point at Objects";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new PointAtObjectsBehavior();
        }
    }
}
