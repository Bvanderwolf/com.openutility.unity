using System;
using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableInt _health;
        
        [Header("Scene References")]
        [SerializeField]
        private GameObject _gameOverScreen;

        private void Awake()
        {
            _health.ValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int newValue)
        {
            if (newValue <= 0)
                _gameOverScreen.SetActive(true);
        }
    }
}
