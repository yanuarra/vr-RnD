using System;
using UnityEngine;
using VRBuilder.Core.Properties.Operations;

namespace VRBuilder.Pro.Properties.Operations
{
    /// <summary>
    /// Multiplies left by right.
    /// </summary>
    public class MaxOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            return Mathf.Max(leftOperand, rightOperand);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "MAX";
        }
    }
}