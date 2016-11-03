using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaChecker.ViewModels;
using Plugins;
using PropertyChanged;

namespace MangaChecker.Models
{
	[ImplementPropertyChanged]
	public class PluginModel
	{
		private IPlugin Plugin { get; }

		public string Name { get; set; }

		public string Version { get; set; }

		public string Status { get; set; } = "Stopped";


		public PluginModel(IPlugin p) {
			Plugin = p;
			Start = new ActionCommand(() => { Task.Run(() => {
				Status = "Running";
				Plugin.Start();
			}); });
			Stop = new ActionCommand(() => { Task.Run(() => {
				Status = "Stopped";
				Plugin.Stop();
			}); });
			Settings = new ActionCommand(() => {
				Task.Run(() => {
					Plugin.Settings();
				});
			});
			Name = p.Name();
			Version = p.Version();
		}

		public ICommand Start { get; }
		public ICommand Stop { get; }
		public ICommand Settings { get; }

	}
}
