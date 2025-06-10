/*
 * https://github.com/jimradford/superputty/blob/master/License.txt
 */

using System;
using System.Linq;
using log4net;
using SuperPutty;
using SuperPutty.Utils;

namespace SuperPuTTY.Scripting
{
    /// <summary>SuperPuTTY Scripting Language</summary>
    public class SPSL
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SPSL));

        /// <summary>Holds the Key and associate Key entry</summary>
        private class SPSLFunction
        {
            internal string command;
            internal Func<string, CommandData> function;

            public SPSLFunction(string cmd, Func<string, CommandData> func)
            {
                command = cmd;
                function = func;
            }
        }

        /// <summary>Available SPSL Functions and their associated handlers</summary>
        private static SPSLFunction[] keywords = new SPSLFunction[]
        {
            new SPSLFunction("SENDKEY", Commands.SendKeyHandler),
            new SPSLFunction("OPENSESSION", Commands.OpenSessionHandler),
            new SPSLFunction("CLOSESESSION", Commands.CloseSessionHandler),
            new SPSLFunction("SENDCHAR", Commands.SendCharHandler),
            new SPSLFunction("SENDLINE", Commands.SendLineHandler),
            new SPSLFunction("SLEEP", Commands.SleepHandler),
            new SPSLFunction("PROMPT", Commands.PromptHandler),
            new SPSLFunction("PWDPROMPT", Commands.PrivatePromptHandler)
        };

        /// <summary>Try to parse a script line into commands and arguments</summary>
        /// <param name="line">The line to parse</param>
        /// <param name="commandData">A <seealso cref="CommandData"/> object with commands and keystrokes</param>
        /// <returns>true on success, false on failure, CommandData will be null for commands not requiring data to be sent</returns>        
        public static bool TryParseScriptLine(String line, out CommandData commandData)
        {
            commandData = null;
            if (string.IsNullOrEmpty(line)
                || line.StartsWith("#")) // a comment line, ignore
            {
                return false;
            }


            string command = string.Empty;
            string args = string.Empty;

            int index = line.IndexOf(' ');
            if (index > 0)
            {
                command = line.Substring(0, index);
                args = line.Substring(index + 1).TrimEnd();
            }
            else
            {
                command = line.ToUpperInvariant().TrimEnd();
            }

            // lookup command and execute action associated with it.                
            Func<String, CommandData> spslCommand = MatchCommand(command);
            if (spslCommand != null)
            {
                commandData = spslCommand(args);
                return true;
            }
            else
            {
                Log.WarnFormat("Command {0} Not Supported", command);
                return false;
            }
        }

        /// <summary>Execute a SPSL script Async</summary>
        /// <param name="scriptArgs">A <seealso cref="ExecuteScriptEventArgs"/> object containing the script to execute and parameters</param>
        public static void BeginExecuteScript(ExecuteScriptEventArgs scriptArgs)
        {
            string[] scriptlines = scriptArgs.Script.Split('\n');
            if (scriptlines.Length > 0
                && scriptArgs.IsSPSL)
            {
                new System.Threading.Thread(delegate ()
                {
                    foreach (string line in scriptlines)
                    {
                        CommandData command;
                        TryParseScriptLine(line, out command);
                        if (command != null)
                        {
                            command.SendToTerminal(scriptArgs.Handle.ToInt32());                        
                        }
                    }
                }).Start();
            }
        }

        /// <summary>Find Valid spsl script commands from lookup table and retrieve the Function to execute</summary>
        /// <param name="command">the SPSL command to lookup</param>
        /// <returns>The Function associated with the command or null of the command is invalid</returns>
        private static Func<string, CommandData> MatchCommand(string command)
        {
            return (from t 
                    in keywords
                    where String.Equals(t.command, command, StringComparison.OrdinalIgnoreCase)
                    select t.function).FirstOrDefault();
        }
    }
}
