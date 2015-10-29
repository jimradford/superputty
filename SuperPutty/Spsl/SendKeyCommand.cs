/*
 * Copyright (c) 2009 - 2015 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"}, to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Windows.Forms;
using SuperPutty.Utils;
using System.Globalization;

namespace SuperPuTTY.Scripting
{
    public static partial class Commands
    {
        /// <summary>Holds the Key and associate Key entry</summary>
        private class KeywordVk
        {
            internal string keyword;
            internal int vk;

            public KeywordVk(string key, int v)
            {
                keyword = key;
                vk = v;
            }
        }

        private const int HAVESHIFT = 0;
        private const int HAVECTRL = 1;
        private const int HAVEALT = 2;

        private static KeywordVk[] keywords = new KeywordVk[]
        {
            new KeywordVk("ENTER", (int)Keys.Return),
            new KeywordVk("TAB",         (int)Keys.Tab),
            new KeywordVk("ESC",         (int)Keys.Escape),
            new KeywordVk("ESCAPE",      (int)Keys.Escape),
            new KeywordVk("HOME",        (int)Keys.Home),
            new KeywordVk("END",         (int)Keys.End),
            new KeywordVk("LEFT",        (int)Keys.Left),
            new KeywordVk("RIGHT",       (int)Keys.Right),
            new KeywordVk("UP",          (int)Keys.Up),
            new KeywordVk("DOWN",        (int)Keys.Down),
            new KeywordVk("PGUP",        (int)Keys.Prior),
            new KeywordVk("PGDN",        (int)Keys.Next),
            new KeywordVk("NUMLOCK",     (int)Keys.NumLock),
            new KeywordVk("SCROLLLOCK",  (int)Keys.Scroll),
            new KeywordVk("PRTSC",       (int)Keys.PrintScreen),
            new KeywordVk("BREAK",       (int)Keys.Cancel),
            new KeywordVk("BACKSPACE",   (int)Keys.Back),
            new KeywordVk("BKSP",        (int)Keys.Back),
            new KeywordVk("BS",          (int)Keys.Back),
            new KeywordVk("CLEAR",       (int)Keys.Clear),
            new KeywordVk("CAPSLOCK",    (int)Keys.Capital),
            new KeywordVk("INS",         (int)Keys.Insert),
            new KeywordVk("INSERT",      (int)Keys.Insert),
            new KeywordVk("DEL",         (int)Keys.Delete),
            new KeywordVk("DELETE",      (int)Keys.Delete),
            new KeywordVk("HELP",        (int)Keys.Help),
            new KeywordVk("F1",          (int)Keys.F1),
            new KeywordVk("F2",          (int)Keys.F2),
            new KeywordVk("F3",          (int)Keys.F3),
            new KeywordVk("F4",          (int)Keys.F4),
            new KeywordVk("F5",          (int)Keys.F5),
            new KeywordVk("F6",          (int)Keys.F6),
            new KeywordVk("F7",          (int)Keys.F7),
            new KeywordVk("F8",          (int)Keys.F8),
            new KeywordVk("F9",          (int)Keys.F9),
            new KeywordVk("F10",         (int)Keys.F10),
            new KeywordVk("F11",         (int)Keys.F11),
            new KeywordVk("F12",         (int)Keys.F12),
            new KeywordVk("F13",         (int)Keys.F13),
            new KeywordVk("F14",         (int)Keys.F14),
            new KeywordVk("F15",         (int)Keys.F15),
            new KeywordVk("F16",         (int)Keys.F16),
            new KeywordVk("MULTIPLY",    (int)Keys.Multiply),
            new KeywordVk("ADD",         (int)Keys.Add),
            new KeywordVk("SUBTRACT",    (int)Keys.Subtract),
            new KeywordVk("DIVIDE",      (int)Keys.Divide),
            new KeywordVk("+",           (int)Keys.Add),
            new KeywordVk("%",           (int)(Keys.D5 | Keys.Shift)),
            new KeywordVk("^",           (int)(Keys.D6 | Keys.Shift))
        };
                
        /// <summary>Parse Keyboard keys from a string</summary>
        /// <param name="arg">The Keyword to parse</param>
        /// <returns>A CommandData object with keystrokes, or null if no matching keyword found</returns>
        internal static CommandData SendKeyHandler(string arg)
        {            
            int key = ParseKeys(arg);

            if (key > 0)
                return new CommandData(new KeyEventArgs((Keys)key));
            else
                return null;

        }

        private static int ParseKeys(String keys)
        {
            int i = 0;

            int[] haveKeys = new int[] { 0, 0, 0 }; // shift, ctrl, alt

            int keysLen = keys.Length;

            while (i < keysLen)
            {
                int repeat = 1;
                char ch = keys[i];
                int vk = 0;

                switch (ch)
                {
                    case '}':
                        throw new ArgumentException("Detected '}' before any '{' was found.");
                    case '{':
                        int j = i + 1;

                        // There's a unique class of strings of the form "{} n}" where 
                        // n is an integer - in this case we want to send n copies of the '}' character. 
                        // Here we test for the possibility of this class of problems, and skip the
                        // first '}' in the string if necessary. 
                        //
                        if (j + 1 < keysLen && keys[j] == '}')
                        {
                            // Scan for the final '}' character
                            int final = j + 1;
                            while (final < keysLen && keys[final] != '}')
                            {
                                final++;
                            }
                            if (final < keysLen)
                            {
                                // Found the special case, so skip the first '}' in the string. 
                                // The remainder of the code will attempt to find the repeat count.
                                j++;
                            }
                        }

                        // okay, we're in a {<keyword>...} situation.  look for the keyword 
                        // 
                        while (j < keysLen && keys[j] != '}'
                               && !Char.IsWhiteSpace(keys[j]))
                        {
                            j++;
                        }

                        if (j >= keysLen)
                        {
                            throw new ArgumentException();
                        }

                        // okay, have our KEYWORD.  verify it's one we know about
                        // 
                        string keyName = keys.Substring(i + 1, j - (i + 1));
                        // see if we have a space, which would mean a repeat count.
                        // 
                        if (Char.IsWhiteSpace(keys[j]))
                        {
                            int digit;
                            while (j < keysLen && Char.IsWhiteSpace(keys[j]))
                            {
                                j++;
                            }

                            if (j >= keysLen)
                            {
                                throw new ArgumentException();
                            }

                            if (Char.IsDigit(keys[j]))
                            {
                                digit = j;
                                while (j < keysLen && Char.IsDigit(keys[j]))
                                {
                                    j++;
                                }
                                repeat = Int32.Parse(keys.Substring(digit, j - digit), CultureInfo.InvariantCulture);
                            }
                        }

                        if (j >= keysLen)
                        {
                            throw new ArgumentException();
                        }
                        if (keys[j] != '}')
                        {
                            throw new ArgumentException();
                        }

                        vk = MatchKeyword(keyName);
                        break;
                    case '+':
                        haveKeys[HAVESHIFT] = (int)Keys.Shift;
                        i++;
                        continue;
                    case '^':
                        haveKeys[HAVECTRL] = (int)Keys.Control;
                        i++;
                        continue;

                    case '%':
                        haveKeys[HAVEALT] = (int)Keys.Alt;
                        i++;
                        continue;
                                             
                    default:                                                                        
                        vk = (int)Char.ToUpperInvariant(keys[i]);
                        break;
                }

                return vk | haveKeys[HAVESHIFT] | haveKeys[HAVECTRL] | haveKeys[HAVEALT];
            }
            return -1;
        }

        /// <summary>given a string, match the keyword to a key.</summary>
        /// <param name="keyword">The Keyword to search</param>
        /// <returns>An Integer corresponding to the matched keyword, or -1 if none exists</returns>
        private static int MatchKeyword(string keyword)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                if (String.Equals(keywords[i].keyword, keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return keywords[i].vk;
                }
            }
            return -1;
        }        
    }
}
