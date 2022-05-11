using System;
using System.Collections.Generic;
using cyberframe.Experiment;
using UnityEngine;

namespace cyberframe.Demo
{
    [CreateAssetMenu(fileName = "DemoSettings", menuName = "cyberframe/Demo/Create experiment settings", order = 10)]
    public class DemoExperimentSettings : ExperimentSettings
    {
        [SerializeField]
        public List<DemoTrialSettings> TrialSettings = new List<DemoTrialSettings>();

        public override TrialSettings GetTrialSettings(int i)
        {
            return TrialSettings[i];
        }

        public override bool Validate()
        {
            return true;
        }
    }

    [Serializable]
    public class DemoTrialSettings : TrialSettings
    {
        public string target;
    }
}
