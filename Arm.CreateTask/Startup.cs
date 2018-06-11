using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Arm.CreateTask.Models;
using Arm.CreateTask.Models.Auth.Claims;
using Arm.CreateTask.Models.DataModel;
using Arm.CreateTask.Models.Extensions;
using Arm.CreateTask.Models.ManagerADM.Repository;
using Arm.CreateTask.Models.ManagerADM.Service;
using Arm.CreateTask.Models.Repository;
using Arm.CreateTask.Models.Service;
using Arm.CreateTask.Tests.Model.Service;
using Cordis.Core.Common.Auth;
using Cordis.Core.Common.Sql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;
using Task = System.Threading.Tasks.Task;

namespace Arm.CreateTask
{
	/// <summary>
	/// Класс запуска приложения.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Инициализирует свойства.
		/// </summary>
		/// <param name="configuration">Интерфейс для работы с конфигурацией.</param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// Интерфейс для работы с конфигурацией.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// Настраивает сервисы. Метод вызывается во время выполнения.
		/// </summary>
		/// <param name="services">Сервисы.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// Классы из Cordis.Core.
			services.Configure<LdapConfig>(Configuration.GetSection("ldap"));
			services.AddScoped<SqlRoleProvider>();
			services.AddScoped<LdapAuthenticationService>();
			services.AddScoped<Cordis.Core.Common.Auth.IAuthenticationService, ItmhAuthenticationService>();
			services.AddScoped<IUserInfoService,UserInfoService>();
			services.AddScoped<ManagerService>();
			services.AddScoped<IManagerRepository, ManagerRepository>();

			services.AddScoped<IBaseConfigurationRepository<Models.DataModel.Task>, TaskRepository>();
			services.AddScoped<IBaseConfigurationRepository<Workflow>, WorkflowRepository>();
			services.AddScoped<IBaseConfigurationRepository<Role2Workflow>, Role2WorkflowRepository>();
			services.AddScoped<IBaseConfigurationRepository<Product>, ProductRepository>();
			services.AddScoped<IBaseConfigurationRepository<Perimeter>, PerimeterRepository>();
			services.AddScoped<IBaseConfigurationRepository<Exploration>, ExplorationRepository>();

			services.AddScoped<TaskCreateService>();
			services.AddScoped<TaskFormValidationService>();
			services.AddScoped<ServiceSettingsService>();
			services.AddScoped<IJiraApiService, JiraApiService>();
			services.AddScoped<ILdapService, LdapService>();

