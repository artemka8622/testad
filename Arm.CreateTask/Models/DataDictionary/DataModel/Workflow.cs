namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Процесс.
	/// </summary>
	public class Workflow
	{
		/// <summary>
		/// Идентификатор.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Идентификатор продукта.
		/// </summary>
		public int ProductId { get; set; }

		/// <summary>
		/// Код задачи.
		/// </summary>
		public string TaskCode { get; set; }

		/// <summary>
		/// Идентификатор периметра.
		/// </summary>
		public int PerimeterId { get; set; }

		/// <summary>
		/// Идентификатор исследования.
		/// </summary>
		public int ExplorationId { get; set; }

		/// <summary>
		/// Признак необходимости коммита.
		/// </summary>
		public bool CommitRequired { get; set; }

		/// <summary>
		/// Шаблон комментария.
		/// </summary>
		public string CommentTemplate { get; set; }

		/// <summary>
		/// Бизнес-процесс.
		/// </summary>
		public string BusinessProcess { get; set; }
	}
}