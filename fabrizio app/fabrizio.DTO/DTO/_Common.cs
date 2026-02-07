using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class PagedResult<T>
	{
		public int TotalCount { get; set; }
		public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
	}


}
