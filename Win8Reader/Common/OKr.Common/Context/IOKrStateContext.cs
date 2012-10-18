﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// http://okr.me

namespace OKr.Common.Context
{
    public interface IOKrStateContext<TData>
    {
        Task<TData> Load();
        Task Save(TData data);
        Task Clear();
    }
}
