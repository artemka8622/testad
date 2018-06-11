using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arm.CreateTask.Models.Extensions;
using Arm.CreateTask.Models.ManagerADM.DataModel;
using Microsoft.AspNetCore.Http;

namespace Arm.CreateTask.Tests.Model.Service
{
	/// <summary>
	/// Сервис получения информации о пользователе.
	/// </summary>
	public class UserInfoService : IUserInfoService
	{
		/// <summary>
		/// Интерфейс для работы с HttpContext.
		/// </summary>
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="http_context_accessor">Интерфейс для работы с HttpContext.</param>
		public UserInfoService(IHttpContextAccessor http_context_accessor)
		{
			_httpContextAccessor = http_context_accessor;
		}

		/// <summary>
		/// Получает информацию о пользователе.
		/// </summary>
		/// <returns>Информация о пользователе.</returns>
		public UserInfo GetUserInfo()
		{
			return _httpContextAccessor.HttpContext.User.Claims.ToList().ToUserInfo();
		}
	}
}
