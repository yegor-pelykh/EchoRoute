# EchoRoute

**EchoRoute** is a lightweight Windows tray application that allows you to duplicate (mirror) audio output from your default playback device to an additional output device in real time. It is ideal for scenarios where you want to play system audio simultaneously through two different audio devices (e.g., speakers and headphones).

---

## Features

- **Audio Duplication:** Mirrors audio from your default output device to a secondary device.
- **Tray Icon Control:** Runs silently in the system tray with a convenient context menu.
- **Device Selection:** Easily choose any active audio output device as the extra destination.
- **Volume Control:** Adjust the volume for the extra device independently.
- **Automatic Device Updates:** Reacts to device changes and default device switches in real time.
- **Minimal UI:** No main window — everything is managed via the tray icon menu.
- **Graceful Resource Management:** Cleans up audio resources and tray icon on exit.

---

## Getting Started

### Prerequisites

- **Windows 7/8/10/11**
- **.NET 8** or higher
- **Visual Studio 2022+** (for building from source)

### Dependencies

- [NAudio](https://github.com/naudio/NAudio) (audio capture and playback library)

### Installation

1. **Clone or Download the Repository**
2. **Open the Solution:**  
   Open `EchoRoute.sln` in Visual Studio.
3. **Restore NuGet Packages:**  
   Ensure NAudio is installed via NuGet.
4. **Build the Project:**  
   Build the solution in Release mode.
5. **Run:**  
   Launch `EchoRoute.exe`.

---

## Usage

1. **Selecting an Extra Output Device:**
   - Right-click the EchoRoute tray icon.
   - Go to **"Extra output device"** and select your desired device.
   - To stop duplication, select **"Off"**.

2. **Adjusting Extra Device Volume:**
   - In the tray menu, select **"Extra device volume"**.
   - Choose a volume level (0%–100%).

3. **Exiting the Application:**
   - Click **"Exit"** in the tray menu.

---

## Contributing

Pull requests and suggestions are welcome! Please open an issue to discuss your ideas or report bugs.

---

## License

MIT License. See the [LICENSE](LICENSE) file.

---

## Acknowledgments

- [NAudio](https://github.com/naudio/NAudio) for audio APIs.