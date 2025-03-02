using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Behavior that sets a <see cref="StateDataProperty{T}"/> to a specific value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class SetStateBehavior : Behavior<SetStateBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetValueBehavior{T}"/> behavior data.
        /// </summary>
        [DisplayName("Set State")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [HideInProcessInspector]
            public MultipleScenePropertyReference<StateDataPropertyBase> DataProperties { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public int NewValue { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string newValue = "[NONE]";

                    if (DataProperties.HasValue())
                    {
                        Type stateType = DataProperties.Values.FirstOrDefault()?.StateType;

                        foreach (StateDataPropertyBase stateDataProperty in DataProperties.Values)
                        {
                            if (stateDataProperty.StateType != stateType)
                            {
                                newValue = "[INCOMPATIBLE STATES]";
                                break;
                            }
                        }

                        if (stateType != null)
                        {
                            newValue = Enum.ToObject(stateType, NewValue).ToString();
                        }
                    }

                    return $"Set {DataProperties} to {newValue}";
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
                foreach (StateDataPropertyBase dataProperty in Data.DataProperties.Values)
                {
                    dataProperty.SetValue(Data.NewValue);
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
        public SetStateBehavior() : this(Guid.Empty, default)
        {
        }

        public SetStateBehavior(Guid propertyGuid, int value)
        {
            Data.DataProperties = new MultipleScenePropertyReference<StateDataPropertyBase>(propertyGuid);
            Data.NewValue = value;
        }

        public SetStateBehavior(StateDataPropertyBase property, int value) : this(ProcessReferenceUtils.GetUniqueIdFrom(property), value)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

    }
}