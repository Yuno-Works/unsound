using System.Collections;
using UnityEngine;

namespace SilverDogGames.Audio
{
    public class BeatVisualizer : BaseAudioSyncer
    {
        public Vector3 BeatScale;
        public Vector3 RestScale;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (m_isBeat) return;

            transform.localScale = Vector3.Lerp(transform.localScale, RestScale, restSmoothTime * Time.deltaTime);
        }

        public override void OnBeat(AudioSourceData? sourceData)
        {
            base.OnBeat(sourceData);

            StopAllCoroutines();
            StartCoroutine(MoveToScale(BeatScale));
        }

        private IEnumerator MoveToScale(Vector3 target)
        {
            Vector3 currentScale = transform.localScale;
            Vector3 initialScale = currentScale;
            float timer = 0;

            while (currentScale != target)
            {
                currentScale = Vector3.Lerp(initialScale, target, timer / timeToBeat);
                timer += Time.deltaTime;

                transform.localScale = currentScale;

                yield return null;
            }

            m_isBeat = false;
        }
    }
}
