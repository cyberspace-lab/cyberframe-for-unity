using System.Collections.Generic;
using cyberframe.Experiment.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Experiment
{
    [CreateAssetMenu(fileName = "ExperimentSettingsHolder", menuName = "cyberframe/Create settings holder", order = 15)]
    public class ExperimentSettingsHolder : SerializedScriptableObject
    {
        public delegate void SettingsChangeHandler();

        public event SettingsChangeHandler OnSettingsChanged;
        public event SettingsChangeHandler OnSettingsParseError;
        
        [InlineEditor()] 
        public List<ExperimentSettings> Settings;
        
        private ExperimentSettings _activeSettings;
        
        [ShowInInspector, InlineEditor()]
        public ExperimentSettings ActiveSettings
        {
            get => !HasSchedule ? _activeSettings : ((ExperimentSchedule)_activeSettings).ActiveSettings;
            private set => _activeSettings = value;
        }

        public bool HasSchedule => _activeSettings != null && (_activeSettings.GetType() != typeof(ExperimentSchedule));

        [ShowInInspector, InlineEditor()]
        public ExperimentSchedule ActiveSchedule
        {
            get
            {
                if (!HasSchedule) return null;
                return (ExperimentSchedule)_activeSettings;
            }
        }

        public bool HasActivesettings => ActiveSettings != null;

        public void TrySetSettings(ExperimentSettings settings)
        {
            if (!settings.Validate())
            {
                OnSettingsParseError?.Invoke();
                return;
            }
            SetSettings(settings);
        }
        
        public void SendSettingsParsingError()
        {
            OnSettingsParseError?.Invoke();
        }

        private void SetSettings(ExperimentSettings settings)
        {
            ActiveSettings = settings;
            OnSettingsChanged?.Invoke();
        }

        public void UnsetSettings()
        {
            ActiveSettings = null;
            OnSettingsChanged?.Invoke();
        }
    }
}

