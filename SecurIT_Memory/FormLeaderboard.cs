using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    /// <summary>
    /// Formulaire affichant le classement des meilleurs scores (leaderboard).
    /// Accessible depuis le menu principal.
    /// </summary>
    public class FormLeaderboard : Form
    {
        private Label lblTitre;
        private Label lblGrille;
        private Button btn4x4;
        private Button btn6x6;
        private Panel panelScores;
        private Button btnFermer;
        private Timer _timerAnim;
        private int _animTick = 0;
        private int _grilleAff = 4;

        public FormLeaderboard()
        {
            InitialiserComposants();
            InitialiserAnim();
            AfficherScores(4);
        }

        private void InitialiserComposants()
        {
            this.Text = "Leaderboard — SecurIT Memory";
            this.Size = new Size(500, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeCyber.NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;
            this.Paint += (s, e) => DessinerFond(e.Graphics);

            lblTitre = new Label();
            lblTitre.Text = "🏆 LEADERBOARD";
            lblTitre.Font = new Font("Courier New", 18, FontStyle.Bold);
            lblTitre.ForeColor = ThemeCyber.VERT_NEON;
            lblTitre.BackColor = Color.Transparent;
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(120, 20);

            lblGrille = new Label();
            lblGrille.Text = "Taille :";
            lblGrille.Font = new Font("Courier New", 9);
            lblGrille.ForeColor = Color.FromArgb(0, 160, 80);
            lblGrille.BackColor = Color.Transparent;
            lblGrille.AutoSize = true;
            lblGrille.Location = new Point(20, 68);

            btn4x4 = CreerBtnGrille("4 × 4", 90, true);
            btn6x6 = CreerBtnGrille("6 × 6", 170, false);
            btn4x4.Click += (s, e) => { _grilleAff = 4; AfficherScores(4); MettreAJourBoutons(true); };
            btn6x6.Click += (s, e) => { _grilleAff = 6; AfficherScores(6); MettreAJourBoutons(false); };

            panelScores = new Panel();
            panelScores.Size = new Size(460, 400);
            panelScores.Location = new Point(20, 110);
            panelScores.BackColor = Color.FromArgb(5, 0, 255, 106);

            btnFermer = new Button();
            btnFermer.Text = "✕  FERMER";
            btnFermer.Font = new Font("Courier New", 10, FontStyle.Bold);
            btnFermer.ForeColor = ThemeCyber.ROUGE_NEON;
            btnFermer.BackColor = Color.FromArgb(5, 255, 0, 76);
            btnFermer.FlatStyle = FlatStyle.Flat;
            btnFermer.FlatAppearance.BorderColor = ThemeCyber.ROUGE_NEON;
            btnFermer.FlatAppearance.BorderSize = 1;
            btnFermer.Size = new Size(150, 38);
            btnFermer.Location = new Point(175, 525);
            btnFermer.Cursor = Cursors.Hand;
            btnFermer.Click += (s, e) => { _timerAnim?.Stop(); this.Close(); };

            this.Controls.Add(lblTitre);
            this.Controls.Add(lblGrille);
            this.Controls.Add(btn4x4);
            this.Controls.Add(btn6x6);
            this.Controls.Add(panelScores);
            this.Controls.Add(btnFermer);
        }

        private Button CreerBtnGrille(string texte, int x, bool actif)
        {
            var btn = new Button();
            btn.Text = texte;
            btn.Font = new Font("Courier New", 9, FontStyle.Bold);
            btn.Size = new Size(65, 26);
            btn.Location = new Point(x, 62);
            btn.Cursor = Cursors.Hand;
            btn.FlatStyle = FlatStyle.Flat;
            AppliquerStyleBtnGrille(btn, actif);
            return btn;
        }

        private void AppliquerStyleBtnGrille(Button btn, bool actif)
        {
            btn.ForeColor = actif ? ThemeCyber.VERT_NEON : Color.FromArgb(0, 100, 50);
            btn.BackColor = actif
                ? Color.FromArgb(15, 0, 255, 106)
                : Color.Transparent;
            btn.FlatAppearance.BorderColor = actif
                ? ThemeCyber.VERT_NEON
                : Color.FromArgb(0, 80, 40);
            btn.FlatAppearance.BorderSize = 1;
        }

        private void MettreAJourBoutons(bool est4x4)
        {
            AppliquerStyleBtnGrille(btn4x4, est4x4);
            AppliquerStyleBtnGrille(btn6x6, !est4x4);
        }

        // ── Affichage des scores ───────────────────────────────────────

        private void AfficherScores(int grille)
        {
            panelScores.Controls.Clear();
            List<EntreeScore> scores = ScoreManager.ObtenirLeaderboard(grille);

            // En-tête tableau
            AjouterLigneEntete();

            if (scores.Count == 0)
            {
                var lblVide = new Label();
                lblVide.Text = "Aucun score enregistré pour cette grille.";
                lblVide.Font = new Font("Courier New", 9);
                lblVide.ForeColor = Color.FromArgb(0, 120, 60);
                lblVide.AutoSize = true;
                lblVide.Location = new Point(60, 60);
                panelScores.Controls.Add(lblVide);
                return;
            }

            for (int i = 0; i < scores.Count; i++)
                AjouterLigneScore(i + 1, scores[i]);
        }

        private void AjouterLigneEntete()
        {
            string[] cols = { "#", "NOM", "TEMPS", "ESSAIS", "DATE" };
            int[] posX = { 5, 35, 145, 240, 320 };
            for (int i = 0; i < cols.Length; i++)
            {
                var l = new Label();
                l.Text = cols[i];
                l.Font = new Font("Courier New", 8, FontStyle.Bold);
                l.ForeColor = Color.FromArgb(0, 180, 90);
                l.AutoSize = true;
                l.Location = new Point(posX[i], 8);
                panelScores.Controls.Add(l);
            }

            // Ligne séparatrice
            var sep = new Label();
            sep.Text = new string('─', 60);
            sep.Font = new Font("Courier New", 7);
            sep.ForeColor = Color.FromArgb(0, 80, 40);
            sep.AutoSize = true;
            sep.Location = new Point(5, 26);
            panelScores.Controls.Add(sep);
        }

        private void AjouterLigneScore(int rang, EntreeScore score)
        {
            int y = 36 + (rang - 1) * 32;
            Color couleur = rang == 1
                ? ThemeCyber.VERT_NEON
                : rang == 2
                    ? ThemeCyber.BLEU_NEON
                    : rang == 3
                        ? ThemeCyber.VIOLET_NEON
                        : Color.FromArgb(150, 200, 150);

            string[] vals = {
                rang == 1 ? "🥇" : rang == 2 ? "🥈" : rang == 3 ? "🥉" : $"{rang}.",
                score.Nom.Length > 10 ? score.Nom.Substring(0, 10) : score.Nom,
                $"{score.Temps.Minutes:00}:{score.Temps.Seconds:00}",
                score.Essais.ToString(),
                score.Date.ToString("dd/MM/yy")
            };
            int[] posX = { 5, 35, 145, 240, 320 };

            for (int i = 0; i < vals.Length; i++)
            {
                var l = new Label();
                l.Text = vals[i];
                l.Font = new Font("Courier New", 9, rang == 1 ? FontStyle.Bold : FontStyle.Regular);
                l.ForeColor = couleur;
                l.AutoSize = true;
                l.Location = new Point(posX[i], y);
                panelScores.Controls.Add(l);
            }
        }

        // ── Fond animé ─────────────────────────────────────────────────

        private void InitialiserAnim()
        {
            _timerAnim = new Timer();
            _timerAnim.Interval = 30;
            _timerAnim.Tick += (s, e) => { _animTick++; this.Invalidate(false); };
            _timerAnim.Start();
        }

        private void DessinerFond(Graphics g)
        {
            using (var pen = new Pen(Color.FromArgb(8, 0, 255, 106), 1))
            {
                for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
            }
            int scanY = (_animTick * 2) % Height;
            using (var sp = new Pen(Color.FromArgb(20, 0, 255, 106), 1))
                g.DrawLine(sp, 0, scanY, Width, scanY);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerAnim?.Stop();
            base.OnFormClosing(e);
        }
    }
}