using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    [RequireComponent(typeof(Text))]
    public class SimpleTextTweener : MonoBehaviour
    {
        private Text text;

        [SerializeField]
        private Color startColor;
        
        [SerializeField]
        private Color endColor;

        [SerializeField]
        private AnimationCurve curve;

        [SerializeField]
        private float duration = 1f;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Tween());
        }

        private IEnumerator Tween()
        {
            var elapsed = 0f;
            while (elapsed <= duration)
            {
                var t = Mathf.Clamp01(elapsed / duration);
                text.color = Color.Lerp(startColor, endColor, curve.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
