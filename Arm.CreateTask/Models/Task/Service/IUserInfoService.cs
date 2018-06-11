using Arm.CreateTask.Models.ManagerADM.DataModel;

namespace Arm.CreateTask.Tests.Model.Service
{
	/// <summary>
	/// Сервис получения информации о пользователе.
	/// </summary>
	public interface IUserInfoService
	{
		/// <summary>
		/// Получает информацию о пользователе.
		/// </summary>
		/// <returns>Информация о пользователе.</returns>
		UserInfo GetUserInfo();
	}
}