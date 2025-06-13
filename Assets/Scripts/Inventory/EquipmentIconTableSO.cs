using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(menuName = "DB/EquipmentIconTable")]
    public class EquipmentIconTableSO : ScriptableObject
    {
        [System.Serializable]
        public class IconEntry
        {
            public string id;        // TSV의 id와 일치
            public Sprite sprite;
        }
        public List<IconEntry> icons;

        private Dictionary<string, Sprite> _iconMap;
        public Sprite GetSprite(string id)
        {
            if (_iconMap == null)
            {
                _iconMap = new Dictionary<string, Sprite>();
                foreach(var entry in icons)
                    _iconMap[entry.id] = entry.sprite;
            }
            return _iconMap.TryGetValue(id, out var s) ? s : null;
        }
    }
}