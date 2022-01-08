using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace cyberframe.Experiment
{
    [CreateAssetMenu(fileName = "ExperimentSettings", menuName = "Cyberframe/Create experiment settings", order = 10)]
    public class ExperimentSettings : SerializedScriptableObject
    {
        public static readonly string ExpectedPath = Path.Combine(Directory.GetCurrentDirectory(), "cyberframe");

        public string ExperimentName = "Monocular";

        [FoldoutGroup("Serialization")]
        [InfoBox("$_serializedSettings")]
        [ShowInInspector]
        [JsonIgnore]
        private string _serializedSettings = "";

        public virtual TrialSettings GetTrialSettings(int i)
        {
            return null;
        }

        public string Description()
        {
            var txt = ExperimentName + "\n";
            return txt;
        }

        [FoldoutGroup("Serialization")]
        [Button]
        public string SerializeSettings()
        {
            var strJson = JsonConvert.SerializeObject(this, SerialisationConstants.SerialisationSettings());
            _serializedSettings = strJson;
            return strJson;
        }

        [FoldoutGroup("Serialization")]
        [Button]
        public bool DeserializeSettings(string content)
        {
            //var settings = ScriptableObject.CreateInstance(GetType());
            try
            {
                JsonConvert.PopulateObject(content, this, new JsonSerializerSettings());
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
            return true;
        }

        [BoxGroup("Validation")]
        [Button]
        public bool Validate()
        {
            // validate color
            // validate images
            return true;
        }


#if UNITY_EDITOR
        [Button("Set as active settings")]
        private void SetActive()
        {
            var guids = AssetDatabase.FindAssets("t:ExperimentSettingsHolder");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var settingsHolder = AssetDatabase.LoadAssetAtPath<ExperimentSettingsHolder>(path);
            settingsHolder.TrySetSettings(this);
        }
#endif
    }
}
