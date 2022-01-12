using UnityEngine;

namespace cyberframe.Player
{
    public abstract class HandController : MonoBehaviour
    {
        public enum HandType
        {
            Left,
            Right
        }

        public HandType Hand;
    }
}
