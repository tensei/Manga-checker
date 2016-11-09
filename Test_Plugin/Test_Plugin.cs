using System.ComponentModel.Composition;
using MangaChecker.Interfaces;

namespace Test_Plugin {
	[Export(typeof(IPlugin))]
	[ExportMetadata("Title", "TestPlugin")]
	[ExportMetadata("Description", "Weow")]
	[ExportMetadata("Version", "0.1")]
	[ExportMetadata("Author", "Tensei")]
	public class TestPlugin : IPlugin {
		public void Dispose() {
			//throw new NotImplementedException();
		}

		public void Initialize() {
			//throw new NotImplementedException();
		}

		public object View() {
			return new TestView();
		}
	}
}