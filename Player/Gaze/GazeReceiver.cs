using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace cyberframe.Player.Gaze
{

    public class GazeReceiver : MonoBehaviour
    {
        [SerializeField]
        private List<UnityEvent> _onGazeOn;

        [SerializeField]
        private List<UnityEvent> _onGazeOff;

        public void GazeOn()
        {
            foreach (var gazeEvent in _onGazeOn)
            {
                gazeEvent.Invoke();
            }
        }

        public void GazeOff()
        {
            foreach (var gazeEvent in _onGazeOff)
            {
                gazeEvent.Invoke();
            }
        }

    }
}
