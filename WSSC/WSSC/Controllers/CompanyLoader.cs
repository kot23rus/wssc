using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;
namespace WSSC.Controllers;

/// <summary>
/// Контроллер загрузки данных компании
/// </summary>
/// <param name="logger">Журнал сервера</param>
/// <param name="db">Контекст подключения к БД</param>
[Route("api/company")]
public class CompanyLoader(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    /// <summary>
    /// Загрузить компанию
    /// </summary>
    /// <param name="Id">Идентификатор компании</param>
    /// <returns>Данные компании, и набор начальных узлов</returns>
    [HttpGet]
    public async Task<ActionResult<CompanyWithChild>> GetCompany([FromQuery]Guid Id)
    {
        try
        {
            var company = await db.Companies.Where(x => x.EntityType == EntityType.Company && x.Id == Id).Select(x => new CompanyWithChild() { Name = x.Name, ID = x.Id }).FirstOrDefaultAsync();
            if (company != null)
            {
                var childs = await db.Companies.Where(x => x.ParentId == Id).OrderBy(x => x.Name).ToListAsync();
                foreach(var child in childs)
                {
                    var cc = new ChildEntity()
                    {
                        EntityType = child.EntityType,
                        Name = child.Name,
                        ID = child.Id
                    };
                    cc.HasChild = await db.Companies.Where(x => x.ParentId == child.Id).AnyAsync();
                    company.Childs.Add(cc);
                }
                return company;
            }
            else
            {
                logger.LogError("Не смогли найти запрошенную компанию {0}", Id);
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "При загрузке данных компании {0} возникла ошибка: {1}", Id, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
