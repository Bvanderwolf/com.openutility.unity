using System;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    internal struct BindingData
    {
        public Type variableType;
        public Type bindingType;

        public BindingData(Type variableType, Type bindingType)
        {
            this.variableType = variableType;
            this.bindingType = bindingType;
        }
    }

    internal struct SelectionData
    {
        public Object variableAsset;
        public Type bindingType;
            
        public SelectionData(Object variableAsset, Type bindingType)
        {
            this.variableAsset = variableAsset;
            this.bindingType = bindingType;
        }
    }
}
