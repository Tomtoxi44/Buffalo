using CommunityToolkit.Mvvm.ComponentModel;

namespace BuffaloApp.ViewModels;

/// <summary>
/// ViewModel pour la page des règles du Buffalo
/// </summary>
public partial class RulesViewModel : ObservableObject
{
    public List<RuleItem> Rules { get; } = new()
    {
        new RuleItem
        {
            Number = 1,
            Title = "La Règle d'Or",
            Description = "Tu dois TOUJOURS boire avec ta main NON-DOMINANTE.\n\n" +
                          "• Si tu es droitier → tu bois avec la main gauche\n" +
                          "• Si tu es gaucher → tu bois avec la main droite",
            Icon = "🍺"
        },
        new RuleItem
        {
            Number = 2,
            Title = "Le Cri du Buffalo",
            Description = "Si tu surprends quelqu'un en train de boire avec sa main dominante, " +
                          "crie \"BUFFALO !\" haut et fort !\n\n" +
                          "Le fautif doit alors finir son verre CUL-SEC.",
            Icon = "🦬"
        },
        new RuleItem
        {
            Number = 3,
            Title = "C'est pour la vie !",
            Description = "Une fois que tu as accepté de jouer au Buffalo, tu joues À VIE.\n\n" +
                          "Il n'y a pas de pause, pas de vacances, pas de retraite. " +
                          "Le Buffalo ne dort jamais !",
            Icon = "♾️"
        },
        new RuleItem
        {
            Number = 4,
            Title = "L'Ardoise",
            Description = "Tu peux REFUSER un Buffalo si tu ne veux pas/peux pas boire cul-sec.\n\n" +
                          "MAIS : la personne qui t'a donné le Buffalo peut te le redonner " +
                          "À TOUT MOMENT plus tard. L'ardoise ne s'efface jamais !",
            Icon = "📝"
        },
        new RuleItem
        {
            Number = 5,
            Title = "Le Faux Buffalo",
            Description = "Si tu cries Buffalo alors que la personne buvait correctement " +
                          "(avec sa main non-dominante), c'est TOI qui dois boire cul-sec !\n\n" +
                          "⚠️ Vérifie bien avant de crier !",
            Icon = "❌"
        },
        new RuleItem
        {
            Number = 6,
            Title = "Les Exceptions",
            Description = "Le Buffalo ne s'applique PAS dans ces situations :\n\n" +
                          "• Quand tu trinques (on lève le verre avec n'importe quelle main)\n" +
                          "• Quand tu passes le verre à quelqu'un\n" +
                          "• Quand tu portes quelque chose dans l'autre main\n" +
                          "• Pour les boissons chaudes (café, thé) - optionnel selon les groupes",
            Icon = "✋"
        },
        new RuleItem
        {
            Number = 7,
            Title = "L'Honneur du Buffalo",
            Description = "Le jeu repose sur l'HONNEUR et la CONFIANCE.\n\n" +
                          "• Accepte tes Buffalos avec le sourire\n" +
                          "• Ne triche pas sur ta main dominante\n" +
                          "• Respecte les ardoises\n" +
                          "• Bois responsablement !",
            Icon = "🤝"
        },
        new RuleItem
        {
            Number = 8,
            Title = "Règles de l'App",
            Description = "Cette application te permet de :\n\n" +
                          "• Détecter les autres joueurs Buffalo à proximité via Bluetooth\n" +
                          "• Envoyer des Buffalos numériques\n" +
                          "• Gérer ton ardoise\n" +
                          "• Voir le classement des meilleurs donneurs de Buffalo\n\n" +
                          "Active le mode Buffalo quand tu es au bar !",
            Icon = "📱"
        }
    };

    public string ImportantNote => "⚠️ RAPPEL IMPORTANT ⚠️\n\n" +
        "Le Buffalo est un jeu FUN qui doit rester RESPONSABLE.\n\n" +
        "• Ne force jamais quelqu'un à boire\n" +
        "• Respecte les limites de chacun\n" +
        "• L'alcool est à consommer avec modération\n" +
        "• Tu peux toujours refuser un Buffalo (il ira sur l'ardoise)";
}

/// <summary>
/// Représente une règle du jeu
/// </summary>
public class RuleItem
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
