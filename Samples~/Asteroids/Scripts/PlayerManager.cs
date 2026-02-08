using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _playArea;

        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _player;

        private void Start()
        {
            _player.SetParent(_playArea);
            _player.CreateValue();
        }
    }
}
