using System.Collections;
using System.IO;
using UnityEngine;

namespace Lexto
{
    [ExecuteInEditMode]
    public class LexTo
    {
        private const string dictionaryName = "lexitron";
        private static LongLexTo Tokenizer;
        private static bool init;

        ///////////////////////////////////////////////////////////////////////////
        // singleton / constructor

        private static LexTo _instance = null;
        public static LexTo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LexTo();
                }
                return _instance;
            }
        }

        private LexTo()
        {
            TextAsset level = Resources.Load<TextAsset>(dictionaryName);
            if (level != null)
            {
                using (StreamReader sr = new StreamReader(new MemoryStream(level.bytes)))
                {
                    Tokenizer = new LongLexTo(sr);
                }
                Debug.Log(" !!! LexTo Initialized ");
                init = true;
            }
            else {
                Tokenizer = new LongLexTo();
                init = false;
                Debug.LogError(" !!! Error: The dictionary file is not found, " + dictionaryName);
            }
        }

        public string InsertLineBreaks(string inputText, char separator = ' ') {
            if (!init) {
                return inputText;
            }
            string result = "";
            ArrayList typeList;
            int begin, end, type;

            Tokenizer.WordInstance(inputText);
            typeList = Tokenizer.GetTypeList();
            begin = Tokenizer.First();
            int i = 0;
            while (Tokenizer.HasNext())
            {
                end = Tokenizer.Next();
                type = (short)typeList[i];
                result += inputText.Substring(begin, end - begin) + separator;
                begin = end;
            }
            return result;
        }

    } // end class

} // end namespace