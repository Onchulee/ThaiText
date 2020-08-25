using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class ThaiText : Text
{
    private static Char SEPARATOR = '\u200B';
    private static Char NEWLINE = '\u000A';
    private static Char SPACE = '\u0020';
    private static Char SPECIAL_TAG = '\uFFF0';

    private bool onWrapChange;
    public UnityEvent OnChanged;
    private float boxwidth;
    private string defaultText = "";
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
            return base.text ;
        }
        set
        {
            OnChanged.Invoke();
            if (defaultText.Equals(value)) {
                return;
            }
            defaultText = value;
            base.text = defaultText;
            if (base.rectTransform.rect.width != 0)
            {
                TextAdjust();
            }
            else {
                WrappingText();
            }
        }
    }

    private void WrappingText()
    {
        onWrapChange = base.horizontalOverflow == HorizontalWrapMode.Wrap;
    }


    public string TextAdjust()
    {
        string value = defaultText.Replace("\\n", "\n");
        float boxwidth = GetBoxWidth();
        onWrapChange = false;
        if (ThaiFontAdjuster.IsThaiString(value))
        {
            if (base.horizontalOverflow == HorizontalWrapMode.Wrap)
            {
                base.text = ThaiWrappingText(value, boxwidth);
            }
            else
            {
                base.text = ThaiFontAdjuster.Adjust(value);
            }
        }

        base.text = System.Text.RegularExpressions.Regex.Unescape(base.text);
        return base.text;
    }
    private string ThaiWrappingText(string value, float boxwidth) {
        List<string> htmlTag;
        string inputText = ParserTag(value, out htmlTag);
        //inputText = ICU4Unity.instance.InsertLineBreaks(inputText, SEPARATOR);
        inputText = Lexto.LexTo.Instance.InsertLineBreaks(inputText, SEPARATOR);
        char[] arr = inputText.ToCharArray();
        Font font = this.font;
        CharacterInfo characterInfo = new CharacterInfo();
        if(font != null)
            font.RequestCharactersInTexture(inputText, base.fontSize, base.fontStyle);
        string outputText = "";
        int lineLength = 0;
        string word = "";
        int wordLength = 0;
        foreach (char c in arr)
        {
            if (c == SEPARATOR)
            {
                AddWordToText(outputText, lineLength, word, wordLength, boxwidth, out outputText, out lineLength);
                word = "";
                wordLength = 0;
                continue;
            } else if (c == NEWLINE) {
                AddNewLineToText(outputText, lineLength, word, wordLength, boxwidth, out outputText, out lineLength);
                word = "";
                wordLength = 0;
                continue;
            }
            else if (font != null && font.GetCharacterInfo(c, out characterInfo, base.fontSize))
            {
                if (c == SPACE)
                {
                    AddSpaceToText(outputText, lineLength, word, wordLength, characterInfo.advance, boxwidth, out outputText, out lineLength);
                    word = "";
                    wordLength = 0;
                } else if (c == SPECIAL_TAG) {
                    AddWordToText(outputText, lineLength, word, wordLength, boxwidth, out outputText, out lineLength);
                    word = "";
                    wordLength = 0;
                    outputText += htmlTag[0];
                    htmlTag.RemoveAt(0);
                }
                else {
                    word += c;
                    wordLength += characterInfo.advance;
                }
            }
        }
        AddWordToText(outputText, lineLength, word, wordLength, boxwidth, out outputText, out lineLength); // Add remaining word
        return ThaiFontAdjuster.Adjust(outputText);
    }

    private string ParserTag(string value, out List<string> htmlTag) {
        TagString[] tagArr = TagStringParser.Parser(value);
        string parserValue = "";
        htmlTag = new List<string>();
        foreach (TagString tag in tagArr)
        {
            if (tag.tag)
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

    private void AddSpaceToText(string inputText, int lineLength, string word, int wordLength, int spaceWidth, float boxwidth, out string outputText, out int totalLength)
    {
        if (lineLength + wordLength + spaceWidth <= boxwidth)
        {
            outputText = inputText + word + SPACE;
            totalLength = lineLength + wordLength + spaceWidth;
        }
        else if (lineLength + wordLength <= boxwidth) {
            outputText = inputText + word + "\n";
            totalLength = 0;
        }
        else
        {
            outputText = inputText + "\n" + word + SPACE;
            totalLength = wordLength + spaceWidth;
        }
    }

    private void AddWordToText(string inputText, int lineLength, string word, int wordLength, float boxwidth, out string outputText, out int totalLength) {
        if (lineLength + wordLength <= boxwidth)
        {
            outputText = inputText + word;
            totalLength = lineLength + wordLength;
        }
        else
        {
            outputText = inputText + "\n" + word;
            totalLength = wordLength;
        }
    }

    private void AddNewLineToText(string inputText, int lineLength, string word, int wordLength, float boxwidth, out string outputText, out int totalLength)
    {
        outputText = inputText + word + "\n";
        totalLength = 0;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        float checkWidth = GetBoxWidth();
        if (checkWidth > 0 && (checkWidth != this.boxwidth || onWrapChange))
        {
            this.boxwidth = checkWidth;
            TextAdjust();
        }
    }

    public float GetBoxWidth() {
        float boxwidth = base.rectTransform.rect.width;
        if (boxwidth <= 0) {
            Vector3[] corners = new Vector3[4];
            base.rectTransform.GetWorldCorners(corners);
            boxwidth = Vector3.Distance(corners[0], corners[3]);
        }
        return boxwidth;
    }

    private void Log(string log) {
        Debug.Log("Text (" + this.transform.parent.gameObject.name + "." + this.gameObject.name + ") = "+ log);
    }
}
