using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Helpers
{
	public static class SequentialGuidGenerator
	{
		public static Guid NewSequentialGuid()
		{
			var guidArray = Guid.NewGuid().ToByteArray();
			var now = DateTime.UtcNow;

			// Get timestamp (ticks = 100ns intervals since 1/1/0001)
			var timestamp = BitConverter.GetBytes(now.Ticks);

			// Replace last 6 bytes with timestamp (common "COMB" Guid strategy)
			Array.Copy(timestamp, 0, guidArray, 10, 6);

			return new Guid(guidArray);
		}
	}


}
