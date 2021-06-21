using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace com.dgn.ThaiText
{
    [Serializable]
    [AddComponentMenu("UI/Thai Text", 10)]
    public class ThaiText : Text
    {
        private static readonly Char SEPARATOR = '\u200B';
        private static readonly Char NEWLINE = '\u000A';
        private static readonly Char SPACE = '\u0020';
        private static readonly Char SPECIAL_TAG = '\uFFF0';

        private static readonly Char APPEND_NEWLINE = '\n';

        private bool onWrapChange;
        private float boxwidth;

        [TextArea(3, 10)]
        [SerializeField]
        private string defaultText = String.Empty;
        public string DefaultText
        {
            get
            {
                return defaultText;
            }
        }
        public override string text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (!defaultText.Equals(value))
                {
                    defaultText = value;
                    m_Text = value;
                    if (rectTransform.rect.width != 0)
                    {
                        m_Text = TextAdjust(defaultText, GetBoxWidth(), horizontalOverflow, GetFontData);
                    }
                    SetAllDirty();
                }
            }
        }

        public FontData GetFontData{
            get {
                return new FontData { font = this.font, fontSize = this.fontSize, fontStyle = this.fontStyle };
            }
        }

        private void WrappingText()
        {
            onWrapChange = horizontalOverflow == HorizontalWrapMode.Wrap;
        }
        
        public static string TextAdjust(string value, float boxwidth, HorizontalWrapMode horizontalOverflow, FontData fontData)
        {
            value = value.Replace("\\n", "\n");
            string ret = value;
            if (ThaiFontAdjuster.ThaiFontAdjuster.IsThaiString(value))
            {
                if (horizontalOverflow == HorizontalWrapMode.Wrap)
                {
                    ret = ThaiWrappingText(ret, boxwidth, fontData);
                }
                ret = ThaiFontAdjuster.ThaiFontAdjuster.Adjust(ret);
            }
            ret = System.Text.RegularExpressions.Regex.Unescape(ret);
            return ret;
        }

        private static string ThaiWrappingText(string value, float boxwidth, FontData fontData)
        {
            List<string> htmlTag;
            string inputText = ParserTag(value, out htmlTag);
            inputText = Lexto.LexTo.Instance.InsertLineBreaks(inputText, SEPARATOR);
            char[] arr = inputText.ToCharArray();
            Font font = fontData.font;
            CharacterInfo characterInfo = new CharacterInfo();
            if (font != null) {
                font.RequestCharactersInTexture(inputText, fontData.fontSize, fontData.fontStyle);
            }
            string outputText = "";
            int lineLength = 0;
            string word = "";
            int wordLength = 0;
            int SEPARATOR_Count = 0;
            foreach (char c in arr)
            {
                if (c == SEPARATOR)
                {
                    outputText = AddWordToText(outputText, lineLength, word, wordLength, boxwidth, out lineLength);
                    word = "";
                    wordLength = 0;
                    SEPARATOR_Count++;
                    continue;
                }
                else if (c == NEWLINE)
                {
                    outputText = AddNewLineToText(outputText, lineLength, word, wordLength, boxwidth, out lineLength);
                    word = "";
                    wordLength = 0;
                    continue;
                }
                else if (font != null && font.GetCharacterInfo(c, out characterInfo, fontData.fontSize))
                {
                    if (c == SPACE)
                    {
                        outputText = AddSpaceToText(outputText, lineLength, word, wordLength, characterInfo.advance, boxwidth, out lineLength);
                        word = "";
                        wordLength = 0;
                    }
                    else if (c == SPECIAL_TAG)
                    {
                        outputText = AddWordToText(outputText, lineLength, word, wordLength, boxwidth, out lineLength);
                        word = "";
                        wordLength = 0;
                        outputText += htmlTag[0];
                        htmlTag.RemoveAt(0);
                    }
                    else
                    {
                        word += c;
                        wordLength += characterInfo.advance;
                    }
                }
            }
            outputText = AddWordToText(outputText, lineLength, word, wordLength, boxwidth, out lineLength); // Add remaining word
            return outputText;
        }

        private static string ParserTag(string value, out List<string> htmlTag)
        {
            TagString[] tagArr = TagStringParser.Parser(value);
            string parserValue = "";
            htmlTag = new List<string>();
            foreach (TagString tag in tagArr)
            {
                if (tag.IsTag)
                {
                    parserValue += SPECIAL_TAG;
                    htmlTag.Add(tag.GetTagString());
                }
                else
                {
                    parserValue += tag.GetTagString();
                }
            }
            return parserValue;
        }

        private static string AddSpaceToText(string inputText, int lineLength, string word, int wordLength, int spaceWidth, float boxwidth, out int totalLength)
        {
            string outputText;
            if (lineLength + wordLength + spaceWidth <= boxwidth)
            {
                outputText = inputText + word + SPACE;
                totalLength = lineLength + wordLength + spaceWidth;
            }
            else if (lineLength + wordLength <= boxwidth)
            {
                outputText = inputText + word + APPEND_NEWLINE;
                totalLength = 0;
            }
            else
            {
                outputText = inputText + APPEND_NEWLINE + word + SPACE;
                totalLength = wordLength + spaceWidth;
            }
            return outputText;
        }

        private static string AddWordToText(string inputText, int lineLength, string word, int wordLength, float boxwidth, out int totalLength)
        {
            string outputText;
            if (lineLength + wordLength <= boxwidth)
            {
                outputText = inputText + word;
                totalLength = lineLength + wordLength;
            }
            else
            {
                outputText = inputText + APPEND_NEWLINE + word;
                totalLength = wordLength;
            }
            return outputText;
        }

        private static string AddNewLineToText(string inputText, int lineLength, string word, int wordLength, float boxwidth, out int totalLength)
        {
            string outputText;
            if (lineLength + wordLength <= boxwidth)
            {
                outputText = inputText + word + APPEND_NEWLINE;
                totalLength = 0;
            }
            else
            {
                outputText = inputText + APPEND_NEWLINE + word;
                totalLength = wordLength;
            }
            return outputText;
        }

        public float GetBoxWidth()
        {
            float boxwidth = base.rectTransform.rect.width;
            if (boxwidth <= 0)
            {
                Vector3[] corners = new Vector3[4];
                base.rectTransform.GetWorldCorners(corners);
                boxwidth = Vector3.Distance(corners[0], corners[3]);
            }
            return boxwidth;
        }

        private void Log(string log)
        {
            Debug.Log("Text (" + this.transform.parent.gameObject.name + "." + this.gameObject.name + ") = " + log);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            float checkWidth = GetBoxWidth();
            if (checkWidth > 0 && (checkWidth != this.boxwidth || onWrapChange))
            {
                this.boxwidth = checkWidth;
                m_Text = TextAdjust(defaultText, GetBoxWidth(), horizontalOverflow, GetFontData);
            }
        }

        protected override void OnEnable()
        {
            m_Text = TextAdjust(defaultText, GetBoxWidth(), horizontalOverflow, GetFontData);
            base.OnEnable();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Font loadFont = Resources.Load("ThaiText/Fonts/RSU_Regular") as Font;
            if(loadFont) font = loadFont;
            text = "ข้อความ";
            color = Color.black;
        }

        // The Text inspector editor can change the font, and we need a way to track changes so that we get the appropriate rebuild callbacks
        // We can intercept changes in OnValidate, and keep track of the previous font reference
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Text = TextAdjust(defaultText, GetBoxWidth(), horizontalOverflow, GetFontData);
        }
#endif // if UNITY_EDITOR

    }
}