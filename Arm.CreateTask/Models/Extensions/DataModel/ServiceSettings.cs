namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Настройки сервиса.
	/// </summary>
	public class ServiceSettings
	{
		/// <summary>
		/// Логин.
		/// </summary>
		public string Login { get; set; }

		/// <summary>
		/// Пароль.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Ссылка.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Порт.
		/// </summary>
		public int? Port { get; set; }
	}
}