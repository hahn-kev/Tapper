using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Tapper.Test.SourceTypes.Space3;
using Xunit;
using Xunit.Abstractions;

namespace Tapper.Tests;

public class HeaderTest
{
    private readonly ITestOutputHelper _output;

    public HeaderTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test_Header()
    {
        var compilation = CompilationSingleton.Compilation;
        var codeGenerator = new TypeScriptCodeGenerator(compilation, Environment.NewLine, 2, SerializerOption.Json, NamingStyle.None, Logger.Empty);

        var targetTypes = compilation.GetSourceTypes();
        var targetTypeLookupTable = targetTypes.ToLookup<INamedTypeSymbol, INamespaceSymbol>(static x => x.ContainingNamespace, SymbolEqualityComparer.Default);

        var type = typeof(NastingNamespaceType);
        var typeSymbol = compilation.GetTypeByMetadataName(type.FullName!)!;

        var group = targetTypeLookupTable.Where(x => SymbolEqualityComparer.Default.Equals(x.Key, typeSymbol.ContainingNamespace)).First()!;

        var writer = new CodeWriter();

        codeGenerator.AddHeader(group, ref writer);

        var code = writer.ToString();
        var gt = @"/* eslint-disable */
/* tslint:disable */
import { CustomType } from './Tapper.Test.SourceTypes.Space1';
import { CustomType2, CustomType3 } from './Tapper.Test.SourceTypes.Space1.Sub';
import { CustomType4 } from './Tapper.Test.SourceTypes.Space2';

";

        _output.WriteLine(code);
        _output.WriteLine(gt);

        Assert.Equal(gt, code, ignoreLineEndingDifferences: true);
    }
}
