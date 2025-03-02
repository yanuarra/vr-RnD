using UnityEngine.Events;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Abstract implementation of the generic event property.
    /// </summary>    
    public abstract class GenericEventProperty<T> : ProcessSceneObjectProperty, IGenericEventProperty<T>
    {
        /// <summary>
        /// The event that will be triggered.
        /// </summary>
        [SerializeField]        
        private UnityEvent<T> triggerableEvent = new UnityEvent<T>();

        /// <inheritdoc/>
        public void TriggerEvent(T arg)
        {
            triggerableEvent.Invoke(arg);
        }
    }
}