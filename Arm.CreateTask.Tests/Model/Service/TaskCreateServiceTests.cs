using System;
using System.Collections.Generic;
using System.Linq;
using Arm.CreateTask.Models;
using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.ManagerADM.DataModel;
using Arm.CreateTask.Models.Service;
using Atlassian.Jira;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Service
{
	[TestFixture]
	public class TaskCreateServiceTests
	{
		private TaskCreateService _taskCreateService;
		private IBaseConfigurationRepository<Workflow> _workflowRepository;
		private IBaseConfigurationRepository<Task> _taskRepository;
		private IBaseConfigurationRepository<Role2Workflow> _role2WorkflowRepository;
		private IBaseConfigurationRepository<Product> _productRepository;
		private IBaseConfigurationRepository<Perimeter> _perimeterRepository;
		private IBaseConfigurationRepository<Exploration> _explorationRepository;
		private IJiraApiService _jiraApiService;
		private ILdapService _ldapService;
		private const string ROLE = "Arm.CreateTask.Workflow1";
		private const int WORKFLOW_ID = 1;
		private const int PRODUCT_ID = 2;
		private const int EXPLORATION_ID = 3;
		private const int PERIMETER_ID = 6;
		private const string EXPLORATION_NAME = "ExploreName";
		private const string PERIMETER_VALUE = "DIR.I5.4";
		private const string PRODUCT_CODE = "product_code";
		private static readonly string _taskCode = TaskCode.Explore.ToString();

		[SetUp]
		public void SetUp()
		{

			_jiraApiService = Substitute.For<IJiraApiService>();
			_ldapService = Substitute.For<ILdapService>();
			_workflowRepository = Substitute.For<IBaseConfigurationRepository<Workflow>>();
			_taskRepository = Substitute.For<IBaseConfigurationRepository<Task>>();
			_role2WorkflowRepository = Substitute.For<IBaseConfigurationRepository<Role2Workflow>>();
			_productRepository = Substitute.For<IBaseConfigurationRepository<Product>>();
			_perimeterRepository = Substitute.For<IBaseConfigurationRepository<Perimeter>>();
			_explorationRepository = Substitute.For<IBaseConfigurationRepository<Exploration>>();

			_workflowRepository.GetAll().Returns(new List<Workflow>
			{
				new Workflow
				{
					Id = WORKFLOW_ID,
					ProductId = PRODUCT_ID,
					TaskCode = TaskCode.Explore.ToString(),
					PerimeterId = PERIMETER_ID,
					ExplorationId = EXPLORATION_ID,
					CommitRequired = true,
					CommentTemplate = "template",
					BusinessProcess = "619"
				}
			});

			_role2WorkflowRepository.GetAll().Returns(new List<Role2Workflow>
			{
				new Role2Workflow
				{
					WorkflowId = WORKFLOW_ID,
					Role = ROLE
				}
			});

			_productRepository.GetAll().Returns(new List<Product>
			{
				new Product
				{
					Id = PRODUCT_ID,
					Code = PRODUCT_CODE
				}
			});

			_explorationRepository.GetAll().Returns(new List<Exploration>
			{
				new Exploration
				{
					Id = EXPLORATION_ID,
					Name = EXPLORATION_NAME
				}
			});

			_taskRepository.GetAll().Returns(new List<Task>
			{
				new Task
				{
					Code = TaskCode.Explore.ToString(),
					Name = EXPLORATION_NAME
				}
			});

			_perimeterRepository.GetAll().Returns(new List<Perimeter>
			{
				new Perimeter
				{
					Key = PERIMETER_ID,
					Value = PERIMETER_VALUE
				}
			});

			_taskCreateService = new TaskCreateService
			(
				_jiraApiService,
				_ldapService, 
				_taskRepository, 
				_workflowRepository, 
				_role2WorkflowRepository, 
				_productRepository,
				_explorationRepository, 
				_perimeterRepository
			);
		}

		[Test]
		public void GetTasks_UserInfoWithRole_NotEmptyTasks()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string>{ ROLE }
			};

			var result = _taskCreateService.GetTasks(user_info);
			result.Should().NotBeNullOrEmpty();
		}

		[Test]
		public void GetTasks_UserInfoWithoutRole_EmptyTasks()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string>()
			};

			var result = _taskCreateService.GetTasks(user_info);
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void GetProducts_UserInfoWithRole_NotEmptyProducts()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string> { ROLE }
			};

			var result = _taskCreateService.GetProducts(user_info, _taskCode);
			result.Should().NotBeNullOrEmpty();
		}

		[Test]
		public void GetProducts_UserInfoWithoutRole_EmptyProducts()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string>()
			};

			var result = _taskCreateService.GetProducts(user_info, _taskCode);
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void GetExplorations_UserInfoWithRole_NotEmptyExplorations()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string> { ROLE }
			};

			var result = _taskCreateService.GetExplorations(user_info, PRODUCT_CODE, _taskCode);
			result.Should().NotBeNullOrEmpty();
		}

		[Test]
		public void GetExplorations_UserInfoWithoutRole_EmptyExplorations()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string> {}
			};

			var result = _taskCreateService.GetExplorations(user_info, PRODUCT_CODE, _taskCode);
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void GetExplorations_WrongTaskCode_EmptyExplorations()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string> { ROLE }
			};

			var result = _taskCreateService.GetExplorations(user_info, PRODUCT_CODE, TaskCode.Develop.ToString());
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void GetExplorations_WrongProductCode_EmptyExplorations()
		{
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string> { ROLE }
			};

			var result = _taskCreateService.GetExplorations(user_info, PRODUCT_CODE + "lorem ipsum", _taskCode);
			result.Should().BeNullOrEmpty();
		}

		[Test]
		public void CreateTask()
		{
			const string login = "login";
			var user_info = new UserInfo()
			{
				Id = 1,
				Roles = new List<string>{ROLE},
				Login = login
			};

			var form = new CreateForm
			{
				Task = TaskCode.Explore,
				ExplorationId = EXPLORATION_ID,
				Notes = "notes",
				InformationSystemCode = PRODUCT_CODE
			};

			_ldapService.GetWatchersByLogin(login).Returns(new List<PersonInfo>());

			try
			{
				_taskCreateService.CreateTask(form, user_info);

			}
			catch
			{
				// ignored
			}

			_jiraApiService.Received().CreateIssue(PERIMETER_VALUE, Arg.Any<string>(), form.Notes, Arg.Any<string>(), login,
				Arg.Any<IEnumerable<ProjectVersion>>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
		}
	}
}