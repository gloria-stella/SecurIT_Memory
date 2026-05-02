using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    /// <summary>
    /// Formulaire principal du jeu Memory SecurIT.
    /// Gère la grille de PictureBox, le HUD, les timers et le rendu visuel.
    /// Blue Team = bleu | Red Team = rouge | Paire trouvée = violet
    /// </summary>
    public class FormJeu : Form
    {
        // ── Constantes ─────────────────────────────────────────────────
        private const int TAILLE_CARTE = 90;
        private const int ESPACEMENT = 8;
        private const int MARGE = 16;
        private const int HAUTEUR_HUD = 72;

        // ── État ───────────────────────────────────────────────────────
        private JeuMemory _jeu;
        private int _tailleGrille;
        private ThemeCartes _theme;
        private bool _modeHardcore;
        private Panel _panelGrille;
        private Label _lblChrono;
        private Label _lblEssais;
        private Label _lblPaires;
        private Label _lblHardcore;
        private Button _btnRejouer;
        private Timer _timerChrono;
        private Timer _timerAnimFond;
        private DateTime _heureDebut;
        private int _animTick = 0;
        private bool _cliquesBloquees = false;

        // PictureBox liées à leurs cartes (comme demandé dans le TP)
        private Dictionary<PictureBox, Carte> _pbVersCarte = new Dictionary<PictureBox, Carte>();

        public FormJeu(int tailleGrille, ThemeCartes theme = ThemeCartes.Cybersecurite, bool hardcore = false)
        {
            _tailleGrille = tailleGrille;
            _theme = theme;
            _modeHardcore = hardcore;
            _jeu = new JeuMemory();

            // Abonner aux événements de la logique de jeu
            _jeu.DemandeRetournement += Jeu_DemandeRetournement;
            _jeu.HardcoreReposition += Jeu_HardcoreReposition;

            SonManager.Initialiser();
            ScoreManager.Initialiser();

            InitialiserUI();
            InitialiserTimers();
            DemarrerPartie();
        }

        // ── Construction de l'interface ────────────────────────────────
        private void InitialiserUI()
        {
            int largGrille = _tailleGrille * (TAILLE_CARTE + ESPACEMENT) + ESPACEMENT;
            int hautGrille = _tailleGrille * (TAILLE_CARTE + ESPACEMENT) + ESPACEMENT;
            int W = largGrille + MARGE * 2 + 16;
            int H = hautGrille + HAUTEUR_HUD + MARGE * 2 + 50;

            this.Text = $"SecurIT Memory — {_tailleGrille}×{_tailleGrille}" +
                                   (_modeHardcore ? " [HARDCORE]" : "");
            this.Size = new Size(W, H);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ThemeCyber.NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.Paint += FormJeu_Paint;

            _lblChrono = CreerLabelHUD("⏱  00:00", ThemeCyber.VERT_NEON, new Point(MARGE, 14));
            _lblEssais = CreerLabelHUD("ESSAIS : 0", ThemeCyber.BLANC, new Point(MARGE + 160, 14));
            _lblPaires = CreerLabelHUD("PAIRES : 0 / ?", Color.FromArgb(0, 200, 100), new Point(MARGE + 310, 14));

            // Label HARDCORE (visible seulement en mode hardcore)
            _lblHardcore = new Label();
            _lblHardcore.Text = "⚡ HARDCORE";
            _lblHardcore.Font = new Font("Courier New", 9, FontStyle.Bold);
            _lblHardcore.ForeColor = ThemeCyber.ROUGE_NEON;
            _lblHardcore.BackColor = Color.Transparent;
            _lblHardcore.AutoSize = true;
            _lblHardcore.Location = new Point(MARGE, 40);
            _lblHardcore.Visible = _modeHardcore;

            _btnRejouer = new Button();
            _btnRejouer.Text = "↺ REJOUER";
            _btnRejouer.Font = new Font("Courier New", 9, FontStyle.Bold);
            _btnRejouer.ForeColor = ThemeCyber.BLEU_NEON;
            _btnRejouer.BackColor = Color.FromArgb(5, 0, 229, 255);
            _btnRejouer.FlatStyle = FlatStyle.Flat;
            _btnRejouer.FlatAppearance.BorderColor = ThemeCyber.BLEU_NEON;
            _btnRejouer.FlatAppearance.BorderSize = 1;
            _btnRejouer.Size = new Size(110, 30);
            _btnRejouer.Location = new Point(W - 130, 20);
            _btnRejouer.Cursor = Cursors.Hand;
            _btnRejouer.Click += (s, e) => DemarrerPartie();

            _panelGrille = new Panel();
            _panelGrille.Size = new Size(largGrille, hautGrille);
            _panelGrille.Location = new Point(MARGE, HAUTEUR_HUD + MARGE);
            _panelGrille.BackColor = Color.FromArgb(10, 0, 10);
            _panelGrille.Paint += PanelGrille_Paint;

            this.Controls.Add(_lblChrono);
            this.Controls.Add(_lblEssais);
            this.Controls.Add(_lblPaires);
            this.Controls.Add(_lblHardcore);
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
            using (var pen = new Pen(ThemeCyber.GRILLE_FOND, 1))
            {
                for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
            }
            int scanY = (_animTick * 2) % Height;
            using (var sp = new Pen(ThemeCyber.SCAN_LIGNE, 1))
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
            _timerChrono.Tick += (s, e) =>
            {
                var t = DateTime.Now - _heureDebut;
                _lblChrono.Text = $"⏱  {t.Minutes:00}:{t.Seconds:00}";
            };

            _timerAnimFond = new Timer();
            _timerAnimFond.Interval = 30;
            _timerAnimFond.Tick += (s, e) =>
            {
                _animTick++;
                this.Invalidate(false);
                _panelGrille.Invalidate(false);
            };
            _timerAnimFond.Start();
        }

        // ── Démarrage d'une partie ─────────────────────────────────────
        private void DemarrerPartie()
        {
            _timerChrono.Stop();
            _cliquesBloquees = false;

            _jeu.NouvellePartie(_tailleGrille, _theme, _modeHardcore);
            MettreAJourHUD();
            ConstruireGrille();

            _heureDebut = DateTime.Now;
            _timerChrono.Start();
        }

        // ── Construction de la grille de PictureBox ────────────────────
        /// <summary>
        /// Génère dynamiquement une PictureBox par carte et la lie à l'objet Carte.
        /// Chaque PictureBox est stockée dans _pbVersCarte (Dictionary).
        /// </summary>
        private void ConstruireGrille()
        {
            // Stocker les images à libérer APRÈS avoir vidé le panel
            var imagesALiberer = new System.Collections.Generic.List<Image>();

            foreach (PictureBox pb in _pbVersCarte.Keys)
            {
                pb.Click -= PictureBox_Click;
                if (pb.Image != null)
                    imagesALiberer.Add(pb.Image);
                pb.Image = null; // Détacher l'image avant de vider le panel
            }

            _panelGrille.Controls.Clear();
            _pbVersCarte.Clear();

            // Libérer les images maintenant que le panel est vidé
            foreach (Image img in imagesALiberer)
                img.Dispose();

            for (int i = 0; i < _jeu.Cartes.Count; i++)
            {
                int col = i % _tailleGrille;
                int row = i / _tailleGrille;
                Carte carte = _jeu.Cartes[i];

                // Créer la PictureBox et la lier à l'objet Carte
                PictureBox pb = new PictureBox();
                pb.Size = new Size(TAILLE_CARTE, TAILLE_CARTE);
                pb.Location = new Point(
                    ESPACEMENT + col * (TAILLE_CARTE + ESPACEMENT),
                    ESPACEMENT + row * (TAILLE_CARTE + ESPACEMENT));
                pb.BackColor = Color.FromArgb(10, 0, 10);
                pb.BorderStyle = BorderStyle.None;
                pb.Cursor = Cursors.Hand;
                pb.SizeMode = PictureBoxSizeMode.Normal;
                pb.Tag = carte;
                pb.Image = GenererImageDos();
                pb.Click += PictureBox_Click;

                _pbVersCarte[pb] = carte;
                _panelGrille.Controls.Add(pb);
            }
        }

        // ── Génération des Bitmap ──────────────────────────────────────

        private Bitmap GenererImageDos()
        {
            int w = TAILLE_CARTE;
            int h = TAILLE_CARTE;
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(new SolidBrush(Color.FromArgb(10, 0, 10)), 0, 0, w, h);
                g.DrawRectangle(new Pen(Color.FromArgb(40, 0, 255, 106), 1), 0, 0, w - 1, h - 1);

                Font fHex = new Font("Segoe UI Emoji", 22);
                SizeF sHex = g.MeasureString("⬡", fHex);
                g.DrawString("⬡", fHex, new SolidBrush(ThemeCyber.DOS_CARTE),
                    (w - sHex.Width) / 2, (h - sHex.Height) / 2 - 6);

                Font fTxt = new Font("Courier New", 6, FontStyle.Bold);
                SizeF sTxt = g.MeasureString("SECURIT", fTxt);
                g.DrawString("SECURIT", fTxt, new SolidBrush(Color.FromArgb(50, 0, 255, 106)),
                    (w - sTxt.Width) / 2, h - 16);

                DessinerCoins(g, w, h, Color.FromArgb(60, 0, 255, 106));
            }
            return bmp;
        }

        /// <summary>
        /// Génère le Bitmap de la face d'une carte.
        /// Blue Team = bleu | Red Team = rouge | Trouvée = violet
        /// </summary>
        private Bitmap GenererImageFace(Carte carte, bool estTrouvee)
        {
            int w = TAILLE_CARTE;
            int h = TAILLE_CARTE;
            Bitmap bmp = new Bitmap(w, h);

            // Choisir les couleurs selon l'équipe et l'état
            Color fond, bordure, couleurNom;
            if (estTrouvee)
            {
                // Violet pour toutes les paires trouvées
                fond = ThemeCyber.FOND_TROUVEE;
                bordure = ThemeCyber.BORDURE_TROUVEE;
                couleurNom = ThemeCyber.VIOLET_NEON;
            }
            else if (carte.EstRedTeam)
            {
                // Rouge pour Red Team (menaces)
                fond = ThemeCyber.FOND_RED_TEAM;
                bordure = ThemeCyber.BORDURE_RED_TEAM;
                couleurNom = ThemeCyber.ROUGE_NEON;
            }
            else
            {
                // Bleu pour Blue Team (défense)
                fond = ThemeCyber.FOND_BLUE_TEAM;
                bordure = ThemeCyber.BORDURE_BLUE_TEAM;
                couleurNom = ThemeCyber.BLEU_NEON;
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(new SolidBrush(fond), 0, 0, w, h);
                g.DrawRectangle(new Pen(bordure, 1), 0, 0, w - 1, h - 1);

                // Grande icône emoji au centre
                Font fIco = new Font("Segoe UI Emoji", 26);
                SizeF sIco = g.MeasureString(carte.CheminImage, fIco);
                g.DrawString(carte.CheminImage, fIco, Brushes.White,
                    (w - sIco.Width) / 2, (h - sIco.Height) / 2 - 8);

                // Nom en bas
                Font fNom = new Font("Courier New", 7, FontStyle.Bold);
                SizeF sNom = g.MeasureString(carte.NomIcone.ToUpper(), fNom);
                g.DrawString(carte.NomIcone.ToUpper(), fNom, new SolidBrush(couleurNom),
                    (w - sNom.Width) / 2, h - 18);

                DessinerCoins(g, w, h, Color.FromArgb(80, bordure.R, bordure.G, bordure.B));
            }
            return bmp;
        }

        private void DessinerCoins(Graphics g, int w, int h, Color c)
        {
            int sz = 6;
            using (Pen cp = new Pen(c, 1))
            {
                g.DrawLine(cp, 2, 2, 2 + sz, 2); g.DrawLine(cp, 2, 2, 2, 2 + sz);
                g.DrawLine(cp, w - 3, 2, w - 3 - sz, 2); g.DrawLine(cp, w - 3, 2, w - 3, 2 + sz);
                g.DrawLine(cp, 2, h - 3, 2 + sz, h - 3); g.DrawLine(cp, 2, h - 3, 2, h - 3 - sz);
                g.DrawLine(cp, w - 3, h - 3, w - 3 - sz, h - 3); g.DrawLine(cp, w - 3, h - 3, w - 3, h - 3 - sz);
            }
        }

        private void RafraichirGrille()
        {
            foreach (var kvp in _pbVersCarte)
            {
                PictureBox pb = kvp.Key;
                Carte carte = kvp.Value;

                // Générer la nouvelle image avant de remplacer
                Bitmap nouvelleImage;
                if (carte.EstTrouvee) nouvelleImage = GenererImageFace(carte, true);
                else if (carte.Etat == EtatCarte.Revelee) nouvelleImage = GenererImageFace(carte, false);
                else nouvelleImage = GenererImageDos();

                // Remplacer proprement sans crash
                Image ancienne = pb.Image;
                pb.Image = nouvelleImage;
                ancienne?.Dispose();
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
                    Bitmap old = pb.Image as Bitmap;
                    pb.Image = GenererImageFace(carte, false);
                    old?.Dispose();
                    break;

                case ResultatClic.NonPaire:
                    SonManager.JouerClic();
                    RafraichirGrille();
                    MettreAJourHUD();
                    // Bloquer les clics jusqu'au retournement (géré par l'événement)
                    _cliquesBloquees = true;
                    break;

                case ResultatClic.PaireTrouvee:
                    SonManager.JouerClic();
                    SonManager.JouerPaire();
                    RafraichirGrille();
                    MettreAJourHUD();
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

        // ── Événements de la logique de jeu ───────────────────────────

        /// <summary>Déclenché par JeuMemory après le délai : retourner les cartes.</summary>
        private void Jeu_DemandeRetournement(object sender, EventArgs e)
        {
            SonManager.JouerErreur();
            RafraichirGrille();
            _cliquesBloquees = false;
        }

        /// <summary>Déclenché par JeuMemory en mode Hardcore : repositionner les cartes.</summary>
        private void Jeu_HardcoreReposition(object sender, EventArgs e)
        {
            // Reconstruire entièrement la grille avec le nouvel ordre
            ConstruireGrille();
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

            TimeSpan temps = DateTime.Now - _heureDebut;
            bool record = ScoreManager.EstNouveauRecord(temps, _jeu.NombreEssais, _tailleGrille);

            // Demander le nom du joueur
            string nom = Microsoft.VisualBasic.Interaction.InputBox(
                "Entre ton prénom pour enregistrer ton score :",
                "SecurIT Memory — Victoire !", "Joueur");
            if (string.IsNullOrWhiteSpace(nom)) nom = "Anonyme";

            // Sauvegarder le score
            ScoreManager.SauvegarderScore(nom, temps, _jeu.NombreEssais, _tailleGrille);

            // Message de victoire
            string recordMsg = record ? "\n\n🏆 NOUVEAU RECORD !" : "";
            string msg = $"MISSION ACCOMPLIE{recordMsg}\n\n" +
                         $"Toutes les paires trouvées !\n\n" +
                         $"⏱  Temps   : {temps.Minutes:00}:{temps.Seconds:00}\n" +
                         $"🖱  Essais  : {_jeu.NombreEssais}\n\n" +
                         $"Rejouer ?";

            if (MessageBox.Show(msg, "SecurIT Memory — Victoire !",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                DemarrerPartie();
            else
                this.Close();
        }

        // ── Nettoyage ──────────────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerChrono?.Stop();
            _timerAnimFond?.Stop();
            _jeu?.Arreter();
            foreach (PictureBox pb in _pbVersCarte.Keys)
                pb.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}