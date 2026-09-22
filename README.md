# HAD1B True Stretch for VALORANT

A lightweight Windows utility that makes switching between native and true stretched resolution in VALORANT quicker and easier.

Instead of repeatedly opening Device Manager and Windows Display Settings, the app handles the required monitor-device and resolution changes using two buttons or configurable global hotkeys.

## Download

Download the latest release here:

https://github.com/hadib123/StrechyValorant/releases/tag/valstrechy

## Features

- Switch between native and stretched resolution.
- Enable or disable monitor device entries automatically.
- Set your own native resolution.
- Set your own stretched resolution.
- Assign custom global hotkeys.
- Use hotkeys while VALORANT is open.
- Minimize or pin the app to the Windows taskbar.
- Automatically remembers your resolutions and hotkeys.
- No command-line window.
- No installation required.

## Requirements

- Windows 10 or Windows 11.
- Administrator access for changing monitor devices.
- Your stretched resolution must already be available in NVIDIA Control Panel or AMD Software.
- VALORANT must be set to **Windowed Fullscreen**.
- VALORANT’s aspect ratio method must be set to **Fill**.

## Installation

1. Open the [latest release](https://github.com/hadib123/StrechyValorant/releases/tag/valstrechy).
2. Download the ZIP file.
3. Extract the entire ZIP to a permanent folder.
4. Open `HAD1B True Stretch.exe`.
5. Accept the Windows administrator prompt.
6. Optionally right-click the running taskbar icon and select **Pin to taskbar**.

Do not run the executable from inside the ZIP. Extract it first.

Windows may display a SmartScreen warning because the application is not digitally signed. Review the repository and only run software you trust.

## How to use it

### Before entering a match

1. Open the app.
2. Enter your preferred native and stretched resolutions.
3. Open VALORANT.
4. Set the display mode to **Windowed Fullscreen**.
5. Set the aspect ratio method to **Fill**.
6. Before the match starts or during Agent Select, press **Native Res**.
7. Once you load into the game, press **Stretched Resolution**.

### Native Res

Pressing **Native Res**:

- Enables all saved monitor device entries.
- Changes Windows to your selected native resolution.

### Stretched Resolution

Pressing **Stretched Resolution**:

- Saves the active monitor device IDs.
- Disables the active monitor device entries.
- Changes Windows to your selected stretched resolution.

## If you forget to switch to native first

You can correct it without leaving the match:

1. Press **Native Res**.
2. Open VALORANT’s video settings.
3. Set the display mode to **Windowed Fullscreen**.
4. Select **Fill** again.
5. Press **Stretched Resolution**.

## Custom resolutions

The default resolutions are:

- Native: `1920 × 1080`
- Stretched: `1280 × 1080`

Both can be changed inside the app. Your selected values are saved automatically.

Your stretched resolution must be supported by Windows and your graphics driver. If Windows rejects it, add the resolution through NVIDIA Control Panel or AMD Software first.

## Global hotkeys

Default hotkeys:

- **Native Res:** `Ctrl + Alt + F11`
- **Stretched Resolution:** `Ctrl + Alt + F12`

To change a hotkey:

1. Press the relevant **Assign Hotkey** button.
2. Press your preferred key combination.
3. The new hotkey will be saved automatically.

Global hotkeys work while the app is running, including when it is minimized.

If a hotkey is already being used by another program, choose a different combination.

## How it works

The application uses standard Windows functions to:

- Enable or disable monitor entries in Device Manager.
- Save the monitor device IDs so they can be enabled again.
- Change the Windows desktop resolution.
- Register optional global keyboard shortcuts.

The app does not need to access VALORANT to perform these actions.

## Anti-cheat and ban-risk disclaimer

This application does not inject code into VALORANT, access game memory, modify game files, provide gameplay macros, control gameplay inputs or intentionally interact with Vanguard.

It automates Windows display and monitor-device changes that can otherwise be performed manually through Device Manager and Windows Display Settings.

However, no unofficial third-party developer can guarantee that an application will never cause an issue, especially after future updates to VALORANT, Vanguard or Windows.

This project is not affiliated with, endorsed by or approved by Riot Games. Use it at your own discretion.

## Troubleshooting

### A monitor did not re-enable

1. Press **Native Res** again.
2. Allow a few seconds for Windows to rescan the monitor devices.
3. If it remains disabled, open Device Manager.
4. Expand **Monitors**.
5. Right-click the affected monitor and select **Enable device**.
6. Restart Windows if the monitor is not listed correctly.

The app saves the exact monitor IDs before disabling them so it can attempt to enable the same devices again.

### The stretched resolution is rejected

Make sure the resolution exists in:

- NVIDIA Control Panel, or
- AMD Software.

You may need to create it as a custom resolution before using the app.

### The image is not stretched

Confirm that VALORANT is using:

- **Display Mode:** Windowed Fullscreen
- **Aspect Ratio Method:** Fill

Press **Native Res**, reselect those settings in VALORANT and then press **Stretched Resolution** again.

### A hotkey does not work

- Make sure the app is still running.
- Check whether another program is using the same shortcut.
- Assign a different combination containing Ctrl, Alt or Shift.

## Support the project

The application is completely free.

If it helped you and you would like to support its development:

https://paypal.me/HadiBennagi

## Feedback and bug reports

Feedback and bug reports are welcome, especially from users with unusual multi-monitor configurations or different native and stretched resolutions.

Please include:

- Your Windows version.
- Your GPU.
- Your monitor models.
- Your native resolution.
- Your stretched resolution.
- The exact error message shown by the app.

## Credits

Made by **HAD1B**.
