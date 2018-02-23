using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class LevelFinish : MonoBehaviour
    {
        private MenuControl _menuControl;
        private CameraFollow _cameraFollow;

        // Use this for initialization
        void Start ()
        {
            _menuControl = GameObject.Find("MenuUI").GetComponent<MenuControl>();
            _cameraFollow = GameObject.Find("MainCamera").GetComponent<CameraFollow>();
        }

        void OnTriggerEnter2D()
        {
            _cameraFollow.SetSceneOverView(true);
            _menuControl.ShowFinishPanel();
        }
    }
}
