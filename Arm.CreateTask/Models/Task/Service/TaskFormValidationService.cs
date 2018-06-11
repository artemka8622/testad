using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Arm.CreateTask.Models.DataModel;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис проверки форм для создания задач.
	/// </summary>
	public class TaskFormValidationService
	{
		#region Constants

		/// <summary>
		/// Максимальное количество файлов.
		/// </summary>
		public const int MAX_FILES = 5;

		/// <summary>
		/// Максимальный суммарный размер приложенных файлов в мегабайтах.
		/// </summary>
		public const int MBS = 10;

		/// <summary>
		/// Максимальный суммарный размер приложенных файлов в байтах.
		/// </summary>
		public const int MAX_SIZE = MBS * 1024 * 1024;

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string EmptySubject = $"Поле \"Тема\" должно быть заполнено.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string EmptySrsLink = $"Поле \"Ссылка на SRS\" должно быть заполнено.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string EmptyNote = $"Поле \"Описание\" должно быть заполнено.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string EmptyExplorationId = $"Поле \"Тип обращение\" должно быть заполнено.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string EmptyFrsLink = $"Поле \"Ссылка на FRS\" должно быть заполнено.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string EmptyParentTask = $"Поле \"Ссылка на родительскую задачу\" должно быть заполнено.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string IncorrectSrsLink = $"Поле \"Ссылка на SRS\" должно быть URL.";
		
		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string IncorrectFrsLink = $"Поле \"Ссылка на FRS\" должно быть URL.";
		
		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string IncorrectParentTaskLink = $"Поле \"Ссылка на родительскую задачу\" должно быть URL.";

		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string MaxFilesMessage = $"Количество файлов не должно превышать {MAX_FILES} файлов.";
		/// <summary>
		/// Сообщение об ошибке.
		/// </summary>
		public static readonly string MaxSizeMessage = $"Максимальный размер файлов {MBS} Mb.";
		
		#endregion

		/// <summary>
		/// Получает список ошибок заполнения формы создания задачи.
		/// </summary>
		/// <param name="form">Форма для создания задачи КСУ.</param>
		/// <returns>Список ошибок.</returns>
		public IEnumerable<ValidationResult> Validate(CreateForm form)
		{
			if (string.IsNullOrWhiteSpace(form.Subject))
				yield return new ValidationResult(EmptySubject, new []{ nameof(form.Subject)});

			switch (form.Task)
			{
				case TaskCode.Explore:
					if (string.IsNullOrWhiteSpace(form.Notes))
						yield return new ValidationResult(EmptyNote, new[] { nameof(form.Notes) });

					if (!form.ExplorationId.HasValue)
						yield return new ValidationResult(EmptyExplorationId, new[] { nameof(form.ExplorationId) });
					break;
				case TaskCode.Develop:
				case TaskCode.Estimate:
					if (string.IsNullOrWhiteSpace(form.SRSLink))
						yield return new ValidationResult(EmptySrsLink, new[] { nameof(form.SRSLink) });
					break;
				case TaskCode.Srs:
					if (string.IsNullOrWhiteSpace(form.FRSLink))
						yield return new ValidationResult(EmptyFrsLink, new[] { nameof(form.FRSLink) });
					break;
				case TaskCode.Integration:
					if (string.IsNullOrWhiteSpace(form.ParentTaskLink))
						yield return new ValidationResult(EmptyParentTask, new[] { nameof(form.ParentTaskLink) });
					break;
			}

			if (!string.IsNullOrWhiteSpace(form.SRSLink) && !Uri.IsWellFormedUriString(form.SRSLink, UriKind.RelativeOrAbsolute))
				yield return new ValidationResult(IncorrectSrsLink, new[] { nameof(form.SRSLink) });
			if (!string.IsNullOrWhiteSpace(form.FRSLink) && !Uri.IsWellFormedUriString(form.FRSLink, UriKind.RelativeOrAbsolute))
				yield return new ValidationResult(IncorrectFrsLink, new[] { nameof(form.FRSLink) });
			if (!string.IsNullOrWhiteSpace(form.ParentTaskLink) && !Uri.IsWellFormedUriString(form.ParentTaskLink, UriKind.RelativeOrAbsolute))
				yield return new ValidationResult(IncorrectParentTaskLink, new[] { nameof(form.ParentTaskLink) });

			if (form.Files != null)
			{
				if (form.Files.Count() > MAX_FILES)
					yield return new ValidationResult(MaxFilesMessage, new[] { nameof(form.Files) });
				
				if (form.Files.Sum(x => x.Length) > MAX_SIZE)
					yield return new ValidationResult(MaxSizeMessage, new[] { nameof(form.Files) });
			}
		}
	}
}