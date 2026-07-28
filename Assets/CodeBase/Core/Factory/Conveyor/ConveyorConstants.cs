using System.Collections.Generic;

namespace CodeBase.Core.Factory.Conveyor
{
    public static class ConveyorConstants
    {
        public const float ItemSize = 0.2f;
        private static readonly Dictionary<int, float> SizeProgress = new();

        public static float GetProgress(int itemPosition) => SizeProgress[itemPosition];
        
        static ConveyorConstants()
        {
            float progress = 0;
            
            for (int i = 0; i < 1f / ItemSize; i++)
            {
                SizeProgress[i] = progress;
                progress += ItemSize;
            }
        }
    }
}