using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ApplicationManager.Utils
{
    public static class MethodExtensions
    {
        /// <summary>
        /// 将adb形如【MemTotal:       11691976 kB】的返回值转为字典值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this string value)
        {
            var strings = value.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var dictionary = strings.Select(
                temp => temp.Split(new[] { ":" }, StringSplitOptions.None)
            ).ToDictionary(
                split => split[0].Trim(), split => split[1].Trim()
            );
            return dictionary;
        }

        /// <summary>
        /// 格式化内存值
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static double FormatMemoryValue(this string memory)
        {
            //11691976*kB
            var newLine = Regex.Replace(memory, @"\s", "*");

            //11691976
            var temp = double.Parse(newLine.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries)[0]);

            //转为GB
            return Math.Round(temp / 1024 / 1024, 2);
        }

        /// <summary>
        /// 遍历删除文件夹下面的文件（夹）
        /// </summary>
        /// <param name="directory"></param>
        public static void DeleteDirectoryFiles(this DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                foreach (var directoryInfo in directory.GetDirectories())
                {
                    directoryInfo.DeleteDirectoryFiles();
                }

                foreach (var file in directory.GetFiles())
                {
                    var index = File.GetAttributes(file.FullName).ToString()
                        .IndexOf("ReadOnly", StringComparison.Ordinal);
                    if (index != -1)
                    {
                        File.SetAttributes(file.FullName, FileAttributes.Normal);
                    }

                    file.Delete();
                }

                directory.Delete();
            }
        }
    }
}