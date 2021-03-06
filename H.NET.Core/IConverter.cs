﻿using System;
using System.Threading.Tasks;

namespace H.NET.Core
{
    public interface IConverter : IDisposable
    {
        Exception Exception { get; }

        Task<string> Convert(byte[] bytes);
    }
}
