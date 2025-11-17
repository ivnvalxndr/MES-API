using MES.Shared;

namespace MES.Business.Interfaces;

public interface IOrderService
{
    IEnumerable<OrderDTO> GetAll();
    OrderDTO GetById(int id);
    long Create(OrderDTO dto);
    void Update(int id, OrderDTO dto);
    void Delete(int id);
}