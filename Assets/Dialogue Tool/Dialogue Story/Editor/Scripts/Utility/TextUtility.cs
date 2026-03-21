using UnityEngine;

namespace E.Story
{
    // 文本实用类
    public static class TextUtility
    {
        /// <summary>
        /// 检测是否是空格
        /// </summary>
        /// <param name="character">字符</param>
        /// <returns>是否是空格</returns>
        public static bool IsWhitespace(this char character)
        {
            switch (character)
            {
                case '\u0020':
                case '\u00A0':
                case '\u1680':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2007':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                case '\u2028':
                case '\u2029':
                case '\u0009':
                case '\u000A':
                case '\u000B':
                case '\u000C':
                case '\u000D':
                case '\u0085':
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 检测是否是特殊字符（字母、数字、空格、短横、下划线、句点、小括号之外的字符）
        /// </summary>
        /// <param name="character">字符</param>
        /// <returns>是否是特殊字符</returns>
        public static bool IsSpecialCharacter(this char character)
        {
            bool isLetterOrDigit = char.IsLetterOrDigit(character);
            bool isWhitespace = character.IsWhitespace();
            bool isOther = character == '-' || character == '_'|| character == '.' || character == '(' || character == ')';

            return !isLetterOrDigit && !isWhitespace && !isOther;
        }

        /// <summary>
        /// 检测是否有空格
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>是否有空格</returns>
        public static bool HasWhitespace(this string text)
        {
            foreach (char c in text)
            {
                // 检测是否是空格
                if (c.IsWhitespace())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检测是否有特殊字符
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>是否有特殊字符</returns>
        public static bool HasSpecialCharacter(this string text)
        {
            foreach (char c in text)
            {
                // 检测是否是特殊字符
                if (c.IsSpecialCharacter())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除空格
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>处理后的文本</returns>
        public static string RemoveWhitespaces(this string text)
        {
            int textLength = text.Length;
            char[] textCharacters = text.ToCharArray();
            int currentWhitespacelessTextLength = 0;

            // 遍历文本中所有字符
            for (int currentCharacterIndex = 0; currentCharacterIndex < textLength; ++currentCharacterIndex)
            {
                // 获取当前字符
                char currentTextCharacter = textCharacters[currentCharacterIndex];

                // 检测是否是空格
                if (currentTextCharacter.IsWhitespace())
                {
                    continue;
                }

                textCharacters[currentWhitespacelessTextLength++] = currentTextCharacter;
            }

            return new string(textCharacters, 0, currentWhitespacelessTextLength);
        }

        /// <summary>
        /// 移除特殊字符
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>处理后的文本</returns>
        public static string RemoveSpecialCharacters(this string text)
        {
            int textLength = text.Length;
            char[] textCharacters = text.ToCharArray();
            int currentWhitespacelessTextLength = 0;

            // 遍历文本中所有字符
            for (int currentCharacterIndex = 0; currentCharacterIndex < textLength; ++currentCharacterIndex)
            {
                // 获取当前字符
                char currentTextCharacter = textCharacters[currentCharacterIndex];

                // 检测是否是特殊字符
                if (currentTextCharacter.IsSpecialCharacter())
                {
                    continue;
                }

                textCharacters[currentWhitespacelessTextLength++] = currentTextCharacter;
            }

            return new string(textCharacters, 0, currentWhitespacelessTextLength);
        }
    }
}