namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Код типа задачи.
	/// </summary>
	public enum TaskCode
	{
		/// <summary>
		/// Эксплуатация ПО.
		/// </summary>
		Explore = 1,

		/// <summary>
		/// Разработать ПО.
		/// </summary>
		Develop = 2,

		/// <summary>
		/// Разработать SRS.
		/// </summary>
		Srs = 3,

		/// <summary>
		/// Оценить SRS.
		/// </summary>
		Estimate = 4,

		/// <summary>
		/// Внедрить ПО.
		/// </summary>
		Integration = 5,

		/// <summary>
		/// Эксплуатация сервисных платформ.
		/// </summary>
		Telsupport = 6
	}
}