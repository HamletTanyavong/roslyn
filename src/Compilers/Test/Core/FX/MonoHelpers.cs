﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;

namespace Microsoft.CodeAnalysis.Test.Utilities
{
    public static class MonoHelpers
    {
        public static bool IsRunningOnMono() => Roslyn.Test.Utilities.ExecutionConditionUtil.IsMonoDesktop;
        public static bool IsRunningOnMonoCore() => Roslyn.Test.Utilities.ExecutionConditionUtil.IsMonoCore;
    }
}
