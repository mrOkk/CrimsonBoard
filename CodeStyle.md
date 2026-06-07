# Code Style

## Braces

Always use braces for control flow statements:

```csharp
if (condition)
{
    DoSomething();
}

for (var i = 0; i < count; i++)
{
    Process(i);
}

while (isRunning)
{
    Update();
}
```

## Class/Struct Member Order

Within a class or struct, organize members in this order:

1. Nested types (enums, structs, classes)
2. Constants
3. Static events
4. Static properties
5. Static fields
6. Static methods
7. Events
8. Properties
9. Fields
10. Constructors
11. Methods

Within each group, sort by access modifier from most to least accessible:
- `public`
- `protected`
- `private`

```csharp
public class Example
{
    public enum State { Idle, Active }

    public const int MaxCount = 100;

    public event Action OnComplete;

    public int Value { get; set; }
    protected int InternalValue { get; set; }
    private int _counter;

    public readonly int ReadOnlyField;
    private readonly List<int> _items;

    public Example() { }

    public void Execute() { }
    public void Reset() { }
    protected void OnStateChanged() { }
    private void Validate() { }
}
```

## Access Modifiers

Always explicitly specify access modifiers:

```csharp
public void DoWork() { }
private int _count;
protected virtual void OnUpdate() { }
```

## Var Keyword

Use `var` when the type is obvious from the right side:

```csharp
var player = GetComponent<PlayerView>();
var enemies = new List<EnemyView>();
var position = transform.position;

int damage = CalculateDamage();
float healthRatio = currentHealth / maxHealth;
```
