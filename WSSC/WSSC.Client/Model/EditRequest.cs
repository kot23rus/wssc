namespace WSSC.Client.Model;

/// <summary>
/// Класс данных для запроса редактирования
/// элемента
/// </summary>
public class EditRequest
{
    /// <summary>
    /// Имя пользователя 
    /// запросившего изменение
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор объекта для изменения
    /// </summary>
    public Guid EntityId { get; set; } = Guid.Empty;
}
