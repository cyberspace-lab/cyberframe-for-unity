using System.Collections.Generic;
using cyberframe.Experiment;

namespace cyberframe.Logging
{
    public class ExperimentLog : MonoLog
    {
        private IExperimentLoggable _experiment;

        protected override string LogName => "test_" + _experiment.Name;

        protected override void BeforeLogSetup()
        {
            //TODO - validation, but it should be fine
            _experiment = ExperimentManager.instance.Experiment;
        }

        protected override void AfterLogSetup()
        {
            Log.WriteBlock("TEST HEADER", _experiment.ExperimentHeaderLog());
            Log.WriteLine("Time;Sender;Index;Type;Event;");
        }

        public void StartLogging()
        {
            Subscribe();
            IsLogging = true;
        }

        public void StopLogging()
        {
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