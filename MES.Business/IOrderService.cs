using MES.Shared;

namespace MES.Business;

public interface IOrderService
{
    IEnumerable<DTO> GetAll();
    DTO GetById(int id);
    long Create(DTO dto);
    void Update(int id, DTO dto);
    void Delete(int id);
}