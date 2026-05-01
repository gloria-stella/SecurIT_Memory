using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace SecurIT_Memory
{
    public class FormJeu : Form
    {
        // ── Constantes ─────────────────────────────────────────────────
        private const int TAILLE_CARTE = 90;
        private const int ESPACEMENT = 8;
        private const int MARGE = 16;
        private const int HAUTEUR_HUD = 72;
        private const int DELAI_MS = 1200;

        // ── Couleurs Cyber Neon ────────────────────────────────────────
        private static readonly Color NOIR = FormMenu.NOIR;
        private static readonly Color VERT_NEON = FormMenu.VERT_NEON;
        private static readonly Color BLEU_NEON = FormMenu.BLEU_NEON;
        private static readonly Color ROUGE_NEON = FormMenu.ROUGE_NEON;

        // ── Icônes Blue Team / Red Team ────────────────────────────────
        private static readonly (string Emoji, string Nom, bool RedTeam)[] ICONES =
        {
            ("🛡",  "Pare-feu",    false),
            ("🔐", "Chiffrement",  false),
            ("🔒", "Cadenas",      false),
            ("🌐", "VPN",          false),
            ("🔑", "Cle USB",      false),
            ("📱", "MFA",          false),
            ("🎣", "Phishing",     true),
            ("🦠", "Virus",        true),
            ("👾", "Hacker",       true),
            ("💀", "Ransomware",   true),
            ("🐛", "Malware",      true),
            ("⚡", "DDoS",         true),
            ("🔍", "Exploit",      false),
            ("🖥",  "Terminal",    true),
            ("📡", "Botnet",       true),
            ("🧬", "Zero-Day",     true),
            ("🔮", "Crypto",       false),
            ("🗝",  "SSH Key",     false),
        };

        // ── État UI ────────────────────────────────────────────────────
        private JeuMemory _jeu;
        private int _tailleGrille;
        private Panel _panelGrille;
        private Label _lblChrono;
        private Label _lblEssais;
        private Label _lblPaires;
        private Button _btnRejouer;
        private Timer _timerChrono;
        private Timer _timerRetour;
        private Timer _timerAnimFond;
        private Timer _timerHardcore;
        private bool _modeHardcore = true; // Active le mode Hardcore
        private DateTime _heureDebut;
        private int _animTick = 0;
        private bool _cliquesBloquees = false;

        // ── Association PictureBox <-> Carte (comme demandé dans le sujet) ──
        private Dictionary<PictureBox, Carte> _pbVersCarte = new Dictionary<PictureBox, Carte>();
        private Dictionary<int, bool> _idEstRedTeam = new Dictionary<int, bool>();

        public FormJeu(int tailleGrille)
        {
            _tailleGrille = tailleGrille;
            _jeu = new JeuMemory();

            ScoreRepository.Initialiser(); // ajout pour initialiser la base de données des scores


            SonManager.Initialiser();

            InitialiserUI(); // calcule la taille de la fenêtre selon la taille de la grille
            InitialiserTimers(); // mis a jour du chrono chaque seconde
            DemarrerPartie(); // c'est le coeur du progr , il initialise le jeu, construit la grille de PictureBox et lance le chrono
        }

        // ── Construction de l'interface ─────────────────
        private void InitialiserUI()
        {
            int largGrille = _tailleGrille * (TAILLE_CARTE + ESPACEMENT) + ESPACEMENT;
            int hautGrille = _tailleGrille * (TAILLE_CARTE + ESPACEMENT) + ESPACEMENT;
            int W = largGrille + MARGE * 2 + 16;
            int H = hautGrille + HAUTEUR_HUD + MARGE * 2 + 50;

            this.Text = $"SecurIT Memory — {_tailleGrille}×{_tailleGrille}";
            this.Size = new Size(W, H);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.Paint += FormJeu_Paint;

            // ── HUD ────────────────────────────────────────────────────
            _lblChrono = CreerLabelHUD("⏱  00:00", VERT_NEON, new Point(MARGE, 14));
            _lblEssais = CreerLabelHUD("ESSAIS : 0", Color.White, new Point(MARGE + 160, 14));
            _lblPaires = CreerLabelHUD("PAIRES : 0 / ?", Color.FromArgb(0, 200, 100), new Point(MARGE + 310, 14));

            _btnRejouer = new Button();
            _btnRejouer.Text = "↺ REJOUER";
            _btnRejouer.Font = new Font("Courier New", 9, FontStyle.Bold);
            _btnRejouer.ForeColor = BLEU_NEON;
            _btnRejouer.BackColor = Color.FromArgb(5, 0, 229, 255);
            _btnRejouer.FlatStyle = FlatStyle.Flat;
            _btnRejouer.FlatAppearance.BorderColor = BLEU_NEON;
            _btnRejouer.FlatAppearance.BorderSize = 1;
            _btnRejouer.Size = new Size(110, 30);
            _btnRejouer.Location = new Point(W - 130, 20);
            _btnRejouer.Cursor = Cursors.Hand;
            _btnRejouer.Click += (s, e) => DemarrerPartie();

            // ── Panel conteneur de la grille ───────────────────────────
            _panelGrille = new Panel();
            _panelGrille.Size = new Size(largGrille, hautGrille);
            _panelGrille.Location = new Point(MARGE, HAUTEUR_HUD + MARGE);
            _panelGrille.BackColor = Color.FromArgb(10, 0, 10);
            _panelGrille.Paint += PanelGrille_Paint;

            this.Controls.Add(_lblChrono);
            this.Controls.Add(_lblEssais);
            this.Controls.Add(_lblPaires);
            this.Controls.Add(_btnRejouer);
            this.Controls.Add(_panelGrille);
        }

        private Label CreerLabelHUD(string texte, Color couleur, Point pos)
        {
            var l = new Label();
            l.Text = texte;
            l.Font = new Font("Courier New", 11, FontStyle.Bold);
            l.ForeColor = couleur;
            l.BackColor = Color.Transparent;
            l.AutoSize = true;
            l.Location = pos;
            return l;
        }

        // ── Fonds animés ───────────────────────────────────────────────
        private void FormJeu_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.FromArgb(8, 0, 255, 106), 1))
            {
                for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
            }
            int scanY = (_animTick * 2) % Height;
            using (var sp = new Pen(Color.FromArgb(20, 0, 255, 106), 1))
                g.DrawLine(sp, 0, scanY, Width, scanY);
            using (var lp = new Pen(Color.FromArgb(50, 0, 255, 106), 1))
                g.DrawLine(lp, MARGE, HAUTEUR_HUD, Width - MARGE, HAUTEUR_HUD);
        }

        private void PanelGrille_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.FromArgb(12, 0, 255, 106), 1))
            {
                for (int x = 0; x < _panelGrille.Width; x += 40) g.DrawLine(pen, x, 0, x, _panelGrille.Height);
                for (int y = 0; y < _panelGrille.Height; y += 40) g.DrawLine(pen, 0, y, _panelGrille.Width, y);
            }
        }

        // ── Timers ─────────────────────────────────────────────────────
        private void InitialiserTimers()
        {
            _timerChrono = new Timer();
            _timerChrono.Interval = 1000;
            _timerChrono.Tick += TimerChrono_Tick;

            _timerRetour = new Timer();
            _timerRetour.Interval = DELAI_MS;
            _timerRetour.Tick += TimerRetour_Tick;

            _timerAnimFond = new Timer();
            _timerAnimFond.Interval = 30;
            _timerAnimFond.Tick += (s, e) =>
            {
                _animTick++;
                this.Invalidate(false);
                _panelGrille.Invalidate(false);
            };
            _timerAnimFond.Start();
            // ── Timer Hardcore : mélange toutes les 30 secondes ───────────────
            _timerHardcore = new Timer();
            _timerHardcore.Interval = 30000; // 30 secondes
            _timerHardcore.Tick += (s, e) =>
            {
                if (_modeHardcore)
                {
                    _jeu.MelangerCartesNonTrouvees();
                    RafraichirGrille();
                }
            };
            _timerHardcore.Start();

        }

        // ── Démarrage d'une partie ─────────────────────────────────────
        private void DemarrerPartie()
        {
            _timerChrono.Stop();
            _timerRetour.Stop();
            _cliquesBloquees = false;
            _idEstRedTeam.Clear();

            _jeu.NouvellePartie(_tailleGrille);

            // Associer chaque IdPaire à son équipe (Blue ou Red)
            foreach (var carte in _jeu.Cartes)
                if (!_idEstRedTeam.ContainsKey(carte.IdPaire))
                    _idEstRedTeam[carte.IdPaire] = EstRedTeam(carte.NomIcone);

            MettreAJourHUD();
            ConstruireGrille();

            _heureDebut = DateTime.Now;
            _timerChrono.Start();
        }

        private bool EstRedTeam(string nomIcone)
        {
            foreach (var ic in ICONES)
                if (ic.Nom == nomIcone) return ic.RedTeam;
            return false;
        }

        // ── Construction de la grille de PictureBox ────────────────────
        /// <summary>
        /// Génère dynamiquement une PictureBox par carte et la lie à l'objet Carte.
        /// Chaque PictureBox est associée à sa Carte via le dictionnaire _pbVersCarte.
        /// </summary>
        private void ConstruireGrille()
        {
            // Nettoyer les anciennes PictureBox et libérer leurs images
            foreach (PictureBox pb in _pbVersCarte.Keys)
            {
                pb.Image = null;
                pb.Click -= PictureBox_Click;
            }
            _panelGrille.Controls.Clear();
            _pbVersCarte.Clear();

            for (int i = 0; i < _jeu.Cartes.Count; i++)
            {
                int col = i % _tailleGrille;
                int row = i / _tailleGrille;
                Carte carte = _jeu.Cartes[i];

                // Créer une PictureBox pour cette carte (comme demandé dans le sujet)
                PictureBox pb = new PictureBox();
                pb.Size = new Size(TAILLE_CARTE, TAILLE_CARTE);
                pb.Location = new Point(
                    ESPACEMENT + col * (TAILLE_CARTE + ESPACEMENT),
                    ESPACEMENT + row * (TAILLE_CARTE + ESPACEMENT));
                pb.BackColor = Color.FromArgb(10, 0, 10);
                pb.BorderStyle = BorderStyle.None;
                pb.Cursor = Cursors.Hand;
                pb.SizeMode = PictureBoxSizeMode.Normal;

                // Lier la PictureBox à son objet Carte
                pb.Tag = carte;
                pb.Image = GenererImageDos(TAILLE_CARTE, TAILLE_CARTE);
                pb.Click += PictureBox_Click;

                _pbVersCarte[pb] = carte;
                _panelGrille.Controls.Add(pb);
            }
        }

        // ── Génération des images Bitmap pour les PictureBox ──────────

        /// <summary>Génère le Bitmap du dos de carte (face cachée).</summary>
        private Bitmap GenererImageDos(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(new SolidBrush(Color.FromArgb(10, 0, 10)), 0, 0, w, h);
                g.DrawRectangle(new Pen(Color.FromArgb(40, 0, 255, 106), 1), 0, 0, w - 1, h - 1);

                // Hexagone vert fantôme
                Font fHex = new Font("Segoe UI Emoji", 22);
                SizeF sHex = g.MeasureString("⬡", fHex);
                g.DrawString("⬡", fHex,
                    new SolidBrush(Color.FromArgb(40, 0, 255, 106)),
                    (w - sHex.Width) / 2, (h - sHex.Height) / 2 - 6);

                // Texte SECURIT en bas
                Font fTxt = new Font("Courier New", 6, FontStyle.Bold);
                SizeF sTxt = g.MeasureString("SECURIT", fTxt);
                g.DrawString("SECURIT", fTxt,
                    new SolidBrush(Color.FromArgb(50, 0, 255, 106)),
                    (w - sTxt.Width) / 2, h - 16);

                // Coins décoratifs
                DessinerCoins(g, w, h, Color.FromArgb(60, 0, 255, 106));
            }
            return bmp;
        }

        /// <summary>Génère le Bitmap de la face d'une carte (icône visible).</summary>
        private Bitmap GenererImageFace(Carte carte, bool estTrouvee)
        {
            int w = TAILLE_CARTE;
            int h = TAILLE_CARTE;
            bool red = _idEstRedTeam.ContainsKey(carte.IdPaire) && _idEstRedTeam[carte.IdPaire];
            Bitmap bmp = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Fond selon état
                Color fond, bord, couleurNom;
                if (estTrouvee)
                {
                    fond = red ? Color.FromArgb(30, 255, 0, 76) : Color.FromArgb(20, 0, 255, 106);
                    bord = red ? ROUGE_NEON : VERT_NEON;
                    couleurNom = red ? ROUGE_NEON : VERT_NEON;
                }
                else
                {
                    fond = Color.FromArgb(15, 0, 229, 255);
                    bord = BLEU_NEON;
                    couleurNom = red ? ROUGE_NEON : VERT_NEON;
                }

                g.FillRectangle(new SolidBrush(fond), 0, 0, w, h);
                g.DrawRectangle(new Pen(bord, 1), 0, 0, w - 1, h - 1);

                // Grande icône emoji au centre
                Font fIco = new Font("Segoe UI Emoji", 26);
                SizeF sIco = g.MeasureString(carte.CheminImage, fIco);
                g.DrawString(carte.CheminImage, fIco, Brushes.White,
                    (w - sIco.Width) / 2, (h - sIco.Height) / 2 - 8);

                // Nom en bas
                Font fNom = new Font("Courier New", 7, FontStyle.Bold);
                SizeF sNom = g.MeasureString(carte.NomIcone.ToUpper(), fNom);
                g.DrawString(carte.NomIcone.ToUpper(), fNom,
                    new SolidBrush(couleurNom),
                    (w - sNom.Width) / 2, h - 18);

                // Coins décoratifs
                DessinerCoins(g, w, h, Color.FromArgb(80, bord.R, bord.G, bord.B));
            }
            return bmp;
        }

        /// <summary>Dessine les 4 coins décoratifs cyber sur un Bitmap.</summary>
        private void DessinerCoins(Graphics g, int w, int h, Color couleur)
        {
            int sz = 6;
            using (Pen cp = new Pen(couleur, 1))
            {
                g.DrawLine(cp, 2, 2, 2 + sz, 2); g.DrawLine(cp, 2, 2, 2, 2 + sz);
                g.DrawLine(cp, w - 3, 2, w - 3 - sz, 2); g.DrawLine(cp, w - 3, 2, w - 3, 2 + sz);
                g.DrawLine(cp, 2, h - 3, 2 + sz, h - 3); g.DrawLine(cp, 2, h - 3, 2, h - 3 - sz);
                g.DrawLine(cp, w - 3, h - 3, w - 3 - sz, h - 3); g.DrawLine(cp, w - 3, h - 3, w - 3, h - 3 - sz);
            }
        }

        /// <summary>Rafraîchit l'image de toutes les PictureBox selon l'état de leur Carte.</summary>
        private void RafraichirGrille()
        {
            foreach (var kvp in _pbVersCarte)
            {
                PictureBox pb = kvp.Key;
                Carte carte = kvp.Value;
                Bitmap ancienne = pb.Image as Bitmap;

                if (carte.EstTrouvee)
                    pb.Image = GenererImageFace(carte, true);
                else if (carte.Etat == EtatCarte.Revelee)
                    pb.Image = GenererImageFace(carte, false);
                else
                    pb.Image = GenererImageDos(TAILLE_CARTE, TAILLE_CARTE);

                ancienne?.Dispose(); // Libérer la mémoire de l'ancienne image
            }
        }

        // ── Clic sur une PictureBox ────────────────────────────────────
        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (_cliquesBloquees) return;

            PictureBox pb = (PictureBox)sender;
            Carte carte = _pbVersCarte[pb];
            ResultatClic r = _jeu.TraiterClic(carte);

            switch (r)
            {
                case ResultatClic.PremiereCarte:
                    SonManager.JouerClic();
                    // Mettre à jour uniquement cette PictureBox
                    Bitmap ancPremiere = pb.Image as Bitmap;
                    pb.Image = GenererImageFace(carte, false);
                    ancPremiere?.Dispose();
                    break;

                case ResultatClic.PaireTrouvee:
                    SonManager.JouerClic();
                    SonManager.JouerPaire();
                    RafraichirGrille();
                    MettreAJourHUD();
                    break;

                case ResultatClic.NonPaire:
                    SonManager.JouerClic();
                    RafraichirGrille();
                    MettreAJourHUD();
                    _cliquesBloquees = true;
                    _timerRetour.Start();
                    break;

                case ResultatClic.Victoire:
                    SonManager.JouerClic();
                    SonManager.JouerPaire();
                    RafraichirGrille();
                    MettreAJourHUD();
                    AfficherVictoire();
                    break;
            }
        }

        // ── Tick Timers ────────────────────────────────────────────────
        private void TimerChrono_Tick(object sender, EventArgs e)
        {
            var t = DateTime.Now - _heureDebut;
            _lblChrono.Text = $"⏱  {t.Minutes:00}:{t.Seconds:00}";
        }

        private void TimerRetour_Tick(object sender, EventArgs e)
        {
            _timerRetour.Stop();
            SonManager.JouerErreur();
            _jeu.RetournerCartesNonPaire();
            RafraichirGrille();
            _cliquesBloquees = false;
        }

        // ── HUD ────────────────────────────────────────────────────────
        private void MettreAJourHUD()
        {
            _lblEssais.Text = $"ESSAIS : {_jeu.NombreEssais}";
            _lblPaires.Text = $"PAIRES : {_jeu.NombrePairesTrouvees} / {_jeu.NombrePairesTotal}";
        }

        // ── Victoire ───────────────────────────────────────────────────
        private void AfficherVictoire()
        {
            _timerChrono.Stop();
            SonManager.JouerVictoire();

            var t = DateTime.Now - _heureDebut;
            string msg = $"MISSION ACCOMPLIE\n\n" +
                         $"Toutes les paires trouvées !\n\n" +
                         $"Temps   : {t.Minutes:00}:{t.Seconds:00}\n" +
                         $"Essais  : {_jeu.NombreEssais}\n\n" +
                         $"Rejouer ?";
            // ── Enregistrement du score SQL ───────────────────────────────
            var duree = DateTime.Now - _heureDebut;
            int tempsSec = (int)duree.TotalSeconds;

            string nom = Interaction.InputBox(
                    "Entrez votre nom pour le classement :",
                    "Nouveau score",
                    "Joueur");

            if (string.IsNullOrWhiteSpace(nom))
                nom = "Joueur";

            ScoreRepository.AjouterScore(new ScoreEntry
            {
                Joueur = nom,
                Grille = _tailleGrille,
                Essais = _jeu.NombreEssais,
                TempsSecondes = tempsSec,
                DatePartie = DateTime.Now
            });

            if (MessageBox.Show(msg, "SecurIT Memory — Victoire !",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                DemarrerPartie();
            else
                this.Close();
        }

        // ── Nettoyage à la fermeture ───────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerChrono?.Stop();
            _timerRetour?.Stop();
            _timerAnimFond?.Stop();

            // Libérer toutes les images des PictureBox
            foreach (PictureBox pb in _pbVersCarte.Keys)
                pb.Image?.Dispose();

            base.OnFormClosing(e);
        }
    }
}