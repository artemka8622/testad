using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.ManagerADM.Repository;
using Cordis.Core.Common.Sql;
using Dapper;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.ManagerADM.Repository
{
	/// <summary>
	/// Тесты репозитория менеджера.
	/// </summary>
	[TestFixture]
	public class ManagerRepositoryTests : TransactionTests
	{
		/// <summary>
		/// Репозиторий менеджера.
		/// </summary>
		private IManagerRepository _managerRepository;

		/// <summary>
		/// Инициализирует репозиторий.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			_managerRepository = new ManagerRepository();
		}

		/// <summary>
		/// Проверяет, что получение менеджера по существующему логину возвращает ненулевой результат.
		/// </summary>
		[Test]
		public void GetManagerByLogin_ExistingLogin_NotNullResult()
		{
			string login;
			using (SqlHelper sqlh = new SqlHelper())
			{
				login = sqlh.GetConnection().ExecuteScalar<string>(@"
select		m.nt_login
from		CRM.manager m 
	join	Employee.employee e on e.manager = m.manager and e.is_fired = 0
	join	CRM.manager_group g on g.manager_group = e.manager_group
	join	CRM.job j on j.job = e.job 
order by	newid()");
			}

			Manager result = _managerRepository.GetManagerByLogin(login);

			Assert.NotNull(result);
		}
	}
}