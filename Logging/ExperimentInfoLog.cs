using cyberframe.Logging.DataStructures;
using Newtonsoft.Json;
using UnityEngine;

namespace cyberframe.Logging
{
    public class ExperimentInfoLog : MonoLog
    {
        [SerializeField]
        private SetupInfo _setupInfo;

        //default instantiates without the player ID
        protected override string LogName => "ExperimentInfo";

        protected override void AfterLogSetup()
        {
            WriteExperimentData();
        }

        private void WriteExperimentData()
        {
            _setupInfo.PopulateInfo();
            var s = JsonConvert.SerializeObject(_setupInfo, Formatting.Indented);
            Log.WriteBlock("EXPERIMENT INFO", s);
        }
    }
}
