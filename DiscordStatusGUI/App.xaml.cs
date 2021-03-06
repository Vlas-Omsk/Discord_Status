﻿using DiscordStatusGUI.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Collections;

namespace DiscordStatusGUI
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        //[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool SetDllDirectory(string lpPathName);
        //var dllDirectory = CurrentDir + "\\libs";
        //SetDllDirectory(dllDirectory);
        //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);
        //Environment.SetEnvironmentVariable("Path", Environment.GetEnvironmentVariable("Path") + ";" + CurrentDir + @"\libs");
        //ConsoleEx.WriteLine("Info", "Libs directory: " + CurrentDir + @"\libs");

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.Write($"[{ConsoleEx.Info}][{DateTime.Now:yyyy-MM-ddTHH:mm:ss.fffzzZ}]   STARTED\r\n");

            CheckCopy();

            string CurrentExe = Environment.GetCommandLineArgs()[0];
            string CurrentDir = Path.GetDirectoryName(CurrentExe);

            ConsoleEx.InitLogger();

            Directory.SetCurrentDirectory(CurrentDir);
            ConsoleEx.WriteLine(ConsoleEx.Info, "Working directory: " + Directory.GetCurrentDirectory());

            InitializeComponent();

            Locales.Lang.Init();
            Static.Init();
            
            Preferences.OpenLocalServer();
        }

        private void CheckCopy()
        {
            var processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            while (processes.Length > 1)
                Thread.Sleep(100);

            ProcessEx.OnProcessOpened += ProcessEx_OnProcessOpened;
        }

        private static void ProcessEx_OnProcessOpened(Processes processes)
        {
            Process current = Process.GetCurrentProcess(), ProcessCopy = processes.GetProcessesByNames(current.ProcessName).FirstOrDefault();
            if (ProcessCopy == null || ProcessCopy.HasExited)
                return;

            var cmdline = ProcessEx.GetProcessCommandLine(ProcessCopy.Id, out _);
            ConsoleEx.WriteLine(ConsoleEx.Info, $"\r\n    [{ProcessCopy.Id}] {ProcessCopy.ProcessName}\r\n    {cmdline}");
            ProcessCopy.Kill();
            var splittedcmdline = ProcessEx.SplitParams(cmdline);
            Preferences.SetPropertiesByCmdLine(splittedcmdline);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(e.ExceptionObject as Exception);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
        }

        void UnhandledException(Exception ex)
        {
            var result = "";
            foreach (DictionaryEntry obj in ex.Data)
            {
                result += "  " + obj.Key;
            }

            if (ConsoleEx.StreamWriter != null)
                try
                {
                    ConsoleEx.StreamWriter.Close();
                }catch { }

            File.AppendAllText("latest.log",
                   $"\r\n-----------------------BEGIN-------------------------" +
                   $"\r\n[MESSAGE]\r\n{ex}" +
                   $"\r\n[STACK TRACE]\r\n{ex.StackTrace}" +
                   $"\r\n[SOURCE]\r\n{ex.Source}" +
                   $"\r\n[TARGET SITE]\r\n{ex.TargetSite}" +
                   $"\r\n[HRESULT]\r\n{ex.HResult}" +
                   $"\r\n[DATA]\r\n{result}" +
                   $"\r\n[HELP LINK]\r\n{ex.HelpLink}" +
                   $"\r\n-------------------------END-------------------------");
        }
    }
}
