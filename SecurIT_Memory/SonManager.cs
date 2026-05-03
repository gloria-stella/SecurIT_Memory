using System;
using System.IO; // verifier si les fichiers existent
using System.Media;

namespace SecurIT_Memory
{
    public static class SonManager
    {
        private static readonly string DOSSIER = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Sounds");
        //construction chemin des chemins des son
        private static readonly string CHEMIN_CLIC = Path.Combine(DOSSIER, "clic.wav");
        private static readonly string CHEMIN_PAIRE = Path.Combine(DOSSIER, "paire.wav");
        private static readonly string CHEMIN_ERREUR = Path.Combine(DOSSIER, "erreur.wav");
        private static readonly string CHEMIN_VICTOIRE = Path.Combine(DOSSIER, "victoire.wav");

        private static SoundPlayer _playerClic; // Un SoundPlayer = un lecteur audio pour un fichier .wav
        private static SoundPlayer _playerPaire;
        private static SoundPlayer _playerErreur;
        private static SoundPlayer _playerVictoire;

        public static void Initialiser()
        {
            _playerClic = ChargerSon(CHEMIN_CLIC);
            _playerPaire = ChargerSon(CHEMIN_PAIRE);
            _playerErreur = ChargerSon(CHEMIN_ERREUR);
            _playerVictoire = ChargerSon(CHEMIN_VICTOIRE);
        }

        private static SoundPlayer ChargerSon(string chemin)
        {
            try
            {
                if (File.Exists(chemin))
                {
                    var player = new SoundPlayer(chemin);
                    player.Load();
                    return player;
                }
            }
            catch { }
            return null;
        }

        public static void JouerClic() => JouerSon(_playerClic);
        public static void JouerPaire() => JouerSon(_playerPaire);
        public static void JouerErreur() => JouerSon(_playerErreur);
        public static void JouerVictoire() => JouerSon(_playerVictoire);

        private static void JouerSon(SoundPlayer player) 
        {
            try { player?.Play(); } // try/catch aucune erreur ne peu faire planter le jeu
            catch { }
        }
    }
}