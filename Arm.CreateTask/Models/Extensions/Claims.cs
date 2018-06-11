using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Arm.CreateTask.Models.Auth.Claims;
using Arm.CreateTask.Models.ManagerADM.DataModel;

namespace Arm.CreateTask.Models.Extensions
{
	/// <summary>
	/// Расширение перечисления <see cref="Claim"/>.
	/// </summary>
	public static class ClaimsExtension
	{
		/// <summary>
		/// Возвращает из перечисления значение <see cref="Claim"/> по типу.
		/// </summary>
		/// <param name="claims">Перечисление <see cref="Claim"/>.</param>
		/// <param name="claim_type">Тип <see cref="Claim"/>.</param>
		/// <returns>Строковое значение <see cref="Claim"/>.</returns>
		public static string SingleOrDefaultValue(this IEnumerable<Claim> claims, string claim_type)
		{
			return claims.SingleOrDefault(x => x.Type == claim_type)?.Value;
		}

		/// <summary>
		/// Возвращает информацию о пользователе, составленную по списку <see cref="Claim"/>.
		/// </summary>
		/// <param name="claims">Список <see cref="Claim"/>.</param>
		/// <returns>Информация о пользователе.</returns>
		public static UserInfo ToUserInfo(this List<Claim> claims)
		{
			return new UserInfo
			{
				Id = claims.SingleOrDefaultValue(CreateTaskClaim.USER_ID)?.ToNullable<int>() ?? 0,
				Sid = claims.SingleOrDefaultValue(ClaimTypes.PrimarySid),
				Email = claims.SingleOrDefaultValue(ClaimTypes.Email),
				JobId = claims.SingleOrDefaultValue(CreateTaskClaim.JOB_ID)?.ToNullable<int>() ?? 0,
				Login = claims.SingleOrDefaultValue(ClaimTypes.NameIdentifier),
				Roles = claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value),
				CityId = claims.SingleOrDefaultValue(CreateTaskClaim.CITY_ID)?.ToNullable<int>(),
				JobName = claims.SingleOrDefaultValue(CreateTaskClaim.JOB_NAME),
				BranchId = claims.SingleOrDefaultValue(CreateTaskClaim.BRANCH_ID)?.ToNullable<int>(),
				FullName = claims.SingleOrDefaultValue(ClaimTypes.GivenName),
				ShortName = claims.SingleOrDefaultValue(ClaimTypes.Name),
				EmployeeId = claims.SingleOrDefaultValue(CreateTaskClaim.EMPLOYEE_ID)?.ToNullable<int>() ?? 0,
				GroupFullName = claims.SingleOrDefaultValue(CreateTaskClaim.GROUP_NAME),
				GroupShortName = claims.SingleOrDefaultValue(CreateTaskClaim.GROUP_SHORT_NAME)
			};
		}
	}
}