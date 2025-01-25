# Debugging Rust Server Executable

> [!IMPORTANT]  
> These instructions are specifically designed for debugging Rust server files.  
> If you are working with other Unity-based executables or games, please refer to the [Debugging Unity Games guide on GitHub](https://github.com/dnSpy/dnSpy/wiki/Debugging-Unity-Games).  
> Note: dnSpy has been archived and no longer provides the necessary files referenced in its documentation. As a result, an additional preliminary step is required before following the same general process.

> [!NOTE]  
> You may want to download either [dnSpy](https://github.com/dnSpy/dnSpy) or its updated version, [dnSpyEx](https://github.com/dnSpyEx/dnSpy).

> [!NOTE]  
> To enable debugging, you need to replace the server build's Mono assemblies with Unity's Mono debug assemblies and create a boot configuration file to allow dnSpy to connect to the Unity Windows Player.  
> Special thanks to [bmgjet](https://github.com/bmgjet) for providing detailed and helpful guidance.

---

### Preparation  

Before you begin, navigate to the directory where your Rust server files are located.

1. Right-click on either **`RustDedicated.exe`** or **`UnityPlayer.dll`**, then select:  
   **Properties ► Details**.  

2. Within the **Details** tab, locate the **`Product version`** field. This value indicates the Unity Editor version used to build the executable or assembly.  
   Example Value: `2022.3.55f1 (9f374180d209)`  

<br>

## First Step  

You will need to download the Unity Editor version that matches the `Product version` identified in the preparation step.

1. Search for the required version in the [Unity version archive](https://unity.com/releases/editor/archive) or download it via [Unity Hub](https://unity.com/unity-hub).  
   
2. Once installed, navigate to the Unity Editor installation directory.  
   
3. Create a temporary, empty folder where you will collect the Mono assemblies and executables from the Unity Editor.  
   
4. Navigate to the following path in the Unity Editor directory:  
   `C:\Program Files\Unity\Hub\Editor\<YOUR_UNITY_EDITOR_VERSION>\Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_player_development_mono`.

<br>

## Second Step  

> [!IMPORTANT]  
> Copy the following files and folder (`.DLL`, `.EXE`, and `Folder`) into the temporary folder you created earlier:

- **Files**:  
  - `WinPixEventRuntime.dll`  
  - `UnityPlayer.dll`  
  - `UnityCrashHandler64.exe`  
  - `WindowsPlayer.exe`  

- **Folder**:  
  - `MonoBleedingEdge`  

<br>

## Third Step  

> [!CAUTION]  
> The Rust server directory contains files identical to those in your temporary directory.  
> It is highly recommended to back up these files in case anything goes wrong during this process.

- Once you have all the required files in your temporary directory:  
  - Rename `WindowsPlayer.exe` to `RustDedicated.exe` in the temporary directory.  
  - Navigate to the directory where your Rust server files are located:  
    - Copy the contents of your temporary directory into the server directory, replacing the existing files.  
    - Navigate into the `.\RustDedicated_Data` directory.  
      - Inside the `RustDedicated_Data` directory, create a new file named `boot.config`.  
      - Open `boot.config` with any text editor of your choice and add the following lines:  


```
player-connection-debug=1
wait-for-native-debugger=0
```
> Setting `wait-for-native-debugger` to `true` will make the server wait until an debugger is attached to it.

<br>

# Fourth Step (WIP)

After all those steps, you can start you server as usually with an batch script.
