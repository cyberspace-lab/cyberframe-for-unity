using System;
using cyberframe.Experiment;
using cyberframe.Logging.DataStructures;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Logging
{
    [RequireComponent(typeof(SessionInfoLog))]
    [RequireComponent(typeof(ExperimentLog))]
    [RequireComponent(typeof(PlayerLogger))]
    public class MasterLogger : MonoBehaviour
    {
        public static MasterLogger instance;
        
        [InlineEditor()]
        public ParticipantInformation ParticipantInfo;

        [BoxGroup("Logging setup"), Tooltip("If false then no logs are being created/made")]
        public bool ShouldLog = true;

        [BoxGroup("Logging setup"), Tooltip("IF true, then all is automatically setup when experiment starts")]
        public bool StartWithExperiment = true;
        
        public string CreationTimestamp { get; private set; }

        [SerializeField] [Required]
        private SessionInfoLog _infoLog = null;
        [SerializeField] [Required]
        private ExperimentLog _experimentLog = null;
        [SerializeField] [Required]
        private PlayerLogger _playerLogger = null;

        [ShowInInspector,FolderPath] private string LogSaveDirectory => Log.LogSaveDirectory;

        #region Monobehaviour
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            if (!StartWithExperiment) return;
            ExperimentManager.instance.OnExperimentReady += PrepareLogs;
            ExperimentManager.instance.OnExperimentStarted += StartLogging;
            ExperimentManager.instance.OnExperimentFinished += StopLogging;
        }

        void OnDestroy()
        {
            StopLogging();
            ExperimentManager.instance.OnExperimentReady -= PrepareLogs;
            ExperimentManager.instance.OnExperimentStarted -= StartLogging;
            ExperimentManager.instance.OnExperimentFinished -= StopLogging;
        }
        #endregion

        #region public API
        [Button]
        public void PrepareLogs()
        {
            if (!ShouldLog) return;
            CreationTimestamp = DateTime.Now.ToString("HH-mm-ss-dd-MM-yyy");
            _infoLog.Setup(CreationTimestamp, ParticipantInfo.Code);
            _experimentLog.Setup(CreationTimestamp, ParticipantInfo.Code);
            _playerLogger.Setup(CreationTimestamp, ParticipantInfo.Code);
        }

        [Button]
        public void StartLogging()
        {
            if (!ShouldLog) return;
            if(_experimentLog.IsValid) _experimentLog.StartLogging();
            _playerLogger.StartLogging();
        }

        [Button]
        public void StopLogging()
        {
            _experimentLog.StopLogging();
            _playerLogger.StopLogging();
        }
        #endregion
    }
}

