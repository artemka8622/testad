using System;
using System.Collections.Generic;
using System.Linq;
using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.Repository;
using Atlassian.Jira;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис создания задач КСУ.
	/// </summary>
	public class TaskCreateService
	{
		#region Constants

		/// <summary>
		/// Тип задачи в КСУ - ""Эксплуатация".
		/// </summary>
		private const int ISSUE_OPERATIVE = 10400;

		/// <summary>
		/// Тип задачи в КСУ - "Оперативная"
		/// </summary>
		private const int ISSUE_OPERATIVE_OLD = 3;

		/// <summary>
		/// Идентификатор проекта Вебург.
		/// </summary>
		private const int WEBURG_PROJECT_ID = 3;

		/// <summary>
		/// Значение "Тип обращения – 'Хотспоты. Подготовить персональный дизайн'". Используется для переопределения типа задачи.
		/// </summary>
		private const int EXPLORATION_HOTSPOT_DESIGN = 10;

		#endregion

		#region Properties

		/// <summary>
		/// Репозиторий типов задач.
		/// </summary>
		private readonly IBaseConfigurationRepository<Task> _taskRepository;

		/// <summary>
		/// Репозиторий для работы с процессами <see cref="Workflow"/>.
		/// </summary>
		private readonly IBaseConfigurationRepository<Workflow> _workflowRepository;

		/// <summary>
		/// Репозиторий для работы со связями роли и процесса <see cref="Role2Workflow"/>.
		/// </summary>
		private readonly IBaseConfigurationRepository<Role2Workflow> _role2WorkflowRepository;

		/// <summary>
		/// Репозиторий для работы с товаром <see cref="Product"/>.
		/// </summary>
		private readonly IBaseConfigurationRepository<Product> _productRepository;

		/// <summary>
		/// Репозиторий для работы с исследованиями <see cref="Exploration"/>.
		/// </summary>
		private readonly IBaseConfigurationRepository<Exploration> _explorationRepository;

		/// <summary>
		/// Репозиторий для работы с периметром <see cref="Perimeter"/>.
		/// </summary>
		private readonly IBaseConfigurationRepository<Perimeter> _perimeterRepository;

		/// <summary>
		/// Сервис для работы с JIRA.
		/// </summary>
		private readonly IJiraApiService _jiraApiService;

		/// <summary>
		/// Сервис Ldap.
		/// </summary>
		private readonly ILdapService _ldapService;

		#endregion

		#region Constructor

		/// <summary>
		/// Конструктор <see cref="TaskCreateService"/>.
		/// </summary>
		/// <param name="jira_api_service">Сервис для работы с JIRA.</param>
		/// <param name="ldap_service">Сервис Ldap.</param>
		/// <param name="task_repository">Репозиторий типов задач.</param>
		/// <param name="workflow_repository">Репозиторий для работы с процессами <see cref="Workflow"/>.</param>
		/// <param name="role2_workflow_repository">Репозиторий для работы со связями роли и процесса <see cref="Role2Workflow"/>.</param>
		/// <param name="product_repository">Репозиторий для работы с товаром <see cref="Product"/>.</param>
		/// <param name="exploration_repository">Репозиторий для работы с исследованиями <see cref="Exploration"/>.</param>
		/// <param name="perimeter_repository">Репозиторий для работы с периметром <see cref="Perimeter"/>.</param>
		public TaskCreateService(
			IJiraApiService jira_api_service, 
			ILdapService ldap_service,
			IBaseConfigurationRepository<Task> task_repository,
			IBaseConfigurationRepository<Workflow> workflow_repository,
			IBaseConfigurationRepository<Role2Workflow> role2_workflow_repository,
			IBaseConfigurationRepository<Product> product_repository,
			IBaseConfigurationRepository<Exploration> exploration_repository, 
			IBaseConfigurationRepository<Perimeter> perimeter_repository)
		{
			_jiraApiService = jira_api_service;
			_ldapService = ldap_service;
			_taskRepository = task_repository;
			_workflowRepository = workflow_repository;
			_role2WorkflowRepository = role2_workflow_repository;
			_productRepository = product_repository;
			_explorationRepository = exploration_repository;
			_perimeterRepository = perimeter_repository;
		}

		#endregion

		#region Methods/Public

		/// <summary>
		/// Получает список типов задач, доступных пользователю.
		/// </summary>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <returns>Список задач.</returns>
		public IEnumerable<Task> GetTasks(UserInfo user_info)
		{
			var wfs = GetAvailableWorkflows(user_info);

			return _taskRepository.GetAll().Join(wfs, x => x.Code, x => x.TaskCode, (x, y) => x).Distinct();
		}

		/// <summary>
		/// Получает список информационных систем.
		/// </summary>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <param name="task_code">Код типа задачи.</param>
		/// <returns>Список информационных систем.</returns>
		public IEnumerable<Product> GetProducts(UserInfo user_info, string task_code)
		{
			var wfs = GetAvailableWorkflows(user_info).Where(x => x.TaskCode == task_code);

			return _productRepository.GetAll().Join(wfs, x => x.Id, x => x.ProductId, (x, y) => x).Distinct();
		}

		/// <summary>
		/// Получает список типов обращений.
		/// </summary>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <param name="information_system_code">Код ИС.</param>
		/// <param name="task_code">Код типа задач.</param>
		/// <returns>Список типов обращений.</returns>
		public IEnumerable<Exploration> GetExplorations(UserInfo user_info, string information_system_code, string task_code)
		{
			var product = GetProduct(information_system_code);

			if (product == null)
			{
				return Enumerable.Empty<Exploration>();
			}

			var wfs = GetAvailableWorkflows(user_info).Where(x => x.ProductId == product.Id && x.TaskCode == task_code);

			return _explorationRepository.GetAll().Join(wfs, x => x.Id, x => x.ExplorationId, (x, y) => x).Distinct();
		}

		/// <summary>
		///	Создает задачу в КСУ.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <returns>Ключ задачи.</returns>
		public string CreateTask(CreateForm form, UserInfo user_info)
		{
			var workflow = GetWorkflow(form, user_info);

			// сгенерировать основной текст задачи
			var task_content = GenerateContent(form);

			// сгенерировать заголовок
			var task_subject = CreateSubject(form);

			// получить маршрут
			var project = GetProject(form, user_info);

			// получить версии
			var versions = GetVersions(form, project);

			// получить метки
			var labels = GetLabels(form);

			// получить наблюдателей
			var watchers = _ldapService.GetWatchersByLogin(user_info.Login).ToList();

			// определить бизнес-процесс
			var business_process = GetWorkflow(form, user_info).BusinessProcess;

			// определить тип задачи
			var issue_type = GetIssueType(form);

			// создать задачу
			var issue = _jiraApiService.CreateIssue(project, task_subject, task_content, issue_type.ToString(), user_info.Login, versions, business_process, business_process, labels);

			// Добавили наблюдателей
			_jiraApiService.AddWatchers(watchers.Select(x => x.Login), issue);

			_jiraApiService.AddComment(form, watchers, issue, user_info.FullName, task_subject, workflow);

			_jiraApiService.AddFiles(form, issue);

			return issue.Key.Value;
		}

		#endregion

		#region Methods/Private

		/// <summary>
		/// Получает тип задачи.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <returns>Тип задачи.</returns>
		private int GetIssueType(CreateForm form)
		{
			if ((form.ExplorationId ?? 0) == EXPLORATION_HOTSPOT_DESIGN || _productRepository.GetAll().FirstOrDefault(x => x.Code == form.InformationSystemCode)?.Id == WEBURG_PROJECT_ID)
				return ISSUE_OPERATIVE_OLD;
			return ISSUE_OPERATIVE;
		}

		/// <summary>
		/// Получает основной текст для задачи.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <returns>Основной текст для задачи.</returns>
		private string GenerateContent(CreateForm form)
		{
			switch (form.Task)
			{
				case TaskCode.Explore: return form.Notes;
				case TaskCode.Develop: return $"<p>Требуется разработать ПО в соответствии с SRS: <a href=\"{form.SRSLink }\">{form.SRSLink}</a></p>";
				case TaskCode.Srs: return $"<p>Требуется разработать SRS на основании FRS: <a href=\"{form.FRSLink}\">{form.FRSLink}</a>";
				case TaskCode.Estimate: return $"<p>Требуется оценить экспертные трудозатраты и срок по SRS: <a href=\"{form.SRSLink}\">{form.SRSLink}</a>";
				case TaskCode.Telsupport: return $"<p>Требуется внедрить ПО, разработанное в результате выполнения задачи: <a href=\"{form.ParentTaskLink}\">{form.ParentTaskLink}</a>";
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Получает заголовок.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <returns>Заголовок.</returns>
		private string CreateSubject(CreateForm form)
		{
			var task = GetTask(form.Task);
			return $"{(task.Code == TaskCode.Explore.ToString() ? GetExploration(form.ExplorationId.Value).Name : task.Name)}. {GetProduct(form.InformationSystemCode).Name}. {form.Subject}";
		}

		/// <summary>
		/// Получает маршрут.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <returns>Маршрут.</returns>
		private string GetProject(CreateForm form, UserInfo user_info)
		{
			var workflow = GetWorkflow(form, user_info);

			return _perimeterRepository.GetAll().First(x => x.Key == workflow.PerimeterId).Value;
		}

		/// <summary>
		/// Получает версии.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <param name="project">Маршрут.</param>
		/// <returns>Список версий.</returns>
		private IEnumerable<ProjectVersion> GetVersions(CreateForm form, string project)
		{
			var period = GetPeriod(form, DateTime.Today);

			var versions = _jiraApiService.GetVersions(project);

			return versions.Where(x => x.Name == period);
		}

		/// <summary>
		/// Получает период.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <param name="date">Дата.</param>
		/// <returns>Период.</returns>
		private string GetPeriod(CreateForm form, DateTime date)
		{
			int days_left = DateTime.DaysInMonth(date.Year, date.Month) - date.Day;
			int current_period = date.Month;
			int current_year = date.Year;

			if (form.Task == TaskCode.Explore)
			{
				if (days_left < 4)
				{
					current_period++;
				}
			}
			else if (DateTime.Today.Day < 24)
			{
				current_period++;
			}
			else
			{
				current_period += 2;
			}

			return new DateTime(current_year, current_period, 1).ToString("yyyy.MM");
		}

		/// <summary>
		/// Получает метки.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <returns>Метки.</returns>
		private string GetLabels(CreateForm form) => form.ExplorationId.HasValue ? GetExploration(form.ExplorationId.Value).Name.Replace(' ', '_') : string.Empty;

		/// <summary>
		/// Получает список процессов.
		/// </summary>
		/// <param name="form">Форма для создания задачи.</param>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <returns>Список процессов</returns>
		private Workflow GetWorkflow(CreateForm form, UserInfo user_info) => GetAvailableWorkflows(user_info)
			.First(x => x.ProductId == GetProduct(form.InformationSystemCode).Id && x.TaskCode == form.Task.ToString() && x.ExplorationId == form.ExplorationId);

		/// <summary>
		/// Получает исследование по идентификатору.
		/// </summary>
		/// <param name="exploration_id">Идентификатор исследования.</param>
		/// <returns>Исследование.</returns>
		private Exploration GetExploration(int exploration_id) => _explorationRepository.GetAll().First(x => x.Id == exploration_id);

		/// <summary>
		/// Получает тип задачи по коду.
		/// </summary>
		/// <param name="code">Код типа задачи.</param>
		/// <returns>Тип задачи.</returns>
		private Task GetTask(TaskCode code) => _taskRepository.GetAll().First(x => x.Code == code.ToString());

		/// <summary>
		/// Получает ИС.
		/// </summary>
		/// <param name="code">Код ИС.</param>
		/// <returns>ИС.</returns>
		private Product GetProduct(string code) => _productRepository.GetAll().FirstOrDefault(x => x.Code == code);

		/// <summary>
		/// Получает список доступных процессов для пользователя.
		/// </summary>
		/// <param name="user_info">Информация о пользователе.</param>
		/// <returns>Список доступных процессов для пользователя.</returns>
		private IEnumerable<Workflow> GetAvailableWorkflows(UserInfo user_info)
		{
			var workflow = _workflowRepository.GetAll();

			var r_to_w = _role2WorkflowRepository.GetAll().Where(x => user_info.Roles != null && user_info.Roles.Contains(x.Role));

			return workflow.Join(r_to_w, x => x.Id, x => x.WorkflowId, (x, y) => x);
		}

		#endregion
	}
}