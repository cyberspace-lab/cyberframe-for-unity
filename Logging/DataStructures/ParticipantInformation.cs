using UnityEngine;

namespace cyberframe.Logging.DataStructures
{
    [CreateAssetMenu(fileName = "Participant information", menuName = "cyberframe/Participant info", order = 20)]
    public class ParticipantInformation : ScriptableObject
    {
        public string Code = "";
        public int Age = -1;
        public string Gender = "";

        public bool IsValid => Gender != "" &&
                               Code != "" &&
                               Age > -1;

        private void Clear()
        {
            Code = "";
            Age = -1;
            Gender = "";
        }

        public string Description()
        {
            //TODO - language implementation
            var txt = "Kód :" + Code + "\n";
            txt += "Věk: " + Age + "\n";
            txt += "Pohlaví: " + Gender + "\n";
            return txt;
        }
    }
}
