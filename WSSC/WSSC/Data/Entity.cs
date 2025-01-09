using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSSC.Data;

/// <summary>
/// Объект данных который хранится в таблице
/// </summary>
[Table("Companies")]
[Index(nameof(Id), IsUnique = true)]
[Index(nameof(ParentId), IsUnique = false)]
[Index(nameof(ExtUser), IsUnique = false)]
public class Entity
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор родительского элемента
    /// </summary>
    [Column("PARENT")]
    [ForeignKey(nameof(Id))]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Имя объекта
    /// </summary>
    [Column("NAME")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Тип элемента данных
    /// </summary>
    [Column("EType")]
    public Client.Model.EntityType EntityType { get; set; } = Client.Model.EntityType.Unknow;

    /// <summary>
    /// Дата и время изменения
    /// </summary>
    [Column("MODIFIED")]
    public DateTime Modified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Идентификатор пользователя
    /// который изменяет элемент
    /// </summary>
    [Column("EXT_USER")]
    public string? ExtUser { get; set; }
}
