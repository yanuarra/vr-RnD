using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Components.DataPropertyUI
{
    /// <summary>
    /// Displays a <see cref="TextDataProperty"/> on a text mesh.
    /// </summary>
    public class TextDisplay : DataPropertyDisplay<string>
    {
        [SerializeField]
        [Tooltip("Associated data property. If empty, it will attempt to find a component on the same game object.")]
        private TextDataProperty dataProperty;

        /// <inheritdoc/>
        public override IDataProperty<string> DataProperty
        {
            get
            {
                if (dataProperty == null)
                {
                    dataProperty = GetComponent<TextDataProperty>();
                }

                return dataProperty;
            }
        }
    }
}