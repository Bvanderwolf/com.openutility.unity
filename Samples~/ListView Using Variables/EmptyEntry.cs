using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class EmptyEntry : MonoBehaviour
    {
        public void SetHeight(float height)
        {
            LayoutElement layout = GetComponent<LayoutElement>();
            layout.preferredHeight = height;
        }
    }
}