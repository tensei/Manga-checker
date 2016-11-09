using System.Collections.Generic;
using System.Linq;
using MangaChecker.Interfaces;

namespace MangaChecker.Common {
	public class AggregatePlugins : IPlugin {
		private readonly IPlugin[] _sites;

		public AggregatePlugins(IEnumerable<IPlugin> sites) {
			_sites = sites.ToArray();
		}

		public void Initialize() {
			foreach (var site in _sites) site.Initialize();
		}

		public object View() {
			return null;
		}


		public void Dispose() {
			foreach (var site in _sites) site.Dispose();
		}
	}
}