using Newtonsoft.Json;
using System;
using System.Collections;
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
    /// Sets an integer parameter on an animator to the specified value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorIntParameterBehavior : Behavior<SetAnimatorIntParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorIntParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Integer")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Object with the animator property.
            /// </summary>
            [DataMember]
            [DisplayName("Animators")]
            public MultipleScenePropertyReference<IAnimatorProperty> Animators { get; set; }

            /// <summary>
            /// Name of the parameter to be changed.
            /// </summary>
            [DataMember]
            [DisplayName("Parameter name")]
            public string ParameterName { get; set; }

            /// <summary>
            /// New value for the selected parameter.
            /// </summary>
            [DataMember]
            [DisplayName("Parameter value")]
            public int ParameterValue { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string parameterName = string.IsNullOrEmpty(ParameterName) ? "[EMPTY]" : ParameterName;
                    return $"Set parameter {parameterName} to {ParameterValue} on {Animators}";
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
                foreach (IAnimatorProperty animator in Data.Animators.Values)
                {
                    animator.SetInteger(Data.ParameterName, Data.ParameterValue);
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

            public override void FastForward()
            {
            }
        }

        [JsonConstructor, Preserve]
        public SetAnimatorIntParameterBehavior() : this(Guid.Empty, "", 0)
        {
        }

        public SetAnimatorIntParameterBehavior(IAnimatorProperty animatorProperty, string parameterName, int parameterValue) : this(ProcessReferenceUtils.GetUniqueIdFrom(animatorProperty), parameterName, parameterValue)
        {
        }

        public SetAnimatorIntParameterBehavior(Guid animatorPropertyGuid, string parameterName, int parameterValue)
        {
            Data.Animators = new MultipleScenePropertyReference<IAnimatorProperty>(animatorPropertyGuid);
            Data.ParameterName = parameterName;
            Data.ParameterValue = parameterValue;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
