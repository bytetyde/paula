using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UiBar : MonoBehaviour
    {

        public float MaxValue;
        public float MinValue;
        public float CurrentValue;
        public Text Text;
        public Image Content;
        public float LerpSpeed;
        public string TextValue;
        public string Prefix;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update ()
        {
            Content.fillAmount = Mathf.Lerp(Content.fillAmount,  GameObjectHelper.Map(CurrentValue, MinValue, MaxValue, 0, 1), Time.deltaTime * LerpSpeed);
            Text.text = TextValue + ": " + (int)CurrentValue + Prefix;
        }


        public void SetValue(float val, float maxVal)
        {
            CurrentValue = val;
            MaxValue = maxVal;
        }
    }
}
