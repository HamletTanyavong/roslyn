﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis.LanguageServerIndexFormat.Generator.Graph;

/// <summary>
/// Represents a single ResultSet for serialization. See https://github.com/Microsoft/language-server-protocol/blob/master/indexFormat/specification.md#result-set for further details.
/// </summary>
internal sealed class ResultSet : Vertex
{
    public ResultSet(IdFactory idFactory)
        : base(label: "resultSet", idFactory)
    {
    }
}
