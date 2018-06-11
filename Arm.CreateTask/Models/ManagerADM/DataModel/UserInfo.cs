using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arm.CreateTask.Models.ManagerADM.DataModel
{
	/// <summary>
	/// Информация о пользователе.
	/// </summary>
	public class UserInfo
	{
		/// <summary>
		/// Идентификатор пользователя.
		/// </summary>
		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор менеджера.
		/// </summary>
		[JsonProperty(PropertyName = "manager_id")]
		public int ManagerId => Id;

		/// <summary>
		/// Логин.
		/// </summary>
		[JsonProperty(PropertyName = "login")]
		public string Login { get; set; }

		/// <summary>
		/// Имя пользователя.
		/// </summary>
		[JsonProperty(PropertyName = "username")]
		public string UserName => Login;

		/// <summary>
		/// Роли, в которые включен пользователь.
		/// </summary>
		[JsonProperty(PropertyName = "roles")]
		public IEnumerable<string> Roles { get; set; }

		/// <summary>
		/// Полное имя.
		/// </summary>
		[JsonProperty(PropertyName = "full_name")]
		public string FullName { get; set; }

		/// <summary>
		/// Краткое имя.
		/// </summary>
		[JsonProperty(PropertyName = "short_name")]
		public string ShortName { get; set; }

		/// <summary>
		/// Идентификатор сотрудника.
		/// </summary>
		[JsonProperty(PropertyName = "employee_id")]
		public int EmployeeId { get; set; }

		/// <summary>
		/// Идентификатор должности.
		/// </summary>
		[JsonProperty(PropertyName = "job_id")]
		public int JobId { get; set; }

		/// <summary>
		/// Наименование должности.
		/// </summary>
		[JsonProperty(PropertyName = "job_name")]
		public string JobName { get; set; }

		/// <summary>
		/// Полное наименование группы пользователя.
		/// </summary>
		[JsonProperty(PropertyName = "group_full_name")]
		public string GroupFullName { get; set; }

		/// <summary>
		/// Краткое наименование группы пользователя.
		/// </summary>
		[JsonProperty(PropertyName = "group_short_name")]
		public string GroupShortName { get; set; }

		/// <summary>
		/// Адрес электронной почты.
		/// </summary>
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }

		/// <summary>
		/// Sid пользователя.
		/// </summary>
		[JsonProperty(PropertyName = "sid")]
		public string Sid { get; set; }

		/// <summary>
		/// Идентификатор города.
		/// </summary>
		[JsonProperty(PropertyName = "city_id")]
		public int? CityId { get; set; }

		/// <summary>
		/// Идентификатор территории.
		/// </summary>
		[JsonProperty(PropertyName = "branch_id")]
		public int? BranchId { get; set; }
	}
}