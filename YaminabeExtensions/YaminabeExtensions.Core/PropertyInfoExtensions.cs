/*
 * Copyright 2020 kzlabo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Reflection;

namespace YaminabeExtensions.Core
{
    /// <summary>
    /// プロパティ情報の拡張メソッドを提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public static class PropertyInfoExtensions
    {
        #region -------------------- method --------------------

        /// <summary>
        /// プロパティ情報をアクセサに変換を行います。
        /// </summary>
        /// <param name="property">プロパティ情報。</param>
        /// <returns>
        /// アクセサを返却します。
        /// </returns>
        public static IAccessor ToAccessor(this PropertyInfo property)
        {
            Type getterDelegateType = typeof(Func<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            Delegate getter = Delegate.CreateDelegate(getterDelegateType, property.GetGetMethod());

            Type setterDelegateType = typeof(Action<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            Delegate setter = Delegate.CreateDelegate(setterDelegateType, property.GetSetMethod());

            Type accessorType = typeof(Accessor<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            IAccessor accessor = (IAccessor)Activator.CreateInstance(accessorType, getter, setter);

            return accessor;
        }

        #endregion
    }
}
