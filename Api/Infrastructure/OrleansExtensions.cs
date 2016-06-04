using Orleans;
using Owin;
using System;
using System.IO;
using System.Reflection;

public static class OrleansMiddleware
{
    public static void UseOrleans(this IAppBuilder app)
    {
        var codeBaseUri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
        var path = Path.GetDirectoryName(Uri.UnescapeDataString(codeBaseUri.Path));
        var configFilePath = Path.Combine(path, "OrleansConfiguration.xml");
        var configFile = new FileInfo(configFilePath);
        if (!configFile.Exists)
        {
            throw new FileNotFoundException(
                $"Cannot find Orleans client config file at {configFile.FullName}",
                configFile.FullName
            );
        }

        app.Use(async (context, next) =>
        {
            if (!GrainClient.IsInitialized)
            {
                GrainClient.Initialize(configFile);
            }

            await next.Invoke();
        });
    }
}