namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Товар.
	/// </summary>
	public class Product
	{
		/// <summary>
		/// Идентификатор.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Код.
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// Наименование.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Владелец.
		/// </summary>
		public string Owner { get; set; }

		/// <summary>
		/// Репозиторий.
		/// </summary>
		public string Repository { get; set; }
	}
}