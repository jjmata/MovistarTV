﻿// Copyright (C) 2014-2016, Codeplex/GitHub user AlphaCentaury
// All rights reserved, except those granted by the governing license of this software. See 'license.txt' file in the project root for complete license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IpTviewr.UiServices.Common.Start
{
    public interface ISplashScreenAwareForm : IDisposable
    {
        event EventHandler FormLoadCompleted;
    } // ISplashScreenAwareForm
} // namespace
