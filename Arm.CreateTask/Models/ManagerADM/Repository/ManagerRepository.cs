using Arm.CreateTask.Models.ManagerADM.DataModel;
using Cordis.Core.Common.Sql;
using Dapper;

namespace Arm.CreateTask.Models.ManagerADM.Repository
{
	/// <summary>
	/// Репозиторий менеджера.
	/// </summary>
	public class ManagerRepository : IManagerRepository
	{
		/// <summary>
		/// Возвращает менеджера по логину.
		/// </summary>
		/// <param name="login">Логин.</param>
		/// <returns>Менеджер.</returns>
		public Manager GetManagerByLogin(string login)
		{
			using (SqlHelper sqlh = new SqlHelper())
			{
				return sqlh.GetConnection().QuerySingle<Manager>($@"
select 
	m.manager			{nameof(Manager.Id)},
	m.full_name_nom		{nameof(Manager.FullName)},
	m.short_name_nom	{nameof(Manager.ShortName)},
	m.email				{nameof(Manager.Email)},
	m.sid				{nameof(Manager.Sid)},
	m.city				{nameof(Manager.CityId)},
	m.branch			{nameof(Manager.BranchId)},
	e.employee			{nameof(Manager.EmployeeId)},
	g.name				{nameof(Manager.GroupName)},
	g.short_name		{nameof(Manager.GroupShortName)},
	j.job				{nameof(Manager.JobId)},
	j.name				{nameof(Manager.JobName)}
from crm.manager m 
	join Employee.employee e on e.manager = m.manager and e.is_fired = 0
	join crm.manager_group g on g.manager_group = e.manager_group
	join crm.job j on j.job = e.job
where m.manager = CRM.fasManagerByNtLogin(@{nameof(login)})", new {login});
			}
		}
	}
}