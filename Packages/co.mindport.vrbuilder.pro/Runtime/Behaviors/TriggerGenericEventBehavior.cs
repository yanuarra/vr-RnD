using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.UI.SelectableValues;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Behavior that triggers a Unity event on a property.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class TriggerGenericEventBehavior<T> : Behavior<TriggerGenericEventBehavior<T>.EntityData>
    {
        /// <summary>
        /// The <see cref="TriggerGenericEventBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Trigger Event (Group)")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// The group of objects that will trigger the event.
            /// </summary>
            [DataMember]
            public MultipleScenePropertyReference<IGenericEventProperty<T>> Targets { get; set; }

            /// <summary>
            /// The data to be passed to the event.
            /// </summary>
            [DataMember]
            public ProcessVariableSelectableValue<T> Argument { get; set; }

            /// <summary>
            /// A property that determines if the event should trigger at activation or deactivation (or both).
            /// </summary>
            [DataMember]
            [DisplayName("Execution stages")]
            public BehaviorExecutionStages ExecutionStages { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string argument = Argument.Value == null ? "[NULL]" : Argument.Value.ToString();
                    return $"Trigger event on {Targets} (Parameter: {argument})";
                }
            }
        }

        private class TriggerEventProcess : StageProcess<EntityData>
        {
            private BehaviorExecutionStages executionStages;

            public TriggerEventProcess(EntityData data, BehaviorExecutionStages executionStages) : base(data)
            {
                this.executionStages = executionStages;
            }

            /// <inheritdoc />
            public override void Start()
            {
                if ((Data.ExecutionStages & executionStages) > 0)
                {
                    foreach (IGenericEventProperty<T> eventProperty in Data.Targets.Values)
                    {
                        eventProperty.TriggerEvent(Data.Argument.Value);
                    }
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
        public TriggerGenericEventBehavior()
        {
            Data.Targets = new MultipleScenePropertyReference<IGenericEventProperty<T>>();
            Data.Argument = new ProcessVariableSelectableValue<T>();
            Data.ExecutionStages = BehaviorExecutionStages.Activation;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new TriggerEventProcess(Data, BehaviorExecutionStages.Activation);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new TriggerEventProcess(Data, BehaviorExecutionStages.Deactivation);
        }
    }
}