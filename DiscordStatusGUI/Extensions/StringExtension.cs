﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordStatusGUI.Extensions
{
    public static class StringEx
    {
        public static string SubStrDel(this string source, string remove)
        {
            try
            {
                int n = source.IndexOf(remove);
                source = source.Remove(n, remove.Length);
            }
            catch { }
            return source;
        }
    }
}
