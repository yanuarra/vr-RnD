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
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Moves an object along a path. The object rotates to follow the path.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class FollowPathBehavior : Behavior<FollowPathBehavior.EntityData>
    {
        /// <summary>
        /// The "follow path" behavior's data.
        /// </summary>
        [DisplayName("Follow Path")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Target scene object to be moved.
            /// </summary>
            [DataMember]
            [DisplayName("Object")]
            public SingleSceneObjectReference TargetObject { get; set; }

            /// <summary>
            /// Path the target is meant to follow.
            /// </summary>
            [DataMember]
            [DisplayName("Path")]
            public SingleScenePropertyReference<IPathProperty> PathProperty { get; set; }

            /// <summary>
            /// If enabled, the object will keep its relative position to the path instead of being teleported on it when the animation starts.
            /// </summary>
            [DataMember]
            [DisplayName("Keep relative position")]
            public bool KeepRelativePosition { get; set; }

            /// <summary>
            /// If enabled, the object will keep its relative rotation to the path instead of having its forward vector pointing forward along the path.
            /// </summary>
            [DataMember]
            [DisplayName("Keep relative rotation")]
            public bool KeepRelativeRotation { get; set; }

            /// <summary>
            /// Duration of the transition. If duration is equal or less than zero, the object will be instantaneously moved at the end of the path.
            /// </summary>
            [DataMember]
            [DisplayName("Duration (in seconds)")]
            public float Duration { get; set; }

            /// <summary>
            /// Determines how fast the object moves at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Velocity curve")]
            public AnimationCurve Velocity { get; set; }

            /// <summary>
            /// If enabled, the object will not rotate while following the path. Settings on <see cref="KeepRelativeRotation"/> will be ignored.
            /// </summary>
            [DataMember]
            [DisplayName("Disable Rotation")]
            public bool DisableRotation { get; set; }

            /// <summary>
            /// If enabled, the animation will play backwards.
            /// </summary>
            [DataMember]
            [DisplayName("Reverse")]
            public bool Reverse { get; set; }

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
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    return $"Move {TargetObject} along {PathProperty}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private float startingTime;
            private float repeats = 0;
            private Vector3 initialPosition;
            private Quaternion initialRotation;

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                startingTime = Time.time;
                initialPosition = Data.TargetObject.Value.GameObject.transform.position;
                initialRotation = Data.TargetObject.Value.GameObject.transform.rotation;

                RuntimeConfigurator.Configuration.SceneObjectManager.RequestAuthority(Data.TargetObject.Value);

                Rigidbody movingRigidbody = Data.TargetObject.Value.GameObject.GetComponent<Rigidbody>();
                if (movingRigidbody != null && movingRigidbody.isKinematic == false)
                {
                    movingRigidbody.linearVelocity = Vector3.zero;
                    movingRigidbody.angularVelocity = Vector3.zero;
                }
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                Transform movingTransform = Data.TargetObject.Value.GameObject.transform;

                while (repeats < Data.Repeats)
                {
                    while (Time.time - startingTime < Data.Duration)
                    {
                        float progress = GetAnimationProgress(Data.Reverse);
                        ProcessAnimation(progress, movingTransform);
                        yield return null;
                    }

                    if (Data.PingPong)
                    {
                        startingTime = Time.time;
                        while (Time.time - startingTime < Data.Duration)
                        {
                            float progress = GetAnimationProgress(!Data.Reverse);
                            ProcessAnimation(progress, movingTransform);
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
                float endValue = Data.PingPong ^ Data.Reverse ? 0 : Data.Velocity.keys.Last().time;
                ProcessAnimation(Data.Velocity.Evaluate(endValue), Data.TargetObject.Value.GameObject.transform);

                Rigidbody movingRigidbody = Data.TargetObject.Value.GameObject.GetComponent<Rigidbody>();
                if (movingRigidbody != null && movingRigidbody.isKinematic == false)
                {
                    movingRigidbody.linearVelocity = Vector3.zero;
                    movingRigidbody.angularVelocity = Vector3.zero;
                }
            }

            public override void FastForward()
            {
            }

            private void ProcessAnimation(float progress, Transform movingTransform)
            {
                RuntimeConfigurator.Configuration.SceneObjectManager.RequestAuthority(Data.TargetObject.Value);

                if (Data.KeepRelativePosition)
                {
                    movingTransform.position = initialPosition + Data.PathProperty.Value.GetPoint(progress) - Data.PathProperty.Value.GetPoint(0);
                }
                else
                {
                    movingTransform.position = Data.PathProperty.Value.GetPoint(progress);
                }

                if (!Data.DisableRotation)
                {
                    if (Data.KeepRelativeRotation)
                    {
                        Quaternion deltaRotation = Quaternion.LookRotation(Data.PathProperty.Value.GetDirection(progress)) * Quaternion.Inverse(Quaternion.LookRotation(Data.PathProperty.Value.GetDirection(0)));
                        movingTransform.rotation = deltaRotation * initialRotation;
                    }
                    else
                    {
                        movingTransform.rotation = Quaternion.LookRotation(Data.PathProperty.Value.GetDirection(progress));
                    }
                }
            }

            private float GetAnimationProgress(bool isReversed)
            {
                if (isReversed)
                {
                    return Mathf.Clamp01(Data.Velocity.Evaluate((Data.Duration - (Time.time - startingTime)) / Data.Duration * Data.Velocity.keys.Last().time));
                }
                else
                {
                    return Mathf.Clamp01(Data.Velocity.Evaluate((Time.time - startingTime) / Data.Duration * Data.Velocity.keys.Last().time));
                }
            }
        }

        [JsonConstructor, Preserve]
        public FollowPathBehavior() : this(Guid.Empty, Guid.Empty, 0f, null)
        {
            Data.Duration = 1;
            Data.Velocity = AnimationCurve.EaseInOut(0, 0, 1, 1);
            Data.Repeats = 1;
        }

        public FollowPathBehavior(ISceneObject target, IPathProperty path, float duration, AnimationCurve velocity, bool keepRelativePosition = false, bool keepRelativeRotation = false, bool reverse = false, bool pingPong = false, int repeats = 1, bool disableRotation = false)
         : this(ProcessReferenceUtils.GetUniqueIdFrom(target), ProcessReferenceUtils.GetUniqueIdFrom(path), duration, velocity, keepRelativePosition, keepRelativeRotation, reverse, pingPong, repeats, disableRotation)
        {
        }

        public FollowPathBehavior(Guid targetGuid, Guid pathGuid, float duration, AnimationCurve velocity, bool keepRelativePosition = false, bool keepRelativeRotation = false, bool reverse = false, bool pingPong = false, int repeats = 1, bool disableRotation = false)
        {
            Data.TargetObject = new SingleSceneObjectReference(targetGuid);
            Data.PathProperty = new SingleScenePropertyReference<IPathProperty>(pathGuid);
            Data.Duration = duration;
            Data.Velocity = velocity;
            Data.KeepRelativePosition = keepRelativePosition;
            Data.KeepRelativeRotation = keepRelativeRotation;
            Data.Reverse = reverse;
            Data.PingPong = pingPong;
            Data.Repeats = repeats;
            Data.DisableRotation = disableRotation;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
