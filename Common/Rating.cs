using System;

namespace BingedIt.Common
{
    [Flags]
    public enum Rating : byte
    {
        Unrated = 0x0,
        Terrible = 0x1,
        Bad = 0x2,
        Okay = 0x4,
        Good = 0x8,
        Perfect = 0x10,
    }
}
