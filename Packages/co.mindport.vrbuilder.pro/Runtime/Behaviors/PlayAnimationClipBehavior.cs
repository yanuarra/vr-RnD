using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Exceptions;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Plays an animation clip and ends when the clip is over.
    /// </summary>
    public class PlayAnimationClipBehavior : Behavior<PlayAnimationClipBehavior.EntityData>
    {
        /// <summary>
        /// Entity data for <see cref="PlayAnimationClipBehavior"/>.
        /// </summary>
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Animation clip resource.
            /// </summary>
            [DataMember]
            [UsesSpecificProcessDrawer("AnimationClipResourceDrawer")]
            [DisplayName("Animation clip")]
            public string AnimationClipPath { get; set; }

            /// <summary>
            /// Object to be animated by the clip.
            /// </summary>
            [DataMember]
            [DisplayName("Object to animate")]
            public SingleScenePropertyReference<IAnimationProperty> AnimationProperty { get; set; }

            [DataMember]
            public Metadata Metadata { get; set; }

            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    return $"Animate {AnimationProperty}";
                }
            }
        }

        public class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            public override void End()
            {
            }

            public override void FastForward()
            {
                if (Data.AnimationProperty.Value.IsPlaying == false)
                {
                    Data.AnimationProperty.Value.Play();
                }

                Data.AnimationProperty.Value.FastForward();
            }

            public override void Start()
            {
                AnimationClip clip = Resources.Load<AnimationClip>(Data.AnimationClipPath);

                if (clip != null)
                {
                    Data.AnimationProperty.Value.SetAnimationClip(clip);
                }

                if (Data.AnimationProperty.Value.HasClip)
                {
                    Data.AnimationProperty.Value.Play();
                }
                else
                {
                    throw new ProcessException($"The {Data.AnimationProperty.Value.GetType().Name} on '{Data.AnimationProperty.Value.SceneObject.GameObject.name}' has no valid animation clip.");
                }
            }

            public override IEnumerator Update()
            {
                while (Data.AnimationProperty.Value.IsPlaying)
                {
                    yield return null;
                }
            }
        }

        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        [JsonConstructor]
        public PlayAnimationClipBehavior() : this("", Guid.Empty)
        {
        }

        public PlayAnimationClipBehavior(string clipPath, Guid objectId)
        {
            Data.AnimationClipPath = clipPath;
            Data.AnimationProperty = new SingleScenePropertyReference<IAnimationProperty>(objectId);
        }
    }
}