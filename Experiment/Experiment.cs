using System;
using cyberframe.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Experiment
{
    public abstract class Experiment : MonoBehaviour, IExperimentLoggable
    {
        public enum ExperimentState
        {
            None,
            Prepared,
            Paused,
            Running,
            Finished
        }

        public enum ExperimentEvent
        {
            Started,
            ForceFinished
        }

        private ExperimentState _currentExperimentState;
        [BoxGroup("Experiment state")]
        [ShowInInspector]
        [EnumToggleButtons]
        public ExperimentState CurrentExperimentState
        {
            get => _currentExperimentState;
            protected set
            {
                if (value == _currentExperimentState)
                {
                    Debug.LogWarning("State is being changed to the same state");
                    return;
                }

                var prevState = _currentExperimentState;
                _currentExperimentState = value;
                OnExperimentStateChanged?.Invoke(_currentExperimentState);
                SendExperimentStateChanged(prevState, _currentExperimentState);
            }
        }

        [ShowInInspector] [BoxGroup("Experiment state")]
        public bool IsRunning => CurrentExperimentState == ExperimentState.Running;

        [BoxGroup("Experiment state")]
        [ShowInInspector]
        private bool _waitingToPause;

        [BoxGroup("Experiment state")]
        [ShowInInspector]
        public bool IsPaused => CurrentExperimentState == ExperimentState.Paused;

        private Trial.TrialState _currentTrialState;

        [BoxGroup("Trial state")]
        [ShowInInspector]
        [EnumToggleButtons]
        public Trial.TrialState CurrentTrialState
        {
            get => _currentTrialState;
            protected set
            {
                if (value == _currentTrialState)
                {
                    Debug.LogWarning("Trial state is being changed to the same state");
                    return;
                }
                var prevState = _currentTrialState;
                _currentTrialState = value;
                OnTrialStateChanged?.Invoke(_currentTrialState);
                SendTrialStateChanged(prevState, _currentTrialState);
            }
        }

        //we need to start the count at -1 as the first trial is called with NextTrial which
        // increments it by 1
        [BoxGroup("Trial state")]
        [ShowInInspector]
        public int iTrial { get; private set; } = -1;

        private float _trialStartTime;

        [ShowInInspector]
        [BoxGroup("Trial state")]
        public float CurrentTrialTime { get; protected set; } = -1;

        protected TrialSettings CurrentTrialsettings => Settings.GetTrialSettings(iTrial);

        [ShowInInspector][InlineEditor()]
        public ExperimentSettings Settings { get; protected set; }

        public delegate void ExperimentStateChangeHandler(ExperimentState state);
        public event ExperimentStateChangeHandler OnExperimentStateChanged;
    
        public delegate void ExperimentStateHandler();
        public event ExperimentStateHandler OnExperimentStarted;
        public event ExperimentStateHandler OnExperimentFinished;
        public event ExperimentStateHandler OnExperimentPaused;
        public event ExperimentStateHandler OnExperimentResumed;

        public delegate void TrialStateChangeHandler(Trial.TrialState state);
        public event TrialStateChangeHandler OnTrialStateChanged;

        public delegate void TrialStateHandler();

        public event TrialStateHandler OnTrialStarted;
        public event TrialStateHandler OnTrialFinished;

        // INTERFACE IEXPERIMENTLOGGABLE IMPLEMENTATION -------------------
        public string Name => Settings == null ? "Settings missing" : Settings.ExperimentName;
        public int TrialNumber => iTrial;
        public int ExperimentNumber => 0;

        public event EventHandler<IExperimentLoggable.ExperimentStateArgs> ExperimentStateChanged;
        public event EventHandler<IExperimentLoggable.ExperimentEventArgs> ExperimentEventSent;
        public event EventHandler<IExperimentLoggable.TrialStateArgs> TrialStateChanged;
        public event EventHandler<IExperimentLoggable.TrialEventArgs> TrialEventSent;
        public event EventHandler<IExperimentLoggable.ExperimentMessageArgs> MessageSent;

        #region Monobehaviour
        void Update()
        {
            if (!IsRunning) return;
            // The time update shoudl always happen before the OnUpdate function as it might depend
            // on the time being correct
            CurrentTrialTime = Time.time - _trialStartTime;
            OnUpdate();
        }
        #endregion

        #region Experiment control
        public void SetupExperiment(ExperimentSettings settings)
        {
            Settings = settings;
            iTrial = -1;
            CurrentTrialTime = -1;
            _waitingToPause = false;
            
            ExperimentOnSetup();
            CurrentExperimentState = ExperimentState.Prepared;
        }

        public void StartExperiment()
        {
            if (CurrentExperimentState != ExperimentState.Prepared)
            {
                throw new Exception("Experiment is not initialized, cannot start;");
            }
            ExperimentOnStart();
            CurrentExperimentState = ExperimentState.Running;
            OnExperimentStarted?.Invoke();
            SendExperimentEvent(ExperimentEvent.Started);
            ExperimentAfterStart();
        }

        public void PauseExperiment()
        {
            _waitingToPause = true;
        }

        private void ReallyPauseExperiment()
        {
            _waitingToPause = false;
            ExperimentOnPause();
            CurrentExperimentState = ExperimentState.Paused;
            OnExperimentPaused?.Invoke();
        }

        public void ResumeExperiment()
        {
            if (!IsPaused) return;
            ExperimentOnResume();
            CurrentExperimentState = ExperimentState.Running;
            OnExperimentResumed?.Invoke();
            NextTrial();
        }

        public void ForceFinishExperiment()
        {
            SendExperimentEvent(ExperimentEvent.ForceFinished);
            ForceFinishTrial();
            FinishExperiment();
        }

        public void ForceFinishTrial()
        {
            SendTrialEvent("ForceFinished");
            TrialOnFinish();
            CurrentTrialState = Trial.TrialState.Finished;
            OnTrialFinished?.Invoke();
            TrialCleanup();
        }

        public void Cleanup()
        {
            ExperimentCleanup();
        }
        #endregion

        #region Trial control
        protected void NextTrial()
        {
            // this shoudl probably never be executed, as the experiment finished
            // is already checked during finish trial, but just in case of some weird mojo
            // is happening
            if (CheckExperimentFinished())
            {
                FinishExperiment();
                return;
            }
            if (_waitingToPause)
            {
                ReallyPauseExperiment();
                return;
            }
            iTrial += 1;
            SetupTrial();
        }

        protected void SetupTrial()
        {
            TrialOnSetup();
            CurrentTrialState = Trial.TrialState.Prepared;
            TrialAfterSetup();
        }

        protected void StartTrial()
        {
            TrialOnStart();
            _trialStartTime = Time.time;
            CurrentTrialState = Trial.TrialState.Running;
            OnTrialStarted?.Invoke();
            TrialAfterStart();
        }

        protected void FinishTrial()
        {       
            TrialOnFinish();
            CurrentTrialState = Trial.TrialState.Finished;
            OnTrialFinished?.Invoke();
            TrialCleanup();
            if (CheckExperimentFinished())
            {
                FinishExperiment();
                return;
            }
            TrialAfterFinish();
        }
        
        protected void FinishExperiment()
        {
            if (CurrentExperimentState != ExperimentState.Running && CurrentExperimentState != ExperimentState.Paused)
            {
                throw new Exception("Experiment is not running, cannot finish");
            }
            // TOOD add finishign of trial in case it is still running
            ExperimentOnFinish();
            CurrentExperimentState = ExperimentState.Finished;
            OnExperimentFinished?.Invoke();
            ExperimentAfterFinish();
        }
        #endregion

        #region Experiment and trial flow
        /// <summary>
        /// CheckExperiment Finished is being checked automatically after each trial is finished and cleaned up before any new trials start
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckExperimentFinished();

        // This gets updated from Experiment manager, which is monobehaviou
        public virtual void OnUpdate(){}
        public virtual void OnFixedUpdate(){}
        protected virtual void ExperimentOnSetup(){}
        protected virtual void ExperimentOnStart(){}
        protected virtual void ExperimentAfterStart(){}
        protected virtual void ExperimentOnPause(){}
        protected virtual void ExperimentOnResume(){}
        protected virtual void TrialOnSetup(){}
        protected virtual void TrialAfterSetup(){}
        protected virtual void TrialOnStart(){}
        protected virtual void TrialAfterStart(){}
        protected virtual void TrialOnFinish(){}
        protected virtual void TrialCleanup(){}
        // happens after information about trial finished is sent
        // and events evaluated. ONLY happens if the experiment is 
        // not finished. Usually moves the experiment forward
        protected virtual void TrialAfterFinish(){}
        protected virtual void ExperimentOnFinish(){}
        protected virtual void ExperimentAfterFinish(){}
        // This is never called on its own. It is meant to be implemented and called as
        // a special function that can be called even after the experiment is finished
        // to basically allow showing participant some final info, but then cleaning
        // up before new experiment start
        protected virtual void ExperimentCleanup(){}
        #endregion

        #region Logging
        public virtual string ExperimentHeaderLog()
        {
            return String.Format("\"settings\":{0}", Settings.SerializeSettings());
        }

        #endregion

        #region Private helpers
        
        protected void SendTrialStateChanged(Trial.TrialState fromState, Trial.TrialState toState)
        {
            TrialStateChanged?.Invoke(this, new IExperimentLoggable.TrialStateArgs{Experiment = this, FromState = fromState.ToString(), ToState = toState.ToString()});
        }
        protected void SendExperimentStateChanged(ExperimentState fromState, ExperimentState toState)
        {
            ExperimentStateChanged?.Invoke(this, new IExperimentLoggable.ExperimentStateArgs{ Experiment = this, FromState = fromState.ToString(), ToState = toState.ToString() });
        }
        protected void SendExperimentEvent(ExperimentEvent experimentEvent)
        {
            ExperimentEventSent?.Invoke(this, new IExperimentLoggable.ExperimentEventArgs{Experiment = this, Event = experimentEvent.ToString()});
        }
        protected void SendTrialEvent(string s)
        {
            TrialEventSent?.Invoke(this, new IExperimentLoggable.TrialEventArgs{Experiment = this, Event = s});
        }
        protected void SendMessage(string s)
        {
            MessageSent?.Invoke(this, new IExperimentLoggable.ExperimentMessageArgs{Experiment = this, Message = s});
        }
        #endregion

    }
}
