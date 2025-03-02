using UnityEngine.UIElements;
using VRBuilder.Core;
using VRBuilder.Core.Editor.UI.GraphView.Instantiators;
using VRBuilder.Core.Editor.UI.GraphView.Nodes;
using VRBuilder.Pro.Editor.UI.GraphView.Nodes;

namespace VRBuilder.Pro.Editor.UI.GraphView.Instantiators
{
    /// <summary>
    /// Instantiator for a <see cref="RandomBranchGraphNode"/>.
    /// </summary>
    public class RandomBranchNodeInstantiator : IStepNodeInstantiator
    {
        /// <inheritdoc/>
        public string Name => "Random Branch";

        /// <inheritdoc/>
        public bool IsInNodeMenu => true;

        /// <inheritdoc/>
        public string StepType => "randomBranch";

        /// <inheritdoc/>
        public int Priority => 200;

        /// <inheritdoc/>
        public ProcessGraphNode InstantiateNode(IStep step)
        {
            return new RandomBranchGraphNode(step);
        }

        /// <inheritdoc/>
        public DropdownMenuAction.Status GetContextMenuStatus(IEventHandler target, IChapter currentChapter)
        {
            return DropdownMenuAction.Status.Normal;
        }
    }
}