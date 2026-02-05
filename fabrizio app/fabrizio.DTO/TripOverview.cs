using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DTO
{
	public class GETTripOverview
	{
		public GETTrip? Previous { get; set; }
		public GETTrip? Current { get; set; }
		public GETTrip? Next { get; set; }
	}

}
