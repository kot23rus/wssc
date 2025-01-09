using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;

namespace WSSC.Controllers;

/// <summary>
/// Контролер отмены
/// извлечения элемента
/// </summary>
[Route("api/canceledit")]
public class CancelEdit(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    /// <summary>
    /// О
    /// </summary>
    /// <param name="request">Запрос на возврат</param>
    /// <returns></returns>
    [HttpPatch]
    public async Task<ActionResult<string>> Patch([FromBody] EditRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            var count = await db.Companies.Where(x => (x.Id == request.EntityId) && (x.ExtUser == request.UserName)).ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.ExtUser, (string?)null)
                        .SetProperty(b => b.Modified, DateTime.UtcNow));
            if (count == 1)
            {
                return StatusCode(StatusCodes.Status202Accepted);
            }
            else
            {
                return StatusCode(StatusCodes.Status423Locked);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "При возврате объекта {0} пользователем {1} возникла ошибка: {2}", request.EntityId, request.UserName, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
