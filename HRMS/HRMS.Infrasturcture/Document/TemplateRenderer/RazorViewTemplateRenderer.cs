using Amazon.Runtime.Internal.Util;
using HRMS.Application.Common.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Document.TemplateRenderer
{
    public class RazorViewTemplateRenderer { }
    //    : ITemplateRenderer
    //{
    //    private readonly IMemoryCache _cache;
    //    private readonly string _templateDirectory;

    //    public RazorViewTemplateRenderer(IMemoryCache cache)
    //    {
    //        _cache = cache;
    //        _templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    //    }
    //    public async Task<string> RenderTemplateAsync(string templateName, object model)
    //    {
    //        string templatePath = Path.Combine(_templateDirectory, templateName);
    //        if (!File.Exists(templatePath))
    //            throw new FileNotFoundException($"Template file not found at {templatePath}");

    //        string templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

    //        // Use cache for compiled templates
    //        if (!_cache.TryGetValue(templateName, out Func<object, string>? compiledTemplate))
    //        {
    //            compiledTemplate = CompileTemplate(templateContent);
    //            _cache.Set(templateName, compiledTemplate, TimeSpan.FromDays(1));
    //        }

    //        return compiledTemplate(model);
    //    }

    //    private Func<object, string> CompileTemplate(string content)
    //    {
    //        // Simple string-based replacement: works for @Model.Property syntax
    //        // We’ll compile this using the official Razor parser pipeline
    //        var engine = RazorProjectEngine.Create(RazorConfiguration.Default, RazorProjectFileSystem.Create("/"), b => { });
    //        var sourceDoc = RazorSourceDocument.Create(content, "template.cshtml");
    //        var codeDoc = engine.Process(sourceDoc);
    //        var csharp = codeDoc.GetCSharpDocument().GeneratedCode;

    //        var syntaxTree = CSharpSyntaxTree.ParseText(csharp);
    //        var assemblyName = Path.GetRandomFileName();
    //        var compilation = CSharpCompilation.Create(assemblyName,
    //            new[] { syntaxTree },
    //            AppDomain.CurrentDomain.GetAssemblies()
    //                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
    //                .Select(a => MetadataReference.CreateFromFile(a.Location)),
    //            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    //        using var ms = new MemoryStream();
    //        var result = compilation.Emit(ms);
    //        if (!result.Success)
    //        {
    //            var errors = string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString()));
    //            throw new InvalidOperationException("Template compilation failed:\n" + errors);
    //        }

    //        ms.Seek(0, SeekOrigin.Begin);
    //        var asm = Assembly.Load(ms.ToArray());

    //        // Find generated class and create a callable method
    //        var templateType = asm.GetTypes().FirstOrDefault(t => t.Name.Contains("Template"));
    //        var executeMethod = templateType?.GetMethod("ExecuteAsync");

    //        return model =>
    //        {
    //            var instance = Activator.CreateInstance(templateType!);
    //            var task = (Task)executeMethod!.Invoke(instance, new[] { model })!;
    //            task.GetAwaiter().GetResult();
    //            var prop = templateType.GetProperty("Result");
    //            return prop?.GetValue(instance)?.ToString() ?? string.Empty;
    //        };
    //    }
    //}
}
