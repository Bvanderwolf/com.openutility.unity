using System;
using System.Collections.Generic;
using OpenUtility.Data;
using UnityEngine;
using UnityEngine.Pool;

namespace OpenUtility.Samples.Data
{
    /// <summary>
    /// Implement this interface if you want to be able to group a game object in a 'GameObjectGroup' variable.
    /// </summary>
    public interface IGroupedGameObject : IEquatable<IGroupedGameObject>
    {
        GameObject Value { get; }
    }

    /// <summary>
    /// Represents a group of game objects. Used with the 'GroupGameObject' component to
    /// organize game objects into groups and access their components.
    /// </summary>
    [DefaultExecutionOrder(0)]
    [CreateAssetMenu(fileName = "GameObjectGroup", menuName = "OpenUtility/Data/GameObjectGroup")]
    public class GameObjectGroup : ScriptableList<IGroupedGameObject>
    {
        private readonly struct GameObjectWrapper : IGroupedGameObject
        {
            public GameObject Value { get; }
            private GameObjectWrapper(GameObject gameObject) => Value = gameObject;

            public bool Equals(IGroupedGameObject other)
            {
                if (other == null)
                    return (false);

                return (Value == other.Value);
            }

            public override string ToString() => Value.name;

            public static GameObjectWrapper Wrap(GameObject gameObject) => new GameObjectWrapper(gameObject);
        }
        
        private readonly struct ScriptableGameObjectWrapper : IGroupedGameObject
        {
            public GameObject Value => _scriptableGameObject.GetValue();
            
            private readonly ScriptableGameObject _scriptableGameObject;
            
            private ScriptableGameObjectWrapper(ScriptableGameObject scriptableGameObject) => _scriptableGameObject = scriptableGameObject;

            public bool Equals(IGroupedGameObject other)
            {
                if (other == null)
                    return (false);

                return (Value == other.Value);
            }

            public override string ToString() => _scriptableGameObject.ToString();

            public static ScriptableGameObjectWrapper Wrap(ScriptableGameObject scriptableGameObject) => new ScriptableGameObjectWrapper(scriptableGameObject);
        }

        public void Add(ScriptableGameObject gameObject) => Add(ScriptableGameObjectWrapper.Wrap(gameObject));

        public void Add(GameObject gameObject) => Add(GameObjectWrapper.Wrap(gameObject));
        
        public void Remove(ScriptableGameObject gameObject) => Remove(ScriptableGameObjectWrapper.Wrap(gameObject));
        
        public void Remove(GameObject gameObject) => Remove(GameObjectWrapper.Wrap(gameObject));
        
        /// <summary>
        /// Tries to get the first component of type T found on any of the game objects in the group.
        /// Returns true if a component was found, false otherwise.
        /// </summary>
        public bool TryGetComponent<T>(out T component) where T : Component
        {
            component = null;
            
            IList<IGroupedGameObject> list = value;
            if (list.Count == 0)
                return (false);
            
            for (int i = 0; i < list.Count; i++)
            {
                GameObject instance = list[i].Value;
                if (instance == null)
                    continue;
                
                if (instance.TryGetComponent(out component))
                    return (true);
            }

            return (false);
        }
        
        /// <summary>
        /// Returns the first component of type T found on any of the game objects in the group. Returns null if no component is found.
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            IList<IGroupedGameObject> list = value;
            if (list.Count == 0)
                return (null);

            for (int i = 0; i < list.Count; i++)
                if (list[i].Value.TryGetComponent(out T component))
                    return (component);

            return (null);
        }

        /// <summary>
        /// Returns an array of all components of type T found on any of the game objects in the group.
        /// Returns an empty array if no components are found.
        /// </summary>
        public T[] GetComponents<T>() where T : Component
        {
            using var pooled = ListPool<T>.Get(out List<T> components);

            IList<IGroupedGameObject> list = value;

            for (int i = 0; i < list.Count; i++)
                if (list[i].Value.TryGetComponent(out T component))
                    components.Add(component);

            return (components.ToArray());
        }

        /// <summary>
        /// Adds components of type T found on any of the game objects in the group to given list.
        /// </summary>
        public void GetComponents<T>(List<T> components) where T : Component
        {
            IList<IGroupedGameObject> list = value;

            for (int i = 0; i < list.Count; i++)
                if (list[i].Value.TryGetComponent(out T component))
                    components.Add(component);
        }
    }
}