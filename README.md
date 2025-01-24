# First step

> [!NOTE]
> To be able to debug, we need to switch the server build's mono assemblies to Unity's mono debug assemblies
> and create a boot configuration so we can use dnSpy to connect to the Unity Windows Player.
> Thanks to [bmgjet]("https://github.com/bmgjet") for providing relevant information.

***

Before you can start doing anything, navigate to the path where your Rust server files are located.

- Either Right-Click on **`RustDedicated.exe` or `UnityPlayer.dll` ► Properties ► Details** .

There you should see the `Productversion` of the executable/assembly which indicates the Unity Editor Version this executable/assembly his been build with. 
Example: 

```
2022.3.55f1 (9f374180d209)
```

***

## Second step
- It is now required to download the Unity Editor with the same version as the executable has. You either Google for the version archive or try it with Unity Hub.
- After installation, navigate to the path where the Unity Editor is installed.
- Create somewhere an temporary empty folder to collect the mono assemblies and executables of Unity Editor.
- Navigate to `C:\Program Files\Unity\Hub\Editor\<YOUR_UNITY_EDITOR_VERSION>\Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_player_development_mono`.

***

## Third Step
> [!IMPORTANT]  
> Copy following assemblies (`.DLL`), executables (`.EXE`) and folders into your temporary created folder:

- `WinPixEventRuntime.dll`
- `UnityPlayer.dll`
- `UnityCrashHandler64.exe`
- `WindowsPlayer.exe`
- **Folder** -> `MonoBleedingEdge`

(More details)[https://github.com/dnSpy/dnSpy/wiki/Debugging-Unity-Games]

(WIP)MoveToDocs
