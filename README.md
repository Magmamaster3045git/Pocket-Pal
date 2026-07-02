# Pocket Pal 🐾

(please note that the following is a work in progress and may not be completed yet)

*A lightweight virtual desktop pet for Windows 11.*

Pocket Pal is a sprite-based desktop companion that lives along the bottom of your screen. It wanders around, idles, runs, sleeps, and reacts naturally while staying out of your way.

The goal of this project is to create a clean, modular foundation that can grow into a fully featured virtual pet with additional behaviors, customization, and mod support.

---

## ✨ Features

### Current

- Transparent desktop window
- Borderless application
- Sprite-sheet animations
- Fixed **8 FPS** animation system
- Walking and running
- Idle behavior
- Sleep state
- Jumping and falling
- Random movement
- Screen edge detection
- Modular architecture

### Planned

- Hunger and happiness system
- Toys and interactions
- Pet accessories
- Multiple pet support
- Save/load system
- Sound effects
- Personality traits
- Plugin support
- Modding support
- Additional animations

---

## 🛠 Tech Stack

- **Language:** C#
- **Framework:** .NET 8
- **UI:** WinUI 3 (preferred) or WPF
- **Rendering:** Sprite-based
- **Target Platform:** Windows 11

---

## 🎞 Animation System

Pocket Pal uses sprite-sheet animations with a retro feel.

- Fixed **8 FPS** playback
- Independent frame counts
- Automatic sprite loading
- Easy to add new animations
- Smooth state transitions

### Asset Structure

```text
Assets/
├── Idle/
├── Walk/
├── Run/
├── Sleep/
├── Jump/
└── Fall/
```

---

## 🐶 Pet States

The pet is controlled using a simple state machine.

Current states include:

- Idle
- Walking
- Running
- Sleeping
- Jumping
- Falling

Only one state is active at any time, making the system easy to maintain and extend.

---

## 🚶 Movement

Pocket Pal can:

- Walk across the bottom of the screen
- Run occasionally
- Stop to idle
- Turn around at screen edges
- Jump using simple gravity
- Choose actions randomly

Movement is smooth while animations remain locked to **8 FPS**.

---

## 📁 Project Structure

```text
PocketPal/
│
├── Assets/
│   ├── Idle/
│   ├── Walk/
│   ├── Run/
│   ├── Sleep/
│   └── Jump/
│
├── Animation/
├── AssetLoader/
├── Movement/
├── Pet/
├── Rendering/
├── StateMachine/
├── Settings/
└── Utilities/
```

---

## 🎯 Goals

- Lightweight and fast
- Low CPU usage
- Clean architecture
- Easy to maintain
- Easy to expand
- Beginner-friendly
- Open source

---

## 🗺 Roadmap

- [ ] Transparent desktop window
- [ ] Sprite renderer
- [ ] Animation manager
- [ ] State machine
- [ ] Movement controller
- [ ] Screen edge detection
- [ ] System tray support
- [ ] Save system
- [ ] User interaction
- [ ] Personality system
- [ ] Accessories
- [ ] Sound effects
- [ ] Plugin API
- [ ] Mod support

---

## 🤝 Contributing

Contributions are welcome!

You can help by:

- Reporting bugs
- Suggesting new features
- Improving documentation
- Submitting pull requests
- Creating new animations or pets

Please keep code clean, modular, and well documented.

---

## 📄 License

This project is licensed under the **MIT License**.

---

## 🌟 Vision

Pocket Pal aims to be a charming desktop companion that feels alive without getting in the way. The project prioritizes clean architecture, smooth performance, and an expandable design so new behaviors, pets, and community-made content can be added over time.
