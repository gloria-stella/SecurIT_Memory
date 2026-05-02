using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    /// <summary>
    /// Résultats possibles d'un clic sur une carte.
    /// </summary>
    public enum ResultatClic
    {
        Ignore,        // Clic ignoré (carte déjà trouvée, révélée ou clics bloqués)
        PremiereCarte, // Première carte retournée, en attente de la seconde
        PaireTrouvee,  // Les deux cartes forment une paire valide
        NonPaire,      // Les deux cartes ne correspondent pas
        Victoire       // Toutes les paires ont été trouvées
    }

    /// <summary>
    /// Gestionnaire principal du jeu Memory SecurIT.
    /// Orchestre la liste des cartes, la logique de vérification,
    /// le mélange aléatoire, les timers et le mode Hardcore.
    /// </summary>
    public class JeuMemory
    {
        // ── Champs privés ──────────────────────────────────────────────
        private List<Carte> _cartes;
        private Carte _premiereCarteSelectionnee;
        private Carte _deuxiemeCarteSelectionnee;
        private int _nombreEssais;
        private int _nombrePairesTrouvees;
        private int _tailleGrille;
        private bool _partieEnCours;
        private bool _modeHardcore;
        private ThemeCartes _themeActuel;
        private Random _rng;

        // ── Timers ─────────────────────────────────────────────────────
        /// <summary>
        /// Timer de délai : attend 1,2s avant de retourner les cartes non-paires.
        /// PIÈGE : bloque tous les clics pendant ce délai (voir TP).
        /// </summary>
        private Timer _timerDelaiRetournement;

        /// <summary>
        /// Timer Hardcore : toutes les 30s, échange aléatoirement
        /// les positions des cartes non-trouvées.
        /// </summary>
        private Timer _timerHardcore;

        // ── Constantes ─────────────────────────────────────────────────
        private const int DELAI_RETOURNEMENT_MS = 1200; // 1,2 secondes
        private const int DELAI_HARDCORE_MS = 30000; // 30 secondes

        // ── Propriétés publiques ───────────────────────────────────────

        /// <summary>Liste de toutes les cartes de la partie (ordre mélangé).</summary>
        public List<Carte> Cartes
        {
            get { return _cartes; }
        }

        /// <summary>Nombre de tentatives effectuées.</summary>
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

        /// <summary>Taille de la grille (4 pour 4x4, 6 pour 6x6).</summary>
        public int TailleGrille
        {
            get { return _tailleGrille; }
        }

        /// <summary>True si le mode Hardcore est activé.</summary>
        public bool ModeHardcore
        {
            get { return _modeHardcore; }
        }

        /// <summary>True si toutes les paires ont été trouvées.</summary>
        public bool EstTerminee
        {
            get { return _nombrePairesTrouvees >= NombrePairesTotal; }
        }

        /// <summary>True si une première carte est retournée et attend la seconde.</summary>
        public bool AttenteDeuxiemeCarte
        {
            get { return _premiereCarteSelectionnee != null && _deuxiemeCarteSelectionnee == null; }
        }

        // ── Événements ─────────────────────────────────────────────────

        /// <summary>Déclenché quand les deux cartes non-paires doivent être retournées.</summary>
        public event EventHandler DemandeRetournement;

        /// <summary>Déclenché en mode Hardcore quand les cartes sont repositionnées.</summary>
        public event EventHandler HardcoreReposition;

        // ── Constructeur ───────────────────────────────────────────────

        public JeuMemory()
        {
            _rng = new Random();
            _cartes = new List<Carte>();
            InitialiserTimers();
        }

        // ── Initialisation des timers ──────────────────────────────────

        private void InitialiserTimers()
        {
            // Timer délai de retournement (one-shot)
            _timerDelaiRetournement = new Timer();
            _timerDelaiRetournement.Interval = DELAI_RETOURNEMENT_MS;
            _timerDelaiRetournement.Tick += TimerDelai_Tick;

            // Timer Hardcore (répétitif toutes les 30s)
            _timerHardcore = new Timer();
            _timerHardcore.Interval = DELAI_HARDCORE_MS;
            _timerHardcore.Tick += TimerHardcore_Tick;
        }

        // ── Méthodes publiques ─────────────────────────────────────────

        /// <summary>
        /// Initialise et démarre une nouvelle partie.
        /// </summary>
        /// <param name="tailleGrille">4 (4x4 = 8 paires) ou 6 (6x6 = 18 paires).</param>
        /// <param name="theme">Thème visuel des cartes.</param>
        /// <param name="modeHardcore">Active le mode Hardcore (échange toutes les 30s).</param>
        public void NouvellePartie(int tailleGrille = 4,
                                   ThemeCartes theme = ThemeCartes.Cybersecurite,
                                   bool modeHardcore = false)
        {
            // Arrêter les timers en cours
            _timerDelaiRetournement.Stop();
            _timerHardcore.Stop();

            // Réinitialiser l'état
            _tailleGrille = tailleGrille;
            _themeActuel = theme;
            _modeHardcore = modeHardcore;
            _nombreEssais = 0;
            _nombrePairesTrouvees = 0;
            _premiereCarteSelectionnee = null;
            _deuxiemeCarteSelectionnee = null;
            _partieEnCours = true;

            // Générer les cartes et les mélanger
            _cartes = GenererCartes(tailleGrille, theme);
            MelangerCartes(_cartes);

            // Démarrer le timer Hardcore si activé
            if (_modeHardcore)
                _timerHardcore.Start();
        }

        /// <summary>
        /// Traite le clic du joueur sur une carte.
        /// </summary>
        /// <param name="carte">La carte cliquée.</param>
        /// <returns>Le résultat de l'action.</returns>
        public ResultatClic TraiterClic(Carte carte)
        {
            // Ignorer si carte déjà trouvée ou déjà révélée
            if (carte.EstTrouvee) return ResultatClic.Ignore;
            if (carte.Etat == EtatCarte.Revelee) return ResultatClic.Ignore;

            carte.Reveler();

            // ── Première carte sélectionnée ────────────────────────────
            if (_premiereCarteSelectionnee == null)
            {
                _premiereCarteSelectionnee = carte;
                return ResultatClic.PremiereCarte;
            }

            // ── Deuxième carte sélectionnée ────────────────────────────
            _deuxiemeCarteSelectionnee = carte;
            _nombreEssais++;

            // Vérification : même IdPaire ?
            if (_premiereCarteSelectionnee.FormePaireAvec(_deuxiemeCarteSelectionnee))
            {
                // ✅ Paire trouvée
                _premiereCarteSelectionnee.MarquerTrouvee();
                _deuxiemeCarteSelectionnee.MarquerTrouvee();
                _nombrePairesTrouvees++;
                ResetSelection();

                return EstTerminee ? ResultatClic.Victoire : ResultatClic.PaireTrouvee;
            }
            else
            {
                // ❌ Pas une paire → déclencher le timer de délai
                // IMPORTANT : les clics doivent être bloqués dans FormJeu pendant ce délai !
                _timerDelaiRetournement.Start();
                return ResultatClic.NonPaire;
            }
        }

        /// <summary>
        /// Remet les deux cartes non-correspondantes face cachée.
        /// Appelé automatiquement par le timer de délai.
        /// </summary>
        public void RetournerCartesNonPaire()
        {
            _premiereCarteSelectionnee?.Cacher();
            _deuxiemeCarteSelectionnee?.Cacher();
            ResetSelection();
        }

        /// <summary>Arrête tous les timers (appeler à la fermeture du formulaire).</summary>
        public void Arreter()
        {
            _timerDelaiRetournement.Stop();
            _timerHardcore.Stop();
            _partieEnCours = false;
        }

        // ── Timers (événements internes) ───────────────────────────────

        /// <summary>
        /// Tick du timer de délai : retourne les cartes et notifie le formulaire.
        /// </summary>
        private void TimerDelai_Tick(object sender, EventArgs e)
        {
            _timerDelaiRetournement.Stop(); // One-shot
            RetournerCartesNonPaire();
            DemandeRetournement?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tick du timer Hardcore (toutes les 30s) :
        /// échange aléatoirement les positions des cartes non-trouvées.
        /// </summary>
        private void TimerHardcore_Tick(object sender, EventArgs e)
        {
            if (!_partieEnCours) return;

            // Récupérer les indices des cartes non-trouvées
            List<int> indicesNonTrouves = new List<int>();
            for (int i = 0; i < _cartes.Count; i++)
                if (!_cartes[i].EstTrouvee)
                    indicesNonTrouves.Add(i);

            // Mélanger ces indices (Fisher-Yates sur la sous-liste)
            for (int i = indicesNonTrouves.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(0, i + 1);
                int tmp = indicesNonTrouves[i];
                indicesNonTrouves[i] = indicesNonTrouves[j];
                indicesNonTrouves[j] = tmp;
            }

            // Extraire les cartes non-trouvées dans l'ordre mélangé
            List<Carte> cartesNonTrouvees = new List<Carte>();
            foreach (int idx in indicesNonTrouves)
                cartesNonTrouvees.Add(_cartes[idx]);

            // Remettre face cachée les cartes révélées
            foreach (Carte c in cartesNonTrouvees)
                c.Reset();

            // Replacer les cartes mélangées aux mêmes indices
            for (int i = 0; i < indicesNonTrouves.Count; i++)
                _cartes[indicesNonTrouves[i]] = cartesNonTrouvees[i];

            // Réinitialiser la sélection
            _premiereCarteSelectionnee = null;
            _deuxiemeCarteSelectionnee = null;

            // Notifier le formulaire pour rafraîchir l'affichage
            HardcoreReposition?.Invoke(this, EventArgs.Empty);
        }

        // ── Méthodes privées ───────────────────────────────────────────

        /// <summary>Génère la liste de cartes en paires pour le thème et la taille donnés.</summary>
        private List<Carte> GenererCartes(int tailleGrille, ThemeCartes theme)
        {
            int nombrePaires = (tailleGrille * tailleGrille) / 2;
            IconeCarte[] icones = CatalogueIcones.ParTheme(theme);
            var liste = new List<Carte>();

            for (int i = 0; i < nombrePaires; i++)
            {
                IconeCarte icone = icones[i % icones.Length];
                // Créer les deux cartes de la paire (même IdPaire)
                liste.Add(new Carte(i, icone.Nom, icone.Emoji, icone.RedTeam));
                liste.Add(new Carte(i, icone.Nom, icone.Emoji, icone.RedTeam));
            }

            return liste;
        }

        /// <summary>Mélange une liste de cartes avec l'algorithme Fisher-Yates.</summary>
        private void MelangerCartes(List<Carte> cartes)
        {
            int n = cartes.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = _rng.Next(0, i + 1);
                Carte tmp = cartes[i];
                cartes[i] = cartes[j];
                cartes[j] = tmp;
            }
        }

        /// <summary>Remet à zéro la sélection courante.</summary>
        private void ResetSelection()
        {
            _premiereCarteSelectionnee = null;
            _deuxiemeCarteSelectionnee = null;
        }
    }
}