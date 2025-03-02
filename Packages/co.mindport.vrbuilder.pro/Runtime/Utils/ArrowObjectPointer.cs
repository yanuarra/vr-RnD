using UnityEngine;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Pro.Utils
{
    /// <summary>
    /// <see cref="ObjectPointer"/> that displays an arrow hovering the floor near the user's feet.
    /// </summary>
    public class ArrowObjectPointer : ObjectPointer
    {
        private UserSceneObject localUser;

        [Header("Parameters")]
        [SerializeField]
        [Tooltip("Height of the arrow from the floor")]
        private float floorHeight = .2f;

        [SerializeField]
        [Tooltip("Distance from the target object below which the arrow disappears.")]
        private float minimumDistance = 1f;

        private Transform targetObject = null;
        private bool isTracking = false;
        private MeshRenderer visualization;

        /// <inheritdoc/>
        public override void StartTracking(Transform target)
        {
            localUser = RuntimeConfigurator.Configuration.LocalUser;
            targetObject = target;
            isTracking = true;
        }

        /// <inheritdoc/>
        public override void StopTracking()
        {
            isTracking = false;
        }

        private void OnEnable()
        {
            visualization = GetComponentInChildren<MeshRenderer>();
            EnteredThreshold.AddListener(OnEnteredThreshold);
            ExitedThreshold.AddListener(OnExitedThreshold);
        }

        private void OnEnteredThreshold(ObjectPointerEventArgs arg0)
        {
            visualization.enabled = false;
        }

        private void OnExitedThreshold(ObjectPointerEventArgs arg0)
        {
            visualization.enabled = true;
        }

        private void Update()
        {
            if (isTracking == false)
            {
                return;
            }

            transform.position = new Vector3(localUser.Head.position.x, localUser.Base.position.y + floorHeight, localUser.Head.position.z);
            Vector3 targetPosition = new Vector3(targetObject.transform.position.x, transform.position.y, targetObject.transform.position.z);
            transform.LookAt(targetPosition, Vector3.up);

            if (Vector3.Distance(transform.position, targetPosition) < minimumDistance != IsWithinThreshold)
            {
                IsWithinThreshold = !IsWithinThreshold;
                if (IsWithinThreshold)
                {
                    EnteredThreshold?.Invoke(new ObjectPointerEventArgs());
                }
                else
                {
                    ExitedThreshold?.Invoke(new ObjectPointerEventArgs());
                }
            }
        }
    }
}