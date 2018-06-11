namespace Arm.CreateTask.Models
{
	/// <summary>
	/// Доступные способы аутентификации.
	/// </summary>
	public enum AwailableSchemes
	{
		/// <summary>
		/// Обычная проверка подлинности HTTP.
		/// </summary>
		Basic,

		/// <summary>
		/// Федеративная проверка.
		/// </summary>
		WsFederation
	}
}