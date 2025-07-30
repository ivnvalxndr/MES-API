using MES.Shared;

namespace MES.Business;

public interface IOrderService
{
    IEnumerable<OrderDTO> GetAll();
    OrderDTO GetById(int id);
    long Create(OrderDTO dto);
    void Update(int id, OrderDTO dto);
    void Delete(int id);
}