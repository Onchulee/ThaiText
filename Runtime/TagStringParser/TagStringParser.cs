
using System.Collections.Generic;

public class TagStringParser
{
    private static readonly string START_OPEN_TAG = "<";
    private static readonly string START_CLOSED_TAG = "</";
    private static readonly string END_TAG = ">";

    public static TagString[] Parser(string str)
    {
        List<TagString> splitList = new List<TagString>();
        string tagString = "";
        string attributeVal = "";
        bool startTag = false;
        bool hasAttribute = false;
        bool inTag = false;

        for (var i = 0; i < str.Length; i++)
        {
            if (inTag && Concat(str[i]) == END_TAG)
            {
                if (startTag)
                {
                    splitList.Add(new TagString(tagString, TagString.Type.Open, attributeVal));
                    tagString = "";
                    attributeVal = "";
                }
                else
                {
                    splitList.Add(new TagString(tagString, TagString.Type.Close));
                    tagString = "";
                    attributeVal = "";
                }
                inTag = false;
                hasAttribute = false;
            }
            else if (!inTag && StartWithCloseTag(str, i))
            {
                if (tagString != "")
                {
                    splitList.Add(new TagString(tagString));
                }
                tagString = "";
                attributeVal = "";
                inTag = true;
                startTag = false;
                i++;
            }
            else if (!inTag && StartWithOpenTag(str, i))
            {
                if (tagString != "")
                {
                    splitList.Add(new TagString(tagString));
                }
                tagString = "";
                attributeVal = "";
                inTag = true;
                startTag = true;
            }
            else if (Concat(str[i]) == "=")
            {
                hasAttribute = true;
            }
            else if (!hasAttribute)
            {
                tagString += str[i];
            }
            else
            {
                attributeVal += str[i];
            }
        }
        if (inTag)
        {
            if (startTag)
            {
                splitList.Add(new TagString(tagString, TagString.Type.Open, attributeVal));
            }
            else
            {
                splitList.Add(new TagString(tagString, TagString.Type.Close));
            }
        }
        else
        {
            splitList.Add(new TagString(tagString));
        }

        return splitList.ToArray();
    }

    private static string Concat(params char[] values)
    {
        string ret = "";
        foreach (char value in values)
        {
            ret += value;
        }
        return ret;
    }

    private static bool StartWithCloseTag(string str, int i) {
        return i + 1 < str.Length && Concat(str[i], str[i + 1]) == START_CLOSED_TAG;
    }

    private static bool StartWithOpenTag(string str, int i)
    {
        return Concat(str[i]) == START_OPEN_TAG;
    }
}