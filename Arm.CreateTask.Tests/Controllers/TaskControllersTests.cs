using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Arm.CreateTask.Controllers;
using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.Repository;
using Arm.CreateTask.Models.Service;
using Arm.CreateTask.Tests.Model.Service;
using Atlassian.Jira;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Controllers
{
	/// <summary>
	/// Интеграционные тесты контроллера.
	/// </summary>
	[TestFixture]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class TaskControllersTests : BaseTests
	{
		/// <summary>
		/// Контроллер задач.
		/// </summary>
		private TaskController _taskController;

		/// <summary>
		/// Класс для работы с сервером JIRA.
		/// </summary>
		private Jira _client;

		/// <summary>Тестовые данные формы.</summary>
		private CreateForm _testForm;

		/// <summary>Сервис пользователей.</summary>
		private IUserInfoService _userInfoService;

		/// <summary>
		/// Установки для контроллера.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			var task_service = (TaskCreateService)WebHost.Services.GetService(typeof(TaskCreateService));
			var validation_service = (TaskFormValidationService)WebHost.Services.GetService(typeof(TaskFormValidationService));
			var settings_service = (ServiceSettingsService)WebHost.Services.GetService(typeof(ServiceSettingsService));
			var logger = (ILogger<TaskController>)WebHost.Services.GetService(typeof(ILogger<TaskController>));
			_userInfoService = Substitute.For<IUserInfoService>();
			_userInfoService.GetUserInfo().Returns(
				t => new UserInfo()
						{
							FullName = "Тестовый Пользователь",
							Login = "rodikov.artyem",
							Roles = new []{ "Arm.CreateTask.Workflow1", "Arm.CreateTask.Workflow4" }
						});
			_testForm = GetTestForm();
			_taskController = new TaskController(task_service, validation_service, settings_service, logger, _userInfoService);
			var settings = settings_service.GetSettingsByCode("jira_api");
			_client = Jira.CreateRestClient(settings.Url, settings.Login, settings.Password);
		}

		/// <summary>Проверяет создание задачи и меншон в комментариях.</summary>
		/// <param name="subject">Тема.</param>
		[Test]
		[TestCase("q")]
		[TestCase("Тестовая задача")]
		[TestCase("Test task")]
		[TestCase("!\"№;%:?*()_ +@#%^&*-={}[]/|\\")]
		public void CreateTask_Comment_Sussess(string subject)
		{
			_testForm.Subject = subject;

			var result = _taskController.CreateTask(_testForm);
			
			var key = GetTaskKey(result);
			var comment = _client.Issues.GetIssueAsync(key).Result.GetCommentsAsync().Result.LastOrDefault();
			_client.Issues.DeleteIssueAsync(key).Wait();
			Assert.NotNull(comment);
			Assert.IsTrue(comment.Body.Contains("Смородников") && comment.Body.Contains("Семенищев"));
		}

		/// <summary>Проверяет создание задачи и меншон в комментариях.</summary>
		[Test]
		public void CreateTask_Comment_Sussess()
		{
			_userInfoService.GetUserInfo().Returns(
				t => new UserInfo()
						{
							FullName = "Джедай",
							Login = "Jedi",
							Roles = new[] { "Arm.CreateTask.Workflow1", "Arm.CreateTask.Workflow4" }
						});

			var result = _taskController.CreateTask(_testForm);

			var key = GetTaskKey(result);
			var comment = _client.Issues.GetIssueAsync(key).Result.GetCommentsAsync().Result.LastOrDefault();
			_client.Issues.DeleteIssueAsync(key).Wait();
			Assert.Null(comment);
		}

		/// <summary>Проверяет создание задачи и меншон в комментариях.</summary>
		[Test]
		public void CreateTask_NoteTable_Sussess()
		{
			_testForm.Task = TaskCode.Explore;
			_testForm.Notes = GetTableNotes();
			_testForm.ExplorationId = 4;

			var result = _taskController.CreateTask(_testForm);

			var key = GetTaskKey(result);
			var description = _client.Issues.GetIssueAsync(key).Result.Description;
			_client.Issues.DeleteIssueAsync(key).Wait();
			Assert.NotNull(description);
			Assert.IsTrue(description.Contains("<table"));
		}

		/// <summary>Получает тестовый набор файлов.</summary>
		/// <returns>Тестовый набор файлов.</returns>
		private IEnumerable<IFormFile> GetTestFiles()
		{
			MemoryStream fs = new MemoryStream();
			TextWriter tx = new StreamWriter(fs);

			tx.WriteLine("1111");
			tx.WriteLine("2222");
			tx.WriteLine("3333");

			tx.Flush();
			fs.Flush();

			byte[] bytes = new byte[fs.Length];
			fs.Read(bytes, 0, (int)fs.Length);

			return new IFormFile[]
						{
							new FormFile(fs, 0, 500, "test","test_file.txt"),
							new FormFile(fs, 0, 500, "test","test_file2.txt"),
						};
		}

		/// <summary>
		/// Проверяет создание задачи и меншон в комментариях.
		/// </summary>
		[Test]
		public void CreateTask_Files_Sussess()
		{
			_testForm.Files = GetTestFiles();

			var result = _taskController.CreateTask(_testForm);

			var key = GetTaskKey(result);
			var files = _client.Issues.GetAttachmentsAsync(key).Result;

			Assert.NotNull(files);
			Assert.AreEqual(files.Count(), _testForm.Files.Count());
			_client.Issues.DeleteIssueAsync(key).Wait();
		}

		/// <summary>
		/// Получает созданный ключ задачи.
		/// </summary>
		/// <param name="result">Результат создания задачи.</param>
		/// <returns>Ключ задачи.</returns>
		private string GetTaskKey(IActionResult result)
		{
			var url = ((RedirectResult)result).Url;
			var key = url.Substring(url.LastIndexOf("/") + 1);
			return key;
		}

		/// <summary>
		/// Получение тестовых данных.
		/// </summary>
		/// <returns>Тестовые данные.</returns>
		private CreateForm GetTestForm()
		{
			var form = new CreateForm()
							{
								Task = TaskCode.Develop,
								ExplorationId = 5,
								InformationSystemCode = "cordis",
								Subject = "Тестовая задач, тестирование ARM \"Создать задачу в КСУ\"",
								FRSLink = "http://Тестовый.ФРС.ru",
								SRSLink = "http://Тестовый.SRS.ru",
								Priority = 0,
								UseMention = true
			};
			return form;
		}

		/// <summary>Возвращает табличку в html.</summary>
		/// <returns>Табличка.</returns>
		private string GetTableNotes()
		{
			return @"<ol>
	<li>Еуые</li>
	<li>ыва</li>
</ol>

<p>ффыва</p>" +

@"<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""width: 500px; border-collapse: collapse;"">
	<tbody>
		<tr>
			<td>может</td>
			<td>
			<p>фыва</p>

			<p>фыва</p>
			</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<td>фыва</td>
			<td>фыва</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
			<td>&nbsp;</td>
		</tr>
	</tbody>
</table>
<p>&nbsp;</p>";
		}
	}
}
