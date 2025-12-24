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

---

## ⚙️ Installation

### Via Unity Package Manager (Git URL)
1. Open the **Package Manager** (`Window > Package Manager`).
2. Click the **+** icon > **"Add package from git URL..."**.
3. Paste: `https://github.com/YourUsername/YourRepoName.git`

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

1. **Create Pool MonoBehaviour:** Create or select a MonoBehaviour you want to put on a game object to pool (e.g. EntityBehaviour : MonoBehaviour)
2. **Create Pool Asset:** Create a new script that inherits from ScriptableObjectBase\<T> that you want to use as pool asset (e.g. EntityPool : ScriptablePoolBase\<EntityBehaviour>)
3. **Implement:** Implement OnCreateInstance, OnGetInstance and OnReleaseInstance. See the ScriptablePool.cs script and Out Of The Box workflow as an example.
4. **Use:** Reference your custom pool asset from anywhere to start creating pooled instances.

##### IPoolGameObject\<T>
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

##### PoolGameObjectBase\<T>
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

## 🤝 Contributing
Working on making this possible..

---

## 📄 License
Distributed under the **MIT License**. 📜

---

### 🌟 Support
If this package saved you time, please give it a **Star**! 
[Visit the Repository](https://github.com/YourUsername/YourRepoName)
