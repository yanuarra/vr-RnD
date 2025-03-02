using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Animates an object's transform over time to a new position, rotation and/or scale.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class ChildExplosionViewBehavior : Behavior<ChildExplosionViewBehavior.EntityData>
    {
        /// <summary>
        /// The "follow path" behavior's data.
        /// </summary>
        [DisplayName("Child Explosion View")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Target scene object to be transformed.
            /// </summary>
            [DataMember]
            [DisplayName("Object")]
            public SingleScenePropertyReference<IExplodableProperty> TargetObject { get; set; }

            /// <summary>
            /// Scale of the explosion. A value of 1 resets the object to the initial state.
            /// </summary>
            [DataMember]
            [DisplayName("Scale")]
            public float Scale { get; set; }

            /// <summary>
            /// Duration of the transition. If duration is equal or less than zero, the new transform will be applied immediately.
            /// </summary>
            [DataMember]
            [DisplayName("Duration (in seconds)")]
            public float Duration { get; set; }

            /// <summary>
            /// Determines the position of the object at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Explosion curve")]
            public AnimationCurve ExplosionCurve { get; set; }

            /// <summary>
            /// If enabled, the animation will repeat in reverse, returning the object to the starting position.
            /// </summary>
            [DataMember]
            [DisplayName("Ping pong")]
            public bool PingPong { get; set; }

            /// <summary>
            /// Number of times the animation should be repeated.
            /// </summary>
            [DataMember]
            [DisplayName("Repeats")]
            public int Repeats { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            public string Name
            {
                get
                {
                    string action;
                    string scale;

                    if (Scale == 1)
                    {
                        action = "Reset exploded view of ";
                        scale = string.Empty;
                    }
                    else if (Mathf.Abs(Scale) > 1)
                    {
                        action = "Explode view of ";
                        scale = $" by {Scale}";
                    }
                    else
                    {
                        action = "Implode view of ";
                        scale = $" by {Scale}";
                    }

                    return $"{action}{TargetObject}{scale}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private float startingTime;
            private float repeats = 0;

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                RuntimeConfigurator.Configuration.SceneObjectManager.RequestAuthority(Data.TargetObject.Value.SceneObject);

                startingTime = Time.time;

                Data.TargetObject.Value.InitializeAnimation();

                foreach (Rigidbody movingRigidbody in Data.TargetObject.Value.SceneObject.GameObject.GetComponentsInChildren<Rigidbody>())
                {
                    if (movingRigidbody.isKinematic == false)
                    {
                        movingRigidbody.linearVelocity = Vector3.zero;
                        movingRigidbody.angularVelocity = Vector3.zero;
                    }
                }
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                while (repeats < Data.Repeats)
                {
                    while (Time.time - startingTime < Data.Duration)
                    {
                        Data.TargetObject.Value.Explode(GetAnimationProgress(Data.ExplosionCurve, false), Data.Scale);
                        yield return null;
                    }

                    if (Data.PingPong)
                    {
                        startingTime = Time.time;
                        while (Time.time - startingTime < Data.Duration)
                        {
                            Data.TargetObject.Value.Explode(GetAnimationProgress(Data.ExplosionCurve, true), Data.Scale);
                            yield return null;
                        }
                    }

                    repeats++;
                    startingTime = Time.time;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                float endTime = Data.PingPong ? 0 : Data.ExplosionCurve.keys.Last().time;
                Data.TargetObject.Value.Explode(Data.ExplosionCurve.Evaluate(endTime), Data.Scale);

                foreach (Rigidbody movingRigidbody in Data.TargetObject.Value.SceneObject.GameObject.GetComponentsInChildren<Rigidbody>())
                {
                    if (movingRigidbody.isKinematic == false)
                    {
                        movingRigidbody.linearVelocity = Vector3.zero;
                        movingRigidbody.angularVelocity = Vector3.zero;
                    }
                }
            }
            public override void FastForward()
            {
            }

            private float GetAnimationProgress(AnimationCurve curve, bool isReversed)
            {
                if (isReversed)
                {
                    return curve.Evaluate((Data.Duration - (Time.time - startingTime)) / Data.Duration * curve.keys.Last().time);
                }
                else
                {
                    return curve.Evaluate((Time.time - startingTime) / Data.Duration * curve.keys.Last().time);
                }
            }
        }

        [JsonConstructor, Preserve]
        public ChildExplosionViewBehavior() : this(Guid.Empty, 2f, 1f, null)
        {
            Data.ExplosionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public ChildExplosionViewBehavior(IExplodableProperty target, float scale, float duration, AnimationCurve explosionCurve, bool pingPong = false, int repeats = 1) : this(ProcessReferenceUtils.GetUniqueIdFrom(target), duration, scale, explosionCurve, pingPong, repeats)
        {
        }

        public ChildExplosionViewBehavior(Guid targetGuid, float scale, float duration, AnimationCurve explosionCurve, bool pingPong = false, int repeats = 1)
        {
            Data.TargetObject = new SingleScenePropertyReference<IExplodableProperty>(targetGuid);
            Data.Duration = duration;
            Data.Scale = scale;
            Data.ExplosionCurve = explosionCurve;
            Data.PingPong = pingPong;
            Data.Repeats = repeats;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
