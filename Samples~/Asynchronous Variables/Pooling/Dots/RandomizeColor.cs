using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeColor : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    private void Start()
    {
        _renderer.color = Random.ColorHSV();
    }
}
