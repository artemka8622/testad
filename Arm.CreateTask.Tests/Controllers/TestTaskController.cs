using Arm.CreateTask.Controllers;
using Arm.CreateTask.Models.Service;
using Arm.CreateTask.Tests.Model.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arm.CreateTask.Tests.Controllers
{
	/// <summary>
	/// Тестируемый контроллер.
	/// </summary>
	class TestTaskController : TaskController
	{
		/// <summary>
		/// Перенаправляет на url.
		/// </summary>
		/// <param name="url">Url.</param>
		/// <returns>Результат перенаправления.</returns>
		public override RedirectResult Redirect(string url)
		{
			return new RedirectResult(url);
		}

		/// <summary>
		/// Конструктор <see cref="TaskController" /> .
		/// </summary>
		/// <param name="task_create_service">Сервис создания задачи в КСУ.</param>
		/// <param name="task_create_validation_service">Сервис проверки форм для создания задач КСУ.</param>
		/// <param name="service_settings_service">Сервис, предоставляющий доступ к API Noc Cordis.</param>
		/// <param name="logger">Логгер</param>
		/// <param name="user_info_service">Интерфейс для работы с HttpContext.</param>
		public TestTaskController(TaskCreateService task_create_service, TaskFormValidationService task_create_validation_service, ServiceSettingsService service_settings_service, ILogger<TaskController> logger, IUserInfoService user_info_service)
			: base(task_create_service, task_create_validation_service, service_settings_service, logger, user_info_service)
		{
		}
	}
}