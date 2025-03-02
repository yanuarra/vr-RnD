using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Components.DataPropertyUI
{
    /// <summary>
    /// Displays a <see cref="NumberDataProperty"/> on a text mesh.
    /// </summary>
    public class NumberDisplay : DataPropertyDisplay<float>
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
    }
}