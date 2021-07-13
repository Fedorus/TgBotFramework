using System;
using System.Collections.Generic;

namespace TgBotFramework.DataStructures
{
    public static class SortedDictionaryExtension
    {
        public static Type PrefixSearch(this SortedDictionary<string, Type> dictionary, string searchValue)
        {
            foreach (KeyValuePair<string, Type> pair in dictionary)
            {
                if (pair.Key[0] < searchValue[0])
                {
                    continue;
                }
                
                if(pair.Key[0] == searchValue[0] && searchValue.Length <= pair.Key.Length)
                {
                    if (CompareStrings(searchValue, pair.Key))
                    {
                        return pair.Value;
                    }
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
        
        private static bool CompareStrings(string searchValue, string other)
        {
            for (var i = 1; i < searchValue.Length; i++)
            {
                if (searchValue[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}