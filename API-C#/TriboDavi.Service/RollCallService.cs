using Common.DTO;
using Common.Functions;
using Microsoft.EntityFrameworkCore;
using TriboDavi.DataAccess.Interface;
using TriboDavi.Domain;
using TriboDavi.DTO;
using TriboDavi.Service.Interface;

namespace TriboDavi.Service
{
    public class RollCallService : IRollCallService
    {
        private readonly IFieldOperationStudentRepository _fieldOperationStudentRepository;
        private readonly IRollCallRepository _rollCallRepository;

        public RollCallService(IFieldOperationStudentRepository fieldOperationStudentRepository, IRollCallRepository rollCallRepository)
        {
            _fieldOperationStudentRepository = fieldOperationStudentRepository;
            _rollCallRepository = rollCallRepository;
        }

        public async Task<ResponseDTO> GenerateRollCall(int? teacherId = null)
        {
            ResponseDTO responseDTO = new();
            try
            {
                var fieldOperationStudents = await _fieldOperationStudentRepository.GetTrackedEntities()
                                                                                   .Include(x => x.FieldOperationTeacher)
                                                                                   .Include(x => x.Student)
                                                                                   .Where(x => teacherId == null ||
                                                                                               x.FieldOperationTeacher.Teacher.Id == teacherId ||
                                                                                               x.FieldOperationTeacher.Teacher.AssistantTeachers.Any(a => a.Id == teacherId))
                                                                                   .ToListAsync();
                var responses = new List<ResponseDTO>();
                foreach (var item in fieldOperationStudents)
                {
                    ResponseDTO response = new();
                    RollCall rollCall = new() { Date = DateOnly.FromDateTime(DateTime.Now), FieldOperationStudent = item, Presence = false };
                    try
                    {
                        rollCall.SetCreatedAt();
                        _rollCallRepository.Insert(rollCall);
                        response.Object = rollCall;
                        await _rollCallRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        response.SetError(ex);
                    }
                    _rollCallRepository.Detach(rollCall);
                    responses.Add(response);
                }
                responseDTO.Object = responses;
            }
            catch (Exception ex)
            {
                responseDTO.SetError(ex);
            }
            return responseDTO;
        }

        public async Task<ResponseDTO> GetRollCall(DateOnly? date = null, int? studentId = null, int? teacherId = null)
        {
            ResponseDTO responseDTO = new();
            try
            {
                responseDTO.Object = await _rollCallRepository.GetEntities()
                                                              .Where(x => (date == null || x.Date == date) &&
                                                                          (studentId == null || x.FieldOperationStudent.Student.Id == studentId) &&
                                                                          (teacherId == null || x.FieldOperationStudent.FieldOperationTeacher.Teacher.Id == teacherId || x.FieldOperationStudent.FieldOperationTeacher.Teacher.AssistantTeachers.Any(a => a.Id == teacherId)))
                                                              .Select(x => new
                                                              {
                                                                  x.Id,
                                                                  x.Date,
                                                                  x.Presence,
                                                                  StudentId = x.FieldOperationStudent.Student.Id,
                                                                  StudentName = x.FieldOperationStudent.Student.Name,
                                                                  StudentUrl = Functions.GetUrl(x.FieldOperationStudent.Student.Url),
                                                                  StudentAge = Functions.CalculateAge(x.FieldOperationStudent.Student.BirthDate),
                                                                  GraduationName = x.FieldOperationStudent.Student.Graduation.Name,
                                                                  GraduationUrl = x.FieldOperationStudent.Student.Graduation.GetUrl(),
                                                              })
                                                              .OrderBy(x => x.Date)
                                                              .ThenBy(x => x.StudentName)
                                                              .ToListAsync();
            }
            catch (Exception ex)
            {
                responseDTO.SetError(ex);
            }
            return responseDTO;
        }

        public async Task<ResponseDTO> SetPresence(PresenceDTO presenceDTO, int? teacherId = null)
        {
            ResponseDTO responseDTO = new();
            try
            {
                var rollCall = await _rollCallRepository.GetTrackedEntities().FirstOrDefaultAsync(x => x.FieldOperationStudent.Student.Id == presenceDTO.StudentId &&
                                                                                                       x.Date == presenceDTO.Date &&
                                                                                                       (teacherId == null || x.FieldOperationStudent.FieldOperationTeacher.Teacher.Id == teacherId || x.FieldOperationStudent.FieldOperationTeacher.Teacher.AssistantTeachers.Any(a => a.Id == teacherId)));
                if (rollCall == null)
                {
                    responseDTO.SetBadInput("Chamada n�o encontrada!");
                    return responseDTO;
                }
                rollCall.Presence = presenceDTO.Presence;
                rollCall.SetUpdatedAt();
                await _rollCallRepository.SaveChangesAsync();
                responseDTO.Object = rollCall;
            }
            catch (Exception ex)
            {
                responseDTO.SetError(ex);
            }
            return responseDTO;
        }
    }
}