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

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Rotates an object around an axis, which can be represented by another transform.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/animations-add-on")]
    public class RotateAroundAxisBehavior : Behavior<RotateAroundAxisBehavior.EntityData>
    {
        public enum RotationAxis
        {
            X, Y, Z
        }

        /// <summary>
        /// The "follow path" behavior's data.
        /// </summary>
        [DisplayName("Rotate Around Axis")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Target scene object to be rotated.
            /// </summary>
            [DataMember]
            [DisplayName("Object")]
            public SingleSceneObjectReference TargetObject { get; set; }

            /// <summary>
            /// Transform representing the axis. If empty, the object's position will be used.
            /// </summary>
            [DataMember]
            [DisplayName("Rotation axis provider")]
            public SingleSceneObjectReference AxisProvider { get; set; }

            /// <summary>
            /// Choose whether the object will be rotated along the X, Y or Z axis.
            /// </summary>
            [DataMember]
            [DisplayName("Rotation axis")]
            public RotationAxis RotationAxis { get; set; }

            /// <summary>
            /// Total duration of the animation.
            /// </summary>
            [DataMember]
            [DisplayName("Duration (in seconds)")]
            public float Duration { get; set; }

            /// <summary>
            /// Amount of degrees the object will be rotated.
            /// </summary>
            [DataMember]
            [DisplayName("Degrees")]
            public float Degrees { get; set; }

            /// <summary>
            /// Determines the position of the object at a given time. The curve is normalized, the duration of the animation can be set in the <see cref="Duration"/> field.
            /// </summary>
            [DataMember]
            [DisplayName("Animation curve")]
            public AnimationCurve AnimationCurve { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string axisTransform = AxisProvider.HasValue() ? AxisProvider.ToString() : "itself";
                    return $"Rotate {TargetObject} on the {RotationAxis} axis of {axisTransform}";
                }
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private float startingTime;
            private Quaternion initialLocalRotation;
            private Vector3 initialLocalPosition;
            private Vector3 axis;
            private Transform movingTransform;
            private Transform axisTransform;

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                RuntimeConfigurator.Configuration.SceneObjectManager.RequestAuthority(Data.TargetObject.Value);

                startingTime = Time.time;
                initialLocalRotation = Data.TargetObject.Value.GameObject.transform.localRotation;
                initialLocalPosition = Data.TargetObject.Value.GameObject.transform.localPosition;

                switch (Data.RotationAxis)
                {
                    case RotationAxis.X:
                        axis = Vector3.right;
                        break;
                    case RotationAxis.Y:
                        axis = Vector3.up;
                        break;
                    case RotationAxis.Z:
                        axis = Vector3.forward;
                        break;
                }

                movingTransform = Data.TargetObject.Value.GameObject.transform;
                axisTransform = Data.AxisProvider.IsEmpty() ? Data.TargetObject.Value.GameObject.transform : Data.AxisProvider.Value.GameObject.transform;

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
                while (Time.time - startingTime < Data.Duration)
                {
                    ProcessAnimation((Time.time - startingTime) / Data.Duration * Data.AnimationCurve.keys.Last().time);
                    yield return null;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                ProcessAnimation(Data.AnimationCurve.keys.Last().time);

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

            private void ProcessAnimation(float progress)
            {
                RuntimeConfigurator.Configuration.SceneObjectManager.RequestAuthority(Data.TargetObject.Value);

                movingTransform.localPosition = initialLocalPosition;
                movingTransform.localRotation = initialLocalRotation;

                Vector3 rotationAxis = axisTransform.rotation * axis;
                float currentRotation = Data.Degrees * Data.AnimationCurve.Evaluate(progress);

                movingTransform.RotateAround(axisTransform.position, rotationAxis, currentRotation);
            }
        }

        [JsonConstructor, Preserve]
        public RotateAroundAxisBehavior() : this(Guid.Empty, Guid.Empty, RotationAxis.X, 0f, 0f, null)
        {
            Data.Duration = 1;
            Data.AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public RotateAroundAxisBehavior(ISceneObject target, ISceneObject axisProvider, RotationAxis rotationAxis, float duration, float degrees, AnimationCurve animationCurve) : this(ProcessReferenceUtils.GetUniqueIdFrom(target), ProcessReferenceUtils.GetUniqueIdFrom(axisProvider), rotationAxis, duration, degrees, animationCurve)
        {
        }

        public RotateAroundAxisBehavior(Guid targetGuid, Guid axisProviderGuid, RotationAxis rotationAxis, float duration, float degrees, AnimationCurve animationCurve)
        {
            Data.TargetObject = new SingleSceneObjectReference(targetGuid);
            Data.AxisProvider = new SingleSceneObjectReference(axisProviderGuid);
            Data.Duration = duration;
            Data.AnimationCurve = animationCurve;
            Data.Degrees = degrees;
            Data.RotationAxis = rotationAxis;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
