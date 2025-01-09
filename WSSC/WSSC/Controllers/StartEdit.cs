using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;
using static MudBlazor.CategoryTypes;

namespace WSSC.Controllers;

/// <summary>
/// Контролер проверки
/// доступности редактирования
/// </summary>
[Route("api/startedit")]
public class StartEdit(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    /// <summary>
    /// Начать редактирование элемента
    /// </summary>
    /// <param name="request">Запрос на извлечение</param>
    /// <returns>Если вернулась не пустая строка, значит извлечение не выполнено</returns>
    [HttpPatch]
    public async Task<ActionResult<string>> Patch([FromBody] EditRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            var entity = await db.Companies.Where(x => x.Id == request.EntityId).FirstOrDefaultAsync();
            if (entity!=null)
            {
                if (entity.ExtUser!=null)
                {
                    if (entity.ExtUser == request.UserName)
                        return StatusCode(StatusCodes.Status202Accepted);
                    else
                        return StatusCode(StatusCodes.Status423Locked, $"Уже редактируется пользователем {entity.ExtUser} c {entity.Modified}");
                }
                else
                {
                    var count = await db.Companies.Where(x => (x.Id == request.EntityId) && (x.ExtUser == null)).ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.ExtUser, request.UserName)
                        .SetProperty(b => b.Modified, DateTime.UtcNow));
                    if (count==1)
                    {
                        return StatusCode(StatusCodes.Status202Accepted);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status423Locked);
                    }
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "При извлечении объекта {0} пользователем {1} возникла ошибка: {2}", request.EntityId, request.UserName, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
