using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Experiment
{
    public class ExperimentManager : SerializedMonoBehaviour
    {
        public delegate void ExperimentStateHandler();
        public event ExperimentStateHandler OnExperimentReady;
        public event ExperimentStateHandler OnExperimentStarted;
        public event ExperimentStateHandler OnExperimentFinished;

        public static ExperimentManager instance;
        
        [BoxGroup("Required objects")]
        [SerializeField]
        [Required] 
        [InlineEditor]
        private ExperimentSettingsHolder _settingsHolder = null;

        [BoxGroup("Required objects"), ShowInInspector, SerializeField, Required]
        private Dictionary<string, Experiment> _experiments;

        [ShowInInspector][InlineEditor()]
        public Experiment Experiment
        {
            get
            {
                if (_settingsHolder == null) return null;
                if (!_settingsHolder.HasActivesettings) return null;
                return _experiments.ContainsKey(_settingsHolder.ActiveSettings.ExperimentName) ? 
                    _experiments[_settingsHolder.ActiveSettings.ExperimentName] : null;
            }
        }

        [BoxGroup("ExperimentState"),ShowInInspector] 
        public bool IsExperimentRunning => Experiment != null && Experiment.IsRunning;

        #region MonoBehaviour
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //TODO - language settings
        }

        void OnDestroy()
        {
            UnsubscribeExperiment(Experiment);
        }
        #endregion

        #region Public API
        [Button(ButtonSizes.Medium)]
        [GUIColor(0,1,0)]
        public void PrepareExperiment()
        {
            if (Experiment == null)
            {
                Debug.LogWarning("No valid experiment set up");
                return;
            }
            if (Experiment.CurrentExperimentState > 0 && Experiment.CurrentExperimentState != Experiment.ExperimentState.Finished)
            {
                Debug.LogWarning("Experiment is already active. Finish it first.");
                return;
            }
            if (_settingsHolder.ActiveSettings == null)
            {
                Debug.LogWarning("There is no active settings selected");
                return;
            }
            Experiment.SetupExperiment(_settingsHolder.ActiveSettings);
            SubscribeToExperiment(Experiment);
            OnExperimentReady?.Invoke();
        }

        [Button(ButtonSizes.Medium)]
        [GUIColor(1,0.2f,0)]
        public void StartFinishExperiment()
        {
            switch (Experiment.CurrentExperimentState)
            {
                case Experiment.ExperimentState.Running:
                case Experiment.ExperimentState.Paused:
                    Experiment.ForceFinishExperiment();
                    break;
                case Experiment.ExperimentState.Finished:
                    PrepareExperiment();
                    Experiment.StartExperiment();
                    break;
                case Experiment.ExperimentState.Prepared:
                    Experiment.StartExperiment();
                    break;
                case Experiment.ExperimentState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StartExperiment()
        {
            if (Experiment.CurrentExperimentState != Experiment.ExperimentState.Prepared)
            {
                Debug.Log("Cannot start unprepared experiment");
            }
            Experiment.StartExperiment();
        }

        public void PauseUnpauseExperiment()
        {
            if (Experiment.IsPaused)
            {
                Experiment.ResumeExperiment();
                return;
            }
            if (Experiment.IsRunning) Experiment.PauseExperiment();
        }

        public void FinishExperiment()
        {
            Experiment.ForceFinishExperiment();
        }

        public void GoToMenu()
        {
        }
        #endregion

        #region Private helpers
        private void SubscribeToExperiment(Experiment experiment)
        {
            experiment.OnExperimentStarted += HandleExperimentStarted;
            experiment.OnExperimentPaused += HandleExperimentPaused;
            experiment.OnExperimentFinished += HandleExperimentFinished;
        }
        
        private void UnsubscribeExperiment(Experiment experiment)
        {
            if (experiment == null) return;
            experiment.OnExperimentFinished -= HandleExperimentFinished;
            experiment.OnExperimentPaused -= HandleExperimentPaused;
            experiment.OnExperimentStarted -= HandleExperimentStarted;
        }
        
        private void HandleExperimentStarted()
        {
            OnExperimentStarted?.Invoke();
        }
        
        private void HandleExperimentPaused()
        {
        }

        private void HandleExperimentFinished()
        {
            //TODO - localization
            OnExperimentFinished?.Invoke();
            UnsubscribeExperiment(Experiment);
        }
        #endregion
    }
}
