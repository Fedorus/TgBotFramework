using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBotFramework.DataStructures;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework.StageManaging
{
    public class StageManager
    {
        public SortedDictionary<string, Type> StagesList { get; set; }
        
        public StageManager(SortedDictionary<string, Type> stagesList)
        {
            StagesList = stagesList;
        }

        public bool Check(string state)
        {
            if (state is null or "" or "default")
                return true;
            
            return StagesList.PrefixSearch(state) != null;
        }
    }
}