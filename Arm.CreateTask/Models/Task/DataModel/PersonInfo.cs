namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Сотрудник в Ldap.
	/// </summary>
	public class PersonInfo
	{
		/// <summary>
		/// Логин.
		/// </summary>
		public string Login { get; set; }

		/// <summary>
		/// ФИО.
		/// </summary>
		public string FullName { get; set; }
	}
}