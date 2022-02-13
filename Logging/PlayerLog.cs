using System.Collections.Generic;
using System.Linq;
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

        public override bool IsValid => _player != null;
        protected override string LogName => _logName;

        [SerializeField]
        private string _logName = "player";

        private float _deltaTime;
        private double _lastTimeWrite;

        [SerializeField]
        private IPlayerLoggable _player;

        //this is for filling in custom number of fields that follow after common fields
        // for example, we need one column for input, but it is not always used, so we need to
        // create empty single column
        private const int NEmpty = 1;

        [BoxGroup("Log output"), InfoBox("$Header"), ShowInInspector]
        private string Header => _player == null ? "" : HeaderLine();

        [ShowInInspector, InfoBox("$ExampleLine"),  BoxGroup("Log output")]
        private string ExampleLine => _player == null ? "" : Log.CreateLine(CollectData());

        #region MonoBehaviour

        void Awake()
        {
            BeforeLogSetup();
        }

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
            if (_player != null) return;
            var player = GetComponent<IPlayerLoggable>();
            if (player != null) _player = player;
        }

        protected override void AfterLogSetup()
        {
            if (_player == null)
            {
                Debug.LogWarning("There is no loggable component on the player log");
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
                Debug.LogWarning("There is no player Game object. Cannot start player log.");
                Log.WriteLine("No valid player has been assigned");
                return;
            }
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
            WriteLine(strgs);
        }
        #endregion

        #region private function
        private string HeaderLine()
        {
            var strgs = new List<string>() {"Time"};
            strgs.AddRange(_player.LogVariableNames());
            strgs.Add("Input");
            strgs.Add("FPS");
            var line = Log.CreateLine(strgs);
            return line;
        }

        private List<string> CollectData()
        {
            //TestData to Write is a parent method that adds some information to the beginning of the player info
            var strgs = _player.LogVariableValues();
            AddTimestamp(ref strgs);
            strgs.Add(FormatPlayerInput(_player.PlayerInput()));
            //adds FPS
            AddValue(ref strgs, (1.0f / _deltaTime).ToString("F4"));
            return strgs;
        }

        private string FormatPlayerInput(List<string> playerInput)
        {
            if (playerInput.Count == 0) return "[]";
            return string.Format("[{0}]", playerInput.Aggregate((i, j) => i + "," + j));
        }
        #endregion
    }
}
