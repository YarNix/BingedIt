using System;

namespace BingedIt.Common
{
    [Flags]
    public enum BingePriority : byte
    {
        Undefined = 0b00000,
        MustBinge = 0b10000,
        BingeNext = 0b01000,
        WillBinge = 0b00100,
        DoneBinge = 0b00010,
        WontBinge = 0b00001,
    }
    public static class BingePriorityHelper
    {
        // TODO: Come up with better fileName
        public static string AsString(this BingePriority priority)
        {
            return priority switch
            {
                BingePriority.MustBinge => "Binge now",
                BingePriority.BingeNext => "Binging",
                BingePriority.WillBinge => "Planning",
                BingePriority.DoneBinge => "Finished",
                BingePriority.WontBinge => "No Plan",
                BingePriority.Undefined => "Others",
                _ => priority.ToString(),
            };
        }
    }
}
