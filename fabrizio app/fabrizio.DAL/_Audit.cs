using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using fabrizio.DAL.Helpers;

namespace fabrizio.DAL.Entities
{

	public abstract class BaseEntityGuid
	{
		public Guid Id { get; private set; }
		public Audit Audit { get; set; } = new Audit();
	}

	public abstract class BaseEntityInt
	{
		public int Id { get; private set; }
		public Audit Audit { get; set; } = new Audit();

	}

	public class Audit
	{
		public DateTime AddTime { get; set; } = DateTime.UtcNow;
		public DateTime EditTime { get; set; } = DateTime.UtcNow;
	}

}
