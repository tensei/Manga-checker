using System;

namespace MangaChecker.Interfaces {
	public interface IPlugin : IDisposable {
		void Initialize();
		object View();
	}
}