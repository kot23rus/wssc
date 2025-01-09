using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;

namespace WSSC.Controllers;

/// <summary>
/// Контролер загрузки
/// </summary>
[Route("api/child")]
public class ChildLoader(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    /// <summary>
    /// Загрузить дочерние элементы объекта
    /// </summary>
    /// <param name="Id">Идентификатор родительского элемента</param>
    /// <returns>Список дочерних объектов</returns>
    [HttpGet]
    public async Task<ActionResult<List<ChildEntity>>> GetChilds([FromQuery] Guid Id)
    {
        try
        {
            var result = new List<ChildEntity>();
            var childs = await db.Companies.Where(x => x.ParentId == Id).OrderBy(x => x.Name).ToListAsync();
            foreach (var child in childs)
            {
                var cc = new ChildEntity()
                {
                    EntityType = child.EntityType,
                    Name = child.Name,
                    ID = child.Id
                };
                cc.HasChild = await db.Companies.Where(x => x.ParentId == child.Id).AnyAsync();
                result.Add(cc);
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "При загрузке дочерних объектов элемента {0} возникла ошибка: {1}", Id, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
