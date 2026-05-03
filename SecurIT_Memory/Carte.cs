using System;

namespace SecurIT_Memory
{
    /// <summary>
    /// Enumération représentant les trois états possibles d'une carte du jeu Memory.
    /// </summary>
    public enum EtatCarte
    {
        Cachee,   // Face verso, image cachée
        Revelee,  // Temporairement visible (en attente de vérification)
        Trouvee   // Paire identifiée, reste visible définitivement
    }

    /// <summary>
    /// Classe représentant une carte du jeu Memory SecurIT.
    /// Respecte le principe d'encapsulation POO via des propriétés get/private set.
    /// </summary>
    public class Carte
    {
        // Champs privés   (pour respecter lencapsulation , on ne peut les modifier que a travers des methodes 
        private int _idPaire;
        private string _nomIcone;
        private string _cheminImage;  // Emoji
        private EtatCarte _etat;
        private bool _estRedTeam;

        // Propriétés publiques (encapsulation) 

        /// <summary>Identifiant de paire. Deux cartes avec le même ID forment une paire.</summary>
        public int IdPaire
        {
            get { return _idPaire; }
            private set { _idPaire = value; }
        }

        /// <summary>Nom de l'icône cybersécurité affichée sur la carte.</summary>
        public string NomIcone
        {
            get { return _nomIcone; }
            private set { _nomIcone = value; }
        }

        /// <summary>Emoji ou chemin vers l'image affichée sur la face de la carte.</summary>
        public string CheminImage
        {
            get { return _cheminImage; }
            private set { _cheminImage = value; }
        }

        /// <summary>État actuel de la carte (Cachee, Revelee ou Trouvee).</summary>
        public EtatCarte Etat
        {
            get { return _etat; }
            private set { _etat = value; }
        }

        /// <summary>Indique si la carte appartient à la Red Team (menace).</summary>
        public bool EstRedTeam
        {
            get { return _estRedTeam; }
            private set { _estRedTeam = value; }
        }

        /// <summary>True si la carte est actuellement visible (révélée ou trouvée).</summary>
        public bool EstVisible
        {
            get { return _etat == EtatCarte.Revelee || _etat == EtatCarte.Trouvee; }
        }

        /// <summary>True si la carte fait partie d'une paire trouvée.</summary>
        public bool EstTrouvee
        {
            get { return _etat == EtatCarte.Trouvee; }
        }

        // Constructeur

        /// <summary>Crée une nouvelle carte avec toutes ses informations </summary>
        /// <param name="idPaire">Identifiant partagé avec la carte jumelle </param>
        /// <param name="nomIcone">Nom affiché sur la face de la carte</param>
        /// <param name="cheminImage">Emoji </param>
        /// <param name="estRedTeam">True si la carte représente une menace Red Team.</param>
        public Carte(int idPaire, string nomIcone, string cheminImage, bool estRedTeam = false)
        {
            _idPaire = idPaire;
            _nomIcone = nomIcone;
            _cheminImage = cheminImage;
            _estRedTeam = estRedTeam;
            _etat = EtatCarte.Cachee;

            // ligne de debug ( pour moi , voir ce qui ne va pas dans la création des cartes )
            System.Diagnostics.Debug.WriteLine($"[CARTE CRÉÉE] {nomIcone} | RedTeam: {estRedTeam}");
        }

        // ─ Méthodes publiques 

        /// <summary>Révèle la carte (retournement face visible temporaire).</summary>
        public void Reveler()
        {
            if (_etat == EtatCarte.Cachee)
                _etat = EtatCarte.Revelee;
        }

        /// <summary>Marque la carte comme faisant partie d'une paire trouvée.</summary>
        public void MarquerTrouvee()
        {
            _etat = EtatCarte.Trouvee;
        }

        /// <summary>Remet la carte face cachée (après échec de correspondance).</summary>
        public void Cacher()
        {
            if (_etat == EtatCarte.Revelee)
                _etat = EtatCarte.Cachee;
        }

        /// <summary>
        /// Remet la carte à son état initial (Cachée).
        /// Utilisé lors du démarrage d'une nouvelle partie.
        /// </summary>
        public void Reset()
        {
            _etat = EtatCarte.Cachee;
        }

        /// <summary>Vérifie si cette carte forme une paire avec une autre carte.</summary>
        /// <param name="autreCarte">La carte à comparer.</param>
        /// <returns>True si même IdPaire et cartes différentes.</returns>
        public bool FormePaireAvec(Carte autreCarte)
        {
            if (autreCarte == null) return false;
            return this._idPaire == autreCarte._idPaire && this != autreCarte;
        }

    }
}