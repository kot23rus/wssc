namespace WSSC.Client.Model;

/// <summary>
/// Запрос на изменение данных в БД
/// </summary>
public class ModifyRequest : EditRequest
{
    /// <summary>
    /// Новое название
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Тип сущности
    /// </summary>
    public EntityType EType { get; set; } = EntityType.Unknow;
}
