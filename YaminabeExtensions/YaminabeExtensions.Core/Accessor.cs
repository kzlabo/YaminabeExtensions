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

namespace YaminabeExtensions.Core
{
    /// <summary>
    /// アクセサを提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public class Accessor<TTarget, TProperty> : IAccessor
    {
        #region -------------------- field --------------------

        protected readonly Func<TTarget, TProperty> _getter;
        protected readonly Action<TTarget, TProperty> _setter;

        #endregion

        #region -------------------- constructor --------------------

        /// <summary>
        /// <see cref="Accessor"/> クラスの新しいインスタンスを作成します。
        /// </summary>
        /// <param name="getter">セッター。</param>
        /// <param name="setter">ゲッター。</param>
        public Accessor(
            Func<TTarget, TProperty> getter,
            Action<TTarget, TProperty> setter
            )
        {
            this._getter = getter;
            this._setter = setter;
        }

        #endregion

        #region -------------------- method --------------------

        /// <inheritdoc/>
        public object GetValue(object target)
        {
            return this._getter((TTarget)target);
        }

        /// <inheritdoc/>
        public void SetValue(object target, object value)
        {
            this._setter((TTarget)target, (TProperty)value);
        }

        #endregion
    }
}
