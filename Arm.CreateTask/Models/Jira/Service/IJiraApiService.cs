using System.Collections.Generic;
using Arm.CreateTask.Models.DataModel;
using Atlassian.Jira;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис для работы с JIRA.
	/// </summary>
	public interface IJiraApiService
	{
		/// <summary>
		/// Добавляет наблюдателей к задаче.
		/// </summary>
		/// <param name="watchers">Список наблюдателей.</param>
		/// <param name="issue">Задача Jira.</param>
		void AddWatchers(IEnumerable<string> watchers, Issue issue);

		/// <summary>
		/// Создает задачу в JIRA.
		/// </summary>
		/// <param name="project">Проект.</param>
		/// <param name="subject">Тема.</param>
		/// <param name="content">Контент.</param>
		/// <param name="issue_type">Тип.</param>
		/// <param name="author">Автор.</param>
		/// <param name="fix_version">Версия.</param>
		/// <param name="wf_producer">Автор специальное поле.</param>
		/// <param name="wf_executor">Исполнитель специальное поле.</param>
		/// <param name="label">Тэг.</param>
		/// <returns>Задача JIRA.</returns>
		Issue CreateIssue(string project, string subject, string content, string issue_type, string author, IEnumerable<ProjectVersion> fix_version, string wf_producer, string wf_executor, string label);

		/// <summary>
		/// Получает версии проекта по коду.
		/// </summary>
		/// <param name="project_code">Код проекта.</param>
		/// <returns>Версия проекта.</returns>
		IEnumerable<ProjectVersion> GetVersions(string project_code);

		/// <summary>
		/// Добавляет файлы в задаче.
		/// </summary>
		/// <param name="form">Форма задачи.</param>
		/// <param name="issue">Задача jira.</param>
		void AddFiles(CreateForm form, Issue issue);

		/// <summary>
		/// Добавляет комментарии к задаче.
		/// </summary>
		/// <param name="form">Форма задачи.</param>
		/// <param name="watchers">Наблюдатели.</param>
		/// <param name="issue">Задача jira.</param>
		/// <param name="author">Автор задачи.</param>
		/// <param name="task_subject">Тема задачи.</param>
		/// <param name="workflow">Правило создания задачи.</param>
		void AddComment(CreateForm form, List<PersonInfo> watchers, Issue issue, string author, string task_subject, Workflow workflow);
	}
}