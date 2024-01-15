// Copyright 2014-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlendModes
{
    /// <summary>
    /// Represents a serializable <see cref="Shader"/> property.
    /// </summary>
    [Serializable]
    public class ShaderProperty : IEquatable<ShaderProperty>
    {
        public string Name { get { return name; } }
        public ShaderPropertyType Type { get { return type; } }
        public Color ColorValue { get { return colorValue; } }
        public Vector4 VectorValue { get { return vectorValue; } }
        public float FloatValue { get { return floatValue; } }
        public Texture TextureValue { get { return textureValue; } }

        [SerializeField] private string name = null;
        [SerializeField] private ShaderPropertyType type = default(ShaderPropertyType);
        [SerializeField] private Color colorValue = Color.white;
        [SerializeField] private Vector4 vectorValue = Vector4.zero;
        [SerializeField] private float floatValue = 0f;
        [SerializeField] private Texture textureValue = null;

        public ShaderProperty (string name, ShaderPropertyType type, object value)
        {
            this.name = name;
            this.type = type;
            SetValue(value);
        }

        public ShaderProperty (ShaderProperty shaderProperty)
        {
            name = shaderProperty.name;
            type = shaderProperty.type;
            colorValue = shaderProperty.colorValue;
            vectorValue = shaderProperty.vectorValue;
            floatValue = shaderProperty.floatValue;
            textureValue = shaderProperty.textureValue;
        }

        public override bool Equals (object obj)
        {
            return Equals(obj as ShaderProperty);
        }

        public bool Equals (ShaderProperty other)
        {
            return other != null &&
                   name == other.name;
        }

        public override int GetHashCode ()
        {
            return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
        }

        public static bool operator == (ShaderProperty property1, ShaderProperty property2)
        {
            return EqualityComparer<ShaderProperty>.Default.Equals(property1, property2);
        }

        public static bool operator != (ShaderProperty property1, ShaderProperty property2)
        {
            return !(property1 == property2);
        }

        public void SetValue (object value)
        {
            if (!ShaderUtilities.CheckPropertyValueType(value, Type)) return;

            switch (Type)
            {
                case ShaderPropertyType.Color:
                    colorValue = (Color)value;
                    break;
                case ShaderPropertyType.Vector:
                    vectorValue = (Vector4)value;
                    break;
                case ShaderPropertyType.Float:
                    floatValue = (float)value;
                    break;
                case ShaderPropertyType.Texture:
                    textureValue = (Texture)value;
                    break;
            }
        }

        public void ApplyToMaterial (Material material)
        {
            if (!material || !material.HasProperty(Name)) return;

            switch (Type)
            {
                case ShaderPropertyType.Color:
                    material.SetColor(Name, ColorValue);
                    break;
                case ShaderPropertyType.Vector:
                    material.SetVector(Name, VectorValue);
                    break;
                case ShaderPropertyType.Float:
                    material.SetFloat(Name, FloatValue);
                    break;
                case ShaderPropertyType.Texture:
                    material.SetTexture(Name, TextureValue);
                    break;
            }
        }
    }
}
