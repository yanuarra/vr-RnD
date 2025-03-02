using System;
using UnityEngine;
using UnityEngine.Events;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Interface for a property handling a single animation at a time.
    /// </summary>
    public interface IAnimationProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Replaces clips in the animation with the specified animation clip.
        /// </summary>
        /// <param name="clip">The animation clip to be set.</param>
        void SetAnimationClip(AnimationClip clip);

        /// <summary>
        /// True if an animation clip is referenced in the animation.
        /// </summary>
        bool HasClip { get; }

        /// <summary>
        /// True while the animation is playing.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Plays the associated animation.
        /// </summary>
        void Play();

        /// <summary>
        /// Instantly brings the animation to its end state.
        /// </summary>
        void FastForward();

        /// <summary>
        /// Raised when the animation starts.
        /// </summary>
        UnityEvent<AnimationPropertyEventArgs> AnimationStarted { get; }

        /// <summary>
        /// Raised when the animation ends.
        /// </summary>
        UnityEvent<AnimationPropertyEventArgs> AnimationEnded { get; }
    }

    /// <summary>
    /// Event args for events related to <see cref="IAnimationProperty"/>.
    /// </summary>
    public class AnimationPropertyEventArgs : EventArgs
    {
    }
}