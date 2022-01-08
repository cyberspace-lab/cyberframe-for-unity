using System;
using System.Collections;
using System.Collections.Generic;
using BrainVR.UnityFramework.UI.InGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Experiment
{
    public class ExperimentManager : MonoBehaviour
    {
        public delegate void ExperimentStateHandler();
        public event ExperimentStateHandler OnExperimentReady;
        public event ExperimentStateHandler OnExperimentStarted;
        public event ExperimentStateHandler OnExperimentFinished;

        public static ExperimentManager instance;
        
        [BoxGroup("Required objects")]
        [SerializeField]
        [Required] 
        private ExperimentCanvasManager _experimentCanvasManager = null;

        
        [BoxGroup("Required objects")]
        [SerializeField]
        [Required] 
        [InlineEditor]
        private ExperimentSettingsHolder _settingsHolder = null;

        [BoxGroup("Required objects")]
        [SerializeField]
        [ShowInInspector]
        [Required]
        private Experiment _experiment;

        public Experiment Experiment => _experiment;

        [BoxGroup("ExperimentState")]
        [ShowInInspector]
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
            Experiment.OnExperimentFinished -= HandleExperimentFinished;
            Experiment.OnExperimentPaused -= HandleExperimentPaused;
            Experiment.OnExperimentStarted -= HandleExperimentStarted;
        }
        #endregion

        #region Public API
        [Button(ButtonSizes.Medium)]
        [GUIColor(0,1,0)]
        public void PrepareExperiment()
        {
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
            Experiment.OnExperimentStarted += HandleExperimentStarted;
            Experiment.OnExperimentPaused += HandleExperimentPaused;
            Experiment.OnExperimentFinished += HandleExperimentFinished;
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

        public void PauseUnpauseExperiment()
        {
            if (_experiment.IsPaused)
            {
                _experiment.ResumeExperiment();
                return;
            }
            if (_experiment.IsRunning) _experiment.PauseExperiment();
        }

        public void FinishExperiment()
        {
            _experiment.ForceFinishExperiment();
        }

        public void GoToMenu()
        {
        }
        #endregion

        #region Private helpers
        private void HandleExperimentStarted()
        {
            OnExperimentStarted?.Invoke();
            Experiment.OnExperimentStarted -= HandleExperimentStarted;
        }
        private void HandleExperimentPaused()
        {
        }

        private void HandleExperimentFinished()
        {
            //TODO - localization
            OnExperimentFinished?.Invoke();
            Experiment.OnExperimentFinished -= HandleExperimentFinished;
            Experiment.OnExperimentPaused -= HandleExperimentPaused;
        }
        #endregion
    }
}
