using System.Collections.Generic;
using Arm.CreateTask.Models.DataModel;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис Ldap. 
	/// </summary>
	public interface ILdapService
	{
		/// <summary>
		/// Получает наблюдателей по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Список наблюдателей.</returns>
		IEnumerable<PersonInfo> GetWatchersByLogin(string login);
	}
}