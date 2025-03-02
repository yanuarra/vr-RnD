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
    /// A behavior that stops a <see cref="TimerProperty"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/track-measure-add-on")]
    public class StopTimerBehavior : Behavior<StopTimerBehavior.EntityData>
    {
        /// <summary>
        /// The <see cref="StopTimerBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Stop Timer")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Timer")]
            public MultipleScenePropertyReference<ITimerProperty> TimerProperties { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    return $"Stop timer on {TimerProperties}";
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
                    timerProperty.StopTimer();
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
        public StopTimerBehavior() : this(Guid.Empty)
        {
        }

        public StopTimerBehavior(Guid propertyGuid)
        {
            Data.TimerProperties = new MultipleScenePropertyReference<ITimerProperty>(propertyGuid);
        }

        public StopTimerBehavior(ITimerProperty property) : this(ProcessReferenceUtils.GetUniqueIdFrom(property))
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
