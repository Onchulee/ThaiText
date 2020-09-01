
using System;

public class TagString
{
    public enum Type {Undefined, Open, Close};
    
    public Type type;
    public string tagstring;
    public string attribute;

    public bool IsTag { get { return type!=Type.Undefined; } }

    public TagString(string str, Type strt, string val)
    {
        tagstring = str;
        type = strt;
        attribute = val;
    }

    public TagString(string str, Type strt)
    {
        tagstring = str;
        type = strt;
    }

    public TagString(string str)
    {
        tagstring = str;
        type = Type.Undefined;
    }

    public string GetTagString()
    {
        string ret = tagstring;
        if (IsTag)
        {
            if (!String.IsNullOrEmpty(attribute)) ret = ret + "=" + attribute;
            if (type == Type.Open) ret = "<" + ret + ">";
            else ret = "</" + ret + ">";
        }
        return ret;
    }

    public override string ToString()
    {
        string ret = "";
        ret += "tag: " + IsTag + ",";
        ret += "type: " + ((type == Type.Open)?"OPEN":"CLOSE") + " tag,";
        ret += "attribute: " + attribute + ",";
        ret += "tagstring: " + tagstring;
        return "[" + ret + "]";
    }
}