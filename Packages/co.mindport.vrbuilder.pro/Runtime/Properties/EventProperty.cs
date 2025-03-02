using UnityEngine.Events;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    public class EventProperty : ProcessSceneObjectProperty, IEventProperty
    {
        /// <summary>
        /// The event that will be triggered.
        /// </summary>
        [SerializeField]
        private UnityEvent triggerableEvent = new UnityEvent();

        /// <inheritdoc/>
        public void TriggerEvent()
        {
            triggerableEvent.Invoke();
        }
    }
}