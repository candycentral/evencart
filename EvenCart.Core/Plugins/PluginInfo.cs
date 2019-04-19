﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EvenCart.Core.Infrastructure;

namespace EvenCart.Core.Plugins
{
    public class PluginInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public string SystemName { get; set; }

        public IList<string> SupportedVersions { get; set; }

        public string Author { get; set; }

        public string AuthorUri { get; set; }

        public string PluginUri { get; set; }

        public string AssemblyName { get; set; }

        public string PluginDirectory { get; set; }

        public Assembly Assembly { get; set; }

        public FileInfo OriginalAssemblyFileInfo { get; set; }

        public Type PluginType { get; set; }

        public bool Installed { get; set; }

        public bool Active { get; set; }

        public IList<IWidget> Widgets { get; set; }

        public string ConfigurationUrl => LoadPluginInstance<IPlugin>().ConfigurationUrl;

        public static PluginInfo Load(string fileName)
        {
            return PluginConfigurator.LoadModuleInfo(fileName);
        }

        public static PluginInfo Load(IPlugin module)
        {
            return PluginConfigurator.LoadModuleInfo(module);
        }

        public T LoadPluginInstance<T>() where T : class, IPlugin
        {
            var instance = DependencyResolver.Resolve(this.PluginType);
            var pluginTypedInstance = instance as T;
            if (pluginTypedInstance != null)
                pluginTypedInstance.PluginInfo = this;
            return pluginTypedInstance;
        }
    }
}