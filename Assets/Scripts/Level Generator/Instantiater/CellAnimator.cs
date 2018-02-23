using System.Collections;
using Assets.Scripts.Level_Generator.Generator;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Instantiater
{
    [ExecuteInEditMode]
    public class CellAnimator : MonoBehaviour
    {
        private bool _startAnimation;
        private float _time;
        private float _waitingTime;
        private GeneratorData _generatorData;

        public Vector3 DestinationPos;
        public float Duration;

        public void Setup(GeneratorData gData, Vector3 start, Vector3 end, float speed, bool vertical)
        {
            _generatorData = gData;
            transform.position = start;
            DestinationPos = end;
            Duration = speed;
            _startAnimation = false;
            _time = 0;
            if (vertical)
            {
                _waitingTime = Duration/_generatorData.Bounds.size.y/Duration*DestinationPos.y;
                //_waitingTime = Duration/ _generatorData.Bounds.size.y*DestinationPos.y/Duration;
            }
            else
            {
                _waitingTime = Duration/_generatorData.Bounds.size.x/Duration*DestinationPos.x;
                //_waitingTime = Duration/ _generatorData.Bounds.size.x*DestinationPos.x/Duration;
            }
            StartCoroutine(StartDelay(_waitingTime, true));
        }

        // Use this for initialization
        private void Start()
        {
        }

        private IEnumerator StartDelay(float val, bool anim)
        {
            yield return new WaitForSeconds(val);
            _startAnimation = anim;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_startAnimation)
            {
                transform.position = Vector3.Lerp(transform.position, DestinationPos, _time);
            }
            if (_time < 1)
            {
                _time += Time.deltaTime/(Duration/1000);
            }
        }
    }
}