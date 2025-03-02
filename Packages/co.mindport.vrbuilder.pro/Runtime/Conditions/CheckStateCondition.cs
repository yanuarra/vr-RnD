using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Properties.Operations;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Conditions
{
    /// <summary>
    /// A condition that compares a <see cref="StateDataProperty{T}"/> to a specified value and completes when the comparison returns true.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class CheckStateCondition : Condition<CheckStateCondition.EntityData>
    {
        /// <summary>
        /// The data for a <see cref="CheckStateCondition"/>
        /// </summary>
        [DisplayName("Check State")]
        public class EntityData : IConditionData
        {
            [DataMember]
            [HideInProcessInspector]
            public SingleScenePropertyReference<StateDataPropertyBase> StateDataProperty { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public IOperationCommand<int, bool> Operation { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public int CompareValue { get; set; }

            /// <inheritdoc />
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string compareValue = StateDataProperty.HasValue() ? Enum.ToObject(StateDataProperty.Value.StateType, CompareValue).ToString() : "[NONE]";

                    return $"Compare ({StateDataProperty} {Operation} {compareValue})";
                }
            }
            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                int left = Data.StateDataProperty.Value.GetValue();
                int right = Data.CompareValue;

                return Data.Operation.Execute(left, right);
            }
        }

        [JsonConstructor, Preserve]
        public CheckStateCondition() : this(Guid.Empty, default, new EqualToOperation<int>())
        {
        }

        public CheckStateCondition(StateDataPropertyBase dataProperty, int compareValue, IOperationCommand<int, bool> operation) :
            this(ProcessReferenceUtils.GetUniqueIdFrom(dataProperty), compareValue, operation)
        {
        }

        public CheckStateCondition(Guid dataProperty, int compareValue, IOperationCommand<int, bool> operation)
        {
            Data.StateDataProperty = new SingleScenePropertyReference<StateDataPropertyBase>(dataProperty);
            Data.CompareValue = compareValue;
            Data.Operation = operation;
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }
    }
}