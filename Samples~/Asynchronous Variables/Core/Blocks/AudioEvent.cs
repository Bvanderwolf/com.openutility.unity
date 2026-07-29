using OpenUtility.Data.Events;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName = "OpenUtility/Scriptable Event/Audio")]
    public class AudioEvent : ScriptableEvent<AudioClip>
    {
    }
}