using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;
using WSSC.Data;
using static MudBlazor.CategoryTypes;
namespace WSSC.Controllers;

/// <summary>
/// Контролер удаления
/// объекта из БД
/// </summary>
[Route("api/delete")]
public class Delete(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    private string userId = string.Empty;

    [HttpDelete]
    public async Task<ActionResult<string>> Remove([FromQuery] Guid id, string user)
    {
        try
        {
            if (string.IsNullOrEmpty(user))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            else
            {
                userId = user;
            }
            if (await HasDelete(id))
            {
                await Remove(id);
                return StatusCode(StatusCodes.Status202Accepted);
            }
            else
            {
                return StatusCode(StatusCodes.Status423Locked, $"Один или более дочерних элементов редактируются, удаление не возможно!");
            }
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "При удалении объекта {0} пользователем {1} возникла ошибка: {2}", id, user, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Удалить элемент и все дочерние
    /// </summary>
    /// <param name="itemId">Идентификатор элемента</param>
    /// <returns></returns>
    public async Task<int> Remove(Guid itemId)
    {
        int removeItems = 0;
        removeItems = await db.Companies.AsNoTracking().Where(x => x.Id == itemId).ExecuteDeleteAsync();
        var childs = await db.Companies.AsNoTracking().Where(x => x.ParentId == itemId).Select(x => x.Id).ToListAsync();
        foreach (var child in childs)
        {
            removeItems += await Remove(child);
        }
        return removeItems;
    }

    public async Task<bool> HasDelete(Guid itemId)
    {
        if (!await HasModify(itemId))
            return false;
        return await HasExtract(itemId);
    }

    /// <summary>
    /// Проверяем все дочерние на блокировку
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    private async Task<bool> HasExtract(Guid itemId)
    {
        var Childs = await db.Companies.AsNoTracking().Where(x => x.ParentId == itemId).Select(x => new { x.Id, x.ExtUser }).ToListAsync();
        if (Childs != null)
        {
            foreach (var child in Childs)
            {
                if (!HasModify(child.ExtUser))
                    return false;
                else
                {
                    if (!await HasExtract(child.Id))
                        return false;
                }
            }
        }
        return true;
    }
    /// <summary>
    /// Проверить может ли текущий пользователь
    /// изменить элемент
    /// </summary>
    /// <param name="user">Ид пользователя который заблокировал элемент</param>
    /// <returns>Истина если можно редактировать</returns>
    private bool HasModify(string? user)
    {
        if (user != null)
        {
            if (user == userId)
                return true;
            else
                return false;
        }
        else
            return true;
    }

    /// <inheritdoc/>
    public async Task<bool> HasModify(Guid itemId)
    {
        var itemUser = await db.Companies.AsNoTracking().Where(x => x.Id == itemId).Select(x => x.ExtUser).FirstOrDefaultAsync();
        return HasModify(itemUser);
    }
}
