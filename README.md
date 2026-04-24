<h1 align="center">AC109R Control</h1>

<p align="center">
  <strong>A lightweight Windows controller for the ACGAM AC-109R keyboard lighting profiles and software macros.</strong>
</p>

<p align="center">
  <img alt="Language" src="https://img.shields.io/badge/C%23-.NET%20Framework%204.8-512bd4">
  <img alt="GUI" src="https://img.shields.io/badge/GUI-WinForms-5d8cff">
  <img alt="Platform" src="https://img.shields.io/badge/Platform-Windows-24292f">
  <img alt="Device" src="https://img.shields.io/badge/Keyboard-ACGAM%20AC--109R-success">
  <img alt="Transport" src="https://img.shields.io/badge/Transport-Windows%20HID-informational">
  <img alt="Version" src="https://img.shields.io/badge/Version-1.0.0-success">
  <img alt="License" src="https://img.shields.io/badge/License-GPL--3.0-blue">
</p>

## Overview

AC109R Control is a small Windows application for the ACGAM AC-109R keyboard. It provides a stable alternative to the official Windows utility for common day-to-day tasks:

- selecting one of the three onboard lighting profiles
- clearing a profile
- sending built-in static presets
- creating personal profiles from built-in presets
- importing, renaming, duplicating, and deleting personal JSON profiles
- registering global software macros for media actions
- starting automatically with Windows and launching minimized to the notification area
- switching the interface between French and English

The USB lighting protocol implementation is based on the reverse-engineering work from [`franlego98/ac109rdriverlinux`](https://gitlab.com/franlego98/ac109rdriverlinux).

## Screenshots

Add screenshots here when publishing the repository.

```text
docs/screenshots/static-profiles.png
docs/screenshots/macros.png
```

Suggested Markdown once screenshots are available:

```html
<p align="center">
  <img alt="Static profiles" src="docs/screenshots/static-profiles.png" width="760">
</p>
```

## Features

### Static profiles

The application includes read-only built-in presets:

- Rainbow horizontal
- Rainbow diagonal
- Pink/blue aurora
- Ocean
- Fire
- Warm white
- AC109 magenta

Built-in presets cannot be edited directly. Use **Create profile** to create an editable user profile.

### User JSON profiles

Editable profiles are stored in:

```text
%LOCALAPPDATA%\AC109RDriverWin\Profiles
```

The JSON format follows the original Linux driver convention:

```json
{
  "esc": [255, 0, 255, 255],
  "f1": [255, 0, 255, 255],
  "space": [255, 255, 255, 255]
}
```

Supported value shapes:

- `[brightness]` applies the same value to red, green, blue, and alpha
- `[red, green, blue]` creates an opaque RGB color
- `[red, green, blue, alpha]` creates an explicit RGBA key color

The application can import, rename, duplicate, delete, and open the managed profile folder directly from the interface.

### Languages

The interface supports French and English. The selected language is stored in the application settings and can be changed from the **Settings** tab.

### Administrator rights

Administrator privileges are not required for normal use. AC109R Control uses the Windows HID API, current-user startup registration, and global hotkeys. Running elevated should only be needed for troubleshooting unusual device permission issues.

### Single instance

Only one copy of AC109R Control can run at a time. A second launch shows a message and exits.

### Changelog

The application includes a built-in **Changelog** tab. Version `1.0.0` is the first stable release.

### Software macros

The macro system uses global Windows hotkeys. It currently supports:

- volume up
- volume down
- mute volume
- play / pause
- next track
- previous track

These macros are software-level Windows shortcuts. They do not rewrite the keyboard's internal macro memory, because that part of the official protocol is not documented in the original Linux driver.

### Startup behavior

The application can register itself under the current user's Windows startup registry key. Manual launches open the main window normally. Startup launches can be minimized using:

```text
--minimized
```

The notification area icon keeps macros alive while the main window is hidden.

## Build

Requirements:

- Windows
- Visual Studio with .NET Framework 4.8 targeting pack
- MSBuild

Build from the solution:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" AC109RWinForms.sln /p:Configuration=Release /p:Platform="Any CPU"
```

The executable is generated at:

```text
bin\Release\Ac109RDriverWin.exe
```

## Technical Notes

The keyboard is accessed through the Windows HID API. No external NuGet package is required.

Known device identifiers:

```text
VID: 0x1EA7
PID: 0x0907
Preferred HID interface: MI_01
Packet length: 64 bytes
CRC: CCITT, start value 0xFFFF
```

The application writes the same profile stream structure used by the Linux project:

1. Optional profile clear
2. Stream header
3. Base key data
4. Per-key RGBA data
5. Zero padding
6. Stream close
7. Profile activation

## Limitations

- Firmware-native lighting effects are not implemented yet. The original Linux driver notes that the profile stream may contain an unknown keyboard-side program area, but it does not document the official firmware effect commands.
- Macro support is implemented with Windows global hotkeys, not firmware-level keyboard macro programming.
- The original public Linux project documents lighting profiles, not the complete official Windows software feature set.
- If the official AC109R software is running, it may lock the HID interface. Close it before using this application.

## Credits

This project builds on the protocol research and Linux implementation by Francisco Sanchez Lopez:

- GitLab: [`franlego98/ac109rdriverlinux`](https://gitlab.com/franlego98/ac109rdriverlinux)

CRC logic is a C# port of the CCITT algorithm used by the original project.

## License

This project is distributed under the GPL-3.0 license, matching the original AC109R Linux driver license.
