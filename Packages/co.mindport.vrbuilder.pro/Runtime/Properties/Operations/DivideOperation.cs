using System;
using VRBuilder.Core.Properties.Operations;

namespace VRBuilder.Pro.Properties.Operations
{
    /// <summary>
    /// Divides left by right.
    /// </summary>
    public class DivideOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            if (rightOperand == 0)
            {
                throw new DivideByZeroException("Process data operation attempted to divide by zero.");
            }

            return leftOperand / rightOperand;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "/";
        }
    }
}