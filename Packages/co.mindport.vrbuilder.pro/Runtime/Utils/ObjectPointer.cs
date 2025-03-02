using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRBuilder.Pro.Utils
{
    /// <summary>
    /// Component that can track another object and point at it in some way.
    /// </summary>
    public abstract class ObjectPointer : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent<ObjectPointerEventArgs> enteredThreshold;

        [SerializeField]
        private UnityEvent<ObjectPointerEventArgs> exitedThreshold;

        /// <summary>
        /// True if the pointer is within a specified threshold relative to the target object.
        /// </summary>
        public bool IsWithinThreshold { get; protected set; }

        /// <summary>
        /// Called when the pointer moves closer to the target object within a specified threshold.
        /// </summary>
        public UnityEvent<ObjectPointerEventArgs> EnteredThreshold => enteredThreshold;

        /// <summary>
        /// Called when the pointer moves away from the target object and exits a specified threshold.
        /// </summary>
        public UnityEvent<ObjectPointerEventArgs> ExitedThreshold => exitedThreshold;

        /// <summary>
        /// Show the pointer and start tracking the target object.
        /// </summary>
        /// <param name="target">Transform to track.</param>
        public abstract void StartTracking(Transform target);

        /// <summary>
        /// Hide the pointer and stop tracking the target object.
        /// </summary>
        public abstract void StopTracking();
    }

    /// <summary>
    /// Event args for <see cref="ObjectPointer"/> events.
    /// </summary>
    public class ObjectPointerEventArgs : EventArgs
    {
    }
}