using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Arm.CreateTask.Controllers
{
	[Authorize]
	public class ApiController : Controller
	{

	}
}
