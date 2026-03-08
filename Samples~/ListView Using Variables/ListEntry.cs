using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class ListEntry : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private TMP_Text _title;

        [SerializeField]
        private TMP_Text _date;
        
        public void SetHeight(float height)
        {
            LayoutElement layout = GetComponent<LayoutElement>();
            layout.preferredHeight = height;
        }

        public void SetData(ListEntryData data)
        {
            name = data.title;
            
            _title.text = data.title;
            _date.text = data.timestamp;
        }
    }
}