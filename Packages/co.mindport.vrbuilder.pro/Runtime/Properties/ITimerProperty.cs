using System;
using UnityEngine.Events;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property that acts as a timer.
    /// </summary>
    public interface ITimerProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Raised when the timer starts.
        /// </summary>
        UnityEvent<TimerPropertyEventArgs> TimerStart { get; }

        /// <summary>
        /// Raised when the timer is paused.
        /// </summary>
        UnityEvent<TimerPropertyEventArgs> TimerStop { get; }

        /// <summary>
        /// Raised when a timer reaches zero.
        /// </summary>
        UnityEvent<TimerPropertyEventArgs> TimerZero { get; }

        /// <summary>
        /// If true, the timer will count down instead of up.
        /// </summary>
        bool IsCountdown { get; set; }

        /// <summary>
        /// True when the timer is active.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void StartTimer();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void StopTimer();
    }

    /// <summary>
    /// Event args for <see cref="TimerProperty"/>.
    /// </summary>
    public class TimerPropertyEventArgs : EventArgs
    {
    }
}