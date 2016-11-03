using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Plugins;

namespace Test_Plugin
{
    public class Test_Plugin : IPlugin {
	    private bool Lul = true;
	    public void Start() {
		    while (Lul) {
			    MessageBox.Show("lul");
			    Thread.Sleep(5000);
		    }
	    }

	    public void Stop() {
		    MessageBox.Show("stopping");
		    Lul = false;
	    }

	    public void Settings() {
		    throw new NotImplementedException();
	    }

	    public string Name() {
		    return "test plugin";
	    }

	    public string Version() {
		    return "0.0.0.1";
	    }
    }
}
