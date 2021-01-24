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

namespace YaminabeExtensions.Core
{
    /// <summary>
    /// アクセサを示します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public interface IAccessor
    {
        #region -------------------- method --------------------

        /// <summary>
        /// プロパティの値を取得します。
        /// </summary>
        /// <param name="target">ターゲット。</param>
        /// <returns>
        /// プロパティの値を返却します。
        /// </returns>
        object GetValue(object target);

        /// <summary>
        /// プロパティに値を設定します。
        /// </summary>
        /// <param name="target">ターゲット。</param>
        /// <param name="value">値。</param>
        void SetValue(object target, object value);

        #endregion
    }
}
