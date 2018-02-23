using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class MenuControl : MonoBehaviour
    {

        public GameObject MenuPanel;
        public GameObject InformationPanel;
        public GameObject CreditsPanel;
        public GameObject AudioPanel;
        public GameObject InGameUi;
        public GameObject FinishPanel;
        public GameObject Camera;
        private CameraFollow _cameraFollow;
        
        // Use this for initialization
        void Start ()
        {
            _cameraFollow = Camera.GetComponent<CameraFollow>();
        }

        public void ShowMenuPanel()
        {
            HideAllPanels();
            MenuPanel.SetActive(true);
        }

        public void ShowInformationPanel()
        {
            HideAllPanels();
            InformationPanel.SetActive(true);
        }

        public void ShowCreditsPanel()
        {
            HideAllPanels();
            CreditsPanel.SetActive(true);
        }

        public void ShowAudioPanel()
        {
            HideAllPanels();
            AudioPanel.SetActive(true);
        }

        public void ShowFinishPanel()
        {
            HideAllPanels();
            FinishPanel.SetActive(true);
        }

        public void HideAllPanels()
        {
            MenuPanel.SetActive(false);
            InformationPanel.SetActive(false);
            CreditsPanel.SetActive(false);
            AudioPanel.SetActive(false);
            FinishPanel.SetActive(false);
            InGameUi.GetComponent<Canvas>().enabled = false;
        }

        public void ShowInGameUI()
        {
            InGameUi.GetComponent<Canvas>().enabled = true;
            _cameraFollow.SetSceneOverView(false);
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (MenuPanel.activeInHierarchy)
                {
                    HideAllPanels();
                    ShowInGameUI();
                }
                else
                {
                    HideAllPanels();
                    ShowMenuPanel();
                    _cameraFollow.SetSceneOverView(true);
                }
            }
        }
    }
}