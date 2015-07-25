﻿using System.Collections.Generic;

namespace BotBits
{
    public interface IBlockLayer<T> : IEnumerable<WorldItem<T>> where T : struct
    {
        int Height { get; }
        int Width { get; }
        T this[int x, int y] { get; }
    }
}