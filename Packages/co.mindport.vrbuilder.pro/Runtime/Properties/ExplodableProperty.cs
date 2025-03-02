using System.Collections.Generic;
using UnityEngine;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property for a game object that has an exploded view.
    /// </summary>
    public class ExplodableProperty : ProcessSceneObjectProperty, IExplodableProperty
    {
        private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();
        private Dictionary<Transform, Vector3> animationStartPositions = new Dictionary<Transform, Vector3>();
        private bool isInitialized = false;

        [SerializeField]
        [Tooltip("List of the children that will be exploded. If empty, all first-level children will be automatically chosen.")]
        private List<Transform> explodableChildObjects = new List<Transform>();

        /// <inheritdoc/>
        public IEnumerable<Transform> ExplodedObjects
        {
            get
            {
                if (isInitialized == false)
                {
                    InitializeChildren();
                }

                return initialPositions.Keys;
            }
        }

        private void Start()
        {
            if (isInitialized == false)
            {
                InitializeChildren();
            }
        }

        private void InitializeChildren()
        {
            foreach (Transform child in explodableChildObjects)
            {
                if (child == null)
                {
                    Debug.LogWarning($"Null value on <b>{typeof(ExplodableProperty).Name}</b> on <i>'{gameObject.name}'</i>.");
                    continue;
                }
                else if (child.IsChildOf(transform))
                {
                    initialPositions.Add(child, child.localPosition);
                }
                else
                {
                    Debug.LogWarning($"<b>{typeof(ExplodableProperty).Name}</b> on <i>'{gameObject.name}'</i> wants to explode object <i>'{child.name}'</i>, but it is not its child.");
                }
            }

            if (initialPositions.Count == 0)
            {
                foreach (Transform child in transform)
                {
                    initialPositions.Add(child, child.localPosition);
                }
            }

            isInitialized = true;
        }

        /// <inheritdoc/>
        public void Explode(float progress, float scale)
        {
            if (initialPositions.Count == 0)
            {
                Debug.LogWarningFormat($"The object {SceneObject.GameObject.name} does not have any children, so it cannot be exploded.");
                return;
            }

            RuntimeConfigurator.Configuration.SceneObjectManager.RequestAuthority(SceneObject);

            foreach (Transform child in animationStartPositions.Keys)
            {
                child.localPosition = Vector3.Lerp(animationStartPositions[child], initialPositions[child] * scale, progress);
            }
        }

        /// <inheritdoc/>        
        public void InitializeAnimation()
        {
            animationStartPositions.Clear();

            foreach (Transform child in initialPositions.Keys)
            {
                animationStartPositions.Add(child, child.localPosition);
            }
        }
    }
}