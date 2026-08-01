using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Entities
{
	//public enum DestinationTypes
	//{
		
	//}

	public class Destination : BaseEntityGuid
	{
		//public DestinationTypes Type { get; set; }

		public string Name { get; set; } = string.Empty;
		public int Order { get; set; }


		public Guid? TripId { get; set; }
		public virtual Trip? Trip { get; set; }

		public int AccountId { get; set; }
		public Account? Account { get; set; }


	}
}
