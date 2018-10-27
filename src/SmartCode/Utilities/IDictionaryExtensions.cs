using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    public static class IDictionaryExtensions
    {
        public static bool Value<TKey, TValue, TTypedValue>(this IDictionary<TKey, TValue> dic, TKey key, out TTypedValue value)
        {
            value = default;
            if (!dic.TryGetValue(key, out TValue val))
            {
                return false;
            }

            object objVal = val;
            var valType = typeof(TValue);
            var typedValType = typeof(TTypedValue);
            if (typeof(IConvertible).IsAssignableFrom(typedValType))
            {
                value = (TTypedValue)Convert.ChangeType(objVal, typedValType);
            }
            else
            {
                value = (TTypedValue)objVal;
            }
            return true;
        }
    }
}
