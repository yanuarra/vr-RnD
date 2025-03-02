using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Event property which supports a single argument.
    /// </summary>
    public interface IGenericEventProperty<T> : ISceneObjectProperty
    {
        /// <summary>
        /// Triggers the associated event.
        /// </summary>
        void TriggerEvent(T arg);
    }
}