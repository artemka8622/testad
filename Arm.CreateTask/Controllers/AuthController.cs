using Arm.CreateTask.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Arm.CreateTask.Controllers
{
	/// <summary>
	/// Контроллер авторизации.
	/// </summary>
	public class AuthController : Controller
	{
		/// <summary>
		/// Интерфейс для работы с конфигурацией.
		/// </summary>
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Инициализирует поля.
		/// </summary>
		/// <param name="configuration">Интерфейс для работы с конфигурацией.</param>
		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Выполняет разлогинивание.
		/// </summary>
		public IActionResult Logout()
		{
			switch (_configuration.GetSection("AuthScheme").Get<AwailableSchemes>())
			{
				case AwailableSchemes.Basic:
					return Challenge();

				case AwailableSchemes.WsFederation:
					HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
					HttpContext.SignOutAsync(WsFederationDefaults.AuthenticationScheme);
					return Redirect("/");

				default:
					throw new Exception("Неизвестная схема авторизации");
			}
		}
	}
}