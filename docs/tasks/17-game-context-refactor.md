# 17: game-context-refactor

## Description

Исправить GameContext в соответствии с одновлениями в Agents.md
- В GameContext не должно быть систем
- Добавить класс GameBoard
- GameBoard должен содержать данные непосредственно доски, активных противников, позиций оружия и павер апов
- Если есть другие системы и нужны уточнения - задать вопросы

## Comments

**[Copilot, 2026-06-05]:** Discovery complete. Approach: создать `GameBoard` как data entity с активными противниками и граничными клетками для спавна, системы получают его через конструктор. Scope: in — GameBoard + рефакторинг 7 файлов; out — OccupancyMap перенос, позиции оружия/power-ups (не реализованы), GameContext.Instance удаление.