using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Craft
{
    public class AudioManager : MonoBehaviour
    {
        public AudioMixer MasterMixer;

        public void SetMusicValue(float val)
        {
            MasterMixer.SetFloat("Music", val);
        }

        public void SetEffectsValue(float val)
        {
            MasterMixer.SetFloat("Effects", val);
        }
    }
}
