namespace WSSC.Client.Model;

/// <summary>
/// Набор данных для создания новой записи
/// </summary>
public class CreateEntity
{
    /// <summary>
    /// Название объекта
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор родительского объекта
    /// </summary>
    public Guid? Parent { get; set; }

    /// <summary>
    /// Тип создаваемого объекта
    /// </summary>
    public EntityType EType { get; set; } = EntityType.Unknow;

    /// <summary>
    /// Имя юзера
    /// </summary>
    public string UserName { get; set; } = "";

}
