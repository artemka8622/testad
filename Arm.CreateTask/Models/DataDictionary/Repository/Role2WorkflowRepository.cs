using Arm.CreateTask.Models.DataModel;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.Repository
{
	/// <summary>
	/// Репозиторий для работы со связями роли и процесса <see cref="Role2Workflow"/>.
	/// </summary>
	public class Role2WorkflowRepository : BaseConfigurationRepository<Role2Workflow>
	{
		/// <summary>
		/// Наименование секции в файле конфигурации.
		/// </summary>
		protected override string SectionName => "role2workflow";

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public Role2WorkflowRepository(IConfiguration configuration) 
			: base(configuration)
		{
		}
	}
}