using TMPro;
using UnityEngine;

namespace cyberframe.Demo
{
    public class DemoTarget : MonoBehaviour
    {
        [SerializeField] private string _targetName;
        [SerializeField] private TextMeshPro _text;

        public DemoExperiment _experiment;

        void OnEnable()
        {
            _text.SetText(_targetName);
        }

        void OnTriggerEnter(Collider other)
        {
            _experiment.ColliderEntered(_targetName);
        }
    }
}