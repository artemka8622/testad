using System.Collections.Generic;

namespace Arm.CreateTask.Models
{
	/// <summary>
	/// Базовый класс - репозиторий для работы с файлом конфигурации.
	/// </summary>
	/// <typeparam name="T">Тип.</typeparam>
	public interface IBaseConfigurationRepository<out T>
	{
		/// <summary>
		/// Получает все экземпляры передаваемого типа из списка.
		/// </summary>
		/// <returns>Список.</returns>
		IEnumerable<T> GetAll();
	}
}