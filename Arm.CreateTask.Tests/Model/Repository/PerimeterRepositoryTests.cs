﻿using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Repository;
using FluentAssertions;
using NUnit.Framework;

namespace Arm.CreateTask.Tests.Model.Repository
{
	[TestFixture]
	public class PerimeterRepositoryTests : BaseTests
	{
		private PerimeterRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_repository = new PerimeterRepository(Configuration);
		}

		[Test]
		public void GetAll_Empty_NotNullAndCorrectType()
		{
			var result = _repository.GetAll();
			result.Should().NotBeNullOrEmpty().And.AllBeOfType<Perimeter>();
		}
	}
}