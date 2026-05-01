using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurIT_Memory
{
    /// <summary>
    /// Gestionnaire principal du jeu Memory SecurIT.
    /// Orchestre les cartes, le score, les tentatives et la logique de vérification.
    /// </summary>
    public class JeuMemory
    {
        // ── Données des icônes cybersécurité disponibles ──────────────
        // Format : (nomIcone, cheminImage)
        private static readonly (string Nom, string Icone)[] ICONES_CYBERS = new[]
        {
            ("Virus",       "🦠"),
            ("Cadenas",     "🔒"),
            ("Pare-feu",    "🛡️"),
            ("Mot de passe","🔑"),
            ("Hacker",      "👾"),
            ("Chiffrement", "🔐"),
            ("Phishing",    "🎣"),
            ("Ransomware",  "💀"),
            ("VPN",         "🌐"),
            ("Antivirus",   "🛡"),
            ("Bug",         "🐛"),
            ("Authentif.",  "📱"),
            ("Certificat",  "📜"),
            ("Backup",      "💾"),
            ("Botnet",      "🤖"),
            ("DDOS",        "⚡"),
            ("Firewall",    "🔥"),
            ("Trojan",      "🐴"),
        };

        // ── Champs privés ──────────────────────────────────────────────
        private List<Carte> _cartes;
        private Carte _premiereCarteSelectionnee;
        private Carte _deuxiemeCarteSelectionnee;
        private int _nombreEssais;
        private int _nombrePairesTrouvees;
        private int _tailleGrille;   // 4 pour 4x4, 6 pour 6x6
        private bool _partieEnCours;
        private Random _rng;

        // ── Propriétés publiques ───────────────────────────────────────

        /// <summary>Liste de toutes les cartes du jeu (mélangées).</summary>
        public List<Carte> Cartes
        {
            get { return _cartes; }
        }

        /// <summary>Nombre de tentatives effectuées par le joueur.</summary>
        public int NombreEssais
        {
            get { return _nombreEssais; }
        }

        /// <summary>Nombre de paires trouvées.</summary>
        public int NombrePairesTrouvees
        {
            get { return _nombrePairesTrouvees; }
        }

        /// <summary>Nombre total de paires dans la partie.</summary>
        public int NombrePairesTotal
        {
            get { return (_tailleGrille * _tailleGrille) / 2; }
        }

        /// <summary>Taille de la grille (4 ou 6).</summary>
        public int TailleGrille
        {
            get { return _tailleGrille; }
        }

        /// <summary>Indique si une partie est en cours.</summary>
        public bool PartieEnCours
        {
            get { return _partieEnCours; }
        }

        /// <summary>Indique si le joueur attend un second clic (une carte déjà retournée).</summary>
        public bool AttenteDeuxiemeCarte
        {
            get { return _premiereCarteSelectionnee != null && _deuxiemeCarteSelectionnee == null; }
        }

        /// <summary>Indique si la partie est terminée (toutes les paires trouvées).</summary>
        public bool EstTerminee
        {
            get { return _nombrePairesTrouvees >= NombrePairesTotal; }
        }

        // ── Constructeur ───────────────────────────────────────────────

        public JeuMemory()
        {
            _rng = new Random();
            _cartes = new List<Carte>();
            _tailleGrille = 4; // Grille par défaut 4x4
        }

        // ── Méthodes publiques ─────────────────────────────────────────

        /// <summary>
        /// Initialise et démarre une nouvelle partie.
        /// </summary>
        /// <param name="tailleGrille">Taille de la grille : 4 (4x4=8 paires) ou 6 (6x6=18 paires).</param>
        public void NouvellePartie(int tailleGrille = 4)
        {
            _tailleGrille = tailleGrille;
            _nombreEssais = 0;
            _nombrePairesTrouvees = 0;
            _premiereCarteSelectionnee = null;
            _deuxiemeCarteSelectionnee = null;
            _partieEnCours = true;

            // Générer et mélanger les cartes
            _cartes = GenererCartes(tailleGrille);
            MelangerCartes();
        }

        /// <summary>
        /// Traite le clic du joueur sur une carte.
        /// </summary>
        /// <param name="carte">La carte cliquée.</param>
        /// <returns>Le résultat de l'action (aucune action / première carte / paire / non-paire).</returns>
        public ResultatClic TraiterClic(Carte carte)
        {
            // Ignorer si la carte est déjà trouvée ou déjà révélée
            if (carte.EstTrouvee) return ResultatClic.Ignore;
            if (carte.Etat == EtatCarte.Revelee) return ResultatClic.Ignore;

            carte.Reveler();

            // Première carte sélectionnée
            if (_premiereCarteSelectionnee == null)
            {
                _premiereCarteSelectionnee = carte;
                return ResultatClic.PremiereCarte;
            }

            // Deuxième carte sélectionnée
            _deuxiemeCarteSelectionnee = carte;
            _nombreEssais++;

            // Vérifier si c'est une paire
            if (_premiereCarteSelectionnee.FormePaireAvec(_deuxiemeCarteSelectionnee))
            {
                // Paire trouvée !
                _premiereCarteSelectionnee.MarquerTrouvee();
                _deuxiemeCarteSelectionnee.MarquerTrouvee();
                _nombrePairesTrouvees++;
                ResetSelection();

                return EstTerminee ? ResultatClic.Victoire : ResultatClic.PaireTrouvee;
            }

            // Pas une paire → le Timer de délai doit se déclencher dans le formulaire
            return ResultatClic.NonPaire;
        }

        /// <summary>
        /// Retourne les deux cartes non-correspondantes face cachée.
        /// Appelé après le délai du Timer.
        /// </summary>
        public void RetournerCartesNonPaire()
        {
            if (_premiereCarteSelectionnee != null)
                _premiereCarteSelectionnee.Cacher();
            if (_deuxiemeCarteSelectionnee != null)
                _deuxiemeCarteSelectionnee.Cacher();

            ResetSelection();
        }

        /// <summary>Réinitialise la sélection courante.</summary>
        private void ResetSelection()
        {
            _premiereCarteSelectionnee = null;
            _deuxiemeCarteSelectionnee = null;
        }

        // ── Méthodes privées ───────────────────────────────────────────

        /// <summary>
        /// Génère la liste de cartes en paires pour la grille donnée.
        /// </summary>
        private List<Carte> GenererCartes(int tailleGrille)
        {
            int nombrePaires = (tailleGrille * tailleGrille) / 2;
            var nouvelles = new List<Carte>();

            for (int i = 0; i < nombrePaires; i++)
            {
                var icone = ICONES_CYBERS[i % ICONES_CYBERS.Length];
                // Créer deux cartes identiques (même IdPaire)
                nouvelles.Add(new Carte(i, icone.Nom, icone.Icone));
                nouvelles.Add(new Carte(i, icone.Nom, icone.Icone));
            }

            return nouvelles;
        }

        /// <summary>
        /// Mélange la liste de cartes avec l'algorithme Fisher-Yates.
        /// </summary>
        private void MelangerCartes()
        {
            int n = _cartes.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = _rng.Next(0, i + 1);
                var temp = _cartes[i];
                _cartes[i] = _cartes[j];
                _cartes[j] = temp;
            }
        }
    }

    /// <summary>
    /// Résultats possibles d'un clic sur une carte.
    /// </summary>
    public enum ResultatClic
    {
        Ignore,        // Clic ignoré (carte déjà trouvée ou révélée)
        PremiereCarte, // Première carte retournée, en attente de la seconde
        PaireTrouvee,  // Les deux cartes forment une paire
        NonPaire,      // Les deux cartes ne correspondent pas (déclencher Timer)
        Victoire       // Toutes les paires ont été trouvées
    }
}