using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Arm.CreateTask
{
	/// <summary>
	/// Класс программы.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Точка входа в программу.
		/// </summary>
		/// <param name="args">Аргументы запуска.</param>
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		/// <summary>
		/// Создает хост приложения.
		/// </summary>
		/// <param name="args">Аргументы запуска.</param>
		/// <returns>Хост приложения <see cref="IWebHost" /> .</returns>
		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.ConfigureLogging(logging =>
					{
						logging.ClearProviders();
						logging.SetMinimumLevel(LogLevel.Trace);
						logging.AddConsole();
						logging.AddDebug();
					})
				.UseNLog()
				.Build();
	}
}
