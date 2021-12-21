using System;
using System.IO;
using Bridge.Commons.System.Contracts.Settings;
using Microsoft.Extensions.Configuration;

namespace Bridge.Commons.System.AspNet.Settings
{
    /// <summary>
    ///     Configurações do .NetCore
    /// </summary>
    public class DotNetSettings : ISettings
    {
        /// <summary>
        ///     Buscar configurações do App
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAppSettings<T>() where T : IAppSettings, new()
        {
            var configurationRoot = GetConfigurationRoot();

            var config = new { AppSettings = new T() };
            configurationRoot.Bind(config);

            return config.AppSettings;
        }

        /// <summary>
        ///     Buscar configurações do root
        /// </summary>
        /// <returns></returns>
        public IConfigurationRoot GetConfigurationRoot()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var jsonFileName = "appsettings.json";

            if (!string.IsNullOrWhiteSpace(environment))
                jsonFileName = $"appsettings.{environment.ToLowerInvariant()}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile(jsonFileName, false)
                .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            return configurationRoot;
        }
    }
}