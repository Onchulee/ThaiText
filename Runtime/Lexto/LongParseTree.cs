using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace Lexto
{
    class LongParseTree
    {
        //Private variables
        private Trie dict;               //For storing words from dictionary
        //Vector
        private ArrayList indexList;        //List of index positions
        private ArrayList typeList;         //List of word types
        private ArrayList frontDepChar;     //Front dependent characters: must have front characters
        private ArrayList rearDepChar;      //Rear dependent characters: must have rear characters
        private ArrayList tonalChar;        //Tonal characters
        private ArrayList endingChar;       //Ending characters
        //Vector

        /*******************************************************************/
        /************************ Constructor ******************************/
        /*******************************************************************/
        public LongParseTree(Trie dict, ArrayList indexList, ArrayList typeList)
        {
            this.dict = dict;
            this.indexList = indexList;
            this.typeList = typeList;
            frontDepChar = new ArrayList();
            rearDepChar = new ArrayList();
            tonalChar = new ArrayList();
            endingChar = new ArrayList();

            //Adding front-dependent characters
            frontDepChar.AddRange(new string[] { "ะ", "ั", "า", "ำ", "ิ", "ี", "ึ", "ื", "ุ", "ู", "ๅ", "็", "์", "ํ" });

            //Adding rear-dependent characters
            rearDepChar.AddRange(new string[] { "ั", "ื", "เ", "แ", "โ", "ใ", "ไ", "ํ" });

            //Adding tonal characters         
            tonalChar.AddRange(new string[] { "่", "้", "๊", "๋" });

            //Adding ending characters
            endingChar.AddRange(new string[] { "ๆ", "ฯ" });
        }

        /****************************************************************/
        /********************** nextWordValid ***************************/
        /****************************************************************/

        private Boolean nextWordValid(int beginPos, String text)
        {
            int pos = beginPos + 1;
            int status;
            if (beginPos == text.Length)
            {
                return true;
            }
            else if (text[beginPos] <= '~')
            { //Englurish alphabets/digits/special characters
                return true;
            }
            else
            {
                while (pos <= text.Length)
                {
                    status = dict.contains(text.Substring(beginPos, (pos - beginPos)));
                    if (status == 1)
                    {
                        return true;
                    }
                    else if (status == 0)
                    {
                        pos++;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            return false;
        }

        /****************************************************************/
        /********************** parseWordInstance ***********************/
        /****************************************************************/
        public int parseWordInstance(int beginPos, String text)
        {
            char prevChar = Convert.ToChar(0); //'\0';      //Previous character
            int longestPos = -1;       //Longest position
            int longestValidPos = -1;  //Longest valid position
            int numValidPos = 0;       //Number of longest value pos (for determining ambiguity)
            int returnPos = -1;        //Returned text position
            int pos, status;

            status = 1;
            numValidPos = 0;
            pos = beginPos + 1;
            while ((pos <= text.Length) && (status != -1))
            {
                status = dict.contains(text.Substring(beginPos, (pos - beginPos)));
                //Record longest so far
                if (status == 1)
                {
                    longestPos = pos;
                    if (nextWordValid(pos, text))
                    {
                        longestValidPos = pos;
                        numValidPos++;
                    }
                }
                pos++;
            } //while

            //For checking rear dependent character
            if (beginPos >= 1)
            {
                prevChar = text[beginPos - 1];
            }

            //Unknow word
            if (longestPos == -1)
            {
                returnPos = beginPos + 1;
                //Combine unknown segments
                bool unknown1 = frontDepChar.Contains("" + text[beginPos]);
                bool unknown2 = (tonalChar.Contains("" + text[beginPos]));
                bool unknown3 = (rearDepChar.Contains("" + prevChar));
                bool unknown4 = typeList.Count > 0 && (((short)typeList[typeList.Count - 1] == 0));
                if ((indexList.Count > 0) && (unknown1 || unknown2 || unknown3 || unknown4))
                {

                    indexList[indexList.Count - 1] = (short)returnPos;
                    typeList[typeList.Count - 1] = (short)0;
                }
                else
                {
                    indexList.Add((short)returnPos);
                    typeList.Add((short)0);
                }
                return returnPos;
            }
            //--------------------------------------------------
            //Known or ambiguous word
            else
            {
                //If there is no merging point
                if (longestValidPos == -1)
                {
                    //Check whether front char requires rear segment
                    if (rearDepChar.Contains("" + prevChar))
                    {
                        indexList[indexList.Count - 1] = (short)longestPos;
                        typeList[typeList.Count - 1] = (short)0;
                    }
                    else
                    {
                        typeList.Add((short)1);
                        indexList.Add((short)longestPos);
                    }
                    return longestPos; //known followed by unknown: consider longestPos
                }
                else
                {
                    //Check whether front char requires rear segment
                    if (rearDepChar.Contains("" + prevChar))
                    {
                        indexList[indexList.Count - 1] = (short)longestValidPos;
                        typeList[typeList.Count - 1] = (short)0;
                    }
                    else if (numValidPos == 1)
                    {
                        typeList.Add((short)1); //know
                        indexList.Add((short)longestValidPos);
                    }
                    else
                    {
                        typeList.Add((short)2); //ambiguous
                        indexList.Add((short)longestValidPos);
                    }
                    return (longestValidPos);
                }
            }
        }
    }
}
