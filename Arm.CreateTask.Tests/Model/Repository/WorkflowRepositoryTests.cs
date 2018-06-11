using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Repository;
using FluentAssertions;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Repository
{
	[TestFixture]
	public class WorkflowRepositoryTests : BaseTests
	{
		private WorkflowRepository _workflowRepository;

		[SetUp]
		public void SetUp()
		{
			_workflowRepository = new WorkflowRepository(Configuration);
		}

		[Test]
		public void GetAll_Empty_NotNullAndCorrectType()
		{
			var result = _workflowRepository.GetAll();
			result.Should().NotBeNullOrEmpty().And.AllBeOfType<Workflow>();
		}
	}
}