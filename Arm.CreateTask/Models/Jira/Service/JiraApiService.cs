using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arm.CreateTask.Models.DataModel;
using Atlassian.Jira;
using MoreLinq;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис для работы с JIRA.
	/// </summary>
	public class JiraApiService : IJiraApiService
	{
		/// <summary>
		/// Автор задачи.
		/// </summary>
		private const string CUSTOM_FIELD__BUSINESS_PROCESS_PRODUCER = "customfield_11292";

		/// <summary>
		/// Исполнитель задачи.
		/// </summary>
		private const string CUSTOM_FIELD__BUSINESS_PROCESS_EXECUTOR = "customfield_12290";

		/// <summary>
		/// Класс для работы с сервером JIRA.
		/// </summary>
		private readonly Jira _client;

		/// <summary>
		/// Класс для работы с сервером JIRA.
		/// </summary>
		private readonly ServiceSettings _settings;

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="settings_service">Сервис, предоставляющий доступ к API Noc Cordis.</param>
		public JiraApiService(ServiceSettingsService settings_service)
		{
			_settings = settings_service.GetSettingsByCode("jira_api");
			_client = Jira.CreateRestClient(_settings.Url, _settings.Login, _settings.Password);
		}

		/// <summary>
		/// Получает версии проекта по коду.
		/// </summary>
		/// <param name="project_code">Код проекта.</param>
		/// <returns>Версия проекта.</returns>
		public IEnumerable<ProjectVersion> GetVersions(string project_code)
		{
			return _client.Versions.GetVersionsAsync(project_code).Result;
		}

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
		public Issue CreateIssue(string project, string subject, string content, string issue_type, string author, IEnumerable<ProjectVersion> fix_version, string wf_producer, string wf_executor, string label)
		{
			var fields = new CreateIssueFields(project);
			var result = _client.CreateIssue(fields);

			result.Summary = subject;
			result.Description = content;
			result.Type = new IssueType(issue_type);
			result.Reporter = author;
			foreach (var v in fix_version)
			{
				result.FixVersions.Add(v);
			}
			result.CustomFields.AddById(CUSTOM_FIELD__BUSINESS_PROCESS_EXECUTOR, wf_executor);
			result.CustomFields.AddById(CUSTOM_FIELD__BUSINESS_PROCESS_PRODUCER, wf_producer);
			result.Labels.Add(label);

			result.SaveChanges();

			return result;
		}

		/// <summary>
		/// Добавляет наблюдателей к задаче.
		/// </summary>
		/// <param name="watchers">Список наблюдателей.</param>
		/// <param name="issue">Задача Jira.</param>
		public void AddWatchers(IEnumerable<string> watchers, Issue issue)
		{
			foreach (var watcher in watchers)
			{
				issue.AddWatcherAsync(watcher);
			}
		}

		/// <summary>Добавляет файлы в задаче.</summary>
		/// <param name="form">Форма задачи.</param>
		/// <param name="issue">Задача jira.</param>
		public void AddFiles(CreateForm form, Issue issue)
		{
			form.Files?.ForEach(
				t =>
					{
						using (var ms = new MemoryStream())
						{
							t.CopyTo(ms);
							issue.AddAttachment(t.Name, ms.ToArray());
						}
					}
			);
		}

		/// <summary>Добавляет комментарии к задаче.</summary>
		/// <param name="form">Форма задачи.</param>
		/// <param name="watchers">Наблюдатели.</param>
		/// <param name="issue">Задача jira.</param>
		/// <param name="author">Автор задачи.</param>
		/// <param name="task_subject">Тема задачи.</param>
		/// <param name="workflow"></param>
		public void AddComment(CreateForm form, List<PersonInfo> watchers, Issue issue, string author, string task_subject, Workflow workflow)
		{
			if (watchers.Any())
			{
				var head = string.Join(", ", watchers.Select(t => GetMention(form.UseMention, t)));
				var comment = workflow.CommentTemplate.Replace("greeting", head);
				comment = comment.Replace("username", author);
				comment = comment.Replace("task_subject", task_subject);
				issue.AddCommentAsync(comment);
			}
		}

		/// <summary>
		/// Получает меншон для пользователя
		/// </summary>
		/// <param name="mention">Признак использовать ли меншон</param>
		/// <param name="t">Информация о пользователе</param>
		/// <returns></returns>
		private string GetMention(bool mention, PersonInfo t)
		{
			if (mention)
			{
				return $"<a class=\"user-hover\" data-context=\"{_settings.Url}\" href=\"{_settings.Url}/secure/ViewProfile.jspa?name={t.Login}\" rel=\"{t.Login}\">{t.FullName}</a>";
			}

			return t.FullName;
		}
	}
}