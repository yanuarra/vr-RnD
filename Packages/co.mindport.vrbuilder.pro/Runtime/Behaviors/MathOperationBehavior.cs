using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Properties.Operations;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.UI.SelectableValues;
using VRBuilder.Core.Utils;
using VRBuilder.Pro.Properties.Operations;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// A behavior that performs an operation on a <see cref="NumberDataProperty"/> and sets it to the new value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class MathOperationBehavior : Behavior<MathOperationBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="MathOperationBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Math Operation")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [HideInProcessInspector]
            public SingleScenePropertyReference<IDataProperty<float>> Modified { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public IOperationCommand<float, float> Operation { get; set; }

            [DataMember]
            [HideInProcessInspector]
            public ProcessVariableSelectableValue<float> Modifier { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    return $"Set {Modified} to ({Modified} {Operation} {Modifier.Value})";
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
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            /// <inheritdoc />
            public override void End()
            {
                Data.Modified.Value.SetValue(Data.Operation.Execute(Data.Modified.Value.GetValue(), Data.Modifier.Value));
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public MathOperationBehavior() : this(Guid.Empty, Guid.Empty, 0f, true, new SumOperation())
        {
        }

        public MathOperationBehavior(Guid modifiedPropertyGuid, Guid modifierPropertyGuid, float modifierValue, bool isModifierConst, IOperationCommand<float, float> operation)
        {
            Data.Modified = new SingleScenePropertyReference<IDataProperty<float>>(modifiedPropertyGuid);
            Data.Modifier = new ProcessVariableSelectableValue<float>(modifierValue, new SingleScenePropertyReference<IDataProperty<float>>(modifierPropertyGuid), isModifierConst);
            Data.Operation = operation;
        }

        public MathOperationBehavior(IDataProperty<float> modifiedProperty, IDataProperty<float> modifierProperty, float value, bool isModifierConst, IOperationCommand<float, float> operation) :
            this(ProcessReferenceUtils.GetUniqueIdFrom(modifiedProperty), ProcessReferenceUtils.GetUniqueIdFrom(modifierProperty), value, isModifierConst, operation)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
