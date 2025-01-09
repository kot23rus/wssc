using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http;
using System.Net.Http.Json;
using WSSC.Client.Model;
using System.Collections.ObjectModel;
namespace WSSC.Client.Pages;

public partial class Home 
{
    /// <summary>
    /// Признак что идет загрузка компаний
    /// </summary>
    private bool Loading { get; set; } = false;

    /// <summary>
    /// Список доступных компаний
    /// </summary>
    private ObservableCollection<Model.Company> Companies { get; set; } = [];

    /// <summary>
    /// Скрыть надпись шо компаний не найдено в БД
    /// </summary>
    private bool NoCompaniesHidden
    {
        get
        {
            if (Loading)
                return Companies.Count > 0;
            return true;
        }
    }
    /// <summary>
    /// Скрыть список компаний
    /// </summary>
    private bool CompaniesListHidden
    {
        get
        {
            if (Loading)
                return Companies.Count == 0;
            return true;
        }
    }
   

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        //TODO: Обрабатывать ошибку.
        //      не сделано, при краше показывает подсказку шо нужно релоадить
        if (httpClient.BaseAddress!=null)
        {
            var result = await httpClient.GetFromJsonAsync<List<Model.Company>>($"{httpClient.BaseAddress.ToString()}api/companies/list");
            if (result != null)
            {
                Loading = true;
                Companies.Clear();
                foreach(var company in result)
                {
                    Companies.Add(company);
                }
            }
        }
    }
    /// <summary>
    /// Получаем ссылку на страницу компании
    /// </summary>
    /// <param name="id">Идентификатор компании</param>
    /// <returns>ссылка на страницу компании</returns>
    private string GetLink(Guid id)
    {
        return $"/company/{id}";
    }
    /// <summary>
    /// Название для создания новой компании
    /// </summary>
    public string NewCompanyName { get; set; } = string.Empty;
    /// <summary>
    /// Признак что идет обработка
    /// запроса на сервер (создание новой компании)
    /// </summary>
    private bool processing = false;
    /// <summary>
    /// Ну а тут мы создаем новую компанию
    /// </summary>
    /// <returns>Да ни чего не возвращаем, метод просто ассинхронный</returns>
    private async Task CreateCompany()
    {
        if (!string.IsNullOrEmpty(NewCompanyName))
        {
            processing = true;
            if (httpClient.BaseAddress != null)
            {
                var req = new CreateEntity()
                {
                    EType = EntityType.Company,
                    Name = NewCompanyName,
                    UserName = userData.Name
                };
                var result = await httpClient.PostAsJsonAsync<CreateEntity>($"{httpClient.BaseAddress.ToString()}api/create", req);
                if (result.IsSuccessStatusCode)
                {
                    var resp = await result.Content.ReadAsStringAsync();
                    if (Guid.TryParse(resp.Trim(['\'', '"']), out Guid id))
                    {
                        Companies.Add(new Model.Company() { Name = NewCompanyName, ID = id });
                        NewCompanyName = "";
                    }
                }
            }
            processing = false;
            
        }
    }

    private async Task ButtonClick(object e)
    {
        if(e is EditButtion btn)
        {
            btn.Processing = true;
            await Task.Delay(1000);
            btn.Processing = false;
        }
    }
}
