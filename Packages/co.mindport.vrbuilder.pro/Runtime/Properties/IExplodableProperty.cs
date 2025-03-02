using System.Collections.Generic;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property for a game object that has an exploded view.
    /// </summary>
    public interface IExplodableProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Objects that will be animated.
        /// </summary>
        IEnumerable<Transform> ExplodedObjects { get; }

        /// <summary>
        /// Explodes the view by the specified ratio.
        /// </summary>
        /// <param name="scale">Positive is outwards, negative is inwards. Zero is the original state.</param>
        void Explode(float progress, float scale);

        /// <summary>
        /// Initializes the property before a new animation.
        /// </summary>
        void InitializeAnimation();
    }
}