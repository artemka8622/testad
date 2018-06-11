namespace Arm.CreateTask.Models.Auth.Claims
{
	/// <summary>
	/// Типы Claims, использующиеся в АРМ Создать задачу.
	/// </summary>
	public class CreateTaskClaim
	{
		/// <summary>
		/// Claim для хранения идентификатора пользователя.
		/// </summary>
		public const string USER_ID = "https://arm.itmh.ru/create_task/claims/user_id";

		/// <summary>
		/// Claim для хранения функциональных ролей приложения.
		/// </summary>
		public const string FUNC_ROLE = "https://arm.itmh.ru/create_task/claims/func_role";

		/// <summary>
		/// Claim для хранения идентификатора города.
		/// </summary>
		public const string CITY_ID = "https://arm.itmh.ru/create_task/claims/city_id";

		/// <summary>
		/// Claim для хранения идентификатора территории.
		/// </summary>
		public const string BRANCH_ID = "https://arm.itmh.ru/create_task/claims/branch_id";

		/// <summary>
		/// Claim для хранения идентификатора сотрудника.
		/// </summary>
		public const string EMPLOYEE_ID = "https://arm.itmh.ru/create_task/claims/employee_id";

		/// <summary>
		/// Claim для хранения названия группы пользователя.
		/// </summary>
		public const string GROUP_NAME = "https://arm.itmh.ru/create_task/claims/group_name";

		/// <summary>
		/// Claim для хранения краткого названия группы пользователя.
		/// </summary>
		public const string GROUP_SHORT_NAME = "https://arm.itmh.ru/create_task/claims/group_short_name";

		/// <summary>
		/// Claim для хранения идентификатора должности сотрудника.
		/// </summary>
		public const string JOB_ID = "https://arm.itmh.ru/create_task/claims/job_id";

		/// <summary>
		/// Claim для хранения названия типа деятельности сотрудника.
		/// </summary>
		public const string JOB_NAME = "https://arm.itmh.ru/create_task/claims/job_name";
	}
}