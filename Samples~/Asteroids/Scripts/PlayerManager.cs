using System.Collections.Generic;
using OpenUtility.Data.Pooling;
using OpenUtility.Samples.Data;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField]
    private RectTransform _playArea;
    
    [Header("Project References")]
    [SerializeField]
    private ScriptableGameObject _player;

    [SerializeField]
    private PoolGameObjectList _asteroids;

    private GameObject _instance;
    
    private void Start()
    {
        _player.SetParent(_playArea);
        _instance = _player.CreateValue();
    }

    private void FixedUpdate()
    {
        IList<PoolGameObject> list = _asteroids.GetValue();

        for (int i = 0; i < list.Count; i++)
        {
            
        }
    }
}
