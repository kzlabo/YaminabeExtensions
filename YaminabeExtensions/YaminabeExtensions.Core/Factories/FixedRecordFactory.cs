﻿/*
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
using System.Collections.Generic;
using System.Linq;
using YaminabeExtensions.Core.Attributes;

namespace YaminabeExtensions.Core.Factories
{
    /// <summary>
    /// 固定長レコードモデルの生成処理を提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2021/01/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public static class FixedRecordFactory
    {
        #region -------------------- field --------------------

        private static Dictionary<string, IAccessor> _accessors = new Dictionary<string, IAccessor>();
        private static Dictionary<string, FixedRecordAttribute> _fixedRecordAttributes = new Dictionary<string, FixedRecordAttribute>();
        private static Dictionary<string, FixedItemAttribute[]> _fixedItemAttributes = new Dictionary<string, FixedItemAttribute[]>();

        #endregion

        #region -------------------- method --------------------

        /// <summary>
        /// アクセサ検索の為のキー値を取得します。
        /// </summary>
        /// <param name="item">データアイテム。</param>
        /// <param name="propertyName">プロパティ名。</param>
        /// <returns>
        /// アクセサ検索の為のキー値を返却します。
        /// </returns>
        /// <remarks>
        /// {オブジェクトの型名}.{プロパティ名}をキー値とします。
        /// </remarks>
        private static string GetAccessorKey(object item, string propertyName)
        {
            return $"{item.GetType().Name}.{propertyName}";
        }

        /// <summary>
        /// データアイテムとプロパティ名に対応したアクセサを取得します。
        /// </summary>
        /// <param name="item">データアイテム。</param>
        /// <param name="propertyName">プロパティ名。</param>
        /// <returns>
        /// データアイテムとプロパティ名に対応したアクセサを返却します。
        /// </returns>
        /// <remarks>
        /// アクセサがリストに存在する場合はリストから取得。
        /// アクセサがリストに存在しない場合はデータアイテムのプロパティから作成します。
        /// </remarks>
        private static IAccessor GetAccessor(object item, string propertyName)
        {
            var accessorKey = GetAccessorKey(item, propertyName);

            if (_accessors.TryGetValue(accessorKey, out var @accessor) == false)
            {
                @accessor = _accessors[accessorKey] = item.GetType().GetProperty(propertyName).ToAccessor();
            }
            return @accessor;
        }

        /// <summary>
        /// 固定長レコードモデル属性キーを取得します。
        /// </summary>
        /// <param name="item">データモデル。</param>
        /// <returns>
        /// 固定長レコードモデル属性キーを返却します。
        /// </returns>
        private static string GetFixedRecordAttributeKey(object item)
        {
            return item.GetType().FullName;
        }

        /// <summary>
        /// 固定長レコードモデル属性を取得します。
        /// </summary>
        /// <param name="item">データモデル。</param>
        /// <returns>
        /// 固定長レコードモデル属性を返却します。
        /// </returns>
        private static FixedRecordAttribute GetFixedRecordAttribute(object item)
        {
            var key = GetFixedRecordAttributeKey(item);
            if (_fixedRecordAttributes.TryGetValue(key, out var attribute) == false)
            {
                attribute = _fixedRecordAttributes[key] = Attribute.GetCustomAttribute(item.GetType(), typeof(FixedRecordAttribute)) as FixedRecordAttribute;
                if (attribute == null)
                {
                    throw new NotImplementedException($"{nameof(FixedRecordAttribute)} が設定されていません。");
                }
            }
            return attribute;
        }

        /// <summary>
        /// 固定長項目モデル属性キーを取得します。
        /// </summary>
        /// <param name="item">データモデル。</param>
        /// <returns>
        /// 固定長項目モデル属性キーを返却します。
        /// </returns>
        private static string GetFixedItemdAttributesKey(object item)
        {
            return item.GetType().FullName;
        }

        /// <summary>
        /// 固定長項目モデル属性リストを取得します。
        /// </summary>
        /// <param name="item">データモデル。</param>
        /// <returns>
        /// 固定長項目モデル属性リストを返却します。
        /// </returns>
        private static FixedItemAttribute[] GetFixedItemAttributes(object item)
        {
            var key = GetFixedItemdAttributesKey(item);
            if (_fixedItemAttributes.TryGetValue(key, out var attributes) == false)
            {
                var fixedAttributes = new List<FixedItemAttribute>();
                foreach (var property in item.GetType().GetProperties())
                {
                    var itemAttribute = Attribute.GetCustomAttribute(property, typeof(FixedItemAttribute)) as FixedItemAttribute;
                    if (itemAttribute == null)
                    {
                        continue;
                    }
                    fixedAttributes.Add(itemAttribute);
                }

                if (fixedAttributes.Count == 0)
                {
                    throw new NotImplementedException($"{nameof(FixedItemAttribute)} が設定されていません。");
                }

                attributes =  _fixedItemAttributes[key] = fixedAttributes.OrderBy(attribute => attribute.Order).ToArray();
            }
            return attributes;
        }

        /// <summary>
        /// 文字列から固定長レコードモデルを生成します。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="source">生成元文字列。</param>
        /// <returns>
        /// 文字列から固定長レコードモデルを生成し、返却します。
        /// </returns>
        public static T CreateFromRecord<T>(string source) where T : new()
        {
            var model = new T();
            var recordAttribute = GetFixedRecordAttribute(model);
            return CreateFromRecord<T>(recordAttribute.Encoding.GetBytes(source));
        }

        /// <summary>
        /// バイト配列から固定長レコードモデルを生成します。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="source">生成元バイト配列。</param>
        /// <returns>
        /// バイト配列から固定長レコードモデルを生成し、返却します。
        /// </returns>
        public static T CreateFromRecord<T>(byte[] source) where T : new()
        {
            var model = new T();
            var recordAttribute = GetFixedRecordAttribute(model);
            var itemAttributes = GetFixedItemAttributes(model);

            if (!recordAttribute.Length.Equals(itemAttributes.Sum(attribute => attribute.Length)))
            {
                throw new InvalidOperationException($"レコード長と項目長の合計が一致しません。");
            }

            var index = 0;
            foreach (var itemAttribute in itemAttributes)
            {
                var accessor = GetAccessor(model, itemAttribute.PropertyName);
                var bs = source.Skip(index).Take(itemAttribute.Length).ToArray();
                var item = recordAttribute.Encoding.GetString(bs);

                accessor.SetValue(model, item);

                index += itemAttribute.Length;
            }

            return model;
        }

        #endregion
    }
}
