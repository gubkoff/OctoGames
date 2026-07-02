# OctoGames

## Questions

> **1. Coding Principles** · *Short Answer*
>
> Describe two coding principles or practices you consider most important when working on real Unity projects that mix:
>
> - 3D gameplay
> - UI systems
> - Iteration by designers
>
> Explain why they matter and where you apply them.

The core idea is maximum independence between parts: isolate them from each other and interact through a transparent API. Design, configs, and UI are kept as separate from code as possible so designers can work on them independently; code stays isolated and communicates only through contracts so programmers can work independently too. An event-driven approach and MV* architecture help achieve this. In this project there wasn't enough time for a full event-driven setup, but interaction could later be implemented via something like MessagePipe. I would also highlight DI for better isolation and layer separation.

---

> **2. Save / Load Utility** · *Production Basics*
>
> Many of our projects require persistent data:
>
> - player progress
> - settings
> - VN state
> - gameplay flags
>
> **Task**
>
> Implement a generic save/load utility that:
>
> - Saves any serializable class to file
> - Loads it back safely
> - Handles missing or invalid data gracefully
>
> **Notes**
>
> - You may use JSON serialization
> - Focus on clean API and robustness
> - Assume this utility will be reused across multiple projects

**Implementation:** `Assets/_OctoGames/Scripts/Core/Persistence/`

| Component | Purpose |
|-----------|---------|
| `IPersistence` | Key/value API: `SaveAsync`, `LoadAsync`, `TryLoadAsync`, `ExistsAsync`, `DeleteAsync` |
| `PersistenceService` | Orchestrates serializer + storage |
| `SystemTextJsonDataSerializer` | `System.Text.Json` (not `JsonUtility`) |
| `JsonStorageProvider` | Files in `persistentDataPath/saves/` |

Principles: async via `Task` + `CancellationToken` (layer without Unity API); atomic writes (`.tmp` → replace); `TryLoadAsync` / default on corrupted data — no throws outward; `PersistenceResult` with explicit status.

---

> **3. Popup / UI System** · *UI + Architecture*
>
> Our games use popups for:
>
> - confirmations
> - story choices
> - warnings
> - tutorials
>
> **Task**
>
> Design a simple popup system that supports:
>
> - Loading a popup
> - Setting:
>   - Title text
>   - Body text
>   - Between 1–5 buttons
> - Assigning callbacks to buttons

Relative to the assignment brief, this ended up as a bit of over-engineering, but it produced an interesting solution.

**Core idea:**
- Popups are obtained through a provider that can be swapped via DI; later, popups could be moved to local Addressables to reduce memory pressure
- View is fully separated from ViewModel and wired through a binder. View knows nothing about ViewModel — only the binder does. Both View and Binder live as components on the popup; ViewModel is a plain class working only with models and events
- A request can be passed into a popup when needed, or an empty default can be used

**Architecture:** MVVM + R3, orchestration via `PopupService` (`ShowAsync` / `CloseAsync`), DI — VContainer.

| Layer | Location |
|-------|----------|
| `IPopupService`, `PopupService`, MVVM base | `Scripts/Core/Popups/` |
| Feature popups (Settings, Confirm, EntityDetail) | `Scripts/Application/Features/Popups/` |
| Prefab catalog | `Content/Configs/PopupsCatalog.asset` (`SOPopupProvider`) |

**Loading:** prefab ref from SO catalog by view type (`ShowAsync<TView>` / `ShowAsync<TView, TRequest>`).

**Show parameters:** `*PopupRequest` records with title, body, button labels, and delegates (`OnConfirm`, `OnCancel`, …). Example: `ConfirmPopupRequest`.

**Queue and policies:** FIFO; `PopupShowPolicy` (Queue / ReuseIfOpen / Replace).

**Implemented popups:** `SettingsPopup`, `ConfirmPopup`, `EntityDetailPopup`.

> **3.1 Unity Components Question**
>
> Which Unity components would you use to build the popup prefab, and why?

I used UGUI because I believe UI Toolkit is still too early for large production solutions.

---