			AddAuthentication(services);
		}

		/// <summary>
		/// Настраивает приложение. Метод вызывается во время выполнения.
		/// </summary>
		/// <param name="app">Приложение.</param>
		/// <param name="env">Окружение.</param>
		/// <param name="logger_provider">Логгер.</param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerProvider logger_provider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			SqlHelper.СonnectionString = Configuration.GetConnectionString("Default");

			app.UseAuthentication();

			app.UseExceptionHandler(new ExceptionHandlerOptions()
			{
				ExceptionHandler = context =>
				{
					var exception_feature = context.Features.Get<IExceptionHandlerPathFeature>();
					logger_provider.CreateLogger("Error").LogError("Ошибка при обработке запроса: " + exception_feature.Error.Message + " StackTrace: " + exception_feature.Error.StackTrace);
					return Task.CompletedTask;
				}
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Task}/{action=Index}");
			});
		}

		/// <summary>
		/// Настраивает аутентификацию.
		/// </summary>
		/// <param name="services">Сервисы.</param>
		private void AddAuthentication(IServiceCollection services)
		{
			var sp = services.BuildServiceProvider();
			var manager_service = sp.GetService<ManagerService>();

			switch (Configuration.GetSection("AuthScheme").Get<AwailableSchemes>())
			{
				case AwailableSchemes.Basic:
					var auth_srv = sp.GetService<Cordis.Core.Common.Auth.IAuthenticationService>();
					services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
					.AddBasicAuthentication(options =>
					{
						options.Realm = "";
						options.Events = new BasicAuthenticationEvents
						{
							OnValidatePrincipal = context =>
							{
								var user = auth_srv.Login(context.UserName, context.Password);
								if (user != null)
								{
									var roles = GetOverridedRolesByLogin(context.UserName) ?? GetRolesBySidList(user.MemberOf.Select(x => x.Sid));

									context.Principal = GetPrincipal(
											context.UserName,
											roles.ToList(),
											manager_service,
											BasicAuthenticationDefaults.AuthenticationScheme);

									return System.Threading.Tasks.Task.CompletedTask;
								}

								return System.Threading.Tasks.Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
							}
						};
					});
					break;

				case AwailableSchemes.WsFederation:
					services.AddAuthentication(shared_options =>
					{
						shared_options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						shared_options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						shared_options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
					})
					.AddWsFederation(options =>
					{
						options.Wtrealm = Configuration["WsFederation:Realm"];
						options.MetadataAddress = Configuration["WsFederation:Metadata"];
						options.Events = new WsFederationEvents
						{
							OnSecurityTokenValidated = context =>
							{
								var login = context.Principal.Claims.SingleOrDefaultValue(ClaimTypes.WindowsAccountName);
								var roles = GetOverridedRolesByLogin(login);
								if (roles == null)
								{
									var allowed_types = new List<string> { ClaimTypes.Sid, ClaimTypes.PrimarySid, ClaimTypes.PrimaryGroupSid, ClaimTypes.GroupSid };
									var sids = context.Principal.Claims.Where(x => allowed_types.Contains(x.Type)).Select(x => x.Value);
									roles = GetRolesBySidList(sids);
								}

								context.Principal = GetPrincipal(login, roles.ToList(), manager_service, WsFederationDefaults.AuthenticationScheme);

								return System.Threading.Tasks.Task.CompletedTask;
							}
						};
					})
					.AddCookie(options =>
					{
						// Чтобы избежать проблем с большим заголовком(если не указать эту опцию, то все claims будут в заголовках запросов, а это несколько десятков кБ).
						// options.SessionStore = new MemoryCacheTicketStore();
					});
					break;
				default:
					throw new Exception("Неизвестная схема авторизации");
			}
		}

		/// <summary>
		/// Возвращает <see cref="ClaimsPrincipal" /> , построенный по данным менеджера.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		/// <param name="roles">Список Функциональных ролей пользователя (В будущем будет приходить из ADFS).</param>
		/// <param name="manager_service">Сервис для получения информации о пользователе из Cordis.</param>
		/// <param name="auth_scheme">Схема аутентификации, в которой нужно создать Principal.</param>
		/// <returns><see cref="ClaimsPrincipal" /> .</returns>
		private ClaimsPrincipal GetPrincipal(string login, List<string> roles, ManagerService manager_service, string auth_scheme)
		{
			var user = manager_service.GetManagerByLogin(login);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, login),
				new Claim(CreateTaskClaim.USER_ID, user.Id.ToString()),
				new Claim(ClaimTypes.GivenName, user.FullName),
				new Claim(ClaimTypes.Name, user.ShortName),
				new Claim(ClaimTypes.PrimarySid, user.Sid != null ? new SecurityIdentifier(user.Sid, 0).ToString() : string.Empty),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(CreateTaskClaim.BRANCH_ID, user.BranchId.ToString()),
				new Claim(CreateTaskClaim.CITY_ID, user.CityId.ToString()),
				new Claim(CreateTaskClaim.EMPLOYEE_ID, user.EmployeeId.ToString()),
				new Claim(CreateTaskClaim.GROUP_NAME, user.GroupName),
				new Claim(CreateTaskClaim.GROUP_SHORT_NAME, user.GroupShortName ?? user.GroupName),
				new Claim(CreateTaskClaim.JOB_ID, user.JobId.ToString()),
				new Claim(CreateTaskClaim.JOB_NAME, user.JobName)
			};
			
			claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

			foreach (var role in roles)
			{
				var secres = Configuration.GetSection($"SecurityResources:{role}")?.Get<string[]>();
				if (secres != null)
				{
					claims.AddRange(secres.Select(x => new Claim(CreateTaskClaim.FUNC_ROLE, x)));
				}
			}

			return new ClaimsPrincipal(new ClaimsIdentity(claims, auth_scheme));
		}

		/// <summary>
		/// Возвращает список ролей, соответствующих сидам групп.
		/// </summary>
		/// <param name="sids">Сид группы.</param>
		/// <returns>Список ролей.</returns>
		private IEnumerable<string> GetRolesBySidList(IEnumerable<string> sids)
		{
			return sids.Select(sid => Configuration.GetSection($"SidToRoles:{sid}")?.Get<string>())
				.Where(role_string => role_string != null)
				// Для групп с несколькими ролями роли перечислены через запятую.
				// TODO организовать перечисление через массив?
				.SelectMany(role_string => role_string.Split(',').Select(role => role.Trim()));
		}

		/// <summary>
		/// Возвращает список ролей по логину пользователя.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Список переопределенных ролей.</returns>
		private IEnumerable<string> GetOverridedRolesByLogin(string login)
		{
			return Configuration.GetSection($"LoginRolesOverride:{GetLoginWithoutDomain(login)}")?.Get<string[]>();
		}

		/// <summary>
		/// Возвращает логин пользователя без доменной приставки.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <param name="domain_delemiter">Разделитель.</param>
		/// <returns>Логин без разделителя.</returns>
		private string GetLoginWithoutDomain(string login, string domain_delemiter = ManagerService.DOMAIN_DELIMITER)
		{
			if (!login.Contains(domain_delemiter))
			{
				return login;
			}

			// Выглядит не очень надежно, но для ПРОТОТИПА сойдет.
			var parts = login.Split(domain_delemiter);

			return parts[1];
		}
	}
}
