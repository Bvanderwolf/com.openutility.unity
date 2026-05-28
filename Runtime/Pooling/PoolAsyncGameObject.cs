namespace OpenUtility.Data.Pooling
{
    public class PoolAsyncGameObject : PoolAsyncGameObjectBase<PoolAsyncGameObject>
    {
        /// <summary>
        /// The list of pooled game objects this game object is part of. Will return null if this component is
        /// not part of a pool or if the pool has no references.
        /// </summary>
        public PoolAsyncGameObjectList List => pool == null ? null : ((AsyncScriptablePool)pool).References;
    }
}
