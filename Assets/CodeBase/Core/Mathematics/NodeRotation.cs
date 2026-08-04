using System;

namespace CodeBase.Core.Mathematics
{
    /// <summary>
    /// NodeRotation encapsulates int32 Angle property inside it, that must always be divisible by 90
    /// <remarks>0 = East; 90 = South; 180 = West; 270 = North</remarks>
    /// </summary>
    public struct NodeRotation
    {
        public int Angle { get; private set; }
        
        public void RotateCw(int times = 1) => Angle = (Angle + 90 * times) % 360;
        public void RotateCcw(int times = 1) => Angle = (Angle - 90 * times) % 360;

        public NodeRotation(int angle)
        {
            if (angle % 90 != 0)
                throw new Exception("Angle must be 90 * n");

            Angle = angle;
        }
        
        public Vector2Int ToVector() => Angle switch
        {
            0   => new Vector2Int(1, 0),
            90  => new Vector2Int(0, -1),
            180 => new Vector2Int(-1, 0),
            270 => new Vector2Int(0, 1),
            _   => throw new Exception("Invalid rotation")
        };
    }
}