namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Связь роли и процесса.
	/// </summary>
	public class Role2Workflow
	{
		/// <summary>
		/// Идентификатор workflow.
		/// </summary>
		public int WorkflowId { get; set; }

		/// <summary>
		/// Роль.
		/// </summary>
		public string Role { get; set; }
	}
}