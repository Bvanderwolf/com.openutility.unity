using System.Collections;
using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using OpenUtility.UI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class Carousel : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private Image _image;
        
        [Header("Project References")]
        [SerializeField]
        private TextureList _textures;

        [Header("Settings")]
        [SerializeField]
        private string[] _fileNames;

        [SerializeField, Min(0.5f)]
        private float _timePerImage = 1f;

        private IEnumerator Start()
        {
            while (true)
            {
                for (int i = 0; i < _fileNames.Length; i++)
                {
                    Promised<Texture2D> promise = _textures.AddFromFile(_fileNames[i]);
                    yield return promise.Yield();

                    if (promise.HasError)
                    {
                        Debug.LogError($"Failed to load texture from file '{_fileNames[i]}': {promise.Error}");
                        continue;
                    }
                
                    Sprite sprite = TextureManager.TextureToSprite(promise.Value);
                    _image.sprite = sprite;

                    yield return FadeInImage();
                    
                    yield return WaitFor.Seconds(_timePerImage);

                    yield return FadeOutImage();
                }
            }
        }

        private IEnumerator FadeInImage()
        {
            float duration = _timePerImage * 0.5f;
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;

                float percentage = currentTime / duration;
                float alpha = percentage * 255f;
                
                _image.SetTransparency(alpha);

                yield return null;
            }
        }

        private IEnumerator FadeOutImage()
        {
            float duration = _timePerImage * 0.5f;
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;

                float percentage = 1f - (currentTime / duration);
                float alpha = percentage * 255f;
                
                _image.SetTransparency(alpha);

                yield return null;
            }
        }
    }
}
