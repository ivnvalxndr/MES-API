using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Business.Interfaces;
using MES.Data.Enums;
using System.Security.Claims;
using MES.Shared.DTOs;

namespace MES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все заказы (с учетом роли пользователя)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var userRole = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)!.Value);

                var orders = await _orderService.GetAllAsync(userId, userRole);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var userRole = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)!.Value);

                var order = await _orderService.GetByIdAsync(id, userId, userRole);

                if (order == null)
                    return NotFound(new { message = $"Order with ID {id} not found" });

                return Ok(order);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to order {OrderId}", id);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Создать новый заказ
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Technologist,Manager,Admin")]
        public async Task<IActionResult> Create([FromBody] OrderDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid model state", errors = ModelState.Values.SelectMany(v => v.Errors) });

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var orderId = await _orderService.CreateAsync(dto, userId);

                return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновить заказ
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Technologist,Manager,Admin")]
        public async Task<IActionResult> Update(long id, [FromBody] OrderDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid model state", errors = ModelState.Values.SelectMany(v => v.Errors) });

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _orderService.UpdateAsync(id, dto, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized update attempt for order {OrderId}", id);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _orderService.DeleteAsync(id, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized delete attempt for order {OrderId}", id);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Запустить заказ в работу (только для операторов)
        /// </summary>
        [HttpPost("{id}/start")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> StartOrder(long id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.StartOrderAsync(id, userId);

                if (result)
                    return Ok(new { message = "Order started successfully" });
                else
                    return BadRequest(new { message = "Failed to start order" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized start attempt for order {OrderId}", id);
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Завершить заказ
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Operator,Technologist,Manager,Admin")]
        public async Task<IActionResult> CompleteOrder(long id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CompleteOrderAsync(id, userId);

                if (result)
                    return Ok(new { message = "Order completed successfully" });
                else
                    return BadRequest(new { message = "Failed to complete order" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Отменить заказ
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Technologist,Manager,Admin")]
        public async Task<IActionResult> CancelOrder(long id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _orderService.CancelOrderAsync(id, userId);

                if (result)
                    return Ok(new { message = "Order cancelled successfully" });
                else
                    return BadRequest(new { message = "Failed to cancel order" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновить статус заказа
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Technologist,Manager,Admin")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] OrderDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Создаем временный DTO только со статусом
                var statusUpdateDto = new OrderDTO
                {
                    OrderID = id,
                    Status = dto.Status
                };

                await _orderService.UpdateAsync(id, statusUpdateDto, userId);
                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Назначить оператора на заказ
        /// </summary>
        [HttpPost("{id}/assign")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> AssignOperator(long id, [FromBody] OrderDTO dto)
        {
            try
            {
                var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                if (!dto.AssignedOperatorId.HasValue)
                    return BadRequest(new { message = "OperatorId is required" });

                var result = await _orderService.AssignOperatorAsync(id, dto.AssignedOperatorId.Value, managerId);

                if (result)
                    return Ok(new { message = "Operator assigned successfully" });
                else
                    return BadRequest(new { message = "Failed to assign operator" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized assign attempt for order {OrderId}", id);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning operator to order {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Получить мои заказы (для операторов)
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var operatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var orders = await _orderService.GetMyOrdersAsync(operatorId);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my orders for operator {OperatorId}",
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Получить заказы по статусу
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var userRole = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)!.Value);

                var allOrders = await _orderService.GetAllAsync(userId, userRole);

                // Преобразуем string в OrderStatus для фильтрации
                if (!Enum.TryParse<EntityStatus>(status, true, out var statusFilter))
                {
                    return BadRequest(new { message = $"Invalid status value: {status}" });
                }

                var filteredOrders = allOrders.Where(o => o.Status == statusFilter);
                return Ok(filteredOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders by status {Status}", status);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}