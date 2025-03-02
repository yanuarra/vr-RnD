using System;
using VRBuilder.Core.Properties.Operations;

namespace VRBuilder.Pro.Properties.Operations
{
    /// <summary>
    /// Multiplies left by right.
    /// </summary>
    public class MultiplyOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            return leftOperand * rightOperand;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "*";
        }
    }
}