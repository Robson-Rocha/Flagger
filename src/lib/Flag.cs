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
        /// <typeparam name="T">The type of the member to be set and reset</typeparam>
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
        /// <typeparam name="T">The type of the member to be set and reset</typeparam>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<T> SetAndUnset<T>(Expression<Func<T>> member, T setValue, T unsetValue)
            => new Flag<T>((MemberExpression)member.Body, setValue, unsetValue);

        /// <summary>
        /// Sets the value of the supplied boolean member to true on context creation, and set it to false on the context disposal
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<bool> SetAndUnset(Expression<Func<bool>> member)
            => SetAndUnset<bool>(member, true, false);

        /// <summary>
        /// If the current value of the supplied member is not equal to the setValue parameter value, set it to the setValue on context creation, and set it to the unsetValue parameter value on the context disposal. Otherwise, keeps it unchanged;
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <param name="setValue">The value to be set at the context creation</param>
        /// <param name="unsetValue">Optional. The value to be set at the context disposal</param>
        /// <typeparam name="T">The type of the member to be set and reset. Must implement IEquatable&lt;T&gt;.</typeparam>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<T> SetAndUnsetIfUnset<T>(Expression<Func<T>> member, T setValue, T unsetValue = default(T))
            where T: IEquatable<T>
        {
            T currentValue = member.Compile()();
            MemberExpression memberExpression = (MemberExpression)member.Body;
            bool isNotSet = true;

            if(currentValue == null)
                isNotSet = setValue != null;                
            else
                isNotSet = !currentValue.Equals(setValue);

            return (isNotSet) ? new Flag<T>(memberExpression, setValue, unsetValue) : new Flag<T>();
        }

        /// <summary>
        /// If the current value of the supplied boolean member is false, set it to true on context creation, and set it to false on the context disposal. Otherwise, keeps it unchanged;
        /// </summary>
        /// <param name="member">Lambda expression which indicates the member to be set.</param>
        /// <returns>Flag context which sets and unsets the member value</returns>
        public static Flag<bool> SetAndUnsetIfUnset(Expression<Func<bool>> member)
            => SetAndUnsetIfUnset<bool>(member, true, false);
    }
}
