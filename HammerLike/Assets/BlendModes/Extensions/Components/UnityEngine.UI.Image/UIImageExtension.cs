using UnityEngine.UI;

namespace BlendModes
{
    [ExtendedComponent(typeof(Image))]
   public class UIImageExtension : MaskableGraphicExtension<Image>
    {
       private static ShaderProperty[] cachedDefaultProperties;

        public override string[] GetSupportedShaderFamilies ()
        {
            return new[] {
                "UIDefault",
                "UIHsbc"
           };
        }

        public override ShaderProperty[] GetDefaultShaderProperties ()
        {
            return cachedDefaultProperties ?? (cachedDefaultProperties = new[] {
                    new ShaderProperty("_Hue", ShaderPropertyType.Float, 0),
                    new ShaderProperty("_Saturation", ShaderPropertyType.Float, 0),
                    new ShaderProperty("_Brightness", ShaderPropertyType.Float, 0),
                    new ShaderProperty("_Contrast", ShaderPropertyType.Float, 0)
                });
        }
    }
}
