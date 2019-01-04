using System;
using System.Linq.Expressions;

namespace Flagger
{
    /// <summary>
    /// Contains several methods which allows to configure contextual flags
    /// </summary>
    public static class Flag
    {
        /// <summary>
        /// Toggles (inverts) the value of the supplied boolean member
        /// </summary>
        /// <param name="member">Lambda expression which indicates the boolean member to be toggled.</param>
        /// <returns>Flag context which toggles the boolean member</returns>
        public static Flag<bool> Toggle(Expression<Func<bool>> member)
        {
            bool value = member.Compile()();
            return new Flag<bool>((MemberExpression)member.Body, !value, value);
        }

        /// <summary>
        /// Sets the value of the supplied boolean member to the provided value, and reset it to its original value on the context disposal
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <param name="setValue">The value to be set at the context creation</param>
        /// <returns>Flag context which sets and resets the member value</returns>
        public static Flag<T> SetAndReset<T>(Expression<Func<T>> member, T setValue)
        {
            T resetValue = member.Compile()();
            return new Flag<T>((MemberExpression)member.Body, setValue, resetValue);
        }

        /// <summary>
        /// Sets the value of the supplied member to the provided set value, and set it to the provided unset value on the context disposal
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <param name="setValue">The value to be set at the context creation</param>
        /// <param name="unsetValue">The value to be set at the contex disposal</param>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<T> SetAndUnset<T>(Expression<Func<T>> member, T setValue, T unsetValue)
        {
            return new Flag<T>((MemberExpression)member.Body, setValue, unsetValue);
        }

        /// <summary>
        /// Sets the value of the supplied boolean member to true on context creation, and set it to false on the context disposal
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<bool> SetAndUnset(Expression<Func<bool>> member)
        {
            return new Flag<bool>((MemberExpression)member.Body, true, false);
        }

        /// <summary>
        /// If the current value of the supplied boolean member is false, set it to true on context creation, and set it to false on the context disposal. Otherwise, keeps it unchanged;
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<bool> SetAndUnsetIfUnset(Expression<Func<bool>> member)
        {
            bool currentValue = member.Compile()();
            MemberExpression memberExpression = (MemberExpression)member.Body;
            return (currentValue == false) ? new Flag<bool>(memberExpression, true, false) : new Flag<bool>(memberExpression, currentValue, currentValue);
        }
    }
}
