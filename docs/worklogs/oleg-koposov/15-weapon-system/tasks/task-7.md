# Task 7: WeaponId PropertyDrawer

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Core/Configs/GameConfig.cs`
- Create: `CB-client/Assets/Scripts/Core/Configs/WeaponIdAttribute.cs`
- Create: `CB-client/Assets/Scripts/Editor/CrimsonBoard.Editor.asmdef`
- Create: `CB-client/Assets/Scripts/Editor/WeaponIdDrawer.cs`

**Commit message:** 15 WeaponId PropertyDrawer: [WeaponId] attribute with dropdown by weapon name

### Steps

1. **GameConfig.cs** — добавить `#if UNITY_EDITOR` блок в конец класса (перед закрывающей `}`):
   ```csharp
   #if UNITY_EDITOR
   private static GameConfig _editorInstance;

   public static GameConfig EditorInstance
   {
       get
       {
           if (_editorInstance != null) return _editorInstance;
           var guids = UnityEditor.AssetDatabase.FindAssets("t:GameConfig");
           if (guids.Length == 0) return null;
           var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
           _editorInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<GameConfig>(path);
           return _editorInstance;
       }
   }
   #endif
   ```

2. **WeaponIdAttribute.cs** — создать в `Core/Configs/`:
   ```csharp
   namespace CrimsonBoard
   {
       public class WeaponIdAttribute : UnityEngine.PropertyAttribute { }
   }
   ```

3. Создать папку `CB-client/Assets/Scripts/Editor/` и в ней файл **CrimsonBoard.Editor.asmdef**:
   ```json
   {
       "name": "CrimsonBoard.Editor",
       "rootNamespace": "CrimsonBoard.Editor",
       "references": ["CB-client"],
       "includePlatforms": ["Editor"],
       "excludePlatforms": [],
       "allowUnsafeCode": false,
       "overrideReferences": false,
       "precompiledReferences": [],
       "autoReferenced": true,
       "defineConstraints": [],
       "versionDefines": [],
       "noEngineReferences": false
   }
   ```

4. **WeaponIdDrawer.cs** — создать в `Scripts/Editor/`:
   ```csharp
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
   ```

## Implementation
**Status:** DONE
**Summary:** `GameConfig.EditorInstance` — кешированный поиск через `AssetDatabase`. Создан `WeaponIdAttribute` (marker). Создана новая Editor-сборка `CrimsonBoard.Editor` с `WeaponIdDrawer` — показывает выпадающий список weapon names, хранит id.
