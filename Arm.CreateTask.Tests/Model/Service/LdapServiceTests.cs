using System.Linq;
using Arm.CreateTask.Models.Service;
using FluentAssertions;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Service
{
	[TestFixture()]
	public class LdapServiceTests : BaseTests
	{
		private LdapService _ldapService;

		private ServiceSettingsService _serviceSettingsService;

		[SetUp]
		public void SetUp()
		{
			_serviceSettingsService = new ServiceSettingsService();
			_ldapService = new LdapService(_serviceSettingsService);
		}

		[Test]
		public void GetWatchersByLogin_EmployeeFromBusinessDepartment_ResultShouldContainBush()
		{
			var result = _ldapService.GetWatchersByLogin("istomina.ekaterina");
			result.Should().Contain(x => x.Login == _ldapService.GetBush.Login);
		}

		[Test]
		public void GetWatchersByLogin_EmployeeFromAutomatizationDepartment_ResultShouldContainDelfin()
		{
			var result = _ldapService.GetWatchersByLogin("shwartz.tereza");
			result.Should().Contain(x => x.Login == _ldapService.GetDelfin.Login);
		}

		[Test]
		public void GetWatchersByLogin_LineEmployeeFromGeneralDirection_ResultShouldNotContainJedi()
		{
			var result = _ldapService.GetWatchersByLogin("istomina.ekaterina");
			result.Should().NotContain(x => x.Login == LdapService.GENERAL_DIRECTOR);
		}

		[Test]
		public void GetWatchersByLogin_GeneralDirection_ResultCountShouldBeLessOrEqualToWatchersCount()
		{
			var result = _ldapService.GetWatchersByLogin("istomina.ekaterina");
			result.Count().Should().BeLessOrEqualTo(LdapService.WATCHERS_COUNT);
		}

		[Test]
		public void GetWatchersByLogin_GeneralDirection_ResultShouldBeNullOrEmpty()
		{
			var result = _ldapService.GetWatchersByLogin(_ldapService.GetBush.Login);
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void GetWatchersByLogin_UnknownLogin_ResultShouldBeNullOrEmpty()
		{
			var result = _ldapService.GetWatchersByLogin("anakin.skywalker");
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void GetWatchersByLogin_NoBossInOu_ResultShouldNotBeNullOrEmpty()
		{
			var result = _ldapService.GetWatchersByLogin("tuzov.pavel");
			result.Count().Should().BeLessOrEqualTo(LdapService.WATCHERS_COUNT);
		}
	}
}