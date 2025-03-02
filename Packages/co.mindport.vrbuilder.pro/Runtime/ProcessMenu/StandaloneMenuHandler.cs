using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using VRBuilder.Core;
using VRBuilder.Core.Configuration;

namespace VRBuilder.Pro.ProcessMenu
{
    /// <summary>
    /// Shows and hides the StandaloneProcessController prefab.
    /// </summary>
    public class StandaloneMenuHandler : MonoBehaviour
    {
        [Tooltip("Initial distance between this controller and the user.")]
        [SerializeField]
        protected float appearanceDistance = 2f;

        [SerializeField]
        protected bool menuHiddenOnStart = true;

        [SerializeField]
        protected InputAction toggleMenuAction;

        private Canvas canvas;
        private Transform user;
        private readonly List<TMP_Dropdown> dropdownsList = new List<TMP_Dropdown>();

        private void Start()
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = false;
            }

            ProcessRunner.Events.ProcessInitialized += SetupMenu;
        }

        private void SetupMenu(object sender, ProcessEventArgs e)
        {
            try
            {
                user = RuntimeConfigurator.Configuration.LocalUser.Head;
                canvas = GetComponentInChildren<Canvas>();
                canvas.worldCamera = Camera.main;

                canvas.enabled = ProcessRunner.Current != null && menuHiddenOnStart == false;
            }
            catch (Exception exception)
            {
                Debug.LogError($"{exception.GetType().Name} while initializing {GetType().Name}.\n{exception.StackTrace}", gameObject);
            }

            Vector3 position = user.position + (user.forward * appearanceDistance);
            Quaternion rotation = new Quaternion(0.0f, user.rotation.y, 0.0f, user.rotation.w);
            position.y = 1f;

            transform.SetPositionAndRotation(position, rotation);
            dropdownsList.AddRange(GetComponentsInChildren<TMP_Dropdown>());

            toggleMenuAction.performed += (context) => ToggleProcessControllerMenu();
        }

        private void OnEnable()
        {
            toggleMenuAction.Enable();
        }

        private void OnDisable()
        {
            toggleMenuAction.Disable();
        }

        private void ToggleProcessControllerMenu()
        {
            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
            {
                Vector3 position = user.position + (user.forward * appearanceDistance);
                Quaternion rotation = new Quaternion(0.0f, user.rotation.y, 0.0f, user.rotation.w);
                position.y = user.position.y;

                transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                HideDropdowns();
            }
        }

        private void HideDropdowns()
        {
            foreach (TMP_Dropdown dropdown in dropdownsList)
            {
                if (dropdown.IsExpanded)
                {
                    dropdown.Hide();
                }
            }
        }
    }
}
