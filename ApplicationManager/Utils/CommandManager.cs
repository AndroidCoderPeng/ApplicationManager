using System;
using System.Diagnostics;

namespace ApplicationManager.Utils
{
    public class CommandManager
    {
        #region 懒汉单例模式

        private static readonly Lazy<CommandManager> Lazy = new Lazy<CommandManager>(() => new CommandManager());

        public static CommandManager Get => Lazy.Value;

        private CommandManager()
        {
        }

        #endregion

        public delegate void TransferValueDelegateHandler(string value);

        public void ExecuteCommand(string exePath, string arguments, TransferValueDelegateHandler delegateHandler) {
            using (var process = new Process())
            {
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = arguments;
                // 必须禁用操作系统外壳程序  
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();

                delegateHandler(process.StandardOutput.ReadToEnd().Trim());

                process.WaitForExit();
                process.Close();
            }
        }
    }
}
