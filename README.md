# Pocket Pal

A lightweight virtual desktop pet for Windows 11. Runs as a transparent,
click-through, borderless overlay that walks/runs/idles/sleeps/jumps along
the bottom of your screen, built as a clean foundation for a much larger
project.

## Tech stack

- C# / .NET 8
- WPF (chosen over WinUI 3 for simpler, more reliable click-through /
  transparent-window support and single-file `.exe` deployment)
- Sprite-sheet animation, fixed at 8 FPS
- Win32 interop for click-through + multi-monitor via `System.Windows.Forms.Screen`

## Requirements to build

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (17.8+) with the ".NET desktop development" workload,
  **or** just the `dotnet` CLI

> This project was authored and organized outside of Windows, so it has not
> been compiled/run in this environment. The code follows standard .NET 8 /
> WPF patterns throughout, but do a build on your machine as the first step
> and report back if anything doesn't compile — happy to fix immediately.

## Running it

```
cd PocketPal
dotnet restore
dotnet run --project PocketPal
```

Or open `PocketPal.sln` in Visual Studio and press F5.

On launch you'll see a small orange placeholder blob-critter walking,
running, idling, sitting, sleeping, and jumping along the bottom of your
primary monitor. Right-click the system tray icon to exit.

## Project layout

```
PocketPal/
  Assets/
    AssetLoader/SpriteAssetLoader.cs   - scans Sprites/ folders, loads PNG frames
    AnimationLibrary.cs                - maps PetState + Direction -> AnimationClip
    Sprites/
      Idle/  Walk_Left/  Walk_Right/  Run_Left/  Run_Right/
      Sit/   Sleep/      Jump/        Fall/
        frame_000.png, frame_001.png, ...
  Animation/
    AnimationClip.cs      - an ordered set of frames
    AnimationPlayer.cs     - plays a clip at a fixed FPS (default 8), independent
                              of how often the game loop ticks
  Core/
    GameLoop.cs            - ~60Hz composition-target tick, drives Update(dt)
    PetEngine.cs            - composition root: state machine -> animation -> render
  Models/
    PetStateType.cs, AnimationKey.cs, Direction.cs, Vector2D.cs
  Movement/
    MovementController.cs   - position/velocity, gravity, edge-of-screen turning
  Native/
    NativeMethods.cs        - Win32 click-through / tool-window styles
  Rendering/
    PetRenderer.cs           - dumb "paint the current frame" layer, no logic
  Settings/
    AppSettings.cs, SettingsManager.cs - JSON persistence in %AppData%\PocketPal
  StateMachine/
    IPetState.cs, PetContext.cs, PetStateMachine.cs
    States/ IdleState, WalkingState, RunningState, SittingState,
            SleepingState, JumpingState, FallingState, PetBehaviorPicker
  Tray/
    TrayIconManager.cs      - system tray icon + Exit menu
  Utilities/
    ScreenHelper.cs          - multi-monitor work-area lookup + resolution-change events
  MainWindow.xaml(.cs)       - transparent borderless window, wires everything together
  App.xaml(.cs)

tools/
  generate_placeholder_sprites.py   - regenerates the placeholder blob art
```

### Why it's split up this way

Each folder maps to one responsibility, and none of them know much about
the others:

- **States** decide *what* the pet is doing and *when* to stop doing it.
- **MovementController** decides *where* the pet is, given a state's request
  to move or jump. It has no idea what a "state" is.
- **AnimationPlayer** decides *which frame* to show right now, on a fixed
  clock, and has no idea what a "pet" is.
- **PetRenderer** just paints whatever it's told. It has no idea what a
  "state," "animation," or "movement" even means.
- **PetEngine** is the only class that talks to all of them, once per tick,
  in a fixed order: state → animation → render.

This means, for example, a future "Hunger" system can watch `PetEngine`'s
state changes and drain hunger while `Walking`/`Running`, without needing
to touch `MovementController`, `AnimationPlayer`, or any state class.

## Adding a new animation (no code changes)

Drop `frame_000.png`, `frame_001.png`, ... into
`Assets/Sprites/<Name>/`. Any frame count works — `AnimationClip` reads
whatever is there. To use it, you only need code changes if you're adding
a **brand-new** state (see below); replacing/expanding an *existing*
animation's frames requires zero code changes.

## Adding a new state

1. Add a value to `PetStateType` (Models/PetStateType.cs).
2. Add a value to `AnimationKey` if it needs new art, and a matching
   `Assets/Sprites/<Name>/` folder.
3. Create a class implementing `IPetState` in `StateMachine/States/`.
4. Map it in `AnimationLibrary.Resolve()`.
5. Have `PetBehaviorPicker` (or whatever triggers it) return it.

No other file needs to change.

## Regenerating placeholder art

```
cd tools
pip install pillow
python generate_placeholder_sprites.py
```

This overwrites everything in `Assets/Sprites/`. Delete/replace any
folder's PNGs with real art at any time — nothing else needs to change.

## Known placeholders / next steps

- **Art**: current sprites are simple generated placeholders (a bobbing
  orange blob), meant only to prove the pipeline end-to-end. Swap in real
  pixel art whenever it's ready.
- **Interactivity**: the window is click-through by design (per your
  choice). `Native/NativeMethods.cs` already has a `MakeInteractive()`
  method ready for when you want clicking/dragging/petting.
- **Multi-monitor**: currently the pet lives on one monitor
  (`AppSettings.PreferredMonitorIndex`, -1 = primary). Spawning one
  `MainWindow` per monitor, or multiple pets, is a natural next step and
  the architecture doesn't need to change to support it — `PetEngine` has
  no static/singleton state.
- **Settings persistence** already round-trips through
  `%AppData%\PocketPal\settings.json` and has unused fields reserved
  (`Hunger`, `Happiness`, `SoundEnabled`) so those future systems have a
  home to write to right away.
- **Sounds, save files, plugins/mods**: not implemented yet, but nothing
  in the current design (fixed 8 FPS clock, decoupled states, folder-based
  assets) will need to be reworked to add them — they're additive.
