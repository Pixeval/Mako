// Copyright (c) Mako.SourceGen.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mako.SourceGen;

[Generator]
public sealed class MakoExtensionGenerator : IIncrementalGenerator
{
    private const string ConstructorAttributeFullName = "Mako.Utilities.MakoExtensionConstructorAttribute";

    private const string GeneratedExtensionAttributeFullName = "Mako.Utilities.GeneratedMakoExtensionAttribute";

    private const string EngineHandleNamespace = "Mako.Engine";

    private static readonly SymbolDisplayFormat TypeDisplayFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributedConstructors = context.SyntaxProvider.ForAttributeWithMetadataName(
            ConstructorAttributeFullName,
            static (_, _) => true,
            static (syntaxContext, _) => TryGetAttributedConstructor(syntaxContext))
            .Where(static symbol => symbol is not null)
            .Select(static (symbol, _) => symbol!);

        // see also: https://github.com/dotnet/roslyn/issues/80511
        // TODO: 傻逼ForAttributeWithMetadataName不支持method:这种主构造器，看什么时候支持
        var primaryConstructors = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is TypeDeclarationSyntax { ParameterList: not null, AttributeLists.Count: > 0 },
            static (syntaxContext, _) => TryGetPrimaryConstructor(syntaxContext))
            .Where(static symbol => symbol is not null)
            .Select(static (symbol, _) => symbol!);

        var extensionConstructors = attributedConstructors
            .Collect()
            .Combine(primaryConstructors.Collect())
            .SelectMany(static (source, _) => DistinctConstructors(source.Left, source.Right));

        var extensionHosts = context.SyntaxProvider.ForAttributeWithMetadataName(
            GeneratedExtensionAttributeFullName,
            static (_, _) => true,
            static (syntaxContext, _) => (INamedTypeSymbol) syntaxContext.TargetSymbol);

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(extensionHosts.Collect().Combine(extensionConstructors.Collect())),
            static (spc, source) => Generate(spc, source.Left, source.Right.Left, source.Right.Right));
    }

    private static void Generate(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<INamedTypeSymbol> hosts,
        ImmutableArray<IMethodSymbol> constructors)
    {
        foreach (var host in DistinctHosts(hosts))
        {
            var methods = constructors
                .Select(constructor => TryCreateMethod(constructor, host, compilation))
                .Where(static method => method is not null)
                .Cast<GeneratedMethod>()
                .OrderBy(static method => method.Name, StringComparer.Ordinal)
                .ThenBy(static method => method.Signature, StringComparer.Ordinal)
                .ToImmutableArray();

            if (methods.IsDefaultOrEmpty)
                continue;

            context.AddSource(
                $"{GetHintName(host)}_MakoExtension.g.cs",
                BuildSource(host, methods));
        }
    }

    private static IEnumerable<INamedTypeSymbol> DistinctHosts(ImmutableArray<INamedTypeSymbol> hosts)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var host in hosts)
        {
            if (seen.Add(host.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                yield return host;
        }
    }

    private static IEnumerable<IMethodSymbol> DistinctConstructors(
        ImmutableArray<IMethodSymbol> constructors,
        ImmutableArray<IMethodSymbol> primaryConstructors)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var constructor in constructors.Concat(primaryConstructors))
        {
            if (seen.Add(constructor.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                yield return constructor;
        }
    }

    private static IMethodSymbol? TryGetAttributedConstructor(GeneratorAttributeSyntaxContext syntaxContext)
    {
        if (syntaxContext.TargetSymbol is IMethodSymbol { MethodKind: MethodKind.Constructor } constructor)
            return constructor;

        if (syntaxContext.TargetSymbol is not INamedTypeSymbol typeSymbol)
            return null;

        if (syntaxContext.TargetNode is not TypeDeclarationSyntax { ParameterList: { } parameterList })
            return null;

        return FindMatchingConstructor(typeSymbol, parameterList, syntaxContext.SemanticModel);
    }

    private static IMethodSymbol? TryGetPrimaryConstructor(GeneratorSyntaxContext syntaxContext)
    {
        if (syntaxContext.Node is not TypeDeclarationSyntax typeDeclaration)
            return null;

        if (!typeDeclaration.AttributeLists.Any(static list => list.Target?.Identifier.IsKind(SyntaxKind.MethodKeyword) == true))
            return null;

        if (syntaxContext.SemanticModel.GetDeclaredSymbol(typeDeclaration) is not { } typeSymbol)
            return null;

        if (typeDeclaration.ParameterList is not { } parameterList)
            return null;

        var constructor = FindMatchingConstructor(typeSymbol, parameterList, syntaxContext.SemanticModel);
        if (constructor is null)
            return null;

        return constructor.GetAttributes().Any(attribute =>
            attribute.AttributeClass?.ToDisplayString() == ConstructorAttributeFullName)
            ? constructor
            : null;
    }

    private static IMethodSymbol? FindMatchingConstructor(
        INamedTypeSymbol typeSymbol,
        ParameterListSyntax parameterList,
        SemanticModel semanticModel)
    {

        foreach (var constructorCandidate in typeSymbol.InstanceConstructors)
        {
            if (constructorCandidate.MethodKind is not MethodKind.Constructor)
                continue;

            if (constructorCandidate.Parameters.Length != parameterList.Parameters.Count)
                continue;

            var matched = true;

            for (var index = 0; index < parameterList.Parameters.Count; index++)
            {
                var syntaxParameter = parameterList.Parameters[index];
                var symbolParameter = constructorCandidate.Parameters[index];

                if (!string.Equals(symbolParameter.Name, syntaxParameter.Identifier.ValueText, StringComparison.Ordinal))
                {
                    matched = false;
                    break;
                }

                if (syntaxParameter.Type is null)
                    continue;

                if (semanticModel.GetTypeInfo(syntaxParameter.Type).Type is not { } parameterType)
                {
                    matched = false;
                    break;
                }

                if (!SymbolEqualityComparer.Default.Equals(symbolParameter.Type, parameterType))
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
                return constructorCandidate;
        }

        return null;
    }

    private static GeneratedMethod? TryCreateMethod(IMethodSymbol constructor, INamedTypeSymbol host, Compilation compilation)
    {
        var fetchResultType = TryGetFetchResultType(constructor.ContainingType);
        if (fetchResultType is null)
            return null;

        var methodTypeParameters = GetTypeParameters(constructor.ContainingType);
        var parameters = ImmutableArray.CreateBuilder<string>();
        var arguments = ImmutableArray.CreateBuilder<string>();
        var hasHostParameter = false;

        foreach (var parameter in constructor.Parameters)
        {
            if (IsSameType(parameter.Type, host))
            {
                hasHostParameter = true;
                arguments.Add("this");
                continue;
            }

            if (IsEngineHandleParameter(parameter))
            {
                if (!(parameter.IsOptional && parameter.Ordinal == constructor.Parameters.Length - 1))
                    arguments.Add("default!");

                continue;
            }

            parameters.Add(BuildParameterSignature(parameter, compilation));
            arguments.Add(BuildArgument(parameter));
        }

        if (!hasHostParameter)
            return null;

        return new GeneratedMethod(
            TrimEngineSuffix(constructor.ContainingType.Name),
            BuildFetchEngineReturnType(fetchResultType),
            DisplayType(constructor.ContainingType),
            constructor.GetDocumentationCommentId(),
            BuildTypeParameterList(methodTypeParameters),
            BuildConstraintClauses(methodTypeParameters),
            parameters.ToImmutable(),
            arguments.ToImmutable());
    }

    private static ITypeSymbol? TryGetFetchResultType(INamedTypeSymbol typeSymbol)
    {
        foreach (var item in typeSymbol.AllInterfaces)
            if (item is { Name: "IFetchEngine", TypeArguments.Length: 1 } && item.ContainingNamespace.ToDisplayString() is "Mako.Engine")
                return item.TypeArguments[0];

        return null;
    }

    private static bool IsSameType(ITypeSymbol typeSymbol, INamedTypeSymbol namedTypeSymbol)
        => GetNormalizedTypeName(typeSymbol) == GetNormalizedTypeName(namedTypeSymbol);

    private static bool IsEngineHandleParameter(IParameterSymbol parameter)
        => parameter.Type is INamedTypeSymbol { Name: "EngineHandle" } typeSymbol
           && typeSymbol.ContainingNamespace.ToDisplayString() == EngineHandleNamespace;

    private static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(INamedTypeSymbol typeSymbol)
    {
        var containingTypes = new Stack<INamedTypeSymbol>();

        for (var current = typeSymbol; current is not null; current = current.ContainingType)
            containingTypes.Push(current);

        var builder = ImmutableArray.CreateBuilder<ITypeParameterSymbol>();

        while (containingTypes.Count > 0)
            builder.AddRange(containingTypes.Pop().TypeParameters);

        return builder.ToImmutable();
    }

    private static string GetNormalizedTypeName(ITypeSymbol typeSymbol)
    {
        var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return name.EndsWith("?", StringComparison.Ordinal) ? name[..^1] : name;
    }

    private static string BuildParameterSignature(IParameterSymbol parameter, Compilation compilation)
    {
        var builder = new StringBuilder();

        if (parameter.IsParams)
            builder.Append("params ");

        builder.Append(GetRefKindKeyword(parameter.RefKind));
        builder.Append(DisplayType(parameter.Type));
        builder.Append(' ');
        builder.Append(EscapeIdentifier(parameter.Name));

        if (TryGetDefaultValue(parameter, compilation) is { } defaultValue)
        {
            builder.Append(" = ");
            builder.Append(defaultValue);
        }

        return builder.ToString();
    }

    private static string BuildArgument(IParameterSymbol parameter)
        => GetRefKindKeyword(parameter.RefKind) + EscapeIdentifier(parameter.Name);

    private static string GetRefKindKeyword(RefKind refKind) => refKind switch
    {
        RefKind.Ref => "ref ",
        RefKind.Out => "out ",
        RefKind.In => "in ",
        _ => string.Empty
    };

    private static string BuildFetchEngineReturnType(ITypeSymbol fetchResultType)
        => $"global::Mako.Engine.IFetchEngine<{DisplayType(fetchResultType)}>";

    private static string DisplayType(ITypeSymbol typeSymbol)
        => typeSymbol.ToDisplayString(TypeDisplayFormat);

    private static string BuildTypeParameterList(ImmutableArray<ITypeParameterSymbol> typeParameters)
        => typeParameters.IsDefaultOrEmpty
            ? string.Empty
            : $"<{string.Join(", ", typeParameters.Select(static parameter => EscapeIdentifier(parameter.Name)))}>";

    private static ImmutableArray<string> BuildConstraintClauses(ImmutableArray<ITypeParameterSymbol> typeParameters)
    {
        var builder = ImmutableArray.CreateBuilder<string>();

        foreach (var typeParameter in typeParameters)
            if (TryBuildConstraintClause(typeParameter) is { } clause)
                builder.Add(clause);

        return builder.ToImmutable();
    }

    private static string? TryBuildConstraintClause(ITypeParameterSymbol typeParameter)
    {
        var constraints = new List<string>();

        if (typeParameter.HasUnmanagedTypeConstraint)
        {
            constraints.Add("unmanaged");
        }
        else if (typeParameter.HasValueTypeConstraint)
        {
            constraints.Add("struct");
        }
        else if (typeParameter.HasReferenceTypeConstraint)
        {
            constraints.Add(typeParameter.ReferenceTypeConstraintNullableAnnotation is NullableAnnotation.Annotated ? "class?" : "class");
        }
        else if (typeParameter.HasNotNullConstraint)
        {
            constraints.Add("notnull");
        }

        constraints.AddRange(typeParameter.ConstraintTypes.Select(DisplayType));

        if (typeParameter.HasConstructorConstraint && !typeParameter.HasValueTypeConstraint && !typeParameter.HasUnmanagedTypeConstraint)
            constraints.Add("new()");

        return constraints.Count is 0
            ? null
            : $"where {EscapeIdentifier(typeParameter.Name)} : {string.Join(", ", constraints)}";
    }

    private static string? TryGetDefaultValue(IParameterSymbol parameter, Compilation compilation)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is ParameterSyntax { Default: { } defaultValue })
        {
            var semanticModel = compilation.GetSemanticModel(defaultValue.SyntaxTree);
            return new DefaultValueRewriter(semanticModel).Rewrite(defaultValue.Value).ToFullString();
        }

        return parameter.ExplicitDefaultValue switch
        {
            null => "null",
            bool boolean => boolean ? "true" : "false",
            string text => SymbolDisplay.FormatLiteral(text, true),
            char character => SymbolDisplay.FormatLiteral(character, true),
            float single => single.ToString("R", CultureInfo.InvariantCulture) + "F",
            double number => number.ToString("R", CultureInfo.InvariantCulture),
            decimal decimalNumber => decimalNumber.ToString(CultureInfo.InvariantCulture) + "M",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => parameter.ExplicitDefaultValue.ToString()
        };
    }

    private static string EscapeIdentifier(string identifier)
        => SyntaxFacts.GetKeywordKind(identifier) is not SyntaxKind.None ? $"@{identifier}" : identifier;

    private static string TrimEngineSuffix(string name)
        => name.EndsWith("Engine", StringComparison.Ordinal) ? name[..^"Engine".Length] : name;

    private static string GetHintName(INamedTypeSymbol symbol)
    {
        var name = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var builder = new StringBuilder(name.Length);

        foreach (var ch in name)
            builder.Append(char.IsLetterOrDigit(ch) ? ch : '_');

        return builder.ToString();
    }

    private static string BuildSource(INamedTypeSymbol host, ImmutableArray<GeneratedMethod> methods)
    {
        var builder = new StringBuilder();

        builder.AppendLine("#nullable enable");
        if (!host.ContainingNamespace.IsGlobalNamespace)
        {
            builder.Append("namespace ").Append(host.ContainingNamespace.ToDisplayString()).AppendLine(";");
            builder.AppendLine();
        }

        builder.Append("partial class ")
            .Append(host.Name)
            .Append(BuildTypeParameterList(host.TypeParameters))
            .AppendLine();

        foreach (var constraintClause in BuildConstraintClauses(host.TypeParameters))
            builder.Append(constraintClause).AppendLine();

        builder.AppendLine("{");

        foreach (var method in methods)
        {
            if (method.ConstructorDocumentationId is { Length: > 0 } documentationId)
                builder.Append("    /// <inheritdoc cref=\"").Append(documentationId).AppendLine("\" />");

            builder.Append("    public ")
                .Append(method.ReturnType)
                .Append(' ')
                .Append(method.Name)
                .Append(method.TypeParameterList)
                .Append('(')
                .Append(string.Join(", ", method.Parameters))
                .AppendLine(")");

            foreach (var constraintClause in method.ConstraintClauses)
                builder.Append("        ").Append(constraintClause).AppendLine();

            builder.AppendLine("    {");
            builder.Append("        return new ")
                .Append(method.ConstructorType)
                .Append('(')
                .Append(string.Join(", ", method.Arguments))
                .AppendLine(");");
            builder.AppendLine("    }");
            builder.AppendLine();
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private sealed class GeneratedMethod(
        string name,
        string returnType,
        string constructorType,
        string? constructorDocumentationId,
        string typeParameterList,
        ImmutableArray<string> constraintClauses,
        ImmutableArray<string> parameters,
        ImmutableArray<string> arguments)
    {
        public string Name { get; } = name;

        public string ReturnType { get; } = returnType;

        public string ConstructorType { get; } = constructorType;

        public string? ConstructorDocumentationId { get; } = constructorDocumentationId;

        public string TypeParameterList { get; } = typeParameterList;

        public ImmutableArray<string> ConstraintClauses { get; } = constraintClauses;

        public ImmutableArray<string> Parameters { get; } = parameters;

        public ImmutableArray<string> Arguments { get; } = arguments;

        public string Signature { get; } = typeParameterList + string.Join(", ", parameters);
    }

    private sealed class DefaultValueRewriter(SemanticModel semanticModel) : CSharpSyntaxRewriter
    {
        public ExpressionSyntax Rewrite(ExpressionSyntax expression)
            => (ExpressionSyntax) Visit(expression)!;

        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (TryGetSymbol(node) is IFieldSymbol { HasConstantValue: true } fieldSymbol)
                return SyntaxFactory.ParseExpression($"{DisplayType(fieldSymbol.ContainingType)}.{fieldSymbol.Name}").WithTriviaFrom(node);

            return base.VisitMemberAccessExpression(node);
        }

        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
            => TryRewriteSimpleName(node) ?? base.VisitIdentifierName(node);

        public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
            => TryRewriteSimpleName(node) ?? base.VisitGenericName(node);

        public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
            => TryRewriteName(node) ?? base.VisitQualifiedName(node);

        public override SyntaxNode? VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
            => TryRewriteName(node) ?? base.VisitAliasQualifiedName(node);

        private SyntaxNode? TryRewriteSimpleName(SimpleNameSyntax node)
        {
            if (node.Parent is QualifiedNameSyntax or AliasQualifiedNameSyntax or MemberAccessExpressionSyntax)
                return null;

            return TryRewriteName(node);
        }

        private SyntaxNode? TryRewriteName(TypeSyntax node)
        {
            return TryGetSymbol(node) switch
            {
                ITypeSymbol typeSymbol => SyntaxFactory.ParseTypeName(DisplayType(typeSymbol)).WithTriviaFrom(node),
                IFieldSymbol { HasConstantValue: true } fieldSymbol => SyntaxFactory.ParseExpression($"{DisplayType(fieldSymbol.ContainingType)}.{fieldSymbol.Name}").WithTriviaFrom(node),
                _ => null
            };
        }

        private ISymbol? TryGetSymbol(SyntaxNode node)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(node);
            return symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
        }
    }
}
