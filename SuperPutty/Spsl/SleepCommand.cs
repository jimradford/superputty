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
using SuperPutty.Utils;
using System.Threading;

namespace SuperPuTTY.Scripting
{
    public static partial class Commands
    {
        /// <summary>The Sleep command, delays execution of script for the specified number of milliseconds</summary>
        /// <param name="arg">The number of milliseconds to sleep for.</param>
        /// <returns>null to prevent any command from being sent to a session</returns>
        internal static CommandData SleepHandler(string arg)
        {
            int duration = 0;            
            if(int.TryParse(arg, out duration) && duration > 0)
            {
                Thread.Sleep(duration);
            }
            else
            {
                throw new ArgumentOutOfRangeException("The argument passed to SLEEP is not valid. Must be a positive whole number.");
            }
            return null;
        }
    }
}
