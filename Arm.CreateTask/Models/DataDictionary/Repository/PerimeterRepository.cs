using Arm.CreateTask.Models.DataModel;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.Repository
{
	/// <summary>
	/// Репозиторий для работы с периметром <see cref="Perimeter"/>.
	/// </summary>
	public class PerimeterRepository : BaseConfigurationRepository<Perimeter>
	{
		/// <summary>
		/// Наименование секции в файле конфигурации.
		/// </summary>
		protected override string SectionName => "perimeter_list";

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public PerimeterRepository(IConfiguration configuration) 
			: base(configuration)
		{
		}
	}
}