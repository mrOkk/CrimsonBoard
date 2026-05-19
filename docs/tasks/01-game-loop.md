Подготовить игровой цикл
Все должно начинаться с EntryPoint
У игры несколько состояний:
- Инициализация. Она же запуск, загрузка ресурсов подготовка UI и конфигов. Ее мы проходим единожды и больше не возвращаемся
- Tap to start. Игровое поле и игрок уже готовы, остается только начать
- Кор геймплей. Ecs-like набор систем, точный список будет описан далее
- Пауза с возможность поменять какие-то настройки (нужно давать возможность перейти сюда из любой стадии после инициализации)
- Завершение игры. Экран победы/смерти, возможность перезапустить

## Comments

**[Copilot, 2026-05-19]:** Discovery complete. Approach: single bootstrap scene with `EntryPoint` MonoBehaviour owning a plain C# FSM (`GameStateMachine`) and a `GameContext` singleton; five state stubs + `GameplaySystemRunner` with `IGameSystem` interface for future ECS-like systems. Scope in: FSM skeleton, all 5 states, context, system runner, asmdef. Scope out: actual gameplay systems, UI, asset loading, configs.