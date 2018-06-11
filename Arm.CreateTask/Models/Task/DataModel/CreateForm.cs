using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arm.CreateTask.Models.DataModel
{
	/// <summary>
	/// Представляет собой форму создания задачи.
	/// </summary>
	public class CreateForm
	{
		/// <summary>
		/// Идентификатор типа задачи.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public TaskCode Task { get; set; }

		/// <summary>
		/// Идентификатор продукта ПО.
		/// </summary>
		public string InformationSystemCode { get; set; }

		/// <summary>
		/// Тема.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Ссылка на SRS.
		/// </summary>
		public string SRSLink { get; set; }


		/// <summary>
		/// Ссылка на FRS.
		/// </summary>
		public string FRSLink { get; set; }

		/// <summary>
		/// Идентификатор типа обращения.
		/// </summary>
		public int? ExplorationId { get; set; }

		/// <summary>
		/// Описание.
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Ссылка на задачу по разработке ПО.
		/// </summary>
		public string ParentTaskLink { get; set; }

		/// <summary>
		/// Приоритет.
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// Приложенные к задаче файлы.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<IFormFile> Files { get; set; }

		/// <summary>
		/// Использовать меншон.
		/// </summary>
		public bool UseMention { get; set; }
	}
}