using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Logging
{
    public class LogHeader
    {
        public string Participant;
        public string Timestamp;

        public LogHeader(string participant, string timestamp)
        {
            Participant = participant;
            Timestamp = timestamp;
        }
    }

    public class Log
    {
        readonly StreamWriter _logFile;
        public string FilePath;
        public string DateString;
        const string RelativePath = "/logs/";

        [ShowInInspector]
        public static string LogSaveDirectory => Application.persistentDataPath + RelativePath;

        /// <summary>
        /// polymorph for init with timepstmp provided by master log 
        /// </summary>
        /// <param name="id">ParticipantId of the participant</param>
        /// <param name="logName">name of the log file, e.g. Player/experiment etc.</param>
        /// <param name="timestamp">timestamp provided by master log</param>
        public Log(string id, string logName, string timestamp)
        {
            DateString = timestamp;
            var folderName = id + "_" + DateTime.Now.ToString("dd-MM-yyy") + "/";
            FilePath = LogSaveDirectory + folderName;
            Directory.CreateDirectory(FilePath);
            var FileName = FilePath + id + "_" + logName + "_" + timestamp + ".txt";
            _logFile = new StreamWriter(FileName, true)
                {AutoFlush = true};
#if CYBERFRAME_DEBUG
            Debug.Log("Log " + FileName + " created and saved" );
#endif
            WriteHeader(id);
        }

        //simple header file for each new star of the experiment
        private void WriteHeader(string id)
        {
            var header = new LogHeader(id, DateString);
            WriteBlock("SESSION HEADER", JsonConvert.SerializeObject(header, Formatting.Indented));
        }

        public void WriteBlock(string blockName, string blockContents)
        {
            _logFile.WriteLine("***" + blockName + "***");
            _logFile.WriteLine(blockContents);
            _logFile.WriteLine("---" + blockName + "---");
        }

        //takes a string and writes it down
        public void WriteLine(string str)
        {
            _logFile.WriteLine(str);
        }

        public void Close()
        {
            _logFile.Close();
        }

        //takes a list of string as an argument and turns them into a one line that is written into the file
        public void WriteList(List<string> data)
        {
            var line = CreateLine(data);
            _logFile.WriteLine(line);
        }

        #region Helpers
        public static string NewLine => Environment.NewLine;

        public static string CreateLine(List<string> data)
        {
            //basically LINQ foreach
            var line = string.Join(";", data);
            //var line = data.Aggregate("", (current, text) => current + text + ";");
            return line;
        }

        public static string VectorString(Vector3 input)
        {
            return input.ToString("F4");
        }

        #endregion
    }
}
