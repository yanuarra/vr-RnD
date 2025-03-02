using VRBuilder.Core;
using VRBuilder.Core.Entities.Factories;
using VRBuilder.Pro.Behaviors;
using VRBuilder.Pro.Conditions;

namespace VRBuilder.Pro.EntityPostProcessing
{
    /// <summary>
    /// <see cref="IStep"/> implementation of <see cref="EntityPostProcessing{T}"/> specific for "randomBranch" steps.
    /// </summary>
    public class RandomBranchPostProcessing : EntityPostProcessing<IStep>
    {
        /// <inheritdoc />
        public override void Execute(IStep entity)
        {
            if (entity.StepMetadata.StepType == "randomBranch")
            {
                entity.Data.Behaviors.Data.Behaviors.Add(new SelectRandomTransitionBehavior());

                for (int i = 0; i < 2; i++)
                {
                    ITransition transition = EntityFactory.CreateTransition();
                    transition.Data.Conditions.Add(new RandomlySelectedCondition());
                    entity.Data.Transitions.Data.Transitions.Add(transition);
                }
            }
        }
    }
}
