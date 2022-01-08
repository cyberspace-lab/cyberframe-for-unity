namespace cyberframe.Experiment
{
    public class Trial
    {
        // Start is called before the first frame update
        public enum TrialState
        {
            None,
            Prepared,
            Running,
            Finished
        }

        public enum TrialEvent
        {
            Started,
            ForceFinished
        }
    }
}
