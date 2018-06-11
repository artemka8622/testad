namespace Arm.CreateTask.Models.ManagerADM.DataModel
{
	/// <summary>
	/// Менеджер.
	/// </summary>
	public class Manager
	{
		/// <summary>
		/// Идентификатор менеджера.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Полное имя.
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Краткое имя.
		/// </summary>
		public string ShortName { get; set; }
		
		/// <summary>
		/// Адрес электронной почты.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Массив Sid пользователя.
		/// </summary>
		public byte[] Sid { get; set; }

		/// <summary>
		/// Идентификатор города.
		/// </summary>
		public int? CityId { get; set; }

		/// <summary>
		/// Идентификатор территории.
		/// </summary>
		public int? BranchId { get; set; }

		/// <summary>
		/// Идентификатор сотрудника.
		/// </summary>
		public int EmployeeId { get; set; }

		/// <summary>
		/// Наименование группы пользователя.
		/// </summary>
		public string GroupName { get; set; }

		/// <summary>
		/// Краткое наименование группы пользователя.
		/// </summary>
		public string GroupShortName { get; set; }

		/// <summary>
		/// Идентификатор должности.
		/// </summary>
		public int JobId { get; set; }

		/// <summary>
		/// Наименование должности.
		/// </summary>
		public string JobName { get; set; }
	}
}