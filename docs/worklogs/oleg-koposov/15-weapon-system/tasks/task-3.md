# Task 3: WeaponView pickup mode

## Plan

**Files:**
- Modify: `CB-client/Assets/Scripts/Entities/WeaponView.cs`

**Commit message:** 15 WeaponView: weapon id, hover height, pickup trigger, dropped/equipped mode

### Steps

1. Добавить сериализованные поля и публичные свойства:
   ```csharp
   [SerializeField] private Collider _pickupCollider;
   [SerializeField] private float _hoverHeight = 1f;

   public int WeaponId { get; private set; }
   public float HoverHeight => _hoverHeight;
   ```

2. Добавить публичный метод `SetWeaponId` (вызывается из пула через onCreate):
   ```csharp
   public void SetWeaponId(int id) => WeaponId = id;
   ```

3. Добавить event и обработчик Unity-триггера:
   ```csharp
   public System.Action<WeaponView, Collider> TriggerEntered;

   private void OnTriggerEnter(Collider other) => TriggerEntered?.Invoke(this, other);
   ```

4. Добавить методы переключения режимов:
   ```csharp
   public void SetDroppedMode(Vector3 basePosition)
   {
       transform.position = basePosition + Vector3.up * _hoverHeight;
       if (_pickupCollider != null)
       {
           _pickupCollider.isTrigger = true;
           _pickupCollider.enabled = true;
       }
   }

   public void SetEquippedMode()
   {
       if (_pickupCollider != null)
           _pickupCollider.enabled = false;
       TriggerEntered = null;
   }
   ```
   Прецедент: аналогичная активация/деактивация через `enabled` — в `ObjectPool<T>.Get/Return`.

## Implementation
**Status:** DONE
**Summary:** Добавлены `WeaponId`, `_hoverHeight`, `_pickupCollider`, event `TriggerEntered`, методы `SetWeaponId`, `SetDroppedMode`, `SetEquippedMode`, обработчик `OnTriggerEnter`.
