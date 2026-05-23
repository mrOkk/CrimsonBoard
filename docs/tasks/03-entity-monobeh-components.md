# 03: entity-monobeh-components

## Description

Подготовить задачу на базовые MonoBehaviour-компоненты игровых сущностей и их специализированные варианты.
Заменить GameObject в конфиге префабов.

Требования по сущностям:
- Общий класс игровой сущности (база для игрока и врага):
  - `MeshFilter`;
  - `Rigidbody`;
  - `Collider`.
- Игрок:
  - `Transform`-локатор оружия.
- Враг:
  - компонент на базе общего класса сущности.
- Оружие:
  - `Transform`-локатор крепления к игроку;
  - меш;
  - `Transform`-локатор дула.
- Павер-ап:
  - `SpriteRenderer`;
  - `Collider`.
- Тайл доски
  - `MeshFilter`
  

Ожидаемый результат:
- заготовки компонентов и/или префабов для перечисленных типов объектов;
- единообразная структура данных для дальнейшей интеграции с конфигами и пулами.

## Comments

**[Copilot, 2026-05-19]:** Добавлена заготовка задачи на MonoBehaviour-компоненты для базовой сущности, игрока, врага, оружия и павер-апов.

**[Copilot, 2026-05-23]:** Discovery complete. Approach: иерархия наследования MonoBehaviour (EntityView → PlayerView/EnemyView) + типизированные ссылки в PrefabsConfig. Scope in: 6 новых C#-классов в Scripts/Entities/, правка PrefabsConfig. Scope out: .prefab-файлы, projectilePrefab, логика поведения.
