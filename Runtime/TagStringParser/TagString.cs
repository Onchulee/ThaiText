
using System;

public class TagString
{
    public bool tag;
    public bool start;
    public string attribute;
    public string tagstring;

    public TagString(string str, bool strt, string val)
    {
        tag = true;
        tagstring = str;
        start = strt;
        attribute = val;
    }

    public TagString(string str, bool strt)
    {
        tag = true;
        tagstring = str;
        start = strt;
    }

    public TagString(string str)
    {
        tagstring = str;
    }

    public string GetTagString()
    {
        string ret = tagstring;
        if (tag)
        {
            if (!String.IsNullOrEmpty(attribute)) ret = ret + "=" + attribute;
            if (start) ret = "<" + ret + ">";
            else ret = "</" + ret + ">";
        }
        return ret;
    }

    public override string ToString()
    {
        string ret = "";
        ret += "tag: " + tag + ",";
        ret += "start: " + start + ",";
        ret += "attribute: " + attribute + ",";
        ret += "tagstring: " + tagstring;
        return "[" + ret + "]";
    }
}