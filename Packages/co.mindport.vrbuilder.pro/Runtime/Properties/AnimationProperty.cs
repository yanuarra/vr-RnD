using UnityEngine;
using UnityEngine.Events;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property controlling an <see cref="Animation"/> component.
    /// </summary>
    [RequireComponent(typeof(Animation))]
    public class AnimationProperty : ProcessSceneObjectProperty, IAnimationProperty
    {
        private Animation animationComponent;

        /// <summary>
        /// Animation component associated with this property.
        /// </summary>
        public Animation Animation
        {
            get
            {
                if (animationComponent == null)
                {
                    animationComponent = GetComponent<Animation>();
                }
                return animationComponent;
            }
        }

        [Header("Events")]
        [SerializeField]
        private UnityEvent<AnimationPropertyEventArgs> animationStarted = new UnityEvent<AnimationPropertyEventArgs>();

        [SerializeField]
        private UnityEvent<AnimationPropertyEventArgs> animationEnded = new UnityEvent<AnimationPropertyEventArgs>();

        /// <inheritdoc/>
        public bool IsPlaying { get; private set; }

        /// <inheritdoc/>
        public bool HasClip => Animation.clip != null;

        /// <inheritdoc/>
        public UnityEvent<AnimationPropertyEventArgs> AnimationStarted => animationStarted;

        /// <inheritdoc/>
        public UnityEvent<AnimationPropertyEventArgs> AnimationEnded => animationEnded;

        /// <inheritdoc/>
        public void Play()
        {
            Animation.Play();
            EmitAnimationStarted();
            IsPlaying = true;
        }

        /// <inheritdoc/>
        public void SetAnimationClip(AnimationClip clip)
        {
            clip.legacy = true;

            if (Animation.GetClip(clip.name) != clip)
            {
                Animation.AddClip(clip, clip.name);
            }

            Animation.clip = clip;
        }

        /// <inheritdoc/>
        public void FastForward()
        {
            AnimationState state = Animation[Animation.clip.name];
            state.time = state.length;
            Animation.Sample();
            Animation.Play();
        }

        private void Update()
        {
            if (IsPlaying && Animation.isPlaying == false)
            {
                EmitAnimationEnded();
                IsPlaying = false;
            }
        }

        protected override void Reset()
        {
            Animation.playAutomatically = false;

            base.Reset();
        }

        private void EmitAnimationStarted()
        {
            animationStarted?.Invoke(new AnimationPropertyEventArgs());
        }

        private void EmitAnimationEnded()
        {
            animationEnded?.Invoke(new AnimationPropertyEventArgs());
        }
    }
}