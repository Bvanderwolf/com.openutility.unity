using OpenUtility.Data;
using TMPro;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class GradientText : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private TMP_Text _renderer;

        [SerializeField]
        private FloatRange _range;
        
        [SerializeField] 
        private Gradient _gradient;

        public void SetGradient(int value)
        {
            if (_renderer == null)
                return;

            if (_gradient == null)
                return;

            float time = _range.GetPercentage(value);
            _renderer.color = _gradient.Evaluate(time);
        }
        
        public void SetGradient(float value)
        {
            if (_renderer == null)
                return;

            if (_gradient == null)
                return;

            float time = _range.GetPercentage(value);
            _renderer.color = _gradient.Evaluate(time);
        }
    }
}