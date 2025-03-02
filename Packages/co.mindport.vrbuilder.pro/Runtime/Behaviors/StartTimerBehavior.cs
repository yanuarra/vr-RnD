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
    /// A behavior that starts a <see cref="TimerProperty"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/track-measure-add-on")]
    public class StartTimerBehavior : Behavior<StartTimerBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="StartTimerBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Start Timer")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Timer")]
            public MultipleScenePropertyReference<ITimerProperty> TimerProperties { get; set; }

            [DataMember]
            [DisplayName("Is Countdown")]
            public bool IsCountdown { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string isCountdown = IsCountdown ? "countdown" : "timer";
                    return $"Start {isCountdown} on {TimerProperties}";
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
                foreach (ITimerProperty timerProperty in Data.TimerProperties.Values)
                {
                    timerProperty.IsCountdown = Data.IsCountdown;
                    timerProperty.StartTimer();
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
        public StartTimerBehavior() : this(Guid.Empty, false)
        {
        }

        public StartTimerBehavior(Guid propertyGuid, bool isCountdown)
        {
            Data.TimerProperties = new MultipleScenePropertyReference<ITimerProperty>(propertyGuid);
            Data.IsCountdown = isCountdown;
        }

        public StartTimerBehavior(ITimerProperty property, bool isCountdown) : this(ProcessReferenceUtils.GetUniqueIdFrom(property), isCountdown)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
