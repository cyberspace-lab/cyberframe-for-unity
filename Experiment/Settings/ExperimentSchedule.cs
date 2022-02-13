using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Experiment.Settings
{
    [CreateAssetMenu(fileName = "Experiment Schedule", menuName = "cyberframe/Settings/Create experiment schedule", order = 15)]
    public class ExperimentSchedule : ExperimentSettings
    {
        [SerializeField, Required]
        public List<ExperimentSettings> Settings { get; private set; }
    
        public override TrialSettings GetTrialSettings(int i)
        {
            throw new System.NotImplementedException();
        }
    }
}
