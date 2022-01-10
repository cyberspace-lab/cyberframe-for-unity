using System.Collections.Generic;
using cyberframe.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

//REquires an objsect with a player tag to be present

namespace cyberframe.Logging
{
    public class PlayerLog : MonoLog
    {
        //HOW OFTEN DO YOU WANNA LOG
        public float LoggingFrequency = 0.005F;

        protected override string LogName => "player";

        private float _deltaTime;
        private double _lastTimeWrite;

        private IPlayerLoggable _player;

        //this is for filling in custom number of fields that follow after common fields
        // for example, we need one column for input, but it is not always used, so we need to
        // create empty single column
        private const int NEmpty = 1;
        #region MonoBehaviour
        void Update()
        {
            //calculating FPS
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }
        void FixedUpdate()
        {
            if (!IsLogging) return;
            if (!(_lastTimeWrite + LoggingFrequency < SystemTimer.timeSinceMidnight)) return;
            LogPlayerUpdate();
            _lastTimeWrite = SystemTimer.timeSinceMidnight;
        }
        #endregion

        #region Monolog variables

        protected override void BeforeLogSetup()
        {
            var player = GetComponent<IPlayerLoggable>();
            if (player != null) _player = player;
        }

        protected override void AfterLogSetup()
        {
            if (_player == null)
            {
                Debug.Log("There is no player assigned.");
                return;
            }
            Log.WriteLine(HeaderLine());
        }
        #endregion

        #region Public API
        public void StartLogging()
        {
            if (IsLogging) return;
            if (_player == null)
            {
                Debug.Log("There is no player Game object. Cannot start player log.");
                Log.WriteLine("No valid player has been assigned");
                return;
            }
            //this is the header line for analysiss software
            _lastTimeWrite = SystemTimer.timeSinceMidnight;
            IsLogging = true;
        }
        public void StopLogging()
        {
            if (!IsLogging) return;
            IsLogging = false;
        }
        public void LogPlayerUpdate()
        {
            var strgs = CollectData();
            //strgs.AddRange(WriteBlank(NEmpty));
            WriteLine(strgs);
        }
        #endregion
        #region private function
        private string HeaderLine()
        {
            var line = "Time;";
            line += _player.HeaderLine();
            line += "FPS";
            return line;
        }
        private List<string> CollectData()
        {
            //TestData to Write is a parent method that adds some information to the beginning of the player info
            var strgs = _player.PlayerInformation();
            AddTimestamp(ref strgs);
            //adds FPS
            AddValue(ref strgs, (1.0f / _deltaTime).ToString("F4"));
            //needs an empty column for possible input information
            return strgs;
        }
        #endregion
    }
}
