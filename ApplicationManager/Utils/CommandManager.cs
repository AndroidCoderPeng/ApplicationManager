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

        public delegate void CommandValueDelegate(string value);

        /// <summary>
        /// 指令单参数adb命令
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="valueDelegate"></param>
        public void ExecuteCommand(string arguments, CommandValueDelegate valueDelegate)
        {
            using (var process = new Process())
            {
                //改为本程序adb路径
                process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"adb.exe";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();

                valueDelegate(process.StandardOutput.ReadToEnd().Trim());

                process.WaitForExit();
                process.Close();
            }
        }
    }
}