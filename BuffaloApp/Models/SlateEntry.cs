using SQLite;

namespace BuffaloApp.Models;

/// <summary>
/// Représente une entrée sur l'ardoise (Buffalo refusé qui peut être réclamé plus tard)
/// </summary>
public class SlateEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// ID de l'événement Buffalo original
    /// </summary>
    public int OriginalBuffaloEventId { get; set; }

    /// <summary>
    /// ID du joueur qui a le droit de réclamer le Buffalo (celui qui l'a donné)
    /// </summary>
    public int CreditorPlayerId { get; set; }

    /// <summary>
    /// ID du joueur qui doit le Buffalo (celui qui a refusé)
    /// </summary>
    public int DebtorPlayerId { get; set; }

    /// <summary>
    /// Date de création de l'ardoise
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Date à laquelle l'ardoise a été réglée (null si pas encore réglée)
    /// </summary>
    public DateTime? SettledDate { get; set; }

    /// <summary>
    /// Indique si l'ardoise est réglée
    /// </summary>
    public bool IsSettled { get; set; }

    /// <summary>
    /// Lieu où l'ardoise a été créée
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Note/commentaire
    /// </summary>
    public string? Note { get; set; }
}
