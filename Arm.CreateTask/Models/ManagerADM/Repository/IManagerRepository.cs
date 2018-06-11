using Arm.CreateTask.Models.ManagerADM.DataModel;

namespace Arm.CreateTask.Models.ManagerADM.Repository
{
	/// <summary>
	/// Интерфейс репозитория менеджера.
	/// </summary>
	public interface IManagerRepository
	{
		/// <summary>
		/// Возвращает менеджера по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Менеджер.</returns>
		Manager GetManagerByLogin(string login);
	}
}