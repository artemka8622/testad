using Arm.CreateTask.Models.DataModel;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.Repository
{
	/// <summary>
	/// Репозиторий для работы с исследованиями <see cref="Exploration"/>.
	/// </summary>
	public class ExplorationRepository : BaseConfigurationRepository<Exploration>
	{
		/// <summary>
		/// Наименование секции в файле конфигурации.
		/// </summary>
		protected override string SectionName => "exploration_list";

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public ExplorationRepository(IConfiguration configuration) 
			: base(configuration)
		{
		}
	}
}