namespace WSSC.Client.Model;

/// <summary>
/// Дочерний элемент компании
/// </summary>
public class ChildEntity
{
    /// <summary>
    /// Идентификатор элемента
    /// </summary>
    public Guid ID { get; set; } = Guid.Empty;

    /// <summary>
    /// Название элемента
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Содержит ли дочерние элементы
    /// </summary>
    public bool HasChild { get; set; } = false;

    public EntityType EntityType { get; set; } = EntityType.Unknow;

}
