using HRMS.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrasturcture.TemplateRenderer
{
    public class RazorLightTemplateRenderer { }
        
    //    : ITemplateRenderer
    //{
    //    private readonly RazorLightEngine _engine;
    //    private readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    //    private readonly string _templateDirectory = GeneralConstants.TemplateDirectory;
    //    private readonly IMemoryCache _cache;
    //    public RazorLightTemplateRenderer(IMemoryCache cache)
    //    {
    //        _cache = cache;
    //        var templateDir = Path.Combine(_baseDirectory, _templateDirectory);

    //        if (!Directory.Exists(templateDir))
    //            throw new DirectoryNotFoundException(string.Format(GeneralConstants.TemplateDirectoryNotFoundError, templateDir));

    //        _engine = new RazorLightEngineBuilder()
    //            .UseFileSystemProject(templateDir)
    //            .UseMemoryCachingProvider()
    //            .SetOperatingAssembly(Assembly.GetExecutingAssembly())
    //            .Build();
    //    }

    //    public async Task<string> RenderTemplateAsync(string templateName, object data)
    //    {
    //        string stringTemplate = await GetTemplateAsync(templateName);
    //        return await _engine.CompileRenderStringAsync(templateName, stringTemplate, data);
    //    }
    //    private async Task<string> GetTemplateAsync(string templateName)
    //    {
    //        if (_cache.TryGetValue(templateName, out string? templateContent) && !string.IsNullOrEmpty(templateContent))
    //            return templateContent;

    //        string templatePath = Path.Combine(_baseDirectory, _templateDirectory, templateName);

    //        if (!File.Exists(templatePath))
    //            throw new FileNotFoundException(string.Format(GeneralConstants.TemplateNotFoundError, templatePath));

    //        var stringTemplate = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);
    //        _cache.Set(templateName, stringTemplate, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)));

    //        return stringTemplate;
    //    }
    //}
}
