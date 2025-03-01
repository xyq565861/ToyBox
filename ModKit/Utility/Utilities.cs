﻿// Copyright < 2021 > Narria (github user Cabarius) - License: MIT
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ModKit {
    public static class Utilties {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default) {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); } // using C# 6
            if (key == null) { throw new ArgumentNullException(nameof(key)); } //  using C# 6

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
        public static object GetPropValue(this object obj, string name) {
            foreach (var part in name.Split('.')) {
                if (obj == null) { return null; }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
        public static T GetPropValue<T>(this object obj, string name) {
            var retval = GetPropValue(obj, name);
            if (retval == null) { return default; }
            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
        public static object SetPropValue(this object obj, string name, object value) {
            var parts = name.Split('.');
            var final = parts.Last();
            if (final == null)
                return null;
            foreach (var part in parts) {
                if (obj == null) { return null; }
                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }
                if (part == final) {
                    info.SetValue(obj, value);
                    return value;
                }
                else {
                    obj = info.GetValue(obj, null);
                }
            }
            return null;
        }
        public static T SetPropValue<T>(this object obj, string name, T value) {
            object retval = SetPropValue(obj, name, value);
            if (retval == null) { return default; }
            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
        public static string StripHTML(this string s) => Regex.Replace(s, "<.*?>", string.Empty);
        public static string UnityRichTextToHtml(string s) {
            s = s.Replace("<color=", "<font color=");
            s = s.Replace("</color>", "</font>");
            s = s.Replace("<size=", "<size size=");
            s = s.Replace("</size>", "</font>");
            s += "<br/>";

            return s;
        }
        public static string[] getObjectInfo(object o) {

            var fields = "";
            foreach (var field in Traverse.Create(o).Fields()) {
                fields = fields + field + ", ";
            }
            var methods = "";
            foreach (var method in Traverse.Create(o).Methods()) {
                methods = methods + method + ", ";
            }
            var properties = "";
            foreach (var property in Traverse.Create(o).Properties()) {
                properties = properties + property + ", ";
            }
            return new string[] { fields, methods, properties };
        }
        public static string SubstringBetweenCharacters(this string input, char charFrom, char charTo) {
            var posFrom = input.IndexOf(charFrom);
            if (posFrom != -1) //if found char
            {
                var posTo = input.IndexOf(charTo, posFrom + 1);
                if (posTo != -1) //if found char
                {
                    return input.Substring(posFrom + 1, posTo - posFrom - 1);
                }
            }

            return string.Empty;
        }
        public static string[] TrimCommonPrefix(this string[] values) {
            var prefix = string.Empty;
            int? resultLength = null;

            if (values != null) {
                if (values.Length > 1) {
                    var min = values.Min(value => value.Length);
                    for (var charIndex = 0; charIndex < min; charIndex++) {
                        for (var valueIndex = 1; valueIndex < values.Length; valueIndex++) {
                            if (values[0][charIndex] != values[valueIndex][charIndex]) {
                                resultLength = charIndex;
                                break;
                            }
                        }
                        if (resultLength.HasValue) {
                            break;
                        }
                    }
                    if (resultLength.HasValue &&
                        resultLength.Value > 0) {
                        prefix = values[0].Substring(0, resultLength.Value);
                    }
                }
                else if (values.Length > 0) {
                    prefix = values[0];
                }
            }
            return prefix.Length > 0 ? values.Select(s => s.Replace(prefix, "")).ToArray() : values;
        }

        public static Dictionary<string, TEnum> NameToValueDictionary<TEnum>(this TEnum enumValue) where TEnum : struct {
            var enumType = enumValue.GetType();
            return Enum.GetValues(enumType)
                .Cast<TEnum>()
                .ToDictionary(e => Enum.GetName(enumType, e), e => e);
        }
        public static Dictionary<TEnum, string> ValueToNameDictionary<TEnum>(this TEnum enumValue) where TEnum : struct {
            var enumType = enumValue.GetType();
            return Enum.GetValues(enumType)
                .Cast<TEnum>()
                .ToDictionary(e => e, e => Enum.GetName(enumType, e));
        }
        public static Dictionary<K, V> Filter<K, V>(this Dictionary<K, V> dict,
        Predicate<KeyValuePair<K, V>> pred) => dict.Where(it => pred(it)).ToDictionary(it => it.Key, it => it.Value);
    }
    public static class MK {
        public static bool IsKindOf(this Type type, Type baseType) => type.IsSubclassOf(baseType) || type == baseType;
    }
}
