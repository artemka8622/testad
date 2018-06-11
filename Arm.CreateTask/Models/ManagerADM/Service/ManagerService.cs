using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.ManagerADM.Repository;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.ManagerADM.Service
{
	/// <summary>
	/// Сервис менеджера.
	/// </summary>
	public class ManagerService
	{
		/// <summary>
		/// Символы, отделяющие доменный префикс.
		/// </summary>
		public const string DOMAIN_DELIMITER = @"\";

		/// <summary>
		/// Интерфейс для работы с конфигурацией.
		/// </summary>
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Репозиторий менеджера.
		/// </summary>
		private readonly IManagerRepository _managerRepository;

		/// <summary>
		/// Инициализирует поля.
		/// </summary>
		/// <param name="configuration">Интерфейс для работы с конфигурацией.</param>
		/// <param name="manager_repository">Репозиторий менеджера.</param>
		public ManagerService(IConfiguration configuration, IManagerRepository manager_repository)
		{
			_configuration = configuration;
			_managerRepository = manager_repository;
		}

		/// <summary>
		/// Возвращает менеджера по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Менеджер.</returns>
		public Manager GetManagerByLogin(string login)
		{
			login = GetLoginWithDomain(login, _configuration["DefaultDomain"]);
			return _managerRepository.GetManagerByLogin(login);
		}

		/// <summary>
		/// Соединяет доменный префикс с логином при необходимости.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <param name="domain">Доменный префикс.</param>
		/// <returns>Логин с доменным префиксом.</returns>
		private string GetLoginWithDomain(string login, string domain)
		{
			return login.Contains(DOMAIN_DELIMITER) ? login : $"{domain}{DOMAIN_DELIMITER}{login}";
		}
	}
}