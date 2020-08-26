﻿using System;
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
            // Try loading lexitron dictionary from a Resources folder in the user project.
            TryInitialize();

            if(!init)
            {
                Tokenizer = new LongLexTo();
#if UNITY_EDITOR
                // Open Resources Importer to import the ThaiText Essential Resources
                com.dgn.ThaiText.PackageResourceImporter importer = new com.dgn.ThaiText.PackageResourceImporter();
                importer.Import();
#else
                 Debug.LogError(" !!! Error: The dictionary file is not found, " + dictionaryName);
#endif
            }
        }

        public void TryInitialize() {
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