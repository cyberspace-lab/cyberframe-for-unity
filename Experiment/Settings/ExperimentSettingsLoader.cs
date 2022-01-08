using System;
using System.Collections;
using System.IO;
using cyberframe.Experiment;
using Newtonsoft.Json;
using SimpleFileBrowser;
using UnityEditor;
using UnityEngine;

namespace cyberframe.FileHandeling
{
    public abstract class ExperimentSettingsLoader : MonoBehaviour
    {
        [SerializeField]
        private ExperimentSettingsHolder _settingsHolder;

        public void OpenSettingsFileAndLoad()
        {
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        //TODO add cases when the files do not load correctly
        IEnumerator ShowLoadDialogCoroutine()
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Settings", ".json"));
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false,
                initialPath: ExperimentSettings.ExpectedPath, null, "Nahrát nastavení", "Load");

            // Dialog is closed
            Debug.Log(FileBrowser.Success);

            if (!FileBrowser.Success) yield break;
            var settings = ParseFile(FileBrowser.Result[0]);

            if (settings == null)
            {
                _settingsHolder.SendSettingsParsingError();
                yield break;
            }

            if (!settings.Validate())
            {
                _settingsHolder.SendSettingsParsingError();
                yield break;
            }

            _settingsHolder.TrySetSettings(settings);
        }

        public ExperimentSettings ParseFile(string pathToFile)
        {
            var inpStm = new StreamReader(pathToFile);
            var txt = "";
            while (!inpStm.EndOfStream)
            {
                txt += inpStm.ReadLine();
            }

            inpStm.Close();
            var settings = ParseString(txt);
            return settings;
        }

        public ExperimentSettings ParseString(string content)
        {
            var settings = ParseSettings<ExperimentSettings>(content);
            if (settings != null)
            {
                settings = LoadSettings(settings.ExperimentName, content);
            }
            return settings;
        }

        public abstract ExperimentSettings LoadSettings(string experimentName, string content);

        protected T ParseSettings<T>(string content) where T : ExperimentSettings
        {
            var settings = ScriptableObject.CreateInstance<T>();
            try
            {
                JsonConvert.PopulateObject(content, settings, new JsonSerializerSettings());
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return null;
            }
            return settings;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ExperimentSettingsLoader), true)]
    public class SettingsLoaderEditor : Editor
    {
        private string _jsonContent;
        private ExperimentSettingsLoader _target;

        void OnEnable()
        {
            _target = (ExperimentSettingsLoader) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _jsonContent = EditorGUILayout.TextArea(_jsonContent, GUILayout.Height(100));
            if (GUILayout.Button("Try serialize"))
            {
                var settings = _target.ParseString(_jsonContent);
                Debug.Log(settings);
            }
        }
    }
#endif
}
