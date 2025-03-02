using System;
using VRBuilder.Core.Properties.Operations;

namespace VRBuilder.Pro.Properties.Operations
{
    /// <summary>
    /// Sums left and right.
    /// </summary>
    public class SumOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            return leftOperand + rightOperand;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "+";
        }
    }
}