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

        public delegate void SettingsResourcesValidationHandler();

        [ShowInInspector]
        [InlineEditor]
        public ExperimentSettings ActiveSettings { get; private set; }

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
    }
}

