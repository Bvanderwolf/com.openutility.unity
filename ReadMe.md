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
2. Click the **+** icon > **"Install package from git URL..."**.
3. Paste: 
```
https://github.com/Bvanderwolf/com.openutility.unity.git#v0.9.0-alpha
```

---
## 🚀 Scriptable Variables for Unity
A robust and lightweight library for managing project-wide variables using Unity's ScriptableObjects.

#### 💡 The USP (Unique Selling Point)
The core strength of this system is its ability to decouple data from specific scenes or scripts. By storing variables as Assets in your project folder, they can be shared across systems effortlessly without the need for complex Singletons, DontDestroyOnLoad, or rigid hard-references.

#### ✨ Key Features
- 🌍 Global Persistence – Data persists across scene changes without extra code.
- 🔗 Clean Decoupling – Scripts talk to data containers rather than directly to each other.
- 🛠️ Runtime Debugging – Modify values in the Inspector during play mode and see changes reflected instantly.
- 🔔 Event-Driven – Built-in UnityEvents allow UI elements or logic to react to data changes automatically.
- ⚙️ Highly Extensible – Easily create custom variables for any data type (Quaternion, Vector3, or even custom data structures).
- ⚡ One-Click Creation – Create new variables instantly using the plus button in the inspector.
- 🔗 Data binding - Keep your UI and logic in perfect sync without a single line of "glue code" using ScriptableObjects as the bridge between your logic and your UI.
- 🧩 Many Sample Implementations - Get started quickly with a variety of pre-built variable types and binding implementations.

#### 🛠️ How It Works
##### 1. The Foundation
The library is built on generic base classes, ensuring a consistent API across all your variable types.

```csharp
public abstract class ScriptableVariable<T> : ScriptableObject
{
    public abstract T GetValue(); // You must be able to retreiving your value.
    public virtual void SetValue(T newValue) { } // Setting your value is optional.
}
```

##### 2. Practical Implementation
Want to track a bool that toggles your game's "Hard Mode"?
- Step 1: Create a `ScriptableBool` variable in your DifficultyManager and your EnemyAI MonoBehaviour classes.
- Step 2: Press the plus icon in the inspector to create a new asset for your variable.
- Step 3: Start listening for value changes!

```csharp
public class EnemyAI : MonoBehaviour 
{
    [SerializeField] 
    private ScriptableBool isHardMode;

    void OnEnable()
    {
        isHardMode.ValueChanged.AddListener(AdjustDifficulty);
    }

    void OnDisable()
    {
         isHardMode.ValueChanged.RemoveListener(AdjustDifficulty);
    }

    void AdjustDifficulty(bool hardModeActive) 
    {
        attackSpeed = hardModeActive ? 2.0f : 1.0f;
    }
}

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] 
    private ScriptableBool isHardMode;

    public void ToggleHardMode()
    {
        bool value = isHardMode.GetValue();
        isHardMode.SetValue(!value);
    }
}
```

##### 3. Available variables
Available variables are `ScriptableFloat`, `ScriptableInt`, `ScriptableBool` and `ScriptableString`. 

#### 🚀 Extension
Need a new type? Inherit from the base:

```csharp
[CreateAssetMenu(fileName = "New Float", menuName = "Variables/Scriptable Float")]
public class MyScriptableClassVariable : ScriptableVariable<MyClass> 
{
    // Add custom logic here
}
```

Want a new implementation for a default type (int, float, string, bool)? Inherit from a base variable type

```csharp
[CreateAssetMenu(fileName = "New Float", menuName = "Variables/Scriptable Float")]
public class ClampableFloatVariable : ScriptableFloat
{
    // Add custom clamping logic here
}
```

#### 📚 Reference Types
##### 1. A short summary
The goal of a scriptable variable reference is to provide a user friendly interface to a value that can either be locally changed or shared between multiple scripts. Defining variables as a reference provides you with the flexability to update a values scope from local to shared without actually having to change any code. 

