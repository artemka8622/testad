using Arm.CreateTask.Models.DataModel;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.Repository
{
	/// <summary>
	/// Репозиторий для работы с задачами <see cref="Task"/>.
	/// </summary>
	public class TaskRepository : BaseConfigurationRepository<Task>
	{
		/// <summary>
		/// Наименование секции в файле конфигурации.
		/// </summary>
		protected override string SectionName => "task_list";

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public TaskRepository(IConfiguration configuration) 
			: base(configuration)
		{
		}
	}
}