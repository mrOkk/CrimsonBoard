# Task 1: Entity view scripts

## Plan

**Files:**
- Create: `CB-client/Assets/Scripts/Entities/EntityView.cs`
- Create: `CB-client/Assets/Scripts/Entities/PlayerView.cs`
- Create: `CB-client/Assets/Scripts/Entities/EnemyView.cs`
- Create: `CB-client/Assets/Scripts/Entities/WeaponView.cs`
- Create: `CB-client/Assets/Scripts/Entities/PowerUpView.cs`
- Create: `CB-client/Assets/Scripts/Entities/BoardTileView.cs`

**Commit message:** `03 Add entity MonoBehaviour view scripts`

### Steps

1. Создать папку `CB-client/Assets/Scripts/Entities/`.

2. Создать `EntityView.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class EntityView : MonoBehaviour
       {
           [SerializeField] private MeshFilter _meshFilter;
           [SerializeField] private Rigidbody _rigidbody;
           [SerializeField] private Collider _collider;

           public MeshFilter MeshFilter => _meshFilter;
           public Rigidbody Rigidbody => _rigidbody;
           public Collider Collider => _collider;
       }
   }
   ```

3. Создать `PlayerView.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class PlayerView : EntityView
       {
           [SerializeField] private Transform _weaponLocator;

           public Transform WeaponLocator => _weaponLocator;
       }
   }
   ```

4. Создать `EnemyView.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class EnemyView : EntityView
       {
       }
   }
   ```

5. Создать `WeaponView.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class WeaponView : MonoBehaviour
       {
           [SerializeField] private MeshFilter _meshFilter;
           [SerializeField] private Transform _playerAttachPoint;
           [SerializeField] private Transform _muzzlePoint;

           public MeshFilter MeshFilter => _meshFilter;
           public Transform PlayerAttachPoint => _playerAttachPoint;
           public Transform MuzzlePoint => _muzzlePoint;
       }
   }
   ```

6. Создать `PowerUpView.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class PowerUpView : MonoBehaviour
       {
           [SerializeField] private SpriteRenderer _spriteRenderer;
           [SerializeField] private Collider _collider;

           public SpriteRenderer SpriteRenderer => _spriteRenderer;
           public Collider Collider => _collider;
       }
   }
   ```

7. Создать `BoardTileView.cs`:
   ```csharp
   using UnityEngine;

   namespace CrimsonBoard
   {
       public class BoardTileView : MonoBehaviour
       {
           [SerializeField] private MeshFilter _meshFilter;

           public MeshFilter MeshFilter => _meshFilter;
       }
   }
   ```

## Implementation
**Status:** DONE
**Summary:** Создано 6 файлов в `Scripts/Entities/`: EntityView (поля MeshFilter/Rigidbody/Collider), PlayerView (+ WeaponLocator), EnemyView (пустой), WeaponView (MeshFilter/PlayerAttachPoint/MuzzlePoint), PowerUpView (SpriteRenderer/Collider), BoardTileView (MeshFilter). Отклонений от плана нет.