##### 2. Practical Implementation
Want to track an enemty's health and trigger a death animation if it reaches critical levels?
- Step 1: Create a `FloatReference` variable in your EnemyBehaviour class.
- Step 2: Determine the value of your 'Local' value in the inspector or use the dropdown to switch to 'Shared' and assign a (new) variable.
- Step 3: Start using your reference's value!
```csharp
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField]
    private FloatReference _health;

    void Update()
    {
        if (_health.Value <= 0) // We don't think in the code about the setup of the reference. We just have to check the value.
        {
            // Add Death Animation logic here
        }
    }
}
```
##### 3. Available types
Available reference types are `FloatReference`, `IntReference`, `BoolReference` and `StringReference`.

##### 4. Custom reference types
Create your own reference type by deriving from the ScriptableVariableReference class.

```csharp
public class ScriptableUserData : ScriptableVariable<UserData>
{
    // Add custom user data logic here.
}
```

```csharp
[Serializable] // Make sure to use the 'Serialize' attribute, otherwise your value won't show properly in the inspector!
public struct UserData
{
    public int id;
    public string name;
}

[Serializable] // Make sure to use the 'Serialize' attribute, otherwise your value won't show properly in the inspector!
public class UserDataReference : ScriptableVariableReference<UserData>
{
    [SerializeField]
    private ScriptableUserData _variable;
    
    protected override ScriptableVariable<UserData> GetScriptableVariable()
    {
        return (_variable);
    }
}
```

#### 🔗 Data Binding System
Keep your UI and logic in perfect sync without a single line of "glue code" in your features. This package facilitates a robust Data Binding system using ScriptableObjects as the bridge between your logic and your UI.

##### 💡 The Core Concept
Instead of hard-coding references between UI elements and scripts, you use ScriptableVariables. Go to your ui
component's inspector, scroll down, and either, press 'bind to scriptable variable' to determine the value of an existing variable, or the '+' button to create and
bind a new one to determine the value of. You can also press 'listen to scriptable variable' to listen to an existing variable or the '+' button to create and listen to a new one.

##### 🚀 Direct bindings
If your variable type is directly compatible with the UI element's value type, you can use the [ScriptableVariableBinder] attribute on it. For example, the ScriptableFloat can  receive the value of a standard Unity Slider out of the box as its 'SetValue' method's signature matches the sliders 'onValueChanged' event.

```csharp
[ScriptableVariableBinder(typeof(Slider), typeof(float), DisplayName = "Default Float Binding")]
[CreateAssetMenu(fileName = "ScriptableFloat", menuName = "OpenUtility/Variables/Float")]
public class ScriptableFloat : ScriptableVariable<float> 
{
    // Automatically handles the sync from slider to variable.
    public float SetValue();
}
```

If the Slider needs to (also) listen to the ScriptableFloat, a `ScriptableFloatEvent` component will be added to the game object. It will listen to the variables 'ValueChanged' event, ensuring invokation of the sliders 'SetValueWithoutNotify' function to update the slider if the variable's value has changed.

```csharp
public class ScriptableFloatEvent : ScriptableVariableEvent<float>
{
    // Automatically handles the sync from variable to slider.
    [Serializable]
    public class ChangedEvent : UnityEvent<float> { }
    public ChangedEvent ValueChanged;
}
```

##### 🛠️ Custom bindings
Sometimes the UI data type doesn't match your variable type (e.g., a Slider outputs a float, but you want to save it as an int). No problem! You can easily select or create a custom binding implementation (choosing from a select list of types (see **binding types table**)):

