using UnityEngine;

namespace BlendModes
{
    [ExtendedComponent(typeof(SkinnedMeshRenderer))]
   public class SkinnedMeshRendererExtension : RendererExtension<SkinnedMeshRenderer>
   {
        private static ShaderProperty[] cachedDefaultProperties;

       public override string[] GetSupportedShaderFamilies ()
        {
           return new[] {
                "UnlitTransparent",
                "DiffuseTransparent"
           };
        }

        public override ShaderProperty[] GetDefaultShaderProperties ()
        {
            return cachedDefaultProperties ?? (cachedDefaultProperties = new[] {
                    new ShaderProperty("_MainTex", ShaderPropertyType.Texture, Texture2D.whiteTexture),
                    new ShaderProperty("_Color", ShaderPropertyType.Color, Color.white)
                });
       }

        protected override string GetDefaultShaderName ()
       {
            return "Legacy Shaders/Diffuse";
        }
   }
}
