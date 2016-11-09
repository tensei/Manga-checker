using System;
using MangaChecker.Interfaces;
using PropertyChanged;

namespace MangaChecker.Models {
	[ImplementPropertyChanged]
	public class PluginModel {
		public PluginModel(Lazy<IPlugin, IPluginMetadata> p) {
			Name = p.Metadata.Title;
			Version = p.Metadata.Version;
			View = p.Value.View();
		}

		public string Name { get; set; }

		public string Version { get; set; }

		public string Status { get; set; } = "Stopped";

		public object View { get; set; }
	}
}