using System.Collections.Generic;
using UnityEngine;

namespace cyberframe.Logging
{
    public class PlayerLogger : MonoBehaviour
    {
        private List<PlayerLog> _logs;

        #region MonoBehaviour
        void Awake()
        {
            var logs = FindObjectsOfType<PlayerLog>();
            _logs = new List<PlayerLog>(logs);
        }
        #endregion
        #region Public API

        public void StartLogging()
        {
            foreach (var log in _logs)
            {
                log.StartLogging();
            }
        }

        public void StopLogging()
        {
            foreach (var log in _logs)
            {
                log.StopLogging();
            }
        }

        public void Setup(string timestamp, string id)
        {
            foreach (var log in _logs)
            {
                log.Setup(timestamp, id);
            }

        }
        #endregion
    }
}
