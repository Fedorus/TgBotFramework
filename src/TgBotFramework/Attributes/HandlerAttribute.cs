using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotFramework.Attributes
{
    public class HandlerAttribute : System.Attribute
    {
        public string Stage { get; }

        public HandlerAttribute(string stage)
        {
            Stage = stage;
        }
    }
}
