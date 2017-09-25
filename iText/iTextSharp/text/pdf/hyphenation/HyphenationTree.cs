using System;
using System.util;
using System.Collections;
using System.IO;
using System.Text;

/*
 * $Id: HyphenationTree.cs,v 1.1.1.1 2003/02/04 02:58:42 geraldhenson Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/**
	 * This tree structure stores the hyphenation patterns in an efficient
	 * way for fast lookup. It provides the provides the method to
	 * hyphenate a word.
	 *
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */
	public class HyphenationTree : TernaryTree, IPatternConsumer {

    /**
     * value space: stores the inteletter values
     */
    protected ByteVector vspace;

    /**
     * This map stores hyphenation exceptions
     */
    protected Hashtable stoplist;

    /**
     * This map stores the character classes
     */
    protected TernaryTree classmap;

    /**
     * Temporary map to store interletter values on pattern loading.
     */
    private TernaryTree ivalues;

    public HyphenationTree() {
        stoplist = new Hashtable(23);    // usually a small table
        classmap = new TernaryTree();
        vspace = new ByteVector();
        vspace.alloc(1);    // this reserves index 0, which we don't use
    }

    /**
     * Packs the values by storing them in 4 bits, two values into a byte
     * Values range is from 0 to 9. We use zero as terminator,
     * so we'll add 1 to the value.
     * @param values a string of digits from '0' to '9' representing the
     * interletter values.
     * @return the index into the vspace array where the packed values
     * are stored.
     */
    protected int packValues(string values) {
        int i, n = values.Length;
        int m = (n & 1) == 1 ? (n >> 1) + 2 : (n >> 1) + 1;
        int offset = vspace.alloc(m);
        byte[] va = vspace.Arr;
        for (i = 0; i < n; i++) {
            int j = i >> 1;
            byte v = (byte)((values[i] - '0' + 1) & 0x0f);
            if ((i & 1) == 1)
                va[j + offset] = (byte)(va[j + offset] | v);
            else
                va[j + offset] = (byte)(v << 4);    // big endian
        }
        va[m - 1 + offset] = 0;    // terminator
        return offset;
    }

    protected string unpackValues(int k) {
        StringBuilder buf = new StringBuilder();
        byte v = vspace[k++];
        while (v != 0) {
            char c = (char)(Util.USR(v, 4) - 1 + '0');
            buf.Append(c);
            c = (char)(v & 0x0f);
            if (c == 0)
                break;
            c = (char)(c - 1 + '0');
            buf.Append(c);
            v = vspace[k++];
        }
        return buf.ToString();
    }

    /**
     * Read hyphenation patterns from an internal format file.
     */
    public void loadInternalPatterns(string filename) {
        PatternInternalParser pp = new PatternInternalParser(this);
        ivalues = new TernaryTree();

        pp.parse(filename);

        // patterns/values should be now in the tree
        // let's optimize a bit
        trimToSize();
        vspace.trimToSize();
        classmap.trimToSize();

        // get rid of the auxiliary map
        ivalues = null;
    }

    /**
     * Read hyphenation patterns from an internal format file.
     */
    public void loadInternalPatterns(Stream istr) {
        PatternInternalParser pp = new PatternInternalParser(this);
        ivalues = new TernaryTree();

        pp.parse(istr);

        // patterns/values should be now in the tree
        // let's optimize a bit
        trimToSize();
        vspace.trimToSize();
        classmap.trimToSize();

        // get rid of the auxiliary map
        ivalues = null;
    }

    public string findPattern(string pat) {
        int k = base.find(pat);
        if (k >= 0)
            return unpackValues(k);
        return "";
    }

    /**
     * string compare, returns 0 if equal or
     * t is a substring of s
     */
    protected int hstrcmp(char[] s, int si, char[] t, int ti) {
        for (; s[si] == t[ti]; si++, ti++)
            if (s[si] == 0)
                return 0;
        if (t[ti] == 0)
            return 0;
        return s[si] - t[ti];
    }

    protected byte[] getValues(int k) {
        StringBuilder buf = new StringBuilder();
        byte v = vspace[k++];
        while (v != 0) {
            char c = (char)(Util.USR(v, 4) - 1);
            buf.Append(c);
            c = (char)(v & 0x0f);
            if (c == 0)
                break;
            c = (char)(c - 1);
            buf.Append(c);
            v = vspace[k++];
        }
        byte[] res = new byte[buf.Length];
        for (int i = 0; i < res.Length; i++)
            res[i] = (byte)buf[i];
        return res;
    }

    /**
     * <p>Search for all possible partial matches of word starting
     * at index an update interletter values. In other words, it
     * does something like:</p>
     * <code>
     * for(i=0; i<patterns.Length; i++) {
     * if ( word.Substring(index).startsWidth(patterns[i]) )
     * update_interletter_values(patterns[i]);
     * }
     * </code>
     * <p>But it is done in an efficient way since the patterns are
     * stored in a ternary tree. In fact, this is the whole purpose
     * of having the tree: doing this search without having to test
     * every single pattern. The number of patterns for languages
     * such as English range from 4000 to 10000. Thus, doing thousands
     * of string comparisons for each word to hyphenate would be
     * really slow without the tree. The tradeoff is memory, but
     * using a ternary tree instead of a trie, almost halves the
     * the memory used by Lout or TeX. It's also faster than using
     * a hash table</p>
     * @param word null terminated word to match
     * @param index start index from word
     * @param il interletter values array to update
     */
    protected void searchPatterns(char[] word, int index, byte[] il) {
        byte[] values;
        int i = index;
        char p, q;
        char sp = word[i];
        p = root;

        while (p > 0 && p < sc.Length) {
            if (sc[p] == 0xFFFF) {
                if (hstrcmp(word, i, kv.Arr, lo[p]) == 0) {
                    values = getValues(eq[p]);    // data pointer is in eq[]
                    int j = index;
                    for (int k = 0; k < values.Length; k++) {
                        if (j < il.Length && values[k] > il[j])
                            il[j] = values[k];
                        j++;
                    }
                }
                return;
            }
            int d = sp - sc[p];
            if (d == 0) {
                if (sp == 0) {
                    break;
                }
                sp = word[++i];
                p = eq[p];
                q = p;

                // look for a pattern ending at this position by searching for
                // the null char ( splitchar == 0 )
                while (q > 0 && q < sc.Length) {
                    if (sc[q] == 0xFFFF) {        // stop at compressed branch
                        break;
                    }
                    if (sc[q] == 0) {
                        values = getValues(eq[q]);
                        int j = index;
                        for (int k = 0; k < values.Length; k++) {
                            if (j < il.Length && values[k] > il[j]) {
                                il[j] = values[k];
                            }
                            j++;
                        }
                        break;
                    } else {
                        q = lo[q];

                        /**
                         * actually the code should be:
                         * q = sc[q] < 0 ? hi[q] : lo[q];
                         * but java chars are unsigned
                         */
                    }
                }
            } else
                p = d < 0 ? lo[p] : hi[p];
        }
    }

    /**
     * Hyphenate word and return a Hyphenation object.
     * @param word the word to be hyphenated
     * @param remainCharCount Minimum number of characters allowed
     * before the hyphenation point.
     * @param pushCharCount Minimum number of characters allowed after
     * the hyphenation point.
     * @return a {@link Hyphenation Hyphenation} object representing
     * the hyphenated word or null if word is not hyphenated.
     */
    public Hyphenation hyphenate(string word, int remainCharCount,
                                 int pushCharCount) {
        char[] w = word.ToCharArray();
        return hyphenate(w, 0, w.Length, remainCharCount, pushCharCount);
    }

    /**
     * Hyphenate word and return an array of hyphenation points.
     * @param w char array that contains the word
     * @param offset Offset to first character in word
     * @param len Length of word
     * @param remainCharCount Minimum number of characters allowed
     * before the hyphenation point.
     * @param pushCharCount Minimum number of characters allowed after
     * the hyphenation point.
     * @return a {@link Hyphenation Hyphenation} object representing
     * the hyphenated word or null if word is not hyphenated.
     */
    public Hyphenation hyphenate(char[] w, int offset, int len,
                                 int remainCharCount, int pushCharCount) {
        int i;
        char[] word = new char[len + 3];

        // normalize word
        char[] c = new char[2];
        for (i = 1; i <= len; i++) {
            c[0] = w[offset + i - 1];
            int nc = classmap.find(c, 0);
            if (nc < 0) {    // found a non-letter character, abort
                return null;
            }
            word[i] = (char)nc;
        }
        int[] result = new int[len + 1];
        int k = 0;

        // check exception list first
        string sw = new string(word, 1, len);
        if (stoplist.ContainsKey(sw)) {
            // assume only simple hyphens (Hyphen.pre="-", Hyphen.post = Hyphen.no = null)
            ArrayList hw = (ArrayList)stoplist[sw];
            int j = 0;
            for (i = 0; i < hw.Count; i++) {
                Object o = hw[i];
                if (o is string) {
                    j += ((string)o).Length;
                    if (j >= remainCharCount && j < (len - pushCharCount))
                        result[k++] = j;
                }
            }
        } else {
            // use algorithm to get hyphenation points
            word[0] = '.';                    // word start marker
            word[len + 1] = '.';              // word end marker
            word[len + 2] = (char)0;                // null terminated
            byte[] il = new byte[len + 3];    // initialized to zero
            for (i = 0; i < len + 1; i++) {
                searchPatterns(word, i, il);
            }

            // hyphenation points are located where interletter value is odd
            for (i = 0; i < len; i++) {
                if (((il[i + 1] & 1) == 1) && i >= remainCharCount
                        && i < (len - pushCharCount)) {
                    result[k++] = i;
                }
            }
        }


        if (k > 0) {
            // trim result array
            int[] res = new int[k];
            Array.Copy(result, 0, res, 0, k);
            return new Hyphenation(new string(w, offset, len), res);
        } else {
            return null;
        }
    }

    /**
     * Add a character class to the tree. It is used by
     * PatternParser as callback to
     * add character classes. Character classes define the
     * valid word characters for hyphenation. If a word contains
     * a character not defined in any of the classes, it is not hyphenated.
     * It also defines a way to normalize the characters in order
     * to compare them with the stored patterns. Usually pattern
     * files use only lower case characters, in this case a class
     * for letter 'a', for example, should be defined as "aA", the first
     * character being the normalization char.
     */
    public void addClass(string chargroup) {
        if (chargroup.Length > 0) {
            char equivChar = chargroup[0];
            char[] key = new char[2];
            key[1] = (char)0;
            for (int i = 0; i < chargroup.Length; i++) {
                key[0] = chargroup[i];
                classmap.insert(key, 0, equivChar);
            }
        }
    }

    /**
     * Add an exception to the tree. It is used by
     * PatternParser class as callback to
     * store the hyphenation exceptions.
     * @param word normalized word
     * @param hyphenatedword a vector of alternating strings and
     * {@link Hyphen hyphen} objects.
     */
    public void addException(string word, ArrayList hyphenatedword) {
        stoplist.Add(word, hyphenatedword);
    }

    /**
     * Add a pattern to the tree. Mainly, to be used by
     * PatternParser class as callback to
     * add a pattern to the tree.
     * @param pattern the hyphenation pattern
     * @param ivalue interletter weight values indicating the
     * desirability and priority of hyphenating at a given point
     * within the pattern. It should contain only digit characters.
     * (i.e. '0' to '9').
     */
    public void addPattern(string pattern, string ivalue) {
        int k = ivalues.find(ivalue);
        if (k <= 0) {
            k = packValues(ivalue);
            ivalues.insert(ivalue, (char)k);
        }
        insert(pattern, (char)k);
    }

    public override void printStats() {
        Console.Error.WriteLine("Value space size = "
                           + vspace.Length.ToString());
        base.printStats();

    }
	}
}
