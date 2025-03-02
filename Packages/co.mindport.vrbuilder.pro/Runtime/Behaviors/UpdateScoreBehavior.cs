using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// A behavior that updates a score and can provide feedback for it.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/track-measure-add-on")]
    public class UpdateScoreBehavior : Behavior<UpdateScoreBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="UpdateScoreBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Update Score")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Score data property")]
            public SingleScenePropertyReference<IDataProperty<float>> ScoreProperty { get; set; }

            [DataMember]
            [DisplayName("Score increase")]
            public float ValueDelta { get; set; }

            [DataMember]
            [DisplayName("Feedback properties")]
            public MultipleScenePropertyReference<IScoreFeedbackProperty> FeedbackProperties { get; set; }

            [DataMember]
            [DisplayName("Feedback position provider")]
            public SingleSceneObjectReference FeedbackPosition { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    return $"Update score on {ScoreProperty} by {ValueDelta}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                Data.ScoreProperty.Value.SetValue(Data.ScoreProperty.Value.GetValue() + Data.ValueDelta);

                foreach (IScoreFeedbackProperty feedbackProperty in Data.FeedbackProperties.Values)
                {
                    Transform positionProvider = Data.FeedbackPosition.Value != null ? Data.FeedbackPosition.Value.GameObject.transform : feedbackProperty.SceneObject.GameObject.transform;
                    feedbackProperty.TriggerFeedback(Data.ValueDelta, Data.ScoreProperty.Value.GetValue(), positionProvider);
                }
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            /// <inheritdoc />
            public override void End()
            {
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public UpdateScoreBehavior() : this(Guid.Empty, Guid.Empty, 0f, Guid.Empty)
        {
        }

        public UpdateScoreBehavior(Guid feedbackPropertyGuid, Guid dataPropertyGuid, float pointDelta, Guid positionProviderGuid)
        {
            Data.FeedbackProperties = new MultipleScenePropertyReference<IScoreFeedbackProperty>(feedbackPropertyGuid);
            Data.ScoreProperty = new SingleScenePropertyReference<IDataProperty<float>>(dataPropertyGuid);
            Data.ValueDelta = pointDelta;
            Data.FeedbackPosition = new SingleSceneObjectReference(positionProviderGuid);
        }

        public UpdateScoreBehavior(IScoreFeedbackProperty feedbackProperty, IDataProperty<float> dataProperty, float pointDelta, ISceneObject positionProvider) :
            this(ProcessReferenceUtils.GetUniqueIdFrom(feedbackProperty), ProcessReferenceUtils.GetUniqueIdFrom(dataProperty), pointDelta, ProcessReferenceUtils.GetUniqueIdFrom(positionProvider))
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
