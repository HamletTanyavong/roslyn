﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Indentation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Utilities;
using Microsoft.CodeAnalysis.Editor.UnitTests.Utilities;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Indentation;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text.Shared.Extensions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Formatting.Indentation;

[UseExportProvider]
[Trait(Traits.Feature, Traits.Features.Formatting)]
[Trait(Traits.Feature, Traits.Features.SmartTokenFormatting)]
public sealed class SmartTokenFormatterFormatRangeTests
{
    [Fact]
    public async Task BeginningOfFile()
    {
        var code = @"        using System;$$";
        var expected = @"        using System;";

        Assert.NotNull(await Record.ExceptionAsync(() => AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.None)));
    }

    [WpfFact]
    public async Task Namespace1()
    {
        var code = @"using System;
namespace NS
{

    }$$";

        var expected = @"using System;
namespace NS
{

}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace2()
    {
        var code = @"using System;
namespace NS
{
        class Class
                {
        }
    }$$";

        var expected = @"using System;
namespace NS
{
    class Class
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace3()
    {
        var code = @"using System;
namespace NS { }$$";

        var expected = @"using System;
namespace NS { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace4()
    {
        var code = @"using System;
namespace NS { 
}$$";

        var expected = @"using System;
namespace NS
{
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace5()
    {
        var code = @"using System;
namespace NS
{
    class Class { } 
}$$";

        var expected = @"using System;
namespace NS
{
    class Class { }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace6()
    {
        var code = @"using System;
namespace NS
{
    class Class { 
} 
}$$";

        var expected = @"using System;
namespace NS
{
    class Class
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace7()
    {
        var code = @"using System;
namespace NS
{
    class Class { 
} 
            namespace NS2
{}
}$$";

        var expected = @"using System;
namespace NS
{
    class Class
    {
    }
    namespace NS2
    { }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Namespace8()
    {
        var code = @"using System;
namespace NS { class Class { } namespace NS2 { } }$$";

        var expected = @"using System;
namespace NS { class Class { } namespace NS2 { } }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Class1()
    {
        var code = @"using System;
    class Class { 
}$$";

        var expected = @"using System;
class Class
{
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Class2()
    {
        var code = @"using System;
    class Class
{
    void Method(int i) {
                }
}$$";

        var expected = @"using System;
class Class
{
    void Method(int i)
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Class3()
    {
        var code = @"using System;
    class Class
{
    void Method(int i) { }
}$$";

        var expected = @"using System;
class Class
{
    void Method(int i) { }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Class4()
    {
        var code = @"using System;
    class Class
{
            delegate void Test(int i);
}$$";

        var expected = @"using System;
class Class
{
    delegate void Test(int i);
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Class5()
    {
        var code = @"using System;
    class Class
{
            delegate void Test(int i);
    void Method()
        {
                }
}$$";

        var expected = @"using System;
class Class
{
    delegate void Test(int i);
    void Method()
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Interface1()
    {
        var code = @"using System;
    interface II
{
            delegate void Test(int i);
int Prop { get; set; }
}$$";

        var expected = @"using System;
interface II
{
    delegate void Test(int i);
    int Prop { get; set; }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Struct1()
    {
        var code = @"using System;
    struct Struct
{
            Struct(int i)
    {
                }
}$$";

        var expected = @"using System;
struct Struct
{
    Struct(int i)
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Enum1()
    {
        var code = @"using System;
    enum Enum
{
                A = 1, B = 2,
    C = 3
            }$$";

        var expected = @"using System;
enum Enum
{
    A = 1, B = 2,
    C = 3
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task AccessorList1()
    {
        var code = @"using System;
class Class
{
    int Prop { get { return 1; }$$";

        var expected = @"using System;
class Class
{
    int Prop { get { return 1; }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task AccessorList2()
    {
        var code = @"using System;
class Class
{
    int Prop { get { return 1; } }$$";

        var expected = @"using System;
class Class
{
    int Prop { get { return 1; } }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.IntKeyword);
    }

    [WpfFact]
    public async Task AccessorList3()
    {
        var code = @"using System;
class Class
{
    int Prop { get { return 1; }  
}$$";

        var expected = @"using System;
class Class
{
    int Prop
    {
        get { return 1; }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.IntKeyword);
    }

    [WpfFact]
    public async Task AccessorList4()
    {
        var code = @"using System;
class Class
{
    int Prop { get { return 1;   
}$$";

        var expected = @"using System;
class Class
{
    int Prop { get
        {
            return 1;
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.GetKeyword);
    }

    [WpfFact]
    public async Task AccessorList5()
    {
        var code = @"using System;
class Class
{
    int Prop {
        get { return 1;   
}$$";

        var expected = @"using System;
class Class
{
    int Prop {
        get { return 1;
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("https://github.com/dotnet/roslyn/issues/16984")]
    public async Task AccessorList5b()
    {
        var code = @"using System;
class Class
{
    int Prop {
        get { return 1;   
}$$
}
}";

        var expected = @"using System;
class Class
{
    int Prop {
        get
        {
            return 1;
        }
}
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task AccessorList6()
    {
        var code = @"using System;
class Class
{
    int Prop 
        { 
get { return 1;   
} }$$";

        var expected = @"using System;
class Class
{
    int Prop
    {
        get
        {
            return 1;
        }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.IntKeyword);
    }

    [WpfFact]
    public async Task AccessorList7()
    {
        var code = @"using System;
class Class
{
    int Prop
    {
        get
        {
return 1;$$
        }
    }";

        var expected = @"using System;
class Class
{
    int Prop
    {
        get
        {
            return 1;
        }
    }";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("https://github.com/dotnet/roslyn/issues/16984")]
    public async Task AccessorList8()
    {
        var code = @"class C
{
    int Prop
    {
get
        {
            return 0;
        }$$
    }
}";

        var expected = @"class C
{
    int Prop
    {
        get
        {
            return 0;
        }
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfTheory, WorkItem("https://github.com/dotnet/roslyn/issues/16984")]
    [InlineData("get")]
    [InlineData("set")]
    [InlineData("init")]
    public async Task AccessorList9(string accessor)
    {
        var code = $@"class C
{{
    int Prop
    {{
{accessor}
        {{
            ;
        }}$$
    }}
}}";

        var expected = $@"class C
{{
    int Prop
    {{
        {accessor}
        {{
            ;
        }}
    }}
}}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("https://github.com/dotnet/roslyn/issues/16984")]
    public async Task AccessorList10()
    {
        var code = @"class C
{
    event EventHandler E
    {
add
        {
        }$$
        remove
        {
        }
    }

}";

        var expected = @"class C
{
    event EventHandler E
    {
        add
        {
        }
        remove
        {
        }
    }

}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("https://github.com/dotnet/roslyn/issues/16984")]
    public async Task AccessorList11()
    {
        var code = @"class C
{
    event EventHandler E
    {
        add
        {
        }
remove
        {
        }$$
    }

}";

        var expected = @"class C
{
    event EventHandler E
    {
        add
        {
        }
        remove
        {
        }
    }

}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.CloseBraceToken);
    }

    [WpfFact]
    public async Task Block1()
    {
        var code = @"using System;
class Class
{
    public int Method()
    { }$$";

        var expected = @"using System;
class Class
{
    public int Method()
    { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block2()
    {
        var code = @"using System;
class Class
{
    public int Method() { }$$";

        var expected = @"using System;
class Class
{
    public int Method() { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block3()
    {
        var code = @"using System;
class Class
{
    public int Method() { 
}$$
}";

        var expected = @"using System;
class Class
{
    public int Method()
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block4()
    {
        var code = @"using System;
class Class
{
    public static Class operator +(Class c1, Class c2) {
            }$$
}";

        var expected = @"using System;
class Class
{
    public static Class operator +(Class c1, Class c2)
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block5()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block6()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        { 
}$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block7()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        { { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        { { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Block8()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        { { 
}$$
        }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        {
            {
            }
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task SwitchStatement1()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        switch (a) {
            case 1:
                break;
}$$
    }
}";

        var expected = @"using System;
class Class
{
    void Method()
    {
        switch (a)
        {
            case 1:
                break;
        }
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task SwitchStatement2()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        switch (true) { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        switch (true) { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task SwitchStatement3()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        switch (true)
        {
            case 1: { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        switch (true)
        {
            case 1: { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.ColonToken);
    }

    [WpfFact]
    public async Task SwitchStatement4()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        switch (true)
        {
            case 1: { 
}$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        switch (true)
        {
            case 1:
                {
                }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.ColonToken);
    }

    [WpfFact]
    public async Task Initializer1()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        var arr = new int[] { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        var arr = new int[] { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task Initializer2()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        var arr = new int[] { 
}$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        var arr = new int[] {
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task Initializer3()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        var arr = new { A = 1, B = 2
}$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        var arr = new
        {
            A = 1,
            B = 2
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task Initializer4()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        var arr = new { A = 1, B = 2 }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        var arr = new { A = 1, B = 2 }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task Initializer5()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        var arr = new[] { 
            1, 2, 3, 4,
            5 }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        var arr = new[] {
            1, 2, 3, 4,
            5 }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task Initializer6()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        var arr = new int[] { 
            1, 2, 3, 4,
            5 }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        var arr = new int[] {
            1, 2, 3, 4,
            5 }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task EmbeddedStatement1()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        if (true) { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        if (true) { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement2()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        if (true) { 
        }$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        if (true)
        {
        }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement3()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        if (true)
        { }$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        if (true)
        { }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement4()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        while (true) {
}$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        while (true)
        {
        }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("https://github.com/dotnet/roslyn/issues/8413")]
    public async Task EmbeddedStatementDoBlockAlone()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        do {
}$$
    }
}";

        var expected = @"using System;
class Class
{
    void Method()
    {
        do
        {
        }
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement5()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        do {
} while(true);$$
    }
}";

        var expected = @"using System;
class Class
{
    void Method()
    {
        do
        {
        } while (true);
    }
}";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement6()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        for (int i = 0; i < 10; i++)             {
}$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        for (int i = 0; i < 10; i++)
        {
        }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement7()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        foreach (var i in collection)            {
}$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        foreach (var i in collection)
        {
        }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement8()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        using (var resource = GetResource())           {
}$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        using (var resource = GetResource())
        {
        }
    }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement9()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        if (true)
                int i = 10;$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        if (true)
            int i = 10;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task FieldlInitializer()
    {
        var code = @"using System;
class Class
{
          string str =              Console.Title;$$
";

        var expected = @"using System;
class Class
{
    string str = Console.Title;
";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task ArrayFieldlInitializer()
    {
        var code = @"using System;
namespace NS
{
    class Class
    {
                    string[] strArr = {           ""1"",                       ""2"" };$$
";

        var expected = @"using System;
namespace NS
{
    class Class
    {
        string[] strArr = { ""1"", ""2"" };
";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task ExpressionValuedPropertyInitializer()
    {
        var code = @"using System;
class Class
{
          public int  Three =>   1+2;$$
";

        var expected = @"using System;
class Class
{
    public int Three => 1 + 2;
";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement10()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
        if (true)
                int i = 10;$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        if (true)
            int i = 10;
    }";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement11()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                using (var resource = GetResource()) resource.Do();$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        using (var resource = GetResource()) resource.Do();";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement12()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                using (var resource = GetResource()) 
    resource.Do();$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        using (var resource = GetResource())
            resource.Do();";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement13()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                using (var resource = GetResource()) 
    resource.Do();$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        using (var resource = GetResource())
            resource.Do();
    }";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement14()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                do i = 10;$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        do i = 10;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement15()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                do
    i = 10;$$";

        var expected = @"using System;
class Class
{
    void Method()
    {
        do
            i = 10;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement16()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                do
    i = 10;$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        do
            i = 10;
    }";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task EmbeddedStatement17()
    {
        var code = @"using System;
class Class
{
    void Method()
    {
                do
    i = 10;
while (true);$$
    }";

        var expected = @"using System;
class Class
{
    void Method()
    {
        do
            i = 10;
        while (true);
    }";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement1()
    {
        var code = @"using System;
class Class
{
                    int i = 10;
                    int i2 = 10;$$";

        var expected = @"using System;
class Class
{
                    int i = 10;
    int i2 = 10;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement2()
    {
        var code = @"using System;
class Class
{
            void Method(int i)
            {
            }

            void Method2()
            {
            }$$
}";

        var expected = @"using System;
class Class
{
            void Method(int i)
            {
            }

    void Method2()
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.CloseBraceToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement3()
    {
        var code = @"using System;
class Class
{
            void Method(int i)
            {
            }

            A a = new A 
            {
                Prop = 1,
                Prop2 = 2
            };$$
}";

        var expected = @"using System;
class Class
{
            void Method(int i)
            {
            }

    A a = new A
    {
        Prop = 1,
        Prop2 = 2
    };
}";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.CloseBraceToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement4()
    {
        var code = @"using System;
class Class
{
            void Method(int i)
            {
                        int i = 10;
             int i2 = 10;$$";

        var expected = @"using System;
class Class
{
            void Method(int i)
            {
                        int i = 10;
        int i2 = 10;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement5()
    {
        var code = @"using System;
class Class
{
            void Method(int i)
            {
                        int i = 10;
                if (true)
i = 50;$$";

        var expected = @"using System;
class Class
{
            void Method(int i)
            {
                        int i = 10;
        if (true)
            i = 50;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement6()
    {
        var code = @"        using System;
        using System.Linq;$$";

        var expected = @"        using System;
using System.Linq;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement7()
    {
        var code = @"            using System;

            namespace NS
            {
            }

        namespace NS2
        {
        }$$";

        var expected = @"            using System;

            namespace NS
            {
            }

namespace NS2
{
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.CloseBraceToken);
    }

    [WpfFact]
    public async Task FollowPreviousElement8()
    {
        var code = @"using System;

namespace NS
{
            class Class
            {
            }

        class Class1
        {
        }$$
}";

        var expected = @"using System;

namespace NS
{
            class Class
            {
            }

    class Class1
    {
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.CloseBraceToken);
    }

    [WpfFact]
    public async Task IfStatement1()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
            if (true)
        {
    }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        if (true)
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task IfStatement2()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
            if (true)
        {
    }
else
        {
                }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        if (true)
        {
        }
        else
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task IfStatement3()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
            if (true)
        {
    }
else    if (false)
        {
                }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        if (true)
        {
        }
        else if (false)
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task IfStatement4()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
            if (true)
        return          ;
else    if (false)
                    return          ;$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        if (true)
            return;
        else if (false)
            return;";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task TryStatement1()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
                try
    {
        }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        try
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task TryStatement2()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
                try
    {
        }
catch    (  Exception       ex)
                {
    }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        try
        {
        }
        catch (Exception ex)
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task TryStatement3()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
                try
    {
        }
catch    (  Exception       ex)
                {
    }
            catch               (Exception          ex2)
                      {
   }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        try
        {
        }
        catch (Exception ex)
        {
        }
        catch (Exception ex2)
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task TryStatement4()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
                try
    {
        }
                                finally
                      {
   }$$";

        var expected = @"using System;

class Class
{
    void Method()
    {
        try
        {
        }
        finally
        {
        }";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("https://github.com/dotnet/roslyn/issues/6645")]
    public async Task TryStatement5()
    {
        var code = @"using System;

class Class
{
    void Method()
    {
        try {
        }$$
    }
}";

        var expected = @"using System;

class Class
{
    void Method()
    {
        try
        {
        }
    }
}";

        await AutoFormatOnCloseBraceAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537555")]
    public async Task SingleLine()
    {
        var code = @"class C { void M() { C.M(    );$$ } }";

        var expected = @"class C { void M() { C.M(); } }";

        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [Fact]
    public async Task StringLiterals()
    {
        var code = @"class C { void M() { C.M(""Test {0}$$";

        var expected = string.Empty;
        await AutoFormatOnMarkerAsync(code, expected, SyntaxKind.StringLiteralToken, SyntaxKind.None);
    }

    [Fact]
    public async Task CharLiterals()
    {
        var code = @"class C { void M() { C.M('}$$";

        var expected = string.Empty;
        await AutoFormatOnMarkerAsync(code, expected, SyntaxKind.CharacterLiteralToken, SyntaxKind.None);
    }

    [Fact, WorkItem("https://github.com/dotnet/roslyn/issues/44423")]
    public async Task CharLiterals1()
    {
        var code = @"';$$";

        var expected = string.Empty;
        await AutoFormatOnMarkerAsync(code, expected, SyntaxKind.CharacterLiteralToken, SyntaxKind.None);
    }

    [Fact]
    public async Task Comments()
    {
        var code = @"class C { void M() { // { }$$";

        var expected = string.Empty;
        await AutoFormatOnMarkerAsync(code, expected, SyntaxKind.OpenBraceToken, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task FirstLineInFile()
    {
        var code = @"using System;$$";

        await AutoFormatOnSemicolonAsync(code, "using System;", SyntaxKind.UsingKeyword);
    }

    [WpfFact]
    public async Task Label1()
    {
        var code = @"class C
{
    void Method()
    {
                L           :               int             i               =               20;$$
    }
}";
        var expected = @"class C
{
    void Method()
    {
    L: int i = 20;
    }
}";
        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Label2()
    {
        var code = @"class C
{
    void Method()
    {
                L           :               
int             i               =               20;$$
    }
}";
        var expected = @"class C
{
    void Method()
    {
    L:
        int i = 20;
    }
}";
        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact]
    public async Task Label3()
    {
        var code = @"class C
{
    void Method()
    {
        int base = 10;
                L           :               
int             i               =               20;$$
    }
}";
        var expected = @"class C
{
    void Method()
    {
        int base = 10;
    L:
        int i = 20;
    }
}";
        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Label4()
    {
        var code = @"class C
{
    void Method()
    {
        int base = 10;
    L:
        int i = 20;
int         nextLine            =           30          ;$$
    }
}";
        var expected = @"class C
{
    void Method()
    {
        int base = 10;
    L:
        int i = 20;
        int nextLine = 30;
    }
}";
        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.SemicolonToken);
    }

    [WpfFact]
    public async Task Label6()
    {
        var code = @"class C
{
    void Method()
    {
    L:
        int i = 20;
int         nextLine            =           30          ;$$
    }
}";
        var expected = @"class C
{
    void Method()
    {
    L:
        int i = 20;
        int nextLine = 30;
    }
}";
        await AutoFormatOnSemicolonAsync(code, expected, SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537776")]
    public async Task DisappearedTokens()
    {
        var code = @"class Class1
{
    int goo()
        return 0;
        }$$
}";

        var expected = @"class Class1
{
    int goo()
        return 0;
        }
}";
        await AutoFormatOnCloseBraceAsync(
            code,
            expected,
            SyntaxKind.ClassKeyword);
    }

    [Fact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537779")]
    public async Task DisappearedTokens2()
    {
        var code = """
            class Class1
            {
                void Goo()
                {
                    Object o=new Object);$$
                }
            }
            """;

        var expected = """
            class Class1
            {
                void Goo()
                {
                    Object o = new Object);
                }
            }
            """;

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537793")]
    public async Task Delegate1()
    {
        var code = @"delegate void MyDelegate(int a,int b);$$";

        var expected = @"delegate void MyDelegate(int a, int b);";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.DelegateKeyword);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537827")]
    public async Task DoubleInitializer()
    {
        var code = @"class C
{
    void Method()
    {
        int[,] a ={{ 1 , 1 }$$
    }
}";

        var expected = @"class C
{
    void Method()
    {
        int[,] a ={{ 1 , 1 }
    }
}";

        await AutoFormatOnCloseBraceAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537825")]
    public async Task MissingToken1()
    {
        var code = @"public class Class1
{
    int a = 1}$$;
}";

        var expected = @"public class Class1
{
    int a = 1};
}";

        await AutoFormatOnCloseBraceAsync(
            code,
            expected,
            SyntaxKind.PublicKeyword);
    }

    [WpfFact]
    public async Task ArrayInitializer1()
    {
        var code = @"public class Class1
{
    var a = new [] 
    {
        1, 2, 3, 4
        }$$
}";

        var expected = @"public class Class1
{
    var a = new[]
    {
        1, 2, 3, 4
        }
}";

        await AutoFormatOnCloseBraceAsync(
            code,
            expected,
            SyntaxKind.NewKeyword);
    }

    [WpfFact]
    public async Task ArrayInitializer2()
    {
        var code = @"public class Class1
{
    var a = new [] 
    {
        1, 2, 3, 4
        }   ;$$
}";

        var expected = @"public class Class1
{
    var a = new[]
    {
        1, 2, 3, 4
        };
}";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537825")]
    public async Task MalformedCode()
    {
        var code = @"namespace ClassLibrary1
{
    public class Class1
    {
        int a}$$;
    }
}";

        var expected = @"namespace ClassLibrary1
{
    public class Class1
    {
        int a};
    }
}";

        await AutoFormatOnCloseBraceAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537804")]
    public async Task Colon_SwitchLabel()
    {
        var code = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
            switch(E.Type)
            {
                    case 1 :$$
            }
        }
    }
}";

        var expected = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
            switch(E.Type)
            {
                case 1:
            }
        }
    }
}";

        await AutoFormatOnColonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/584599")]
    public async Task Colon_SwitchLabel_Comment()
    {
        var code = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
            switch(E.Type)
            {
                        // test
                    case 1 :$$
            }
        }
    }
}";

        var expected = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
            switch(E.Type)
            {
                // test
                case 1:
            }
        }
    }
}";

        await AutoFormatOnColonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/584599")]
    public async Task Colon_SwitchLabel_Comment2()
    {
        var code = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
            switch(E.Type)
            {
                case 2:
                    // test
                    case 1 :$$
            }
        }
    }
}";

        var expected = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
            switch(E.Type)
            {
                case 2:
                // test
                case 1:
            }
        }
    }
}";

        await AutoFormatOnColonAsync(
            code,
            expected,
            SyntaxKind.ColonToken);
    }

    [Fact]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/537804")]
    [WorkItem("https://github.com/dotnet/roslyn/issues/13981")]
    public async Task Colon_Label()
    {
        var code = """
            namespace ClassLibrary1
            {
                public class Class1
                {
                    void Test()
                    {
                                label   :$$
                    }
                }
            }
            """;

        var expected = """
            namespace ClassLibrary1
            {
                public class Class1
                {
                    void Test()
                    {
                    label:
                    }
                }
            }
            """;

        await AutoFormatOnColonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538793")]
    public async Task Colon_Label2()
    {
        var code = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
                    label   :   Console.WriteLine(10) ;$$
        }
    }
}";

        var expected = @"namespace ClassLibrary1
{
    public class Class1
    {
        void Test()
        {
        label: Console.WriteLine(10);
        }
    }
}";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem(3186, "DevDiv_Projects/Roslyn")]
    public async Task SemicolonInElseIfStatement()
    {
        var code = @"using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        int a = 0;
        if (a == 0)
            a = 1;
        else if (a == 1)
            a=2;$$
        else
            a = 3;

    }
}";

        var expected = @"using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        int a = 0;
        if (a == 0)
            a = 1;
        else if (a == 1)
            a = 2;
        else
            a = 3;

    }
}";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.SemicolonToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538391")]
    public async Task SemicolonInElseIfStatement2()
    {
        var code = @"public class Class1
{
    void Method()
    {
        int a = 1;
        if (a == 0)
            a = 8;$$
                    else
                        a = 10;
    }
}";

        var expected = @"public class Class1
{
    void Method()
    {
        int a = 1;
        if (a == 0)
            a = 8;
        else
            a = 10;
    }
}";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.SemicolonToken);
    }

    [WpfFact, WorkItem(8385, "DevDiv_Projects/Roslyn")]
    public async Task NullCoalescingOperator()
    {
        var code = @"class C
{
    void M()
    {
        object o2 = null??null;$$
    }
}";

        var expected = @"class C
{
    void M()
    {
        object o2 = null ?? null;
    }
}";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541517")]
    public async Task SwitchDefault()
    {
        var code = @"using System;
class Program
{
    static void Main()
    {
        switch (DayOfWeek.Monday)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
                break;
            case DayOfWeek.Wednesday:
                break;
                default:$$
        }
    }
}";

        var expected = @"using System;
class Program
{
    static void Main()
    {
        switch (DayOfWeek.Monday)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
                break;
            case DayOfWeek.Wednesday:
                break;
            default:
        }
    }
}";

        await AutoFormatOnColonAsync(
            code,
            expected,
            SyntaxKind.SemicolonToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542538")]
    public async Task MissingTokens1()
    {
        var code = @"class Program
{
    static void Main(string[] args)
    {
        gl::$$
    }
}";

        var expected = @"class Program
{
    static void Main(string[] args)
    {
        gl::
    }
}";

        await AutoFormatOnMarkerAsync(
            code,
            expected,
            SyntaxKind.ColonColonToken,
            SyntaxKind.OpenBraceToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542538")]
    public async Task MissingTokens2()
    {
        var code = @"class C { void M() { M(() => { }$$ } }";

        var expected = @"class C { void M() { M(() => { } } }";

        await AutoFormatOnCloseBraceAsync(
            code,
            expected,
            SyntaxKind.EqualsGreaterThanToken);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542953")]
    public async Task UsingAlias()
    {
        var code = @"using Alias=System;$$";

        var expected = @"using Alias = System;";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.UsingKeyword);
    }

    [WpfFact, WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542953")]
    public async Task NoLineChangeWithSyntaxError()
    {
        var code = @"struct Goo { public int member; }
class Program{
    void Main()
    {
        var f = new Goo { member;$$ }
    }
}";

        var expected = @"struct Goo { public int member; }
class Program{
    void Main()
    {
        var f = new Goo { member; }
    }
}";

        await AutoFormatOnSemicolonAsync(
            code,
            expected,
            SyntaxKind.None);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/620568")]
    public void SkippedTokens1(bool useTabs)
    {
        var code = @";$$*";

        var expected = @";*";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530830")]
    public void AutoPropertyAccessor(bool useTabs)
    {
        var code = @"class C
{
    int Prop {          get             ;$$
}";

        var expected = @"class C
{
    int Prop {          get;
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530830")]
    public void AutoPropertyAccessor2(bool useTabs)
    {
        var code = @"class C
{
    int Prop {          get;                set             ;$$
}";

        var expected = @"class C
{
    int Prop {          get;                set;
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/530830")]
    public void AutoPropertyAccessor3(bool useTabs)
    {
        var code = @"class C
{
    int Prop {          get;                set             ;           }$$
}";

        var expected = @"class C
{
    int Prop { get; set; }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/784674")]
    public void AutoPropertyAccessor4(bool useTabs)
    {
        var code = @"class C
{
    int Prop {          get;$$             }
}";

        var expected = @"class C
{
    int Prop { get; }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/924469")]
    public void AutoPropertyAccessor5(bool useTabs)
    {
        var code = @"class C
{
    int Prop {          get;                set             ;$$           }
}";

        var expected = @"class C
{
    int Prop { get; set; }
}";
        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/924469")]
    public void AutoPropertyAccessor6(bool useTabs)
    {
        var code = @"class C
{
    int Prop { get;set;$$}
}";

        var expected = @"class C
{
    int Prop { get; set; }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/924469")]
    public void AutoPropertyAccessor7(bool useTabs)
    {
        var code = @"class C
{
    int Prop     { get;set;$$}    
}";

        var expected = @"class C
{
    int Prop     { get; set; }    
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/912965")]
    public void NestedUsingStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        using (null)
            using(null)$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        using (null)
        using (null)
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/912965")]
    public void NestedNotUsingStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        using (null)
            for(;;)$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        using (null)
            for(;;)
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void UsingStatementWithNestedFixedStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        using (null)
        fixed (void* ptr = &i)
        {
        }$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        using (null)
            fixed (void* ptr = &i)
            {
            }
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void UsingStatementWithNestedCheckedStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        using (null)
        checked
        {
        }$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        using (null)
            checked
            {
            }
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void UsingStatementWithNestedUncheckedStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        using (null)
        unchecked
        {
        }$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        using (null)
            unchecked
            {
            }
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void FixedStatementWithNestedUsingStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        fixed (void* ptr = &i)
        using (null)$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        fixed (void* ptr = &i)
            using (null)
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void FixedStatementWithNestedFixedStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        fixed (void* ptr1 = &i)
            fixed (void* ptr2 = &i)
            {
            }$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        fixed (void* ptr1 = &i)
        fixed (void* ptr2 = &i)
        {
        }
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void FixedStatementWithNestedNotFixedStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        fixed (void* ptr = &i)
        if (false)
        {
        }$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        fixed (void* ptr = &i)
            if (false)
            {
            }
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    public void NotFixedStatementWithNestedFixedStatement(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {
        if (false)
        fixed (void* ptr = &i)
        {
        }$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        if (false)
            fixed (void* ptr = &i)
            {
            }
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/954386")]
    public void FormattingRangeForFirstStatementOfBlock(bool useTabs)
    {
        var code = @"class C
{
    public void M()
    {int s;$$
    }
}";

        var expected = @"class C
{
    public void M()
    {
        int s;
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/954386")]
    public void FormattingRangeForFirstMemberofType(bool useTabs)
    {
        var code = @"class C
{int s;$$
    public void M()
    {
    }
}";

        var expected = @"class C
{
    int s;
    public void M()
    {
    }
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/954386")]
    public void FormattingRangeForFirstMethodMemberofType(bool useTabs)
    {
        var code = @"interface C
{void s();$$
}";

        var expected = @"interface C
{
    void s();
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("https://github.com/dotnet/roslyn/issues/17257")]
    public void FormattingRangeForConstructor(bool useTabs)
    {
        var code = @"class C
{public C()=>f=1;$$
}";

        var expected = @"class C
{
    public C() => f = 1;
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("https://github.com/dotnet/roslyn/issues/17257")]
    public void FormattingRangeForDestructor(bool useTabs)
    {
        var code = @"class C
{~C()=>f=1;$$
}";

        var expected = @"class C
{
    ~C() => f = 1;
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("https://github.com/dotnet/roslyn/issues/17257")]
    public void FormattingRangeForOperator(bool useTabs)
    {
        var code = @"class C
{public static C operator +(C left, C right)=>field=1;$$
    static int field;
}";

        var expected = @"class C
{
    public static C operator +(C left, C right) => field = 1;
    static int field;
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/954386")]
    public void FormattingRangeForFirstMemberOfNamespace(bool useTabs)
    {
        var code = @"namespace C
{delegate void s();$$
}";

        var expected = @"namespace C
{
    delegate void s();
}";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/981821")]
    public void FormatDirectiveTriviaAlwaysToColumnZero(bool useTabs)
    {
        var code = @"class Program
{
    static void Main(string[] args)
    {
#if
        #$$
    }
}
";

        var expected = @"class Program
{
    static void Main(string[] args)
    {
#if
#
    }
}
";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/981821")]
    public void FormatDirectiveTriviaAlwaysToColumnZeroWithCode(bool useTabs)
    {
        var code = @"class Program
{
    static void Main(string[] args)
    {
#if
        int s = 10;
        #$$
    }
}
";

        var expected = @"class Program
{
    static void Main(string[] args)
    {
#if
        int s = 10;
#
    }
}
";

        AutoFormatToken(code, expected, useTabs);
    }

    [WpfTheory]
    [CombinatorialData]
    [WorkItem("http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/981821")]
    public void FormatDirectiveTriviaAlwaysToColumnZeroWithBrokenElseDirective(bool useTabs)
    {
        var code = @"class Program
{
    static void Main(string[] args)
    {
#else
        #$$
    }
}
";

        var expected = @"class Program
{
    static void Main(string[] args)
    {
#else
#
    }
}
";

        AutoFormatToken(code, expected, useTabs);
    }

    internal static void AutoFormatToken(string markup, string expected, bool useTabs)
    {
        if (useTabs)
        {
            markup = markup.Replace("    ", "\t");
            expected = expected.Replace("    ", "\t");
        }

        using var workspace = EditorTestWorkspace.CreateCSharp(markup);

        var subjectDocument = workspace.Documents.Single();
        var textBuffer = subjectDocument.GetTextBuffer();
        var optionsService = workspace.GetService<EditorOptionsService>();
        var editorOptions = optionsService.Factory.GetOptions(textBuffer);
        editorOptions.SetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId, !useTabs);

        var commandHandler = workspace.GetService<FormatCommandHandler>();
        var typedChar = textBuffer.CurrentSnapshot.GetText(subjectDocument.CursorPosition.Value - 1, 1);
        commandHandler.ExecuteCommand(new TypeCharCommandArgs(subjectDocument.GetTextView(), textBuffer, typedChar[0]), () => { }, TestCommandExecutionContext.Create());

        var newSnapshot = textBuffer.CurrentSnapshot;

        Assert.Equal(expected, newSnapshot.GetText());
    }

    private static Task AutoFormatOnColonAsync(string codeWithMarker, string expected, SyntaxKind startTokenKind)
        => AutoFormatOnMarkerAsync(codeWithMarker, expected, SyntaxKind.ColonToken, startTokenKind);

    private static Task AutoFormatOnSemicolonAsync(string codeWithMarker, string expected, SyntaxKind startTokenKind)
        => AutoFormatOnMarkerAsync(codeWithMarker, expected, SyntaxKind.SemicolonToken, startTokenKind);

    private static Task AutoFormatOnCloseBraceAsync(string codeWithMarker, string expected, SyntaxKind startTokenKind)
        => AutoFormatOnMarkerAsync(codeWithMarker, expected, SyntaxKind.CloseBraceToken, startTokenKind);

    private static async Task AutoFormatOnMarkerAsync(string initialMarkup, string expected, SyntaxKind tokenKind, SyntaxKind startTokenKind)
    {
        await AutoFormatOnMarkerAsync(initialMarkup, expected, useTabs: false, tokenKind, startTokenKind).ConfigureAwait(false);
        await AutoFormatOnMarkerAsync(initialMarkup.Replace("    ", "\t"), expected.Replace("    ", "\t"), useTabs: true, tokenKind, startTokenKind).ConfigureAwait(false);
    }

    private static async Task AutoFormatOnMarkerAsync(string initialMarkup, string expected, bool useTabs, SyntaxKind tokenKind, SyntaxKind startTokenKind)
    {
        using var workspace = EditorTestWorkspace.CreateCSharp(initialMarkup);

        var testDocument = workspace.Documents.Single();
        var buffer = testDocument.GetTextBuffer();
        var position = testDocument.CursorPosition.Value;

        var document = workspace.CurrentSolution.GetDocument(testDocument.Id);
        var documentSyntax = await ParsedDocument.CreateAsync(document, CancellationToken.None);
        var rules = Formatter.GetDefaultFormattingRules(document);

        var root = (CompilationUnitSyntax)await document.GetSyntaxRootAsync();
        var endToken = root.FindToken(position);
        if (position == endToken.SpanStart && !endToken.GetPreviousToken().IsKind(SyntaxKind.None))
        {
            endToken = endToken.GetPreviousToken();
        }

        Assert.Equal(tokenKind, endToken.Kind());

        var options = new IndentationOptions(
            new CSharpSyntaxFormattingOptions() { LineFormatting = new() { UseTabs = useTabs } });

        var formatter = new CSharpSmartTokenFormatter(options, rules, (CompilationUnitSyntax)documentSyntax.Root, documentSyntax.Text);

        var tokenRange = FormattingRangeHelper.FindAppropriateRange(endToken);
        if (tokenRange == null)
        {
            Assert.Equal(SyntaxKind.None, startTokenKind);
            return;
        }

        Assert.Equal(startTokenKind, tokenRange.Value.Item1.Kind());
        if (tokenRange.Value.Item1.Equals(tokenRange.Value.Item2))
        {
            return;
        }

        var changes = formatter.FormatRange(tokenRange.Value.Item1, tokenRange.Value.Item2, CancellationToken.None);
        var actual = GetFormattedText(buffer, changes);
        Assert.Equal(expected, actual);
    }

    private static string GetFormattedText(ITextBuffer buffer, IList<TextChange> changes)
    {
        using (var edit = buffer.CreateEdit())
        {
            foreach (var change in changes)
            {
                edit.Replace(change.Span.ToSpan(), change.NewText);
            }

            edit.Apply();
        }

        return buffer.CurrentSnapshot.GetText();
    }
}
