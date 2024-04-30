using System.Collections.Generic;
using System.Text;

namespace ApplicationManager.Utils
{
    public class CommandCreator : List<string>
    {
        public CommandCreator Init()
        {
            Clear();
            return this;
        }

        public CommandCreator Append(string value)
        {
            Add(value);
            return this;
        }

        /// <summary>
        /// 构建命令
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            var command = new StringBuilder();
            for (var i = 0; i < Count; i++)
            {
                command.Append(this[i]);
                if (i < Count - 1)
                {
                    command.Append(" ");
                }
            }

            return command.ToString();
        }
    }
}