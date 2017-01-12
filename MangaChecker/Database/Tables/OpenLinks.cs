using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MangaChecker.Database.Tables {
	public class OpenLinks {
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		[NotNull]
		public bool Open { get; set; }
	}
}
