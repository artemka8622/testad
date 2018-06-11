using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arm.CreateTask.Models.DataModel;
using Novell.Directory.Ldap;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис Ldap. 
	/// </summary>
	public class LdapService : ILdapService
	{
		#region Constants

		/// <summary>
		/// Разделитель домена.
		/// </summary>
		private const string DOMAIN_DELIMITER = @"\";

		/// <summary>
		/// Домен.
		/// </summary>
		private const string DEFAULT_DOMAIN = "CORP";

		/// <summary>
		/// Top domain.
		/// </summary>
		private const string DEFAULT_DN = "OU=Холдинг,DC=corp,DC=itmh,DC=ru";

		/// <summary>
		/// Атрибут для канонического имени.
		/// </summary>
		private const string FULL_NAME_ATTRIBUTE = "cn";

		/// <summary>
		/// Шаблон для поиска OU.
		/// </summary>
		private const string REGEX_EXTRACT_OU = "OU=(.+?),";

		/// <summary>
		/// Атрибут хранит название организационного юнита для сотрудника.
		/// </summary>
		private const string DEPARTMENT_ATTRIBUTE = "department";

		/// <summary>
		/// Атрибут extensionAttribute14 хранит признак, является ли сотрудник руководителем организационного юнита (значение атрибута = 2),
		/// заместителем руководителя (значение атрибута = 1) или сотрудником (значение атрибута = 0).
		/// </summary>
		public const string EMPLOYEE_TYPE_ATTRIBUTE = "extensionAttribute14";

		/// <summary>
		/// Константа для атрибута sAMAccountName.
		/// </summary>
		private const string SAM_ACCOUNT_NAME_ATTRIBUTE = "sAMAccountName";

		/// <summary>
		/// Константа для атрибута memberOf.
		/// </summary>
		private const string MEMBER_OF_ATTRIBUTE = "memberOf";

		/// <summary>
		/// Логин генерального директора.
		/// </summary>
		public const string GENERAL_DIRECTOR = "jedi";

		/// <summary>
		/// Код сервиса ldap.
		/// </summary>
		private const string LDAP_SERVICE_CODE = "ldap";

		/// <summary>
		/// Название группы CRP.GM.0.0 Генеральный директор и его заместители.
		/// </summary>
		private const string GENERAL_DIRECTOR_GROUP = "CRP.GM.0.0";

		/// <summary>
		/// Название группы CRP.GM.0.2 Отдел автоматизации бизнес-процессов.
		/// </summary>
		private const string CRP_GM_0_2_GROUP_NAME = "CRP.GM.0.2 Отдел автоматизации бизнес-процессов";

		/// <summary>
		/// Название группы CRP.GM.0.3 Отдел оптимизации бизнес-процессов.
		/// </summary>
		private const string CRP_GM_0_3_GROUP_NAME = "CRP.GM.0.3 Отдел оптимизации бизнес-процессов";

		/// <summary>
		/// Уволенные сотрудники.
		/// </summary>
		private const string FIRED_GROUP = "Ушедшие в долину смерти";

		/// <summary>
		/// Максимальное число наблюдателей.
		/// </summary>
		public const int WATCHERS_COUNT = 2;

		/// <summary>
		/// Возвращает заместителя генерального директора по автоматизации бизнес-процессов.
		/// </summary>
		public PersonInfo GetDelfin => new PersonInfo
		{
			Login = "delfin",
			FullName = "Демин Алексей Сергеевич"
		};

		/// <summary>
		/// Возвращает заместителя генерального директора по оптимизации бизнес-процессов.
		/// </summary>
		public PersonInfo GetBush => new PersonInfo
		{
			Login = "bush",
			FullName = "Бушков Игорь Ефимович"
		};

		#endregion

		#region Properties

		/// <summary>
		/// Конфигурация.
		/// </summary>
		private readonly ServiceSettings _config;

		/// <summary>
		/// LDAP подключение.
		/// </summary>
		private readonly LdapConnection _connection;
		
		#endregion

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="settings_service">Сервис, предоставляющий доступ к API Noc Cordis.</param>
		public LdapService(ServiceSettingsService settings_service)
		{
			_config = settings_service.GetSettingsByCode(LDAP_SERVICE_CODE);
			_connection = new LdapConnection
			{
				SecureSocketLayer = true
			};
		}

		/// <summary>
		/// Получает наблюдателей по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Список наблюдателей.</returns>
		public IEnumerable<PersonInfo> GetWatchersByLogin(string login)
		{
			try
			{
				_connection.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, policy_errors) => true;
				_connection.Connect(_config.Url, /*_config.Port ??*/ LdapConnection.DEFAULT_SSL_PORT);

				if (!login.Contains(DOMAIN_DELIMITER))
				{
					login = DEFAULT_DOMAIN + DOMAIN_DELIMITER + login;
				}

				_connection.Bind(_config.Login, _config.Password);

				if (_connection.Bound)
				{
					return GetWatchers(login);
				}

				return null;
			}
			catch (Exception)
			{
				_connection.Disconnect();

				throw;
			}
		}

		/// <summary>
		/// Получает наблюдателей по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Список наблюдателей.</returns>
		private IEnumerable<PersonInfo> GetWatchers(string login)
		{
			var pure_login = GetPureLogin(login);

			var ou = GetDepartmentsByLogin(pure_login);

			if (IsMemberOfGeneralDirectorate(pure_login))
			{
				return Enumerable.Empty<PersonInfo>();
			}

			var overwatchers = GetOverWatchers(ou);

			var bosses = ou.Select(GetBossForOu).Where(x => x != null).ToList();

			var sam_account_name = pure_login;

			bosses.RemoveAll(x => string.Equals(x.Login, GENERAL_DIRECTOR, StringComparison.CurrentCultureIgnoreCase)
								|| string.Equals(x.Login, sam_account_name, StringComparison.CurrentCultureIgnoreCase));

			overwatchers.AddRange(bosses);

			return overwatchers.Take(WATCHERS_COUNT);
		}

		#region Methods/Private

		/// <summary>
		/// Получает начальника для операционного юнита.
		/// </summary>
		/// <param name="ou">Операционный юнит.</param>
		/// <returns>Начальник.</returns>
		private PersonInfo GetBossForOu(string ou)
		{
			var filter = $"(&({EMPLOYEE_TYPE_ATTRIBUTE}=2)({DEPARTMENT_ATTRIBUTE}={ou}))";

			var result = Search(DEFAULT_DN, filter, new []
			{
				EMPLOYEE_TYPE_ATTRIBUTE,
				DEPARTMENT_ATTRIBUTE,
				SAM_ACCOUNT_NAME_ATTRIBUTE,
				FULL_NAME_ATTRIBUTE
			});
			
			while (result.HasMore())
			{
				var ldap_info = result.Next();

				var login = ldap_info.getAttribute(SAM_ACCOUNT_NAME_ATTRIBUTE).StringValue;
				var boss_ou = GetDepartmentsByLogin(login);
				if (boss_ou.Contains(FIRED_GROUP))
				{
					continue;
				}

				var person_info = new PersonInfo
				{
					Login = ldap_info.getAttribute(SAM_ACCOUNT_NAME_ATTRIBUTE).StringValue,
					FullName = ldap_info.getAttribute(FULL_NAME_ATTRIBUTE).StringValue
				};

				return person_info;
			}

			return null;
		}

		/// <summary>
		/// Получает список операционных юнитов по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Список операционных юнитов по логину.</returns>
		private List<string> GetDepartmentsByLogin(string login)
		{
			var result = GetUserCanonicalNameByLogin(GetPureLogin(login));
			var rgx = new Regex(REGEX_EXTRACT_OU, RegexOptions.IgnoreCase);
			var operational_units = rgx.Matches(result).Select(x => x.Groups[1].Value).ToList();
			if (operational_units.Count > 0 && operational_units.Contains("Холдинг"))
			{
				operational_units.RemoveAt(operational_units.Count-1);
				return operational_units;
			}

			return new List<string>();
		}

		/// <summary>
		/// Возвращает каноническое имя пользователя.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		/// <returns>Каноническое имя пользователя.</returns>
		private string GetUserCanonicalNameByLogin(string login)
		{
			var filter = $"(&({SAM_ACCOUNT_NAME_ATTRIBUTE}={login}))";
			var data = Search(DEFAULT_DN, filter);
			return !data.HasMore() ? "" : data.Next().DN;
		}

		/// <summary>
		/// Определяется принадлежит ли логин группе CRP.GM.0.0 Генеральный директор и его заместители.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>True если принадлежит группе CRP.GM.0.0 Генеральный директор и его заместители.</returns>
		private bool IsMemberOfGeneralDirectorate(string login)
		{
			var filter = $"(&({SAM_ACCOUNT_NAME_ATTRIBUTE}={login}))";

			var result = Search(DEFAULT_DN, filter, new[]
			{
				SAM_ACCOUNT_NAME_ATTRIBUTE,
				MEMBER_OF_ATTRIBUTE
			});

			if (!result.HasMore())
			{
				return false;
			}

			var ldap_info = result.Next();

			var member_of = ldap_info.getAttribute(MEMBER_OF_ATTRIBUTE).StringValue;

			return member_of.Contains(GENERAL_DIRECTOR_GROUP);
		}

		/// <summary>
		/// Получает наблюдателей:
		/// Если автором задачи является сотрудник CRP.GM.0.3, то в наблюдателях указывается начальник CRP.GM.0.3 и заместитель генерального директора по оптимизации бизнес-процессов.
		/// Если автором задачи является сотрудник CRP.GM.0.2, то в наблюдателях указывается начальник CRP.GM.0.2 и заместитель генерального директора по информационным технологиям;
		/// </summary>
		/// <param name="ou">Список операционных юнитов для сотрудника.</param>
		/// <returns>Список наблюдателей.</returns>
		private List<PersonInfo> GetOverWatchers(List<string> ou)
		{
			var overwatchers = new List<PersonInfo>();

			if (ou.Contains(CRP_GM_0_2_GROUP_NAME))
			{
				overwatchers.Add(GetDelfin); 
			}

			if (ou.Contains(CRP_GM_0_3_GROUP_NAME))
			{
				overwatchers.Add(GetBush);
			}

			return overwatchers;
		}

		/// <summary>
		/// Возвращает результаты поиска по фильтру.
		/// </summary>
		/// <param name="dn">Базовое DN для каталога.</param>
		/// <param name="filter">Поисковый фильтр.</param>
		/// <param name="attributes">Массив необходимых атрибутов.</param>
		/// <returns>Результаты поиска по фильтру</returns>
		private LdapSearchResults Search(string dn, string filter, string[] attributes = null)
		{
			if (attributes == null)
			{
				attributes = new[] {"ou", "cn"};
			}
			
			var result = _connection.Search(
				dn,
				LdapConnection.SCOPE_SUB,
				filter,
				attributes,
				false
			);

			return result;
		}

		/// <summary>
		/// Получает логин без домена.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Логин без домена.</returns>
		private static string GetPureLogin(string login) => login.Replace(DEFAULT_DOMAIN + DOMAIN_DELIMITER, "");

		#endregion
	}
}