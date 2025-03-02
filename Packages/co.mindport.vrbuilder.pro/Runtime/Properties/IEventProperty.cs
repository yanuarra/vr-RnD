using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Base event property with no payload.
    /// </summary>
    public interface IEventProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Triggers the associated event.
        /// </summary>
        void TriggerEvent();
    }
}