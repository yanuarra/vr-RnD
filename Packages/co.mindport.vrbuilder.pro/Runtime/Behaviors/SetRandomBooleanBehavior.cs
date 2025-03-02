using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// A behavior that sets a boolean data property to a random value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/randomization-add-on")]
    public class SetRandomBooleanBehavior : Behavior<SetRandomBooleanBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetRandomBooleanBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Set Random Boolean")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Data Properties")]
            public MultipleScenePropertyReference<IDataProperty<bool>> DataProperties { get; set; }

            [DataMember]
            [DisplayName("Probability to be True")]
            [UsesSpecificProcessDrawer("NormalizedFloatDrawer")]
            public float TrueProbability { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    return $"Randomize {DataProperties} ({TrueProbability * 100f}% True)";
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
                foreach (IDataProperty<bool> dataProperty in Data.DataProperties.Values)
                {
                    float randomValue = UnityEngine.Random.value;
                    dataProperty.SetValue(randomValue <= Data.TrueProbability && randomValue != 0f);
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
        public SetRandomBooleanBehavior() : this(Guid.Empty)
        {
        }

        public SetRandomBooleanBehavior(Guid propertyGuid, float trueProbability = 0.5f)
        {
            Data.DataProperties = new MultipleScenePropertyReference<IDataProperty<bool>>(propertyGuid);
            Data.TrueProbability = trueProbability;
        }

        public SetRandomBooleanBehavior(IDataProperty<bool> property, float trueProbability) : this(ProcessReferenceUtils.GetUniqueIdFrom(property), trueProbability)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
