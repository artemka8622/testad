using Cordis.Core.Common.Sql;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Arm.CreateTask.Tests
{
	/// <summary>
	/// Настройщик для всех тестов.
	/// </summary>
	// Затрагивает только тесты в определенном выше namespace!
	[SetUpFixture]
	public class TestsSetup : BaseTests
	{
		/// <summary>
		/// Инициирует строку подключения.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			SqlHelper.СonnectionString = Configuration.GetConnectionString("Default");
		}
	}
}
