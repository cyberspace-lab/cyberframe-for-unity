using UnityEngine;

namespace cyberframe.Logging.DataStructures
{
    [CreateAssetMenu(fileName = "Setup Info", menuName = "cyberframe/Create Setup info", order = 50)]
    public class SetupInfo : ScriptableObject
    {
        public string ProductName;
        public string Version;
        public string UnityVersion;
        public string BuildGUID;
        public string Platform;

        public void OnEnable()
        {
            PopulateInfo();
        }

        public void PopulateInfo()
        {
            ProductName = Application.productName;
            Version = Application.version;
            UnityVersion = Application.unityVersion;
            BuildGUID = Application.buildGUID;
            Platform = Application.platform.ToString();
        }
    }
}
