using Common.DTO;
using Common.Infrastructure;
using TriboDavi.DTO;

namespace TriboDavi.Service.Interface
{
    public interface ILegalParentService : IServiceBase<LegalParentDTO>
    {
        Task<ResponseDTO> GetLegalParentsForListbox();
    }
}