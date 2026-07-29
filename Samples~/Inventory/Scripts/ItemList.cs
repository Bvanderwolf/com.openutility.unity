using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "ItemList", menuName = "OpenUtility/Inventory/ItemList")]
    public class ItemList : ScriptableList<ScriptableItem>
    {
    }
}