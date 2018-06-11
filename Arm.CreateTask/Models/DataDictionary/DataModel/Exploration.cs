namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Исследование.
	/// </summary>
	public class Exploration
	{
		/// <summary>
		/// Идентификатор.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Наименование.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Признак только начальник.
		/// </summary>
		public bool? BossOnly { get; set; }
	}
}