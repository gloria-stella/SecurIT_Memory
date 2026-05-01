using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    /// <summary>
    /// Gestionnaire centralisé des sons du jeu SecurIT Memory.
    /// Les fichiers .wav doivent être dans le dossier Sounds/ à côté de l'exe.
    /// Si un fichier est manquant, le jeu continue sans planter.
    /// </summary>
    public static class SonManager
    {
        // ── Chemins des fichiers sons ──────────────────────────────────
        private static readonly string DOSSIER = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Sounds");

        private static readonly string CHEMIN_CLIC = Path.Combine(DOSSIER, "clic.wav");
        private static readonly string CHEMIN_PAIRE = Path.Combine(DOSSIER, "paire.wav");
        private static readonly string CHEMIN_ERREUR = Path.Combine(DOSSIER, "erreur.wav");
        private static readonly string CHEMIN_VICTOIRE = Path.Combine(DOSSIER, "victoire.wav");

        // ── Joueurs ────────────────────────────────────────────────────
        private static SoundPlayer _playerClic;
        private static SoundPlayer _playerPaire;
        private static SoundPlayer _playerErreur;
        private static SoundPlayer _playerVictoire;

        // ── Initialisation (à appeler une seule fois au démarrage) ─────
        public static void Initialiser()
        {
            _playerClic = ChargerSon(CHEMIN_CLIC);
            _playerPaire = ChargerSon(CHEMIN_PAIRE);
            _playerErreur = ChargerSon(CHEMIN_ERREUR);
            _playerVictoire = ChargerSon(CHEMIN_VICTOIRE);
        }

        /// <summary>Charge un fichier .wav sans planter si absent.</summary>
        private static SoundPlayer ChargerSon(string chemin)
        {
            try
            {
                if (File.Exists(chemin))
                {
                    var player = new SoundPlayer(chemin);
                    player.Load(); // Précharge en mémoire
                    return player;
                }
            }
            catch { }
            return null; // Fichier absent = pas de son, pas de bug
        }

        // ── Méthodes publiques ─────────────────────────────────────────

        /// <summary>Son quand le joueur clique sur une carte.</summary>
        public static void JouerClic() => JouerSon(_playerClic);

        /// <summary>Son quand une paire est trouvée.</summary>
        public static void JouerPaire() => JouerSon(_playerPaire);

        /// <summary>Son quand les deux cartes ne correspondent pas.</summary>
        public static void JouerErreur() => JouerSon(_playerErreur);

        /// <summary>Son de victoire quand toutes les paires sont trouvées.</summary>
        public static void JouerVictoire() => JouerSon(_playerVictoire);

        /// <summary>Joue un son de façon sécurisée (ne plante jamais).</summary>
        private static void JouerSon(SoundPlayer player)
        {
            try { player?.Play(); }
            catch { }
        }
    }
}