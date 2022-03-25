using System.Collections.Generic;
using cyberframe.Experiment;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Logging
{
    public class ExperimentLog : MonoLog
    {
        [ShowInInspector]
        private IExperimentLoggable _experiment;

        private readonly List<string> _headerLine = new List<string>() { "Time", "Sender", "Index", "Type", "Event" };

        public override bool IsValid => _experiment != null;

        protected override string LogName => "experiment_" + _experiment.Name;

        protected override void BeforeLogSetup()
        {
            //TODO - validation, but it should be fine
            _experiment = ExperimentManager.instance.Experiment;
            if (!IsValid)
            {
                Debug.LogWarning("There is no valid experiment setup. Will not create experiment log");
            }
        }

        protected override void AfterLogSetup()
        {
            if (!IsValid) return;
            Log.WriteBlock("TEST HEADER", _experiment.ExperimentHeaderLog());
            Log.WriteList(_headerLine);
        }

        public void StartLogging()
        {
            if (!IsValid) return;
            Subscribe();
            IsLogging = true;
        }

        public void StopLogging()
        {
            if (!IsValid) return;
            Unsubscribe();
            IsLogging = false;
            Close();
        }

        #region Private helpers
        private void Subscribe()
        {
            _experiment.ExperimentStateChanged += LogExperimentStateChanged;
            _experiment.TrialStateChanged += LogTrialStateChanged;
            _experiment.ExperimentEventSent += LogExperimentEvent;
            _experiment.TrialEventSent += LogTrialEvent;
            _experiment.MessageSent += LogCustomExperimentMessage;
        }

        private void Unsubscribe()
        {
            _experiment.ExperimentStateChanged -= LogExperimentStateChanged;
            _experiment.TrialStateChanged -= LogTrialStateChanged;
            _experiment.MessageSent -= LogCustomExperimentMessage;
            _experiment.ExperimentEventSent -= LogExperimentEvent;
            _experiment.TrialEventSent -= LogTrialEvent;
        }

        private void WriteEvent(List<string> strgs)
        {
            AddTimestamp(ref strgs);
            WriteLine(strgs);
        }

        private void LogCustomExperimentMessage(object obj, IExperimentLoggable.ExperimentMessageArgs args)
        {
            var toWrite = new List<string> {args.Message};
            WriteEvent(toWrite);
        }

        private void LogTrialStateChanged(object obj, IExperimentLoggable.TrialStateArgs args)
        {
            var toWrite = new List<string>
                {"Trial", args.Experiment.TrialNumber.ToString(), "StateChange", args.ToState};
            WriteEvent(toWrite);
        }

        private void LogTrialEvent(object obj, IExperimentLoggable.TrialEventArgs args)
        {
            var toWrite = new List<string> {"Trial", args.Experiment.TrialNumber.ToString(), "Event", args.Event};
            WriteEvent(toWrite);
        }

        private void LogExperimentStateChanged(object obj, IExperimentLoggable.ExperimentStateArgs args)
        {
            var toWrite = new List<string>
                {"Experiment", args.Experiment.ExperimentNumber.ToString(), "StateChange", args.ToState};
            WriteEvent(toWrite);
        }

        private void LogExperimentEvent(object obj, IExperimentLoggable.ExperimentEventArgs args)
        {
            var toWrite = new List<string>
                {"Experiment", args.Experiment.ExperimentNumber.ToString(), "Event", args.Event};
            WriteEvent(toWrite);
        }

        #endregion
    }
}
