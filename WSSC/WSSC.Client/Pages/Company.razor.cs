using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using WSSC.Client.Model;
using static MudBlazor.CategoryTypes;

namespace WSSC.Client.Pages;

public partial class Company
{
    [Parameter]
    public string? Id { get; set; }


    public Model.Company? ShowCompany { get; set; }

    private List<ChildEntity> Items = [];

    public Guid CompanyID
    {
        get
        {
            if (ShowCompany != null)
                return ShowCompany.ID;
            else
                return Guid.Empty;
        }
    }

    private List<TreeItemData<Guid>> InitialChilds { get; set; } = new();

    public bool HideTool
    {
        get
        {
            return ShowCompany == null;
        }
    }

    public bool HideLoad
    {
        get
        {
            return ShowCompany != null;
        }
    }

    public Company()
    {

    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (httpClient.BaseAddress != null)
        {
            var result = await httpClient.GetFromJsonAsync<CompanyWithChild>($"{httpClient.BaseAddress.ToString()}api/company?id={Id}");
            if (result != null)
            {
                ShowCompany = new Model.Company()
                {
                    Name = result.Name,
                    ID = result.ID
                };
                InitialChilds.Clear();
                int number = 1;
                foreach (var child in result.Childs)
                {
                    InitialChilds.Add(new TreeItemData<Guid>()
                    {
                        Value = child.ID,
                        Expandable = child.HasChild,
                        Text = $"{number}. {child.Name}"
                    });
                    number++;
                }
                Items.AddRange(result.Childs);
            }
        }
    }

    public async Task<IReadOnlyCollection<TreeItemData<Guid>>> LoadServerData(Guid parentValue)
    {
        var result = new List<TreeItemData<Guid>>();
        if (httpClient.BaseAddress != null)
        {
            var resp = await httpClient.GetFromJsonAsync<List<ChildEntity>>($"{httpClient.BaseAddress.ToString()}api/child?id={parentValue}");
            if (resp != null)
            {
                int number = 1;
                foreach (var child in resp)
                {
                    result.Add(new TreeItemData<Guid>()
                    {
                        Value = child.ID,
                        Expandable = child.HasChild,
                        Text = $"{number}. {child.Name}"
                    });
                    number++;
                }
                Items.AddRange(resp);
            }
        }
        return result;
    }

