namespace CodeBase.Core.Material
{
    public enum MaterialType
    {
        Copper,
        Iron,
        Gold,
        Coal,
        Aluminum
    }
    
    public class Material
    {
        public MaterialType MaterialType { get; }

        public Material(MaterialType materialType) => MaterialType = materialType;
    }
}