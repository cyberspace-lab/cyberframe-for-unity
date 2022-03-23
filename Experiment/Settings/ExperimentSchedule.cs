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

        public int iActiveSettings { get; private set; }

        void OnEnable()
        {
            SetFirstSettings();
        }
        
        public void SetNextSettings()
        {
            iActiveSettings += 1;
            if (iActiveSettings <= Settings.Count - 1) return;
            Debug.LogWarning("There are not more settings.");
            iActiveSettings = Settings.Count - 1;
        }
        
        public void SetFirstSettings()
        {
            iActiveSettings = 0;
        }
        
        public ExperimentSettings ActiveSettings => Settings[iActiveSettings];
    
        public override TrialSettings GetTrialSettings(int i)
        {
            throw new System.NotImplementedException();
        }
    }
}
