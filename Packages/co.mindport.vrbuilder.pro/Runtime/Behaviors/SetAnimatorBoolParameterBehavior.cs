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
    /// Sets a boolean parameter on an animator to the specified value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorBoolParameterBehavior : Behavior<SetAnimatorBoolParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorBoolParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Boolean")]
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
            public bool ParameterValue { get; set; }

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
                    animator.SetBool(Data.ParameterName, Data.ParameterValue);
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
        public SetAnimatorBoolParameterBehavior() : this(Guid.Empty, "", false)
        {
        }

        public SetAnimatorBoolParameterBehavior(IAnimatorProperty animatorProperty, string parameterName, bool parameterValue) : this(ProcessReferenceUtils.GetUniqueIdFrom(animatorProperty), parameterName, parameterValue)
        {
        }

        public SetAnimatorBoolParameterBehavior(Guid animatorPropertyGuid, string parameterName, bool parameterValue)
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
