using System;
using System.Linq.Expressions;

namespace Flagger
{
    /// <summary>
    /// Allows for setting a value to the supplied member while the context lasts, and resetting the value on the context disposal
    /// </summary>
    /// <typeparam name="T">The type of the member to be set and reset</typeparam>
    public class Flag<T> : IDisposable
    {
        private readonly MemberExpression _member;
        private readonly ConstantExpression _setValue;
        private readonly ConstantExpression _unsetValue;

        private void AssignConstantValueToMember(ConstantExpression constant)
        {
            BinaryExpression assignment = Expression.Assign(_member, constant);
            Expression<Func<T>> lambdaExpression = Expression.Lambda<Func<T>>(assignment);
            Func<T> compiledLambda = lambdaExpression.Compile();
            T assignmentResult = compiledLambda();
        }

        /// <summary>
        /// Creates a Flag context, assigning the setValue to the supplied member, and storing the unsetValue for reseting the member on the context disposal
        /// </summary>
        /// <param name="member"><see cref="MemberExpression"/>MemberExpression</see> representing the member to be set and unset.</param>
        /// <param name="setValue">The value to be set at the context creation</param>
        /// <param name="unsetValue">The value to be set at the contex disposal</param>
        internal Flag(MemberExpression member, T setValue, T unsetValue)
        {
            _member = member;
            _setValue = Expression.Constant(setValue);
            _unsetValue = Expression.Constant(unsetValue);
            AssignConstantValueToMember(_setValue);
        }

        /// <summary>
        /// Creates an empty Flag context, wich does nothing neither at creation nor at disposal
        /// </summary>
        internal Flag()
        {
        }

        /// <summary>
        /// Disposes of the flag context, resetting the supplied member value
        /// </summary>
        public void Dispose()
        {
            if (_member != null)
                AssignConstantValueToMember(_unsetValue);
        }
    }
}
