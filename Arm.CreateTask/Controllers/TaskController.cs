using System.Linq;
using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Extensions;
using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.Roles;
using Arm.CreateTask.Models.Service;
using Arm.CreateTask.Tests.Model.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Arm.CreateTask.Controllers
{
	/// <summary>
	/// Контроллер для работы с задачами КСУ.
	/// </summary>
	[Authorize(Roles = Role.Arm.CreateTask.Access)]
	public class TaskController : Controller
	{
		/// <summary>
		/// Код сервиса Jira.
		/// </summary>
		private const string JIRA_SERVICE_CODE = "jira_api";

		/// <summary>
		/// Настройки сервиса Jira.
		/// </summary>
		private readonly ServiceSettings _jiraServiceSettings;

		/// <summary>
		/// Сервис создания задачи в КСУ.
		/// </summary>
		private readonly TaskCreateService _taskCreateService;

		/// <summary>
		/// Сервис проверки форм для создания задач КСУ.
		/// </summary>
		private readonly TaskFormValidationService _taskFormValidationService;

		/// <summary>
		/// Логгер.
		/// </summary>
		private readonly ILogger<TaskController> _logger;

		/// <summary>
		/// Код сервиса Jira.
		/// </summary>
		private readonly IUserInfoService _userInfoService;

		/// <summary>
		/// Конструктор <see cref="TaskController"/>.
		/// </summary>
		/// <param name="task_create_service">Сервис создания задачи в КСУ.</param>
		/// <param name="task_form_validation_service">Сервис проверки форм для создания задач КСУ.</param>
		/// <param name="service_settings_service">Сервис, предоставляющий доступ к API Noc Cordis.</param>
		/// <param name="logger">Логгер.</param>
		/// <param name="user_info_service">Сервис получени информации о пользователе.</param>
		public TaskController(TaskCreateService task_create_service, 
			TaskFormValidationService task_form_validation_service, 
			ServiceSettingsService service_settings_service, 
			ILogger<TaskController> logger,
			IUserInfoService user_info_service)
		{
			_jiraServiceSettings = service_settings_service.GetSettingsByCode(JIRA_SERVICE_CODE);
			_taskCreateService = task_create_service;
			_logger = logger;
			_userInfoService = user_info_service;
			_taskFormValidationService = task_form_validation_service;
		}

		/// <summary>
		/// Первая страница.
		/// </summary>
		/// <returns>Страница индекса <see cref="IActionResult"/>.</returns>
		[HttpGet]
		public IActionResult Index()
		{
			_logger.LogTrace("Index");
			return View();
		}

		/// <summary>
		/// Получение справочника информационных систем.
		/// </summary>
		/// <param name="code">Код типа задач.</param>
		/// <returns>Справочник информационных систем.</returns>
		[HttpGet]
		public IActionResult GetInformationSystems(string code)
		{
			_logger.LogDebug($"GetInformationSystems {nameof(code)} - {code}");

			var items = _taskCreateService.GetProducts(_userInfoService.GetUserInfo(), code).Select(x => new
			{
				name = x.Name,
				code = x.Code
			}).ToList();
			return new JsonResult(items);
		}

		/// <summary>
		/// Получение типов обращений.
		/// </summary>
		/// <param name="information_system_code">Код информационной системы.</param>
		/// <param name="task_code">Код типа задачи.</param>
		/// <returns>Список типов обращений.</returns>
		[HttpGet]
		public IActionResult GetExploration(string information_system_code, string task_code)
		{
			_logger.LogDebug($"GetExploration {nameof(information_system_code)} : {information_system_code}, {nameof(task_code)} : {task_code}");
			
			var items = _taskCreateService.GetExplorations(_userInfoService.GetUserInfo(), information_system_code, task_code).Select(x => new
			{
				id = x.Id,
				name = x.Name
			}).ToList();
			return new JsonResult(items);
		}

		/// <summary>
		/// Получение справочника типов задач.
		/// </summary>
		/// <returns>Справочник типов задач.</returns>
		[HttpGet]
		public IActionResult GetTypeTasks()
		{
			_logger.LogDebug("GetTypeTasks");
			var tasks = _taskCreateService.GetTasks(_userInfoService.GetUserInfo()).Select(x => new
			{
				code = x.Code,
				name = x.Name
			}).ToList();
			return new JsonResult(tasks);
		}

		/// <summary>
		/// Получает информацию о пользователе.
		/// </summary>
		/// <returns>Информация о пользователе.</returns>
		[HttpGet]
		public IActionResult GetUser()
		{
			_logger.LogDebug("GetUser");
			
			var info = _userInfoService.GetUserInfo();

			return new JsonResult(new
			{
				login = info.Login,
				name = info.FullName
			});
		}

		/// <summary>
		/// Создает задачу в КСУ.
		/// </summary>
		/// <param name="create_form">Форма создания задачи.</param>
		/// <returns>Список ошибок или ссылка на задачу в КСУ.</returns>
		[HttpPost]
		public IActionResult CreateTask(CreateForm create_form)
		{
			_logger.LogInformation($"CreateTask {nameof(create_form)} - {JsonConvert.SerializeObject(create_form)}");

			var errors = _taskFormValidationService.Validate(create_form).ToList();

			if (!errors.Any())
			{
				var issue_key = _taskCreateService.CreateTask(create_form, _userInfoService.GetUserInfo());
				return Redirect(MakeUrlToIssue(issue_key));
			}

			return new JsonResult(errors);
		}

		#region Methods/private

		/// <summary>
		/// Получает ссылку на задачу.
		/// </summary>
		/// <param name="key">Ключ задачи.</param>
		/// <returns>Ссылка на задачу в Jira.</returns>
		private string MakeUrlToIssue(string key)
		{
			var url = _jiraServiceSettings.Url;
			if (!url.EndsWith('/'))
			{
				url += '/';
			}

			return $"{url}browse/{key}";
		}


		#endregion
	}
}
