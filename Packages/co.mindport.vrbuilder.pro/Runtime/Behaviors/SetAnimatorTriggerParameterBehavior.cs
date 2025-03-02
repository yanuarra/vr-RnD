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
    /// Sets a trigger parameter on an animator.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class SetAnimatorTriggerParameterBehavior : Behavior<SetAnimatorTriggerParameterBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="SetAnimatorTriggerParameterBehavior"/> behavior's data.
        /// </summary>
        [DisplayName("Set Animator Trigger")]
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

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string parameterName = string.IsNullOrEmpty(ParameterName) ? "[EMPTY]" : ParameterName;
                    return $"Trigger parameter {parameterName} on {Animators}";
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
                    animator.SetTrigger(Data.ParameterName);
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
        public SetAnimatorTriggerParameterBehavior() : this(Guid.Empty, "")
        {
        }

        public SetAnimatorTriggerParameterBehavior(IAnimatorProperty animatorProperty, string parameterName) : this(ProcessReferenceUtils.GetUniqueIdFrom(animatorProperty), parameterName)
        {
        }

        public SetAnimatorTriggerParameterBehavior(Guid animatorPropertyGuid, string parameterName)
        {
            Data.Animators = new MultipleScenePropertyReference<IAnimatorProperty>(animatorPropertyGuid);
            Data.ParameterName = parameterName;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
