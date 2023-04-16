using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Demo
{
    public class DemoExperiment : Experiment.Experiment
    {
        private DemoExperimentSettings MySettings => (DemoExperimentSettings) Settings;
        private string CurrentTargetName => MySettings.TrialSettings[iTrial].target;

        [ShowInInspector]
        private List<DemoTarget> _targets = new List<DemoTarget>();
        
        public override bool CheckExperimentFinished()
        {
            return iTrial >= (_targets.Count - 2);
        }

        protected override bool ExperimentOnSetup()
        {
            var targets = FindObjectsOfType<DemoTarget>();
            _targets = new List<DemoTarget>(targets);
            foreach (var target in _targets)
            {
                target._experiment = this;
            }
            return true;
        }

        protected override void ExperimentAfterStart()
        {
            NextTrial();
        }

        protected override void TrialAfterSetup()
        {
            StartTrial();
        }

        protected override void TrialAfterFinish()
        {
            NextTrial();
        }

        public void ColliderEntered(string targetName)
        {
            Debug.Log("Collider entered: " + targetName);
            if(targetName == CurrentTargetName) FinishTrial();

        }
    }
}