```csharp
[ScriptableVariableBinder(typeof(Slider), typeof(int), BindingGoal.ReceiveValue, DisplayName = "Default Integer Variable")]
public class DefaultIntegerSliderBinding : IntegerSliderBinding
{
    // Convert the float from the slider to an int for your variable
    public override void SetValue(float newValue)
    {
        var casted = (int)newValue;
        
        variable.SetValue(casted);
    }
}

[ScriptableVariableBinder(typeof(Slider), typeof(int), BindingGoal.DetermineValue, DisplayName = "Default Integer Variable")]
public class DefaultIntegerSliderEventBinding : IntegerSliderEventBinding
{
    // Convert the integer from your variable to an float for the slider.
    protected override float ConvertIntegerToDecimal(int newValue) => newValue;
}
```

#### 📊 Binding Types Table

| UI Element | ScriptableFloat | ScriptableBool | ScriptableInt | ScriptableString |
| :--- | :---: | :---: | :---: | :---: |
| **Slider** | ✅ Yes | ❌ No | ✅ Yes | ❌ No | 
| **TMP_InputField** | ✅ Yes | ❌ No | ✅ Yes | ✅ Yes |
| **Toggle** | ❌ No | ✅ Yes | ❌ No | ❌ No |
| **TMP_Text** | ✅ Yes | ❌ No | ✅ Yes | ✅ Yes |
| **TMP_Dropdown** | ❌ No | ❌ No | ✅ Yes | ❌ No

#### 🌍 Share and Group GameObjects

During development the following design problems are common:
- A component in a prefab requires a reference to a game object or component in a scene. You can't set this reference in the prefab itself, because the scene is not part of the prefab.
- A game object in your scene requires a reference to another game object or component in another scene. You can't set this reference in the inspector, because the other scene is not loaded yet.
- A component requires references to multiple game objects that globally provide the same functionality (e.g. authentication provider & license provider = service providers). As you require more dependencies, you now have to manually find and assign more of these references for each component instance.
- A script requires references to multiple game objects that globally provide the same functionality (e.g. authentication provider & license provider = service providers). As you require more dependencies, arguments have to be added to function calls.

Introducing the **ScriptableGameObject** and **GameObjectGroup**
Created to work as a proxy for single or multiple (grouped) GameObject references, they make it possible to share references across scenes and prefabs.

Use the **ScriptableGameObject** variable to share a reference to a GameObject across different scenes and prefabs. 

- **Share Script:** Right click on a csharp script in your project window and press 'Share' via > `OpenUtility > Share`. This will set up a shared prefab source setup automatically for you using the selected script.
- **Share Prefab:** Right click on a prefab in your project window and press 'Share' via > `OpenUtility > Share`. This will set up a shared prefab source setup automatically for you.
- **Share Scene Object:** Right click on a game object in your scene hierarchy and press 'Share' via > `OpenUtility > Share`. This will set up a shared scene source setup automatically for you using the selected game object.
- **Share Component in Scene:** Open the context menu on a component using the triple dots. Press share via `OpenUtility > Share`. This will set up a shared scene source automatically for you using the selected component.

or (manual setup)

1. **Create Asset:** Create a new **ScriptableGameObject** asset via `Create > OpenUtility > Data > ScriptableGameObject`.
2. **Set Source:** Either set the source to 'Prefab', assign a prefab to the 'prefab' field and instantiate it at runtime. Or set the source to 'Scene' and assign a value directly to the variable by adding a **ShareGameObject** component to a GameObject in a scene.

> ℹ️ The `DefaultExecutionOrder` attribute on the `ShareGameObject` component ensures values are set before any `Awake` methods are called. 

Use the **GameObjectGroup** variable to share a group of GameObject instances across different scenes and prefabs.

1. **Create Asset:** Create a new **GameObjectGroup** asset via `Create > OpenUtility > Data > GameObjectGroup`.
2. **Add Values:** Add **ScriptableGameObject** values by assigning their values in the inspector. You can also add values by adding a **GroupGameObject** component to game objects in a scene.

> ℹ️ The `DefaultExecutionOrder` attribute on the **GroupGameObject** component ensures values are set before any `Awake` methods are called. 

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
