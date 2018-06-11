using System.Collections.Generic;
using System.Linq;
using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Service;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Service
{
	[TestFixture()]
	public class TaskFormValidationServiceTests
	{
		public const string CORRECT_URL = "http://www.example.com/index.html";

		public const string INCORRECT_URL = "fd,swlp;fmdw;knfk qajd qd qwdpqwoodqw&&&!&2903921093021FACEROLL";

		private TaskFormValidationService _taskFormValidationService;

		[SetUp]
		public void SetUp()
		{
			_taskFormValidationService = new TaskFormValidationService();
		}

		[Test]
		[TestCase(TaskCode.Develop)]
		[TestCase(TaskCode.Estimate)]
		public void Validate_TaskCodeWithSrs_EmptySrsLinkMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.EmptySrsLink);
		}

		[Test]
		[TestCase(TaskCode.Srs)]
		public void Validate_TaskCodeWithFrs_EmptyFrsLinkMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.EmptyFrsLink);
		}

		[Test]
		[TestCase(TaskCode.Integration)]
		public void Validate_TaskCodeWithParentTask_EmptyParentTaskLinkMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.EmptyParentTask);
		}

		[Test]
		[TestCase(TaskCode.Explore)]
		public void Validate_Explore_EmptyNoteMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.EmptyNote);
		}

		[Test]
		[TestCase(TaskCode.Explore)]
		public void Validate_Explore_EmptyExplorationIdMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.EmptyExplorationId);
		}

		[Test]
		[TestCase(TaskCode.Srs)]
		public void Validate_TaskCodeWithFrs_IncorrectFrsLinkMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code,
				FRSLink = INCORRECT_URL
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.IncorrectFrsLink);
		}

		[Test]
		[TestCase(TaskCode.Develop)]
		[TestCase(TaskCode.Estimate)]
		public void Validate_TaskCodeWithSrs_IncorrectSrsLinkMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code,
				SRSLink = INCORRECT_URL
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.IncorrectSrsLink);
		}

		[Test]
		[TestCase(TaskCode.Integration)]
		public void Validate_TaskCodeWithParentTaskLink_IncorrectParentTaskLinkMessage(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code,
				ParentTaskLink = INCORRECT_URL
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.IncorrectParentTaskLink);
		}

		[Test]
		[TestCase(TaskCode.Develop)]
		[TestCase(TaskCode.Explore)]
		[TestCase(TaskCode.Srs)]
		[TestCase(TaskCode.Estimate)]
		[TestCase(TaskCode.Integration)]
		[TestCase(TaskCode.Telsupport)]
		public void Validate_CorrectForm_EmptyErrors(TaskCode task_code)
		{
			var form = new CreateForm
			{
				Task = task_code,
				InformationSystemCode = "code",
				ExplorationId = 1,
				Notes = "notes",
				FRSLink = CORRECT_URL,
				SRSLink = CORRECT_URL,
				ParentTaskLink = CORRECT_URL,
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void Validate_FilesCount_MaxFilesMessage()
		{
			var files = new List<IFormFile>();
			for (var i = 0; i < TaskFormValidationService.MAX_FILES + 1; i++)
			{
				files.Add(new FormFile(null, 0, 10 * 1024, "SmallFile", "SmallFile"));
			}

			var form = new CreateForm
			{
				Task = TaskCode.Estimate,
				Files = files
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.MaxFilesMessage);
		}

		[Test]
		public void Validate_LargeFile_MaxSizeMessage()
		{
			var form = new CreateForm
			{
				Task = TaskCode.Estimate,
				Files = new List<IFormFile>() { new FormFile(null, 0, TaskFormValidationService.MAX_SIZE + 1, "BigFile" , "BigFile") }
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.MaxSizeMessage);
		}

		[Test]
		public void Validate_LargeFiles_MaxSizeMessage()
		{
			var form = new CreateForm
			{
				Task = TaskCode.Estimate,
				Files = new List<IFormFile>()
				{
					new FormFile(null, 0, TaskFormValidationService.MAX_SIZE - 1, "NotSoBigFile", "NotSoBigFile"),
					new FormFile(null, 0, TaskFormValidationService.MAX_SIZE - 1, "NotSoBigFile", "NotSoBigFile"),
				}
			};

			var result = _taskFormValidationService.Validate(form).ToList();
			result.Should().Contain(x => x.ErrorMessage == TaskFormValidationService.MaxSizeMessage);
		}
	}
}