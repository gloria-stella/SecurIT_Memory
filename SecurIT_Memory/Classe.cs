using System;
using System.Drawing;

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
    /// Respecte le principe d'encapsulation via des propriétés get/set.
    /// </summary>
    public class Carte
    {
        // ── Champs privés ──────────────────────────────────────────────
        private int _idPaire;
        private string _nomIcone;
        private string _cheminImage;
        private EtatCarte _etat;

        // ── Propriétés publiques (encapsulation) ───────────────────────

        /// <summary>Identifiant de la paire. Deux cartes avec le même ID forment une paire valide.</summary>
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

        /// <summary>Chemin vers le fichier image de la face de la carte.</summary>
        public string CheminImage
        {
            get { return _cheminImage; }
            set { _cheminImage = value; }
        }

        /// <summary>État actuel de la carte (Cachee, Revelee ou Trouvee).</summary>
        public EtatCarte Etat
        {
            get { return _etat; }
            set { _etat = value; }
        }

        /// <summary>Indique si la carte est actuellement visible (révélée ou trouvée).</summary>
        public bool EstVisible
        {
            get { return _etat == EtatCarte.Revelee || _etat == EtatCarte.Trouvee; }
        }

        /// <summary>Indique si la carte fait partie d'une paire trouvée.</summary>
        public bool EstTrouvee
        {
            get { return _etat == EtatCarte.Trouvee; }
        }

        // ── Constructeur ───────────────────────────────────────────────

        /// <summary>
        /// Crée une nouvelle carte avec son identifiant de paire et son icône.
        /// </summary>
        /// <param name="idPaire">Identifiant partagé avec la carte jumelle.</param>
        /// <param name="nomIcone">Nom de l'icône cybersécurité.</param>
        /// <param name="cheminImage">Chemin vers l'image de la face.</param>
        public Carte(int idPaire, string nomIcone, string cheminImage)
        {
            _idPaire = idPaire;
            _nomIcone = nomIcone;
            _cheminImage = cheminImage;
            _etat = EtatCarte.Cachee; // Une carte commence toujours face cachée
        }

        // ── Méthodes publiques ─────────────────────────────────────────

        /// <summary>Révèle la carte (la retourne face visible temporairement).</summary>
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

        /// <summary>Remet la carte face cachée (utilisé quand une paire ne correspond pas).</summary>
        public void Cacher()
        {
            if (_etat == EtatCarte.Revelee)
                _etat = EtatCarte.Cachee;
        }

        /// <summary>Vérifie si cette carte forme une paire avec une autre carte.</summary>
        /// <param name="autreCarte">La carte à comparer.</param>
        /// <returns>True si les deux cartes ont le même IdPaire.</returns>
        public bool FormePaireAvec(Carte autreCarte)
        {
            if (autreCarte == null) return false;
            return this._idPaire == autreCarte._idPaire && this != autreCarte;
        }

        public override string ToString()
        {
            return $"Carte[ID={_idPaire}, Icone={_nomIcone}, Etat={_etat}]";
        }
    }
}