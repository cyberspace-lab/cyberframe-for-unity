using System;

namespace cyberframe.Logging
{
    public interface IExperimentLoggable
    {
        public class ExperimentStateArgs
        {
            public IExperimentLoggable Experiment;
            public string FromState;
            public string ToState;
        }

        public struct ExperimentMessageArgs
        {
            public IExperimentLoggable Experiment;
            public string Message;
        }

        public struct TrialEventArgs
        {
            public IExperimentLoggable Experiment;
            public string Event;
        }

        public struct TrialStateArgs
        {
            public IExperimentLoggable Experiment;
            public string FromState;
            public string ToState;
        }

        public struct ExperimentEventArgs
        {
            public IExperimentLoggable Experiment;
            public string Event;
        }

        string Name { get; }
        int TrialNumber { get; }
        int ExperimentNumber { get; }

        event EventHandler<ExperimentStateArgs> ExperimentStateChanged;
        event EventHandler<ExperimentEventArgs> ExperimentEventSent;
        event EventHandler<TrialStateArgs> TrialStateChanged;
        event EventHandler<TrialEventArgs> TrialEventSent;
        event EventHandler<ExperimentMessageArgs> MessageSent;
        string ExperimentHeaderLog();
    }
}
