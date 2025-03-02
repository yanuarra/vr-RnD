using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Demo
{
    /// <summary>
    /// Handles two buttons to increase and decrease the pressure while they are being touched.
    /// </summary>
    public class PressureControl : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer increaseButton, decreaseButton;

        [SerializeField]
        private float increaseSpeed = 1f;

        private ISnappableProperty snappableProperty;

        private NumberDataProperty pressureData;

        private bool isTouchingIncrease;
        private bool isTouchingDecrease;

        public event EventHandler PressureChanged;

        private void Start()
        {
            snappableProperty = GetComponent<ISnappableProperty>();

            snappableProperty.AttachedToSnapZone.AddListener(OnObjectSnapped);
            snappableProperty.DetachedFromSnapZone.AddListener(OnObjectUnsnapped);
        }

        public void OnIncreaseHovered(HoverEnterEventArgs args)
        {
            OnIncreaseTouched(args.interactorObject);
        }

        public void OnIncreaseUnhovered(HoverExitEventArgs args)
        {
            OnIncreaseUntouched(args.interactorObject);
        }

        public void OnDecreaseHovered(HoverEnterEventArgs args)
        {
            OnDecreaseTouched(args.interactorObject);
        }

        public void OnDecreaseUnhovered(HoverExitEventArgs args)
        {
            OnDecreaseUntouched(args.interactorObject);
        }

        private void OnIncreaseTouched(IXRHoverInteractor interactor)
        {
            if (interactor is XRSocketInteractor)
            {
                return;
            }

            if (isTouchingDecrease)
            {
                OnDecreaseUntouched(interactor);
            }

            isTouchingIncrease = true;
            increaseButton.material.color = pressureData == null ? Color.red : Color.green;
        }

        private void OnDecreaseTouched(IXRHoverInteractor interactor)
        {
            if (interactor is XRSocketInteractor)
            {
                return;
            }

            if (isTouchingIncrease)
            {
                OnIncreaseUntouched(interactor);
            }

            isTouchingDecrease = true;
            decreaseButton.material.color = pressureData == null ? Color.red : Color.green;
        }

        private void OnIncreaseUntouched(IXRHoverInteractor interactor)
        {
            isTouchingIncrease = false;
            increaseButton.material.color = Color.white;
        }

        private void OnDecreaseUntouched(IXRHoverInteractor interactor)
        {
            isTouchingDecrease = false;
            decreaseButton.material.color = Color.white;
        }

        private void OnObjectSnapped(SnappablePropertyEventArgs args)
        {
            pressureData = snappableProperty.SnappedZone.SceneObject.GameObject.GetComponent<NumberDataProperty>();
        }

        private void OnObjectUnsnapped(SnappablePropertyEventArgs args)
        {
            pressureData = null;
        }

        private void Update()
        {
            if (pressureData == null)
            {
                return;
            }

            if (isTouchingIncrease && !isTouchingDecrease)
            {
                pressureData.SetValue(pressureData.GetValue() + increaseSpeed * Time.deltaTime);
                PressureChanged?.Invoke(this, EventArgs.Empty);
            }

            if (!isTouchingIncrease && isTouchingDecrease)
            {
                pressureData.SetValue(pressureData.GetValue() - increaseSpeed * Time.deltaTime);
                PressureChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}