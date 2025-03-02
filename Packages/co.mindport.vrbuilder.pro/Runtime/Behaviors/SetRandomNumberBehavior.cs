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

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// A behavior that sets a boolean data property to a random value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/randomization-add-on")]
    public class SetRandomNumberBehavior : Behavior<SetRandomNumberBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetRandomNumberBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Set Random Number")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Data Property")]
            public MultipleScenePropertyReference<IDataProperty<float>> DataProperties { get; set; }

            [DataMember]
            [DisplayName("Min Value")]
            public float MinValue { get; set; }

            [DataMember]
            [DisplayName("Max Value")]
            public float MaxValue { get; set; }

            [DataMember]
            [DisplayName("Randomize Integer")]
            public bool RandomizeInteger { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string randomizeInteger = RandomizeInteger ? "integer" : "float";
                    return $"Randomize {DataProperties} ({randomizeInteger} between {MinValue} and {MaxValue})";
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
                foreach (IDataProperty<float> dataProperty in Data.DataProperties.Values)
                {
                    float randomValue;

                    if (Data.RandomizeInteger)
                    {
                        randomValue = UnityEngine.Random.Range((int)Mathf.Floor(Data.MinValue), (int)Mathf.Floor(Data.MaxValue) + 1);
                    }
                    else
                    {
                        randomValue = UnityEngine.Random.Range(Data.MinValue, Data.MaxValue);
                    }

                    dataProperty.SetValue(randomValue);
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
        public SetRandomNumberBehavior() : this(Guid.Empty, 0f, 0f, false)
        {
        }

        public SetRandomNumberBehavior(Guid propertyGuid, float minValue, float maxValue, bool randomizeInteger)
        {
            Data.DataProperties = new MultipleScenePropertyReference<IDataProperty<float>>(propertyGuid);
            Data.MinValue = minValue;
            Data.MaxValue = maxValue;
            Data.RandomizeInteger = randomizeInteger;
        }

        public SetRandomNumberBehavior(IDataProperty<float> property, float minValue, float maxValue, bool randomizeInteger) : this(ProcessReferenceUtils.GetUniqueIdFrom(property), minValue, maxValue, randomizeInteger)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