> **4. UI Performance & Refactoring** · *Core Unity Skill*
>
> In one of our scenes, the UI shows live information about active gameplay entities.  
> A junior developer wrote the following code that:
>
> - Produces incorrect results
> - Causes performance issues
> - Updates far too often

```csharp
public class CharactersView : MonoBehaviour
{
    [SerializeField] private List<Transform> _characters;

    void FixedUpdate()
    {
        float totalValue = 0f;

        foreach (Transform characterTransform in _characters)
        {
            Character character =
                characterTransform.gameObject.GetComponents<Character>();

            totalValue += character != null ? character.Value : 0f;
        }

        string text = string.Format(
            "Characters: {0} Avg value: {1}",
            _characters.Length,
            _characters.Length / totalValue
        );

        gameObject.GetComponent<Text>().text = text;
        Debug.Log(text);
    }
}
```

> **Your Goals**
>
> 1. Fix bugs and logical errors
> 2. Improve code quality and structure
> 3. Optimize performance (practical + theoretical)
> 4. Limit UI updates to once every X frames or a fixed interval
> 5. Briefly explain why you made your changes
>
> You may rewrite the code entirely.

**How I understood the system logic:** we have a fixed list of objects; a `Character` component can be added or removed on them, and a `Value` can be set. If an object has a `Character` component, it counts as active and its `Value` participates in computing some aggregate.

**What was done:**
- fixed the average calculation formula
- cached the text element
- added UI updates once per second; interval is configurable in the editor
- added a conditional guard for `Debug.Log` because it is an expensive operation and can affect production performance
- added null checks
- replaced `FixedUpdate` with `Update` because this is UI, not physics

**Alternative approach:**
The project includes `BaseHUD` and `BaseHUDViewModel` — you can look there for my implementation of displaying active entities.

---

> **5. Gameplay / State Logic** · *3D + Systems Thinking*
>
> This task focuses on gameplay logic, not UI.
>
> **Context**
>
> We have multiple gameplay entities in a scene (e.g. enemies, interactables, story actors).  
> Some of them become inactive due to gameplay events (destroyed, disabled, completed, etc.).
>
> **Task**
>
> Design and implement a method or small system that:
>
> - Tracks gameplay entities
> - Returns only active entities
> - Cleanly handles:
>   - entities being removed
>   - entities being disabled
> - Is safe and readable for production use
>
> You may choose:
>
> - OOP approach
> - Event-driven approach
> - Simple manager or service
>
> Explain your reasoning briefly.

**Approach:** Repository + services (OOP), push events at boundaries. A move toward event-driven interaction via MessagePipe is possible later. The idea is to keep services isolated from each other.

| Component | Role |
|-----------|------|
| `IRepository<IGameplayEntity>` | Entity registry by `Guid`; `Add` / `Remove` / `TryGet` / `GetAll`; `Changed` event |
| `IGameplayEntity.IsActive` | `State == Active` — filter for active only |
| `GameplayEntityService` | Load / save / reset / add; snapshot ↔ `IPersistence` |
| `GameplayEntityStateService` | State transitions; `Changed` event on state change |
| `GameplayEntitySpawner` | Spawn/despawn from prefab catalog |

**Active entities:** `repository.GetAll()` + filter `entity.IsActive` (see `BaseHUDViewModel`).

**Removed:** `DestroyEntity` → `Despawn` → `Remove` from repository → `Changed`.

**Disabled / Completed:** `ApplyState` → entity stays in repository, but `IsActive == false`.

---

> **Bonus** · *Optional*
>
> - How would you scale these systems for larger projects?

There is already a foundation for scaling. UI and prefabs could be moved to Addressables — local and downloadable — to reduce memory pressure and support LiveOps; configs could be moved to remote for the same LiveOps use case.

> - How would designers interact with this code?

Designers can work freely with prefabs; views are fully separated from code. They can also edit configs independently.

> - How would you profile or debug performance issues?

- Unity Profiler: CPU Usage, GC Alloc in hot paths
- Frame Debugger — unnecessary UI redraws
- Memory Profiler — leaks on Addressables load/release
- Profiling on real devices as well
