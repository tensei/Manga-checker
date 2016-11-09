using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using MangaChecker.Interfaces;

namespace MangaChecker.Common {
	public class PluginHost : IDisposable {
		private const string PluginsDirectory = "Plugins";


		private readonly CompositionContainer container;
		private static readonly PluginHost instance = new PluginHost();

		static PluginHost() {
		}


		private PluginHost() {
			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			var current = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			if (current != null) {
				var pluginsPath = Path.Combine(current, PluginsDirectory);
				if (Directory.Exists(pluginsPath)) {
					var dcat = new DirectoryCatalog(pluginsPath);
					catalog.Catalogs.Add(dcat);
				}
			}

			container = new CompositionContainer(catalog);
		}

		public static PluginHost Instance {
			get { return instance ; }
		}

		[ImportMany]
		public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }

		public void Dispose() {
			container.Dispose();
		}

		public void Initialize() {
			container.ComposeParts(this);
			GetSites().Initialize();
		}

		private IPlugin GetSites() {
			return new AggregatePlugins(Plugins.Select(s => s.Value));
		}
	}
}