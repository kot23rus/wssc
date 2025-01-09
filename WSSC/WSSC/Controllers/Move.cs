using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WSSC.Client.Model;
using static MudBlazor.CategoryTypes;
namespace WSSC.Controllers;

/// <summary>
/// Контролер перемещения
/// элемента данных
/// </summary>
[Route("api/move")]
public class Move(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    /// <summary>
    /// Переместить элемент
    /// </summary>
    /// <param name="request">Запрос на изменение</param>
    /// <returns>Если вернулась не пустая строка, значит извлечение не выполнено</returns>
    [HttpPatch]
    public async Task<ActionResult<string>> Patch([FromBody] MoveRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            
            var count = await db.Companies.Where(x => (x.Id == request.EntityId)).ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Modified, DateTime.UtcNow)
                        .SetProperty(b => b.ParentId, request.ParentId));
            if (count == 1)
            {
                return StatusCode(StatusCodes.Status202Accepted);
            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "При перемещении объекта {0} пользователем {1} возникла ошибка: {2}", request.EntityId, request.UserName, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
