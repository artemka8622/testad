using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Arm.CreateTask.Models.Auth.Claims;
using Arm.CreateTask.Models.ManagerADM.Service;
using Cordis.Core.Common.Sql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Arm.CreateTask.Tests
{
	public class BaseTests
	{
		/// <summary>Хост приложения.</summary>
		protected IWebHost WebHost { get; private set; }

		/// <summary>Конфигурация приложения.</summary>
		protected IConfiguration Configuration { get; set; }

		/// <summary>Конструктор базового класса <see cref="BaseTests"/> class.</summary>
		public BaseTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
			SqlHelper.СonnectionString = Configuration.GetConnectionString("Default");
			WebHost = Program.BuildWebHost(new string[0]);
		}
	}
}
