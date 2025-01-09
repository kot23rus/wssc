namespace WSSC.Client.Model;

/// <summary>
/// Элемент списка компаний
/// </summary>
public class Company
{
    /// <summary>
    /// Список доступных компаний
    /// </summary>
    public Guid ID { get; set; } = Guid.Empty;
    /// <summary>
    /// Имя компании
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
