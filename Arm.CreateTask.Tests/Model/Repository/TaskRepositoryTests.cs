using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Repository;
using FluentAssertions;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Repository
{
	[TestFixture]
	public class TaskRepositoryTests : BaseTests
	{
		private TaskRepository _taskRepository;

		[SetUp]
		public void SetUp()
		{
			_taskRepository = new TaskRepository(Configuration);
		}

		[Test]
		public void GetAll_Empty_NotNullAndCorrectType()
		{
			var result = _taskRepository.GetAll();
			result.Should().NotBeNullOrEmpty().And.AllBeOfType<Task>();
		}
	}
}