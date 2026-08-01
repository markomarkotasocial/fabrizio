using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App
{
	public class UnauthorizedException : Exception
	{
		public UnauthorizedException() : base("User is not authorized") { }
	}

}
