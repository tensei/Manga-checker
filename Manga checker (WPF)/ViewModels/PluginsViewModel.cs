using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MangaChecker.Common;
using MangaChecker.Models;
using Plugins;
using PropertyChanged;

namespace MangaChecker.ViewModels
{
	[ImplementPropertyChanged]
	public class PluginsViewModel
	{
		public  ReadOnlyObservableCollection<PluginModel> Plugins { get; }
		public PluginsViewModel() {

			Plugins = new ReadOnlyObservableCollection<PluginModel>(GlobalVariables.PluginsInternal);
			LoadPlugins();
		}

		private static void LoadPlugins() {
			var pluginfolder = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
			if (!Directory.Exists(pluginfolder)) {
				return;
			}
			var pluginfolders = Directory.GetDirectories(pluginfolder);
			if (pluginfolders.Length == 0) {
				return;
			}
			foreach (var folder in pluginfolders) {
				foreach (var file in Directory.GetFiles(folder)) {
					if (file.EndsWith(".dll")) {
						Assembly.LoadFrom(file);

					}
				}
			}
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes())
				{
					if (type.GetInterface("IPlugin") == null) continue;
					IPlugin p = Activator.CreateInstance(type) as IPlugin;

					GlobalVariables.PluginsInternal.Add(new PluginModel(p));
				}
			}
		}
	}
}
