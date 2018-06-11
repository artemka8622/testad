using Arm.CreateTask.Models.DataModel;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Models.Repository
{
	/// <summary>
	/// Репозиторий для работы с товаром <see cref="Product"/>.
	/// </summary>
	public class ProductRepository : BaseConfigurationRepository<Product>
	{
		/// <summary>
		/// Наименование секции в файле конфигурации.
		/// </summary>
		protected override string SectionName => "product_list";

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="configuration">Конфигурация.</param>
		public ProductRepository(IConfiguration configuration) 
			: base(configuration)
		{
		}
	}
}