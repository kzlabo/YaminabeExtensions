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
    /// 文字列操作拡張メソッドを提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/08" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public static class StringExtensions
    {
        #region -------------------- method --------------------

        /// <summary>
        /// 文字列の末尾が指定した文字列で終了するかどうか判断し、終了していない場合は末尾に追加します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="end">比較する文字列。</param>
        /// <returns>
        /// 文字列の末尾が指定した文字列で終了するように変換した文字列を返却します。
        /// </returns>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         // パスの末尾が区切り文字で終わっているパターンと終わっていないパターン
        ///         var path1 = @"c:\test";
        ///         var path2 = @"c:\test\";
        ///         
        ///         var filePath1 = $"{path1.EndsWithAdd(@"\")}dummy.txt";
        ///         var filePath2 = $"{path2.EndsWithAdd(@"\")}dummy.txt";
        ///         
        ///         // filePath1: c:\test\dummy.txt
        ///         // filePath2: c:\test\dummy.txt
        ///     </code>
        /// </example>
        public static string EndsWithAdd(this string value, string end)
        {
            if(value == null || value == string.Empty)
            {
                return value;
            }
            return value.EndsWith(end, System.StringComparison.Ordinal) ? value : $"{value}{end}";
        }

        #region -------------------- Path --------------------

        /// <summary>
        /// <see cref="System.IO.Path.GetDirectoryName(string)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// ファイルパスのディレクトリパスを返却します。
        /// </returns>
        public static string GetPathDirectoryName(this string value)
        {
            return System.IO.Path.GetDirectoryName(value);
        }

        /// <summary>
        /// <see cref="System.IO.Path.GetExtension(string)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// ファイルパスの拡張子を返却します。
        /// </returns>
        public static string GetPathExtension(this string value)
        {
            return System.IO.Path.GetExtension(value);
        }

        /// <summary>
        /// <see cref="System.IO.Path.GetFileName(string)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// ファイルパスのファイル名を返却します。
        /// </returns>
        public static string GetPathFileName(this string value)
        {
            return System.IO.Path.GetFileName(value);
        }

        /// <summary>
        /// <see cref="System.IO.Path.GetFileNameWithoutExtension(string)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// ファイルパスのファイル名（拡張子なし）を返却します。
        /// </returns>
        public static string GetPathFileNameWithoutExtension(this string value)
        {
            return System.IO.Path.GetFileNameWithoutExtension(value);
        }

        #endregion

        #region -------------------- Regex --------------------

        /// <summary>
        /// <see cref="System.Text.RegularExpressions.Regex.Replace(string, string, System.Text.RegularExpressions.MatchEvaluator, System.Text.RegularExpressions.RegexOptions, TimeSpan)" /> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="evaluator">置換カスタムメソッド。</param>
        /// <param name="options">一致オプション。</param>
        /// <param name="matchTimeout">タイムアウト期間。</param>
        /// <returns>
        /// 置換後の文字列を返却します。
        /// </returns>
        public static string RegReplace(this string value, string pattern, System.Text.RegularExpressions.MatchEvaluator evaluator, System.Text.RegularExpressions.RegexOptions options, TimeSpan matchTimeout)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, evaluator, options, matchTimeout);
        }

        /// <summary>
        /// <see cref="System.Text.RegularExpressions.Regex.Replace(string, string, string, System.Text.RegularExpressions.RegexOptions, TimeSpan)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="replacement">置換文字列。</param>
        /// <param name="options">一致オプション。</param>
        /// <param name="matchTimeout">タイムアウト期間。</param>
        /// <returns>
        /// 置換後の文字列を返却します。
        /// </returns>
        public static string RegReplace(this string value, string pattern, string replacement, System.Text.RegularExpressions.RegexOptions options, TimeSpan matchTimeout)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, replacement, options, matchTimeout);
        }

        /// <summary>
        /// <see cref="System.Text.RegularExpressions.Regex.Replace(string, string, System.Text.RegularExpressions.MatchEvaluator, System.Text.RegularExpressions.RegexOptions)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="evaluator">置換カスタムメソッド。</param>
        /// <param name="options">一致オプション。</param>
        /// <returns>
        /// 置換後の文字列を返却します。
        /// </returns>
        public static string RegReplace(this string value, string pattern, System.Text.RegularExpressions.MatchEvaluator evaluator, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, evaluator, options);
        }

        /// <summary>
        /// <see cref="System.Text.RegularExpressions.Regex.Replace(string, string, string, System.Text.RegularExpressions.RegexOptions)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="replacement">置換文字列。</param>
        /// <param name="options">一致オプション。</param>
        /// <returns>
        /// 置換後の文字列を返却します。
        /// </returns>
        public static string RegReplace(this string value, string pattern, string replacement, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, replacement, options);
        }

        /// <summary>
        /// <see cref="System.Text.RegularExpressions.Regex.Replace(string, string, System.Text.RegularExpressions.MatchEvaluator)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="evaluator">置換カスタムメソッド。</param>
        /// <returns>
        /// 置換後の文字列を返却します。
        /// </returns>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         // マッチした文字列に対してカスタムメソッドで置換
        ///         var s1 = "abcABCabc";
        ///         var s2 = s1.RegReplace("[a-z]", m => { return $"{m.Value}{m.Value}"; };
        ///         
        ///         // s2: aabbccABCaabbcc
        ///     </code>
        /// </example>
        public static string RegReplace(this string value, string pattern, System.Text.RegularExpressions.MatchEvaluator evaluator)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, evaluator);
        }

        /// <summary>
        /// <see cref="System.Text.RegularExpressions.Regex.Replace(string, string, string)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="pattern">正規表現パターン。</param>
        /// <param name="replacement">置換文字列。</param>
        /// <returns>
        /// 置換後の文字列を返却します。
        /// </returns>
        public static string RegReplace(this string value, string pattern, string replacement)
        {
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, replacement);
        }

        #endregion

        #region -------------------- Convert --------------------

        /// <summary>
        /// 文字列を指定した型に変換します。
        /// </summary>
        /// <typeparam name="T">型指定。</typeparam>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// 変換した値を返却します。
        /// </returns>
        /// <exception cref="NotSupportedException">未対応。</exception>
        public static T Convert<T>(this string value)
        {
            if(typeof(T) == typeof(int))
            {
                return (T)(object)ConvertToInt(value);
            }

            if(typeof(T) == typeof(DateTime))
            {
                return (T)(object)ConvertToDateTime(value);
            }

            throw new NotSupportedException(); 
        }

        /// <summary>
        /// 文字列を <see cref="int"/> 型に変換します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="def">初期値。</param>
        /// <returns>
        /// <see cref="int"/> 型に変換した値を返却します。
        /// </returns>
        /// <remarks>
        /// 変換エラーの場合は def を返却します。
        /// </remarks>
        public static int ConvertToInt(this string value, int def = 0)
        {
            try
            {
                return int.Parse(value);
            }
            catch(OverflowException)
            {
                throw;
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// 文字列を <see cref="DateTime"/> 型に変換します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="def">初期値。</param>
        /// <returns>
        /// <see cref="DateTime"/> 型に変換した値を返却します。
        /// </returns>
        /// <remarks>
        /// 変換エラーの場合は def を返却します。
        /// </remarks>
        public static DateTime ConvertToDateTime(this string value, DateTime? def = null)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch
            {
                return (DateTime)(def == null ? DateTime.MinValue : def);
            }
        }

        #endregion

        #endregion
    }
}
