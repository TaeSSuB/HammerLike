// Copyright 2014-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace BlendModes
{
    [System.Serializable]
    public class ComponentExtensionState
    {
        public Component ExtendedComponent { get { return extendedComponent; } set { extendedComponent = value; } }
        public ShaderProperty[] ShaderProperties { get { return shaderProperties; } set { shaderProperties = value; } }

        [SerializeField] private Component extendedComponent = null;
        [SerializeField] private ShaderProperty[] shaderProperties = new ShaderProperty[0];
    }
}
