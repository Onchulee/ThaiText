
using System.Collections.Generic;

public class TagStringParser
{
    private static string[] tagDef = { "</", "<", ">" };

    public static TagString[] Parser(string str)
    {
        List<TagString> splitList = new List<TagString>();
        string tagStr = "";
        string val = "";
        bool start = false;
        bool takingValue = false;
        bool inTag = false;

        for (var i = 0; i < str.Length; i++)
        {
            if (i + 1 < str.Length && "" + str[i] + str[i + 1] == tagDef[0])
            {
                if (tagStr != "")
                {
                    splitList.Add(new TagString(tagStr));
                }
                tagStr = "";
                val = "";
                inTag = true;
                start = false;
                i++;
            }
            else if ("" + str[i] == tagDef[1])
            {
                if (tagStr != "")
                {
                    splitList.Add(new TagString(tagStr));
                }
                tagStr = "";
                val = "";
                inTag = true;
                start = true;
            }
            else if ("" + str[i] == tagDef[2])
            {
                if (start)
                {
                    splitList.Add(new TagString(tagStr, start, val));
                    tagStr = "";
                    val = "";
                }
                else
                {
                    splitList.Add(new TagString(tagStr, start));
                    tagStr = "";
                    val = "";
                }
                inTag = false;
                takingValue = false;
            }
            else if ("" + str[i] == "=")
            {
                takingValue = true;
            }
            else if (!takingValue)
            {
                tagStr += str[i];
            }
            else
            {
                val += str[i];
            }
        }
        if (inTag)
        {
            if (start)
            {
                splitList.Add(new TagString(tagStr, start, val));
            }
            else
            {
                splitList.Add(new TagString(tagStr, start));
            }
        }
        else
        {
            splitList.Add(new TagString(tagStr));
        }

        return splitList.ToArray();
    }
}