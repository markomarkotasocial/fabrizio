using fabrizio.App.Pages.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public class AuthService
	{
		private bool _isLoggingOut;

		public async Task LogoutAsync()
		{
			if (_isLoggingOut) return;
			_isLoggingOut = true;

			try
			{
				SecureStorage.Remove("jwt_token");
				MainThread.BeginInvokeOnMainThread(() =>
				{
					Application.Current.MainPage = new LoginPage();
				});
			}
			finally
			{
				_isLoggingOut = false;
			}
		}
	}

}
