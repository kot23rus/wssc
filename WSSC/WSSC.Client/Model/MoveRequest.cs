namespace WSSC.Client.Model;

/// <summary>
/// Запрос на перемещение
/// объекта
/// </summary>
public class MoveRequest : EditRequest
{
    /// <summary>
    /// В какой элемент перенести ответ
    /// </summary>
    public Guid ParentId { get; set; } = Guid.Empty;

}