    public async Task ShowCompanyEdit(object sender)
    {
        if (ShowCompany != null)
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = false,
                BackdropClick = false
            };
            var parameters = new DialogParameters<CompanyEditDialog>
                {
                    { x => x.Company, new Model.Company(){
                    ID = ShowCompany.ID,
                    Name = ShowCompany.Name}
                }
                };
            var dialog = await DialogService.ShowAsync<CompanyEditDialog>("Редактирование названия", parameters, options);
            var dialogResult = await dialog.Result;
            if ((dialogResult != null) && (!dialogResult.Canceled))
            {
                if (dialogResult.Data is string newName)
                {
                    ShowCompany.Name = newName;
                }
            }

        }
        if (sender is EditButtion btn)
        {
            btn.Processing = false;
        }
    }

    public async Task ShowEntityEdit(object sender)
    {
        if (sender is EditButtion btn)
        {
            var editItem = Items.Where(x => x.ID == btn.ItemId).FirstOrDefault();
            if (editItem != null)
            {
                var options = new DialogOptions
                {
                    CloseOnEscapeKey = false,
                    BackdropClick = false
                };
                var editForm = new Model.CreateEntity()
                {
                    EType = editItem.EntityType,
                    Name = editItem.Name,
                    UserName = userData.Name
                };
                var parameters = new DialogParameters<EntityCreateDialog>
                {
                    { x => x.Entity, editForm },
                    { x=> x.IsEdit, true }
                };
                var dialog = await DialogService.ShowAsync<EntityCreateDialog>("Редактирование элемента", parameters, options);
                var dialogResult = await dialog.Result;
                if ((dialogResult != null) && (httpClient.BaseAddress != null))
                {
                    if (dialogResult.Canceled)
                    {
                        var response = await httpClient.PatchAsJsonAsync<Model.EditRequest>($"{httpClient.BaseAddress.ToString()}api/canceledit", new Model.EditRequest()
                        {
                            EntityId = btn.ItemId,
                            UserName = userData.Name
                        });
                    }
                    else
                    {
                        if (dialogResult.Data is Model.CreateEntity editData)
                        {
                            var request = new ModifyRequest()
                            {
                                EntityId = btn.ItemId,
                                EType = editData.EType,
                                Name = editData.Name,
                                UserName = userData.Name
                            };
                            var response = await httpClient.PatchAsJsonAsync<Model.ModifyRequest>($"{httpClient.BaseAddress.ToString()}api/modify", request);
                            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                            {
                                if ((btn.ItemData != null) && (btn.ItemData.Text != null))
                                {
                                    var oldName = btn.ItemData.Text.Split(".");
                                    btn.ItemData.Text = $"{oldName.First()}.{request.Name}";
                                }
                                editItem.Name = editData.Name;
                                editItem.EntityType = editData.EType;
                            }
                        }
                    }
                }
            }
            btn.Processing = false;
        }
    }


    public async Task CreateChild(Guid? id)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = false,
            BackdropClick = false
        };

        var parameters = new DialogParameters<EntityCreateDialog>
                {
                    { x => x.Entity, new() { EType = EntityType.Department} },
                    { x=> x.IsEdit, false }
                };
        var dialog = await DialogService.ShowAsync<EntityCreateDialog>("Создание элемента", parameters, options);
        var dialogResult = await dialog.Result;
        if (httpClient.BaseAddress != null)
        {
            if ((dialogResult != null) && (!dialogResult.Canceled))
            {
                if (dialogResult.Data is Model.CreateEntity editData)
                {
                    var req = new CreateEntity()
                    {
                        EType = editData.EType,
                        Name = editData.Name,
                        Parent = id,
                        UserName = userData.Name
                    };
                    var result = await httpClient.PostAsJsonAsync<CreateEntity>($"{httpClient.BaseAddress.ToString()}api/create", req);
                    if (result.IsSuccessStatusCode)
                    {
                        Navigation.Refresh(true);
                    }
                }
            }
        }
    }

    public async Task Delete(Guid? id, string? name, string? navUrl = null)
    {
        if ((httpClient.BaseAddress != null) && (id.HasValue))
        {
            bool? result = await DialogService.ShowMessageBox(
                "Внимание",
                $"Вы точно хотите удалить {name}. Данное действие не обратимо",
                yesText: "Удалить", cancelText: "Отмена");
            if (result.GetValueOrDefault(false))
            {
                var response = await httpClient.DeleteAsync($"{httpClient.BaseAddress.ToString()}api/delete?id={id}&&user={userData.Name}");

                if (response.IsSuccessStatusCode)
                {
                    if (navUrl != null)
                    {
                        Navigation.NavigateTo(navUrl, true);
                    }
                    else
                    {
                        Navigation.Refresh(true);
                    }
                }
                else
                {
                    var respText = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(respText))
                    {
                        await DialogService.ShowMessageBox(
                        "Недоступно",
                        respText,
                        "ОК"
                        );
                    }
                }
            }
        }
    }

    private string GetTransfer(Guid id)
    {
        return $"event.dataTransfer.setData('{id}', event.target.id);";
    }

    private async Task OnDrop(DragEventArgs e, Guid? zone)
    {
        if (Guid.TryParse(e.DataTransfer.Types.FirstOrDefault(), out Guid itemId))
        {
            if((zone.HasValue)&&(zone!=itemId))
            {
                if (httpClient.BaseAddress!=null)
                {
                    var request = new MoveRequest()
                    {
                        EntityId = itemId,
                        ParentId = zone.Value,
                        UserName = userData.Name
                    };
                    var response = await httpClient.PatchAsJsonAsync<Model.MoveRequest>($"{httpClient.BaseAddress.ToString()}api/move", request);
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        Navigation.Refresh(true);
                    }
                }
            }
        }
    }
}
