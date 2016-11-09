using System.Collections.ObjectModel;
using MangaChecker.Common;
using MangaChecker.Models;
using PropertyChanged;

namespace MangaChecker.ViewModels {
	[ImplementPropertyChanged]
	public class PluginsViewModel {
		private readonly ObservableCollection<PluginModel> _plugins = new ObservableCollection<PluginModel>();

		public PluginsViewModel() {
			Plugins = new ReadOnlyObservableCollection<PluginModel>(_plugins);
			LoadPlugins();
		}

		public ReadOnlyObservableCollection<PluginModel> Plugins { get; }

		private void LoadPlugins() {
			foreach (var plugin in PluginHost.Instance.Plugins)
				_plugins.Add(new PluginModel(plugin));
		}
	}
}