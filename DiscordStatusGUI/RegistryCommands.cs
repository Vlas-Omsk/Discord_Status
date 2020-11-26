﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinkJson.Parser;

namespace DiscordStatusGUI
{
    public class RegistryCommands
    {
        public static string CurrentExe => Environment.GetCommandLineArgs()[0];
        public static string Protocol => protocol + "://";

        private static readonly string protocol = $"discordstatus";
        private static readonly string open_command = $"\"{CurrentExe}\" --url \"%1\"";
        private static readonly string autorun_command = $"\"{CurrentExe}\" --tray";
        private static readonly RegistryKey CLASSES_ROOT = Registry.ClassesRoot;
        private static readonly RegistryKey AUTORUN = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\", true);

        private static bool IsProtocolRegistered()
        {
            return CLASSES_ROOT?.OpenSubKey("discordstatus")?.OpenSubKey("shell")?.OpenSubKey("open")?.OpenSubKey("command")?.GetValue("")?.ToString() == open_command;
        }

        public static void CreateProtocol()
        {
            if (!IsProtocolRegistered())
            {
                var discordstatus = CLASSES_ROOT.CreateSubKey(protocol);
                var shell = discordstatus.CreateSubKey("shell");
                var command = shell.CreateSubKey("open").CreateSubKey("command");

                discordstatus.SetValue("", "URL:" + Static.Titile);
                discordstatus.SetValue("URL Protocol", "");
                shell.SetValue("", "open");
                command.SetValue("", open_command);

                discordstatus.Close();
                shell.Close();
                command.Close();
            }
        }

        public enum AutoRun
        {
            Registered,
            OtherPath,
            UnRegistered
        }

        public static AutoRun AutoRunStatus()
        {
            if (AUTORUN?.GetValue(Static.Titile)?.ToString() == autorun_command)
                return AutoRun.Registered;
            else if (AUTORUN.GetValueNames().Contains(Static.Titile))
                return AutoRun.OtherPath;
            else
                return AutoRun.UnRegistered;
        }

        public static void CreateAutoRun()
        {
            if (AutoRunStatus() != AutoRun.Registered)
            {
                AUTORUN.SetValue(Static.Titile, autorun_command);
                AUTORUN.Flush();
            }
        }

        public static void RemoveAutoRun()
        {
            if (AutoRunStatus() == AutoRun.Registered)
            {
                AUTORUN.DeleteValue(Static.Titile);
                AUTORUN.Flush();
            }
        }
    }
}
