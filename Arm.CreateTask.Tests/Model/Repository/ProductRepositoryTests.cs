using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Repository;
using FluentAssertions;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Repository
{
	[TestFixture]
	public class ProductRepositoryTests : BaseTests
	{
		private ProductRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_repository = new ProductRepository(Configuration);
		}

		[Test]
		public void GetAll_Empty_NotNullAndCorrectType()
		{
			var result = _repository.GetAll();
			result.Should().NotBeNullOrEmpty().And.AllBeOfType<Product>();
		}
	}
}