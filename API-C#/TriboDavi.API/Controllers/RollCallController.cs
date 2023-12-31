using Common.Functions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TriboDavi.Domain.Enum;
using TriboDavi.DTO;
using TriboDavi.Service.Interface;

namespace TriboDavi.API.Controllers
{
    public class RollCallController : BaseController
    {
        private readonly IRollCallService _rollCallService;

        public RollCallController(IRollCallService rollCallService)
        {
            _rollCallService = rollCallService;
        }

        [HttpGet("")]
        [Authorize(Roles = $"{nameof(RoleName.Admin)}, {nameof(RoleName.Teacher)}, {nameof(RoleName.AssistantTeacher)}")]
        public async Task<IActionResult> GetRollCall([FromQuery] DateOnly? date, [FromQuery] int? studentId)
        {
            var graduation = await _rollCallService.GetRollCall(date, studentId, User.GetIdStudentTeacher());
            return StatusCode(graduation.Code, graduation);
        }

        [HttpPost("Presence")]
        [Authorize(Roles = $"{nameof(RoleName.Admin)}, {nameof(RoleName.Teacher)}, {nameof(RoleName.AssistantTeacher)}")]
        public async Task<IActionResult> SetPresence([FromBody] PresenceDTO presenceDTO)
        {
            var graduation = await _rollCallService.SetPresence(presenceDTO, User.GetIdStudentTeacher());
            return StatusCode(graduation.Code, graduation);
        }

        [HttpPost("Generate")]
        [Authorize(Roles = $"{nameof(RoleName.Admin)}, {nameof(RoleName.Teacher)}, {nameof(RoleName.AssistantTeacher)}")]
        public async Task<IActionResult> GenerateRollCall()
        {
            var graduation = await _rollCallService.GenerateRollCall(User.GetIdStudentTeacher());
            return StatusCode(graduation.Code, graduation);
        }
    }
}