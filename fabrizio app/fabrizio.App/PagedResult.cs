using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App
{
	public class PagedResult<T>
	{
		public int TotalCount { get; set; }
		public List<T> Items { get; set; } = new();
	}
}
