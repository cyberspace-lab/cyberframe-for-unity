using UnityEngine;

namespace cyberframe.Player.Gaze
{
    public class GazeController : MonoBehaviour
    {
        public LayerMask LayerMask;

        private GazeReceiver _lastGazeReceiver;

        public bool DebugOn;

        void Update()
        {
            if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                Mathf.Infinity, LayerMask))
            {
                SendGazeOff();
                if (DebugOn)
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 500, Color.red);
                }
                return;
            }
            var receiver = hit.transform.GetComponent<GazeReceiver>();
            if (DebugOn)
            {
                var color = receiver ? Color.green : Color.yellow;
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, color);
            }
            if (!receiver)
            {
                SendGazeOff();
                return;
            }
            receiver.GazeOn();
            _lastGazeReceiver = receiver;
        }

        private void SendGazeOff()
        {
            if (_lastGazeReceiver == null) return;
            _lastGazeReceiver.GazeOff();
            _lastGazeReceiver = null;
        }
    }
}
