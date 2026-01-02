# 📦 Open Utility

![Unity Version](https://img.shields.io/badge/Unity-6000+-blue?logo=unity)
![Dependencies](https://img.shields.io/badge/dependencies-2-brightgreen)
![License](https://img.shields.io/badge/License-MIT-green)

A streamlined suite of utilities for Unity. 🚀

---

## 🛠 Dependencies
This package requires the following Unity official packages to function correctly. These should automatically resolve if installed via Git URL, but it's good to verify them in your **Package Manager**:

| Package Name | Minimum Version |
| :--- | :--- |
| **Localization** (`com.unity.localization`) | `1.5.9` |
| **Addressables** (`com.unity.addressables`) | `2.7.6` |
| **Newtonsoft** (`com.unity.nuget.newtonsoft-json`) | `3.2.1` |

---

## ⚙️ Installation

### Via Unity Package Manager (Git URL)
1. Open the **Package Manager** (`Window > Package Manager`).
2. Click the **+** icon > **"Add package from git URL..."**.
3. Paste: `https://github.com/Bvanderwolf/com.openutility.unity.git`

---
## 🚀 Scriptable Variables for Unity
A robust, lightweight architecture for managing project-wide variables using Unity's ScriptableObjects.

#### 💡 The USP (Unique Selling Point)
The core strength of this system is its ability to decouple data from specific scenes or scripts. By storing variables as Assets in your project folder, they can be shared across systems effortlessly without the need for complex Singletons, DontDestroyOnLoad, or rigid hard-references.

#### ✨ Key Features
- 🌍 Global Persistence – Data persists across scene changes without extra code.
- 🔗 Clean Decoupling – Scripts talk to data containers rather than directly to each other.
- 🛠️ Runtime Debugging – Modify values in the Inspector during play mode and see changes reflected instantly.
- 🔔 Event-Driven – Built-in UnityEvents allow UI elements or logic to react to data changes automatically.
- ⚙️ Highly Extensible – Easily create custom variables for any data type (Int, Vector3, or even custom data structures).
- ⚡ One-Click Creation – Create new variables instantly using the plus button in the inspector.

#### 🛠️ How It Works
##### 1. The Foundation (Abstraction)
The system is built on a generic base class, ensuring a consistent API across all your variable types.

```csharp
public abstract class ScriptableVariable<T> : ScriptableObject
{
    public abstract T GetValue();
    public virtual void SetValue(T newValue) => 
        throw new NotImplementedException($"Setter for {typeof(T)} is not implemented.");
}
```

##### 2. Practical Implementation
Want to track a bool that toggles your game's "Hard Mode"?
- Step 1: Create a `ScriptableBool` variable in your DifficultyManager and your EnemyAI MonoBehaviour classes.
- Step 2: Press the plus icon in the inspector to create a new asset for your variable.
- Step 3: Subscribe to the events!

```csharp
public class EnemyAI : MonoBehaviour 
{
    [SerializeField] private ScriptableBool isHardMode;

    void OnEnable() => isHardMode.ValueChanged.AddListener(AdjustDifficulty);
    void OnDisable() => isHardMode.ValueChanged.RemoveListener(AdjustDifficulty);

    void AdjustDifficulty(bool hardModeActive) 
    {
        attackSpeed = hardModeActive ? 2.0f : 1.0f;
    }
}

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private ScriptableBool isHardMode;

    public void ToggleHardMode(bool newValue)
    {
        isHardMode.SetValue(newValue);
    }
}
```

#### 📊 Comparison Table

| Feature | Local Variable | Scriptable Variable |
| :--- | :---: | :---: |
| **Scene Persistence** | ❌ No | ✅ Yes |
| **Inspector Debugging** | ⚠️ Local Only | ✅ Global Asset |
| **UI Decoupling** | ❌ Hard-coded | ✅ Event-driven |
| **Memory Overhead** | Minimal | Minimal |
| **Architecture** | Spagetti-prone | Modular & Clean |

#### 🚀 Extension
Need a specific type? Just inherit from the base:

```csharp
[CreateAssetMenu(fileName = "New Float", menuName = "Variables/Scriptable Float")]
public class ScriptableFloat : ScriptableVariable<float> 
{
    // Add custom logic like Clamping or Math operations here
}
```

---

## 🌊 Pooling

A simple but effective Object Pooling setup for Unity, inspired by the `UnityEngine.Pool` namespace. This package simplifies pooling of game objects and is set up to be scalable. 🚀

#### 📦 Out Of The Box Setup
Get started quickly without writing custom pool logic:

1. **Create Pool Asset:** Right-click in project window `Create > OpenUtility > Pooling > GameObject Pool`.
2. **Create Prefab:** Right-click in project window > `Create > Scene > Prefab`.
3. **Add Component:** Attach the `PoolGameObject` component to your prefab.
4. **Link:** Drag your prefab into the **Prefab** field of the `GameObjectPool` asset.
5. **Use:** Reference the `GameObjectPool` asset in your scripts to start spawning!

---

#### 🛠 Custom Pooling Setup
Create your own custom pool ogic:

1. **Create Pool MonoBehaviour:** Create or select a MonoBehaviour you want to put on a game object to pool (e.g. EntityBehaviour : MonoBehaviour)
2. **Create Pool Asset:** Create a new script that inherits from `ScriptableObjectBase<T>` that you want to use as pool asset (e.g. EntityPool : `ScriptablePoolBase\<EntityBehaviour>)
3. **Implement:** Implement `OnCreateInstance`, `OnGetInstance` and `OnReleaseInstance`. See the ScriptablePool.cs script and Out Of The Box workflow as an example.
4. **Use:** Reference your custom pool asset from anywhere to start creating pooled instances.

##### `IPoolGameObject<T>`
- Implement this interface on a MonoBehaviour to receive notification upon creation (after Awake and before Start) and add the opportunity to implement release behaviour.
- Note T should always be the type of the implementer (e.g. EntityBehaviour : MonoBehaviour, IPoolGameObject\<EntityBehaviour>)
```csharp
public class EntityBehaviour : MonoBehaviour, IPoolGameObject<EntityBehaviour>
{
    public void OnCreatedByPool(ScriptablePoolBase<EntityBehaviour> pool) 
    { 
        /* Implement custom create logic here */ 
    }

    public bool Release() 
    { 
        /* Implement custom release logic here */  
        return true; 
    }
}
```

##### `PoolGameObjectBase<T>`
- Derive from this class to receive notification upon creation (after Awake and before Start). It implements default release behaviour which can be overriden.
- Note T should always be the type of the implementer (e.g. EntityBehaviour : PoolGameObjectBase\<EntityBehaviour>)
```csharp
public class EntityBehaviour : PoolGameObjectBase<EntityBehaviour>
{
        public override void OnCreatedByPool(ScriptablePoolBase<EntityBehaviour> scriptablePool)
        {
            /* Override creation logic here */
        }

        public override bool Release()
        {
            /* Override release logic here */
            return (true);
        }
    }
```
---

## 📦 Unity Addressables Simplified
A streamlined, high-level wrapper for the Unity Addressables Resource Management System. This library lowers the entry barrier for developers by providing a clean, static interface for catalog management, content downloading, and secure web requests.

#### ✨ Key Features
- ⚡ Simplified Workflow: Single-line methods for downloading catalogs and content.
- 🔐 SAS Token Integration: Built-in support for Azure Blob Storage SAS tokens with automatic URL appending.
- &#128260; Update Checks: Easy-to-use methods to detect and download catalog updates.
- 📊 Progress Tracking: Built-in support for download status and progress callbacks.
- 🧹 Cache Management: Advanced utilities to check, clear, and verify local caches.

#### 🚀 Getting Started
1. Initialize with SAS Tokens (Optional)

If your assets are hosted on private cloud storage (like Azure), enable SAS tokens globally:

```csharp
// Use a static token or a factory method for refreshing tokens
AddressableContentManager.EnableSasTokenUsage(
    () => MyBackend.GetFreshToken(), 
    "https://yourstorage.blob.core.windows.net/"
);
```

2. Build content url's based on your project requirements

A `AddressableContentSettings` scriptable object can be created to (dynamically) set endpoints
based on your project requirement. This is especially handy if you are creating content for a 
development and production environments. Or are creating different content for different clients.

This is where the `storageName` and `storageUrl` properties come into play. Create a new asset using
`Create > OpenUtility > AddressableContentSettings`, Assign a scriptable string variable and start
using the property values in your addressable profile using the following syntax:

- `[OpenUtility.Data.Addressable.Editor.AddressableBuildProperties.buildTarget]`
- `[OpenUtility.Data.Addressable.Editor.AddressableBuildProperties.storageUrl]`
- `[OpenUtility.Data.Addressable.Editor.AddressableBuildProperties.storageName]`

See the 'Advanced Addressables Setup` sample for examples on the implementation.

3. Downloading a Catalog
Load or download a remote catalog to see what content is available:

```csharp
AddressableContentManager.DownloadContentCatalog(catalogUrl, (result) => {
    if (result.success) {
        Debug.Log("Catalog ready for use!");
    }
});
```

4. Downloading Content

Once the catalog is loaded, you can download all content or filter by specific keys:

```csharp
// Download everything in the loaded catalogs
AddressableContentManager.DownloadContent(
    result => Debug.Log("Download Complete"),
    status => Debug.Log($"Progress: {status.Percent * 100}%")
);
```

#### 🛠 API Overview
`AddressableContentManager`

The primary entry point for most developers.
- `DownloadContentCatalog`: Fetch remote catalogs.
- `GetDownloadSize`: Calculate how many bytes need to be downloaded.
- `GetCatalogKeys`: Filter and retrieve keys available for download.
- `DownloadUpdatedCatalogs`: Synchronize local catalogs with remote changes.

`AddressableContent`

Lower-level utility methods for fine-grained control.

- `DeleteCacheFiles`: Completely wipe addressable data from the device.
- `CacheExists`: Check if a specific catalog or its dependencies are already stored locally.
- `IsContentCatalogLoaded`: Verify if a specific path is already registered.

---

## 🤝 Contributing
Working on making this possible..

---

## 📄 License
Distributed under the **MIT License**. 📜

---

### 🌟 Support
If this package saved you time, please give it a **Star**! 
[Visit the Repository](https://github.com/Bvanderwolf/com.openutility.unity)
