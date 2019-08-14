﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using log4net;


namespace MANDRAKE_Events.Util
{
    /// <summary>
    /// Pulled from original SPIPware. Helps Translate Gcode error messages and loading settings
    /// </summary>
    static class GrblCodeTranslator
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static Dictionary<int, string> Errors = new Dictionary<int, string>();
        static Dictionary<int, string> Alarms = new Dictionary<int, string>();
        /// <summary>
        /// setting name, unit, description
        /// </summary>
        public static Dictionary<int, Tuple<string, string, string>> Settings = new Dictionary<int, Tuple<string, string, string>>();

        private static void LoadErr(Dictionary<int, string> dict, string path)
        {
            if (!File.Exists(path))
            {
                _log.Error(String.Format("File Missing: {0}", path));
                return;
            }

            string FileContents;

            try
            {
                FileContents = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return;
            }

            Regex LineParser = new Regex(@"""([0-9]+)"",""[^\n\r""]*"",""([^\n\r""]*)""");     //test here https://regex101.com/r/hO5zI1/4

            MatchCollection mc = LineParser.Matches(FileContents);

            foreach (Match m in mc)
            {
                try //shouldn't be needed as regex matched already
                {
                    int number = int.Parse(m.Groups[1].Value);

                    dict.Add(number, m.Groups[2].Value);
                }
                catch { }
            }
        }

        private static void LoadSettings(Dictionary<int, Tuple<string, string, string>> dict, string path)
        {
            if (!File.Exists(path))
            {
                _log.Error(String.Format("File Missing: {0}", path));
                return;
            }

            string FileContents;

            try
            {
                FileContents = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return;
            }

            Regex LineParser = new Regex(@"""([0-9]+)"",""([^\n\r""]*)"",""([^\n\r""]*)"",""([^\n\r""]*)""");

            MatchCollection mc = LineParser.Matches(FileContents);

            foreach (Match m in mc)
            {
                try //shouldn't be needed as regex matched already
                {
                    int number = int.Parse(m.Groups[1].Value);

                    dict.Add(number, new Tuple<string, string, string>(m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value));
                }
                catch { }
            }
        }

        static GrblCodeTranslator()
        {
            //NEED TO CHANGED LOCATIONS OF CODES AND ADD THEM
            _log.Info("Loading GRBL Code Database");

            LoadErr(Errors, "Resources\\error_codes_en_US.csv");
            LoadErr(Alarms, "Resources\\alarm_codes_en_US.csv");
            LoadSettings(Settings, "Resources\\setting_codes_en_US.csv");

            _log.Info("Loaded GRBL Code Database");
        }

        public static string GetErrorMessage(int errorCode, bool alarm = false)
        {
            if (!alarm)
            {
                if (Errors.ContainsKey(errorCode))
                    return Errors[errorCode];
                else
                    return $"Unknown Error: {errorCode}";
            }
            else
            {
                if (Alarms.ContainsKey(errorCode))
                    return Alarms[errorCode];
                else
                    return $"Unknown Alarm: {errorCode}";
            }
        }

        static Regex ErrorExp = new Regex(@"error:(\d+)");
        private static string ErrorMatchEvaluator(Match m)
        {
            return GetErrorMessage(int.Parse(m.Groups[1].Value));
        }

        static Regex AlarmExp = new Regex(@"ALARM:(\d+)");
        private static string AlarmMatchEvaluator(Match m)
        {
            return GetErrorMessage(int.Parse(m.Groups[1].Value), true);
        }

        public static string ExpandError(string error)
        {
            string ret = ErrorExp.Replace(error, ErrorMatchEvaluator);
            return AlarmExp.Replace(ret, AlarmMatchEvaluator);
        }
    }
}
