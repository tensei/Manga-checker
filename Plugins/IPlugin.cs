﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
	public interface IPlugin {
		void Start();
		void Stop();
		void Settings();
		string Name();
		string Version();
	}
}
