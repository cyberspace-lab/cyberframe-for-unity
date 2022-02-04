using cyberframe.Logging.DataStructures;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Logging
{
    public class SessionInfoLog : MonoLog
    {
        [SerializeField, InlineEditor()]
        private SetupInfo _setupInfo;

        public override bool IsValid => true;

        //default instantiates without the player ID
        protected override string LogName => "SessionInfo";

        protected override void AfterLogSetup()
        {
            WriteExperimentData();
        }

        private void WriteExperimentData()
        {
            _setupInfo.PopulateInfo();
            var s = JsonConvert.SerializeObject(_setupInfo, Formatting.Indented);
            Log.WriteBlock("SESSION INFO", s);
        }
    }
}
