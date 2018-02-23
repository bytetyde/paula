using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class InputManager : MonoBehaviour
    {
        private bool _inputAvailable;
        private bool _respawning;

        public void SetInputAvailable(bool val)
        {
            _inputAvailable = val;
        }

        public void SetRespawning(bool val)
        {
            _respawning = val;
        }

        public bool GetInputAvailable()
        {
            return _inputAvailable && _respawning;
        }
    }
}
