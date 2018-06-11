using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models
{
	/// <summary>
	/// Базовый класс - репозиторий для работы с файлом конфигурации.
	/// </summary>
	/// <typeparam name="T">Тип.</typeparam>
	public class BaseConfigurationRepository <T> : IBaseConfigurationRepository<T>
	{
		/// <summary>
		/// Наименование секции.
		/// </summary>
		protected virtual string SectionName => null;

		/// <summary>
		/// Интерфейс для работы с конфигурацией.
		/// </summary>
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public BaseConfigurationRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Получает все экземпляры передаваемого типа из списка.
		/// </summary>
		/// <returns>Список.</returns>
		public IEnumerable<T> GetAll()
		{
			var task_list = _configuration.GetSection(SectionName).Get<IEnumerable<T>>();
			return task_list;
		}
	}
}