using System;
using System.Drawing;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    public class FormMenu : Form
    {
        private Label lblTitre;
        private Label lblSousTitre;
        private Button btnJouer;
        private Button btnOptions;
        private Button btnQuitter;
        private Label lblVersion;
        private Timer _timerAnim;
        private int _animTick = 0;
        private int _tailleGrille = 4;

        // Palette Cyber Neon
        public static readonly Color NOIR = Color.FromArgb(5, 5, 5);
        public static readonly Color VERT_NEON = Color.FromArgb(0, 255, 106);
        public static readonly Color BLEU_NEON = Color.FromArgb(0, 229, 255);
        public static readonly Color ROUGE_NEON = Color.FromArgb(255, 0, 76);
        public static readonly Color VIOLET_NEON = Color.FromArgb(179, 0, 255);
        public static readonly Color GRIS_DARK = Color.FromArgb(26, 26, 26);

        public FormMenu()
        {
            InitialiserComposants();
            InitialiserTimerAnim();
        }

        private void InitialiserComposants()
        {
            this.Text = "SecurIT Memory";
            this.Size = new Size(500, 620);
            this.MinimumSize = new Size(500, 620);
            this.MaximumSize = new Size(500, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.Paint += FormMenu_Paint;

            lblTitre = new Label();
            lblTitre.Text = "SECURIT";
            lblTitre.Font = new Font("Courier New", 38, FontStyle.Bold);
            lblTitre.ForeColor = VERT_NEON;
            lblTitre.BackColor = Color.Transparent;
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(105, 55);

            lblSousTitre = new Label();
            lblSousTitre.Text = "MEMORY CHALLENGE  —  CYBER EDITION";
            lblSousTitre.Font = new Font("Courier New", 8);
            lblSousTitre.ForeColor = BLEU_NEON;
            lblSousTitre.BackColor = Color.Transparent;
            lblSousTitre.AutoSize = true;
            lblSousTitre.Location = new Point(82, 118);

            btnJouer = CreerBouton("►  JOUER", 200, VERT_NEON);
            btnOptions = CreerBouton("⚙  OPTIONS", 290, BLEU_NEON);
            btnQuitter = CreerBouton("✕  QUITTER", 380, ROUGE_NEON);

            btnJouer.Click += BtnJouer_Click;
            btnOptions.Click += BtnOptions_Click;
            btnQuitter.Click += BtnQuitter_Click;

            lblVersion = new Label();
            lblVersion.Text = "// SecurIT v1.0  —  Salon de l'Innovation Tech";
            lblVersion.Font = new Font("Courier New", 7);
            lblVersion.ForeColor = Color.FromArgb(0, 80, 40);
            lblVersion.BackColor = Color.Transparent;
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(90, 572);

            this.Controls.Add(lblTitre);
            this.Controls.Add(lblSousTitre);
            this.Controls.Add(btnJouer);
            this.Controls.Add(btnOptions);
            this.Controls.Add(btnQuitter);
            this.Controls.Add(lblVersion);
        }

        private void FormMenu_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Grille de fond verte fantôme
            using (Pen pen = new Pen(Color.FromArgb(10, 0, 255, 106), 1))
            {
                for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
            }

            // Ligne de scan animée
            int scanY = (_animTick * 3) % Height;
            using (Pen sp = new Pen(Color.FromArgb(35, 0, 255, 106), 1))
                g.DrawLine(sp, 0, scanY, Width, scanY);

            // Séparateur sous le sous-titre
            using (Pen lp = new Pen(Color.FromArgb(70, 0, 255, 106), 1))
                g.DrawLine(lp, 50, 152, Width - 50, 152);

            // Coins décoratifs
            DessinerCoin(g, 10, 10, 1, 1);
            DessinerCoin(g, Width - 26, 10, -1, 1);
            DessinerCoin(g, 10, Height - 26, 1, -1);
            DessinerCoin(g, Width - 26, Height - 26, -1, -1);
        }

        private void DessinerCoin(Graphics g, int x, int y, int dx, int dy)
        {
            using (Pen p = new Pen(Color.FromArgb(130, 0, 255, 106), 1))
            {
                g.DrawLine(p, x, y, x + dx * 16, y);
                g.DrawLine(p, x, y, x, y + dy * 16);
            }
        }

        private void InitialiserTimerAnim()
        {
            _timerAnim = new Timer();
            _timerAnim.Interval = 30;
            _timerAnim.Tick += (s, e) => { _animTick++; this.Invalidate(); };
            _timerAnim.Start();
        }

        private Button CreerBouton(string texte, int posY, Color couleur)
        {
            var btn = new Button();
            btn.Text = texte;
            btn.Font = new Font("Courier New", 12, FontStyle.Bold);
            btn.ForeColor = couleur;
            btn.BackColor = Color.FromArgb(8, couleur.R, couleur.G, couleur.B);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = couleur;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, couleur.R, couleur.G, couleur.B);
            btn.Size = new Size(280, 55);
            btn.Location = new Point(110, posY);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void BtnJouer_Click(object sender, EventArgs e)
        {
            _timerAnim.Stop();
            new FormJeu(_tailleGrille).ShowDialog(this);
            _timerAnim.Start();
        }

        private void BtnOptions_Click(object sender, EventArgs e)
        {
            var fo = new FormOptions(_tailleGrille);
            if (fo.ShowDialog(this) == DialogResult.OK)
                _tailleGrille = fo.TailleGrilleChoisie;
        }

        private void BtnQuitter_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Quitter SecurIT Memory ?", "Confirmer",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerAnim?.Stop();
            base.OnFormClosing(e);
        }
    }
}