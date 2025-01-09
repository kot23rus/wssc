using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;
namespace WSSC.Controllers;

/// <summary>
/// Контролер создания новой элемента данных
/// </summary>
[Route("api/create")]
public class Create(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateObject([FromBody] CreateEntity req)
    {
        logger.LogInformation("Пользователь {0} пытается создать объект [{1}]{2}", req.UserName, req.EType, req.Name);
        try
        {
            var entity = new Data.Entity()
            {
                EntityType = req.EType,
                Name = req.Name,
                ParentId = req.Parent
            };
            await db.Companies.AddAsync(entity);
            var save = await db.SaveChangesAsync();
            if (save == 1)
            {
                logger.LogInformation("Пользователь {0} создал объект {1}", req.UserName, entity.Id);
                return entity.Id;
            }
            else
            {
                logger.LogError("Не получилось сохранить данные в БД");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка создания объекта в БД: {0}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
