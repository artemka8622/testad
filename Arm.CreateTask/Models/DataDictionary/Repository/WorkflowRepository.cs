using Arm.CreateTask.Models.DataModel;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.Repository
{
	/// <summary>
	/// Репозиторий для работы с процессами <see cref="Workflow"/>.
	/// </summary>
	public class WorkflowRepository : BaseConfigurationRepository<Workflow>
	{
		/// <summary>
		/// Наименование секции в файле конфигурации.
		/// </summary>
		protected override string SectionName => "workflow_list";

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public WorkflowRepository(IConfiguration configuration) 
			: base(configuration)
		{
		}
	}
}