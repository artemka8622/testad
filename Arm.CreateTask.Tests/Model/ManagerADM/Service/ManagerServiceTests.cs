using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.ManagerADM.Repository;
using Arm.CreateTask.Models.ManagerADM.Service;
using NSubstitute;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.ManagerADM.Service
{
	/// <summary>
	/// Тесты сервиса менеджера.
	/// </summary>
	[TestFixture]
	public class ManagerServiceTests : BaseTests
	{
		/// <summary>
		/// Тестовый логин.
		/// </summary>
		private const string LOGIN = "tester";

		/// <summary>
		/// Репозиторий менеджера.
		/// </summary>
		private IManagerRepository _managerRepository;

		/// <summary>
		/// Сервис менеджера.
		/// </summary>
		private ManagerService _managerService;

		/// <summary>
		/// Инициализирует моки и сервис.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			_managerRepository = Substitute.For<IManagerRepository>();
			_managerService = new ManagerService(Configuration, _managerRepository);
		}

		/// <summary>
		/// Проверяет, что получение менеджера по логину возвращает результат репозитория.
		/// </summary>
		/// <param name="login">Тестовй логин.</param>
		[Test]
		[TestCase(LOGIN)]
		[TestCase("test_domain" + ManagerService.DOMAIN_DELIMITER + LOGIN)]
		public void GetManagerByLogin_SomeLogin_ResultFromRepository(string login)
		{
			var expected_manager = new Manager {Id = -5};
			_managerRepository.GetManagerByLogin(Arg.Is<string>(l => l.EndsWith($"{ManagerService.DOMAIN_DELIMITER}{LOGIN}")))
				.Returns(expected_manager);

			Manager result = _managerService.GetManagerByLogin(login);

			Assert.AreEqual(expected_manager.Id, result.Id);
		}
	}
}