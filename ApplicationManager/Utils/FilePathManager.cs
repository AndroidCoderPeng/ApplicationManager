using System;
using System.Security.Principal;

namespace ApplicationManager.Utils
{
    public class FilePathManager
    {
        #region 懒汉单例模式

        private static readonly Lazy<FilePathManager> Lazy = new Lazy<FilePathManager>(() => new FilePathManager());

        public static FilePathManager Get => Lazy.Value;

        private FilePathManager()
        {
        }

        #endregion

        public string GetDesktopPath()
        {
            //获取默认输入路径
            var current = WindowsIdentity.GetCurrent();
            var currentName = current.Name;
            var userName = currentName.Split('\\')[1];
            return $@"C:\Users\{userName}\Desktop";
        }
    }
}