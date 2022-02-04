using System.Collections.Generic;
using System.Linq;
using cyberframe.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace cyberframe.Player
{
    public abstract class PlayerController : MonoBehaviour, IPlayerLoggable
    {
        private List<Vector3> _previousPositions = new List<Vector3>();
        private Vector3 _lastPosition;
        private const int POSITIONS_TO_KEEP = 100;
        private const int DEFAULT_UNSTUCK = 50;

        public static PlayerController Instance;

        #region MonoBehaviour

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            UpdatePreviousPositions();
            OnUpdate();
        }

        protected virtual void OnUpdate(){}
        #endregion
        #region Public API
        #region Moving
        public abstract void EnableMovement(bool bo = true);
        public abstract Vector3 Position { get; set; }
        public Vector2 Vector2Position
        {
            get => new Vector2(Position.x, Position.z);
            set => Position = new Vector3(value.x, Position.y, value.y);
        }

        [Button("Move to position"), BoxGroup("Movement")]
        public abstract void MoveToPosition(Vector2 position);
        [Button("MoveToPosition"), BoxGroup("Movement")]
        public abstract void MoveToPosition(Vector3 position);
        
        public void Unstuck()
        {
            Unstuck(DEFAULT_UNSTUCK);
        }

        public void Unstuck(ushort i)
        {
            var iPosition = ((_previousPositions.Count - i > 1) ? _previousPositions.Count - i : 1) - 1;
            Position = _previousPositions[iPosition];
            if (iPosition == 0)
            {
                Debug.Log("Can't unstuck further");
                _previousPositions.Clear();
            }
            else
            {
                _previousPositions.RemoveRange(iPosition, i);
            }
            EnableMovement();
        }
        #endregion

        #region Rotating
        public abstract Vector2 Rotation { get; set; }
        public abstract void EnableRotation(bool bo = true);
        public abstract void LookAtPosition(Vector2 point);
        public abstract void LookAtPosition(Vector3 point);
        #endregion

        #region Setting parameters
        public abstract void SetHeight(float height);
        public abstract void SetSpeed(float speed);
        #endregion
        #region Information
        public abstract Vector2 PointingDirection { get; }
        #endregion
        #endregion

        #region PRIVATE FUNCTIONS
        private void UpdatePreviousPositions()
        {
            if (Position == _lastPosition) return;
            _lastPosition = Position;
            _previousPositions.Add(_lastPosition);
            if (_previousPositions.Count < POSITIONS_TO_KEEP) return;
            _previousPositions.RemoveAt(0);
        }
        #endregion

        #region Logging interface implementation
        public abstract string HeaderLine();
        public abstract List<string> PlayerInformation();
        public abstract Dictionary<string, string> PlayerInformationDictionary();
        #endregion
    }
}
