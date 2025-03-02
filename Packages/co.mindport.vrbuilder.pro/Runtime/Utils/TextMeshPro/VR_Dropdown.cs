using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRBuilder.Pro.Utils.TextMeshPro
{
    /// <summary>
    /// Extension of the TextMeshPro Dropdown that prevents the dropdown canvas from
    /// being drawn in front of everything.
    /// </summary>
    public class VR_Dropdown : TMP_Dropdown
    {
        /// <inheritdoc/>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            UndoCanvasOverrideSorting();
        }

        /// <inheritdoc/>
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            UndoCanvasOverrideSorting();
        }

        /// <inheritdoc/>
        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);
            UndoCanvasOverrideSorting();
        }

        private void UndoCanvasOverrideSorting()
        {
            foreach (Canvas canvas in GetComponentsInChildren<Canvas>())
            {
                canvas.overrideSorting = false;
            }
        }
    }
}