using fabrizio.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Services.Abstractions
{
	public interface IAccountState
	{
		AccountDto? Account { get; set; }
	}

	public class AccountState : IAccountState
	{
		public AccountDto? Account { get; set; }
	}
}
