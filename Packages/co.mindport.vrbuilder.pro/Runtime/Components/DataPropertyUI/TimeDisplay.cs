using System;
using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Components.DataPropertyUI
{
    /// <summary>
    /// Displays a <see cref="NumberDataProperty"/> representing a value in seconds as time on a text mesh.
    /// </summary>
    public class TimeDisplay : DataPropertyDisplay<float>
    {
        [SerializeField]
        [Tooltip("Associated data property. If empty, it will attempt to find a component on the same game object.")]
        private NumberDataProperty dataProperty;

        /// <inheritdoc/>
        public override IDataProperty<float> DataProperty
        {
            get
            {
                if (dataProperty == null)
                {
                    dataProperty = GetComponent<NumberDataProperty>();
                }

                return dataProperty;
            }
        }

        /// <inheritdoc/>
        protected override void UpdateText()
        {
            textMesh.text = string.Format(text, new TimeSpan(0, 0, 0, 0, (int)(DataProperty.GetValue() * 1000)), DataProperty.SceneObject.GameObject.name);
        }
    }
}