using UnityEditor;
using UnityEngine;

namespace CrimsonBoard.Editor
{
    [CustomPropertyDrawer(typeof(WeaponIdAttribute))]
    public class WeaponIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "[WeaponId] requires int field");
                return;
            }

            var config = GameConfig.EditorInstance;
            if (config == null || config.weapons == null || config.weapons.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var weapons = config.weapons;
            var names = new string[weapons.Length];
            var ids = new int[weapons.Length];
            int selectedIndex = 0;
            for (int i = 0; i < weapons.Length; i++)
            {
                ids[i] = weapons[i].id;
                names[i] = $"{weapons[i].name} (id={weapons[i].id})";
                if (ids[i] == property.intValue) selectedIndex = i;
            }

            EditorGUI.BeginProperty(position, label, property);
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, names);
            property.intValue = ids[newIndex];
            EditorGUI.EndProperty();
        }
    }
}
