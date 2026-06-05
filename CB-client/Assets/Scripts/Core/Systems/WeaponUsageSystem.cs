using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CrimsonBoard
{
    public class WeaponUsageSystem : IGameSystem
    {
        private readonly GameContext _context;
        private readonly Dictionary<int, WeaponView> _equippedWeapons = new();
        private readonly HashSet<int> _attachedIds = new();
        private int? _activeWeaponId;
        private bool _isSwitching;
        private float _shotTimer;
        private UniTask _switchTask;
        private CancellationTokenSource _switchCts;
        private Transform _weaponLocator;
        private bool _initialized;

        public WeaponUsageSystem(GameContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _weaponLocator = _context.Player.WeaponLocator;
            EnsureWeaponsAttached();
            UpdateActiveWeapon();
        }

        public void Tick(float deltaTime)
        {
            if (_isSwitching) return;

            EnsureWeaponsAttached();
            UpdateActiveWeapon();

            var activeId = _context.Inventory.ActiveWeaponId;
            if (activeId == null) return;

            var cfg = GetWeaponConfig(activeId.Value);
            if (cfg == null) return;

            HideAllWeaponsExcept(activeId.Value);

            if (!CanShoot(cfg)) return;

            var nearest = FindNearestEnemyInRange(cfg.range);
            if (nearest == null) return;

            RotateTowardsEnemy(nearest, cfg.rotationSpeed, deltaTime);

            _shotTimer -= deltaTime;
            if (_shotTimer <= 0f)
            {
                TryFireShot(cfg, nearest);
                _shotTimer = _context.Config.timing.beatDuration / cfg.shotsPerBeat;
            }
        }

        public void Dispose()
        {
            _switchCts?.Cancel();
            _switchCts?.Dispose();
            _switchCts = null;

            foreach (var kv in _equippedWeapons)
            {
                var pool = _context.Pools.GetWeaponPool(kv.Key);
                pool?.Return(kv.Value);
            }
            _equippedWeapons.Clear();
            _attachedIds.Clear();
            _activeWeaponId = null;
            _initialized = false;
        }

        private void EnsureWeaponsAttached()
        {
            foreach (var wid in _context.Inventory.WeaponIds)
            {
                if (!_attachedIds.Contains(wid))
                {
                    AttachWeapon(wid);
                }
            }
        }

        private void AttachWeapon(int weaponId)
        {
            var pool = _context.Pools.GetWeaponPool(weaponId);
            if (pool == null) return;

            var wv = pool.Get();
            wv.transform.SetParent(_weaponLocator, false);
            wv.transform.localPosition = wv.PlayerAttachPoint != null
                ? _weaponLocator.InverseTransformPoint(wv.PlayerAttachPoint.position)
                : Vector3.zero;
            wv.transform.localRotation = Quaternion.identity;
            wv.SetEquippedMode();

            _equippedWeapons[weaponId] = wv;
            _attachedIds.Add(weaponId);
        }

        private void UpdateActiveWeapon()
        {
            var newActive = _context.Inventory.ActiveWeaponId;
            if (newActive == _activeWeaponId) return;

            _switchCts?.Cancel();
            _switchCts = new CancellationTokenSource();
            _switchTask = SwitchWeaponRoutine(_activeWeaponId, newActive, _switchCts.Token);
            _activeWeaponId = newActive;
        }

        private async UniTask SwitchWeaponRoutine(int? oldId, int? newId, CancellationToken ct)
        {
            _isSwitching = true;

            if (oldId != null && _equippedWeapons.TryGetValue(oldId.Value, out var oldWv))
            {
                var oldCfg = GetWeaponConfig(oldId.Value);
                if (oldCfg != null && oldWv.RotationPoint != null)
                    await AnimateRotation(oldWv, -90f, oldCfg.holsterTime, ct);
                oldWv.gameObject.SetActive(false);
            }

            if (newId != null && _equippedWeapons.TryGetValue(newId.Value, out var newWv))
            {
                newWv.gameObject.SetActive(true);
                var newCfg = GetWeaponConfig(newId.Value);
                if (newCfg != null && newWv.RotationPoint != null)
                {
                    SetLocalRotationAroundPoint(newWv, newWv.RotationPoint, -90f);
                    await AnimateRotation(newWv, 0f, newCfg.drawTime, ct);
                }
            }

            _isSwitching = false;
        }

        private async UniTask AnimateRotation(WeaponView wv, float targetAngle, float duration, CancellationToken ct)
        {
            if (wv.RotationPoint == null) return;

            float startAngle = GetLocalRotationAroundPoint(wv, wv.RotationPoint);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                ct.ThrowIfCancellationRequested();
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float angle = Mathf.Lerp(startAngle, targetAngle, t);
                SetLocalRotationAroundPoint(wv, wv.RotationPoint, angle);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            SetLocalRotationAroundPoint(wv, wv.RotationPoint, targetAngle);
        }

        private float GetLocalRotationAroundPoint(WeaponView wv, Transform pivot)
        {
            var toWeapon = wv.transform.position - pivot.position;
            var localDir = pivot.InverseTransformDirection(toWeapon.normalized);
            return Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        }

        private void SetLocalRotationAroundPoint(WeaponView wv, Transform pivot, float angleX)
        {
            Vector3 pivotPos = pivot.position;
            Vector3 localPos = wv.transform.InverseTransformPoint(pivotPos);
            wv.transform.RotateAround(pivotPos, pivot.right, angleX - GetLocalRotationAroundPoint(wv, pivot));
        }

        private void HideAllWeaponsExcept(int? exceptId)
        {
            foreach (var kv in _equippedWeapons)
            {
                kv.Value.gameObject.SetActive(kv.Key == exceptId);
            }
        }

        private bool CanShoot(WeaponConfig cfg)
        {
            if (_context.InputState.MoveCommand != null) return false;
            if (!cfg.infiniteAmmo && _context.Inventory.GetAmmo(cfg.id) <= 0) return false;
            return true;
        }

        private EnemyView FindNearestEnemyInRange(float range)
        {
            EnemyView nearest = null;
            float nearestDist = range * range;

            var playerPos = _context.Player.transform.position;
            var spawnSystem = _context.EnemySpawnSystem;
            if (spawnSystem == null) return null;

            foreach (var enemy in spawnSystem.ActiveEnemies)
            {
                if (enemy == null || enemy.Health.IsDead) continue;
                float dist = Vector3.SqrMagnitude(playerPos - enemy.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        private void RotateTowardsEnemy(EnemyView enemy, float rotationSpeed, float deltaTime)
        {
            var dir = (enemy.transform.position - _context.Player.transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;

            var targetRot = Quaternion.LookRotation(dir);
            _context.Player.transform.rotation = Quaternion.RotateTowards(
                _context.Player.transform.rotation, targetRot, rotationSpeed * deltaTime * 360f);
        }

        private void TryFireShot(WeaponConfig cfg, EnemyView target)
        {
            if (!_equippedWeapons.TryGetValue(cfg.id, out var wv)) return;

            var muzzle = wv.MuzzlePoint;
            if (muzzle == null) return;

            var dir = (target.transform.position - muzzle.position).normalized;
            dir = ApplySpread(dir, cfg.spread);

            var proj = _context.Pools.Projectiles.Get();
            proj.transform.position = muzzle.position;
            proj.transform.rotation = Quaternion.LookRotation(dir);
            proj.Launch(dir, 30f, cfg.damage, cfg.maxTargetsPerBullet, cfg.range);

            if (!cfg.infiniteAmmo)
                _context.Inventory.AddAmmo(cfg.id, -1);
        }

        private Vector3 ApplySpread(Vector3 dir, float spread)
        {
            if (spread <= 0f) return dir;
            float angle = Random.Range(-spread, spread);
            return Quaternion.Euler(0f, angle, 0f) * dir;
        }

        private WeaponConfig GetWeaponConfig(int weaponId)
        {
            foreach (var w in _context.Config.weapons)
                if (w.id == weaponId) return w;
            return null;
        }
    }
}
