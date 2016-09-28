﻿[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/log4net.config", Watch = true)]
namespace FT.Utility.Helper
{
    using log4net;
    using System;
    using System.Diagnostics;
    /// <summary>
    /// log4net 日志处理
    /// </summary>
    public class Log
    {
        public static void Debug(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Debug(message);
        }

        public static void Debug(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Debug(message, ex);
        }

        public static void Error(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(message, exception);
        }
        private static string GetCurrentMethodFullName()
        {
            try
            {
                StackFrame frame;
                string str2;
                int num = 2;
                StackTrace trace = new StackTrace();
                int length = trace.GetFrames().Length;
                do
                {
                    frame = trace.GetFrame(num++);
                    str2 = frame.GetMethod().DeclaringType.ToString();
                }
                while (str2.EndsWith("Excption") && (num < length));
                string name = frame.GetMethod().Name;
                return (str2 + "." + name);
            }
            catch
            {
                return null;
            }
        }

        public static void Info(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(message);
        }

        public static void Info(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(message, ex);
        }

        public static void Warn(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(message);
        }

        public static void Warn(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(message, ex);
        }
    }
}
