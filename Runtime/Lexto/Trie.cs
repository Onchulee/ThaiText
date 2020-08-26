using System;
using System.Linq;
using System.Text;

namespace Lexto
{
    class Trie
    {
        protected Trie parent = null;
        protected Trie[] child = new Trie[1];
        protected int numChildren = 0;
        protected char ch;
        protected Boolean isWord = false;

        //Creates a Trie using the root symbol as the character
        public Trie()
        {
            ch = (char)251;
        }
        public Trie(char c)
        {
            ch = c;
        }

        //Used to create the trie nodes when a string is added to a trie
        protected Trie createNode(char c)
        {
            return new Trie(c);
        }

        //Inserts the trie as the last child
        protected void addChild(Trie t)
        {
            insertChild(t, numChildren);
        }

        //Inserts the trie at the specified index.  
        // If successful, the parent of the specified trie is updated to be this trie.
        public void insertChild(Trie t, int index)
        {
            if (index < 0 || index > numChildren)
            {
                throw new ArgumentException("required: index >= 0 && index <= numChildren");
            }

            if (t == null)
            {
                throw new ArgumentException("cannot add null child");
            }

            if (t.parent != null)
            {
                throw new ArgumentException("specified child still belongs to parent");
            }
            if (hasChar(t.ch))
            {
                throw new ArgumentException("duplicate chars not allowed");
            }

            if (isDescendent(t))
            {
                throw new ArgumentException("cannot add cyclic reference");
            }

            t.parent = this;

            if (numChildren == child.Length)
            {
                Trie[] arr = new Trie[2 * (numChildren + 1)];
                for (int i = 0; i < numChildren; i++)
                    arr[i] = child[i];
                child = arr;
            }

            for (int i = numChildren; i > index; i--)
                child[i] = child[i - 1];
            child[index] = t;
            numChildren++;
        }

        //Returns true if this node is a descendent of the specified node or this node and the specified
        //node are the same node, false otherwise.
        public Boolean isDescendent(Trie t)
        {
            Trie r = this;
            while (r != null)
            {
                if (r == t)
                {
                    return true;
                }
                r = r.parent;
            }
            return false;
        }

        //End of tree-level operations.  Start of string operations.
        //Adds the string to the trie.  Returns true if the string is added or false if the string
        //is already contained in the trie.

        public Boolean add(String s)
        {
            return add(s, 0);
        }

        private Boolean add(String s, int index)
        {
            if (index == s.Length)
            {
                if (isWord)
                {
                    return false;
                }
                isWord = true;
                return true;
            }
            char c = s[index]; //char c=s.charAt(index);
            for (int i = 0; i < numChildren; i++)
            {
                if (child[i].ch == c)
                {
                    return child[i].add(s, index + 1);
                }
            }
            // this code adds from the bottom to the top because the addChild method
            // checks for cyclic references.  This prevents quadratic runtime.
            int ii = s.Length - 1;
            Trie t = createNode(s[ii--]);
            t.isWord = true;
            while (ii >= index)
            {
                Trie n = createNode(s[ii--]);
                n.addChild(t);
                t = n;
            }
            addChild(t);
            return true;
        }

        //Returns the child that has the specified character or null if no child has the specified character.
        public Trie getNode(char c)
        {
            for (int i = 0; i < numChildren; i++)
            {
                if (child[i].ch == c)
                {
                    return child[i];
                }
            }
            return null;
        }

        //Returns the last trie in the path that prefix matches the specified prefix string
        //rooted at this node, or null if there is no such prefix path.
        public Trie getNode(String prefix)
        {
            return getNode(prefix, 0);
        }

        private Trie getNode(String prefix, int index)
        {
            if (index == prefix.Length)
            {
                return this;
            }
            char c = prefix[index]; //char c=prefix.charAt(index);
            for (int i = 0; i < numChildren; i++)
            {
                if (child[i].ch == c)
                {
                    return child[i].getNode(prefix, index + 1);
                }
            }
            return null;
        }

        //Returns the number of nodes that define isWord as true, starting at this node and including
        //all of its descendents.  This operation requires traversing the tree rooted at this node.
        public int size()
        {
            int size = 0;
            if (isWord)
            {
                size++;
            }
            for (int i = 0; i < numChildren; i++)
            {
                size += child[i].size();
            }
            return size;
        }

        //Returns all of the words in the trie that begin with the specified prefix rooted at this node.
        //An array of length 0 is returned if there are no words that begin with the specified prefix.
        public String[] getWords(String prefix)
        {
            Trie n = getNode(prefix);
            if (n == null)
            {
                return new String[0];
            }
            String[] arr = new String[n.size()];
            n.getWords(arr, 0);
            return arr;
        }

        private int getWords(String[] arr, int x)
        {
            if (isWord)
            {
                arr[x++] = ToString();
            }
            for (int i = 0; i < numChildren; i++)
            {
                x = child[i].getWords(arr, x);
            }
            return x;
        }

        //Returns true if the specified string has a prefix path starting at this node.
        //Otherwise false is returned.
        public Boolean hasPrefix(String s)
        {
            Trie t = getNode(s);
            if (t == null)
            {
                return false;
            }
            return true;
        }

        //Check if the specified string is in the trie
        //Retrun value if contains, 0 if hasPrefix, else -1
        public int contains(String s)
        {
            Trie t = getNode(s);
            if (t == null)
            {
                return -1;
            }
            if (t.isWord)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //Returns true if this node has a child with the specified character.
        public Boolean hasChar(char c)
        {
            for (int i = 0; i < numChildren; i++)
            {
                if (child[i].ch == c)
                {
                    return true;
                }
            }
            return false;
        }

        //Returns the number of nodes from this node up to the root node.  The root node has height 0.
        public int getHeight()
        {
            int h = -1;
            Trie t = this;
            while (t != null)
            {
                h++;
                t = t.parent;
            }
            return h;
        }

        //Returns a string containing the characters on the path from this node to the root, but
        //not including the root character.  The last character in the returned string is the
        //character at this node.

        public String toString()
        {
            StringBuilder sb = new StringBuilder(getHeight());
            Trie t = this;
            while (t.parent != null)
            {
                sb.Append(t.ch);
                t = t.parent;
            }
            return new string(sb.ToString().ToCharArray().Reverse().ToArray());
        }
        // End 
    }
}
