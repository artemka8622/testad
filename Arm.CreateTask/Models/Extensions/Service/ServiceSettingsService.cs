using System.Linq;
using Arm.CreateTask.Models.DataModel;
using Cordis.Core.Common.Sql;
using Dapper;

namespace Arm.CreateTask.Models.Service
{
	/// <summary>
	/// Сервис, предоставляющий доступ к API Noc Cordis.
	/// </summary>
	public class ServiceSettingsService
	{
		/// <summary>
		/// Получение настроек сервиса по коду.
		/// </summary>
		/// <param name="service_code">Код сервиса.</param>
		/// <returns>Настройки.</returns>
		public ServiceSettings GetSettingsByCode(string service_code)
		{
			using (var sqlh = new SqlHelper())
			{
				return sqlh.GetConnection().Query<ServiceSettings>($@"
select 
	ws.extra.value('(/ServiceSettings/login/node())[1]', 'nvarchar(max)')			{nameof(ServiceSettings.Login)},
	ws.extra.value('(/ServiceSettings/password/node())[1]', 'nvarchar(max)')		{nameof(ServiceSettings.Password)},
	ws.extra.value('(/ServiceSettings/service_url/node())[1]', 'nvarchar(max)')		{nameof(ServiceSettings.Url)},
	ws.extra.value('(/ServiceSettings/port/node())[1]', 'nvarchar(max)')			{nameof(ServiceSettings.Port)}
from Tuning.web_service ws
where ws.code = @{nameof(service_code)}
", new { service_code }).Single();
			}
		}
	}
}