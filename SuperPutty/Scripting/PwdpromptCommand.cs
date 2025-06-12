/*
 * https://github.com/jimradford/superputty/blob/master/License.txt
 */

using System;
using System.Windows.Forms;
using SuperPutty;
using SuperPutty.Utils;

namespace SuperPuTTY.Scripting
{
    public static partial class Commands
    {
        
        /// <summary>prompt a user for hidden input</summary>
        /// <param name="arg">The pre-parsed string to send</param>
        /// <returns>A string containing commands to send with variables replaced with a carriage return sent at the end</returns>
        internal static CommandData PrivatePromptHandler(string arg)
        {
            String result;
            frmPrivatePrompt testDialog = new frmPrivatePrompt("Insert your password");
            if (testDialog.ShowDialog() == DialogResult.OK)
            {
                result = testDialog.GetResult();
            }
            else
            {
                result = "Cancelled";
            }
            testDialog.Dispose();
            
            CommandData data = new CommandData(result, new KeyEventArgs(Keys.Enter), TimeSpan.FromMilliseconds(50));
            return data;
        }
    }
}
