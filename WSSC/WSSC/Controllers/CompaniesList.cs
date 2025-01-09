using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSSC.Client.Model;
namespace WSSC.Controllers;

/// <summary>
/// Контролер получения списка компаний
/// </summary>
[Route("api/companies/list")]
public class CompaniesList(ILogger<CompaniesList> logger, Data.CompanyDbContext db) : ControllerBase
{
    /// <summary>
    /// Извлекаем полный список компаний из БД
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<Company>>> GetCompanies()
    {
        try
        {
            var companies = await db.Companies.Where(x => x.EntityType == EntityType.Company && x.ParentId == null).Select(x => new Company(){ Name = x.Name, ID = x.Id}).OrderBy(x=> x.Name).ToListAsync();
            if(companies!=null)
            {
                return companies;
            }
            else
            {
                return new List<Company>();
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "При загрузке списка компаний возникла ошибочка:{0}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
    }

}
