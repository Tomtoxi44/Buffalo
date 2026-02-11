using SQLite;

namespace BuffaloApp.Models;

/// <summary>
/// Représente un événement Buffalo (quand quelqu'un crie BUFFALO!)
/// </summary>
public class BuffaloEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// ID du joueur qui a donné le Buffalo
    /// </summary>
    public int GiverId { get; set; }

    /// <summary>
    /// ID du joueur qui a reçu le Buffalo
    /// </summary>
    public int ReceiverId { get; set; }

    /// <summary>
    /// Date et heure de l'événement
    /// </summary>
    public DateTime EventDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Statut de l'événement
    /// </summary>
    public BuffaloStatus Status { get; set; } = BuffaloStatus.Pending;

    /// <summary>
    /// Lieu (optionnel - nom du bar)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Commentaire optionnel
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Statut d'un Buffalo
/// </summary>
public enum BuffaloStatus
{
    /// <summary>
    /// En attente de réponse
    /// </summary>
    Pending,

    /// <summary>
    /// Accepté - le joueur a bu cul-sec
    /// </summary>
    Accepted,

    /// <summary>
    /// Refusé - mis sur l'ardoise
    /// </summary>
    OnSlate,

    /// <summary>
    /// Ardoise réglée - Buffalo rendu plus tard
    /// </summary>
    SlateSettled
}
