# 12: ui-system-iteration-1

## Description

Реализовать UI через единый фасад (первая итерация).

Требования:
- в первой версии все UI-элементы хранятся в одном префабе;
- фасад представляет собой MonoBehaviour-класс со ссылками на все View;
- для View нужен единый абстрактный MonoBehaviour с возможностью `Show()` и `Hide()`;
- при старте фасада все View регистрируются по типу;
- фасад должен уметь открывать View по типу;
- обновление UI вызывается из `EntryPoint` после `_fsm.Tick(...)`.

Ожидаемый результат:
- базовая инфраструктура UI-фасада и абстрактных View;
- регистрация и открытие View по типу;
- интеграционная точка обновления UI в игровом цикле через `EntryPoint`.

## Comments

**[User, 2026-05-30]:** Добавить следующую задачу: UI system. Iteration 1.

**[Copilot, 2026-05-30]:** Discovery complete. Approach: единый фасад `UiRoot` (MonoBehaviour) с авто-регистрацией View по типу через `GetComponentsInChildren`. Scope in: BaseView, UiRoot, GameContext.UiRoot, EntryPoint-интеграция. Scope out: конкретные View, UI-префаб, привязка состояний.
