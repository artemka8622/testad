using System.Collections.Generic;
using System.Security.Claims;
using Arm.CreateTask.Models.Auth.Claims;
using Arm.CreateTask.Models.Extensions;
using Arm.CreateTask.Models.ManagerADM.DataModel;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Extensions
{
	/// <summary>
	/// Тесты расширения перечисления <see cref="Claim"/>.
	/// </summary>
	[TestFixture]
	public class ClaimsExtensionTests
	{
		/// <summary>
		/// Проверяет, что получение информации о пользователе возвращает корректный идентификатор.
		/// </summary>
		[Test]
		public void ToUserInfo_ClaimsWithUserId_CorrectId()
		{
			int expected_id = -45;
			var claims = new List<Claim> {new Claim(CreateTaskClaim.USER_ID, expected_id.ToString())};

			UserInfo result = claims.ToUserInfo();

			Assert.AreEqual(expected_id, result.Id);
		}
	}
}