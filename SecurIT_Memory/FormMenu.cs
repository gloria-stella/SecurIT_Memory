using System;
using System.Drawing;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    /// <summary>
    /// Menu principal de l'application SecurIT Memory.
    /// Donne accès à : Jouer, Leaderboard, Options, Quitter.
    /// </summary>
    public class FormMenu : Form
    {
        private Label _lblTitre;
        private Label _lblSousTitre;
        private Button _btnJouer;
        private Button _btnLeaderboard;
        private Button _btnOptions;
        private Button _btnQuitter;
        private Label _lblVersion;
        private Timer _timerAnim;
        private int _animTick = 0;

        // Paramètres de jeu (choisis dans Options)
        private int _tailleGrille = 4;
        private ThemeCartes _theme = ThemeCartes.Cybersecurite;
        private bool _hardcore = false;

        // Palette accessible par les autres classes
        public static readonly Color NOIR = ThemeCyber.NOIR;
        public static readonly Color VERT_NEON = ThemeCyber.VERT_NEON;
        public static readonly Color BLEU_NEON = ThemeCyber.BLEU_NEON;
        public static readonly Color ROUGE_NEON = ThemeCyber.ROUGE_NEON;

        public FormMenu()
        {
            ScoreManager.Initialiser();
            InitialiserComposants();
            InitialiserAnim();
        }

        private void InitialiserComposants()
        {
            this.Text = "SecurIT Memory";
            this.Size = new Size(500, 660);
            this.MinimumSize = new Size(500, 660);
            this.MaximumSize = new Size(500, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ThemeCyber.NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.Paint += FormMenu_Paint;

            _lblTitre = new Label();
            _lblTitre.Text = "SECURIT";
            _lblTitre.Font = new Font("Courier New", 38, FontStyle.Bold);
            _lblTitre.ForeColor = ThemeCyber.VERT_NEON;
            _lblTitre.BackColor = Color.Transparent;
            _lblTitre.AutoSize = true;
            _lblTitre.Location = new Point(105, 55);

            _lblSousTitre = new Label();
            _lblSousTitre.Text = "MEMORY CHALLENGE  —  CYBER EDITION";
            _lblSousTitre.Font = new Font("Courier New", 8);
            _lblSousTitre.ForeColor = ThemeCyber.BLEU_NEON;
            _lblSousTitre.BackColor = Color.Transparent;
            _lblSousTitre.AutoSize = true;
            _lblSousTitre.Location = new Point(82, 118);

            _btnJouer = CreerBouton("►  JOUER", 200, ThemeCyber.VERT_NEON);
            _btnLeaderboard = CreerBouton("🏆  LEADERBOARD", 280, ThemeCyber.VIOLET_NEON);
            _btnOptions = CreerBouton("⚙  OPTIONS", 360, ThemeCyber.BLEU_NEON);
            _btnQuitter = CreerBouton("✕  QUITTER", 440, ThemeCyber.ROUGE_NEON);

            _btnJouer.Click += BtnJouer_Click;
            _btnLeaderboard.Click += BtnLeaderboard_Click;
            _btnOptions.Click += BtnOptions_Click;
            _btnQuitter.Click += BtnQuitter_Click;

            _lblVersion = new Label();
            _lblVersion.Text = "// SecurIT v1.0  —  Salon de l'Innovation Tech";
            _lblVersion.Font = new Font("Courier New", 7);
            _lblVersion.ForeColor = Color.FromArgb(0, 80, 40);
            _lblVersion.BackColor = Color.Transparent;
            _lblVersion.AutoSize = true;
            _lblVersion.Location = new Point(90, 615);

            this.Controls.Add(_lblTitre);
            this.Controls.Add(_lblSousTitre);
            this.Controls.Add(_btnJouer);
            this.Controls.Add(_btnLeaderboard);
            this.Controls.Add(_btnOptions);
            this.Controls.Add(_btnQuitter);
            this.Controls.Add(_lblVersion);
        }

        private void FormMenu_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.FromArgb(10, 0, 255, 106), 1))
            {
                for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
            }
            int scanY = (_animTick * 3) % Height;
            using (var sp = new Pen(Color.FromArgb(35, 0, 255, 106), 1))
                g.DrawLine(sp, 0, scanY, Width, scanY);
            using (var lp = new Pen(Color.FromArgb(70, 0, 255, 106), 1))
                g.DrawLine(lp, 50, 152, Width - 50, 152);

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

        private void InitialiserAnim()
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
            btn.Size = new Size(280, 52);
            btn.Location = new Point(110, posY);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void BtnJouer_Click(object sender, EventArgs e)
        {
            _timerAnim.Stop();
            new FormJeu(_tailleGrille, _theme, _hardcore).ShowDialog(this);
            _timerAnim.Start();
        }

        private void BtnLeaderboard_Click(object sender, EventArgs e)
        {
            new FormLeaderboard().ShowDialog(this);
        }

        private void BtnOptions_Click(object sender, EventArgs e)
        {
            var fo = new FormOptions(_tailleGrille, _theme, _hardcore);
            if (fo.ShowDialog(this) == DialogResult.OK)
            {
                _tailleGrille = fo.TailleGrilleChoisie;
                _theme = fo.ThemeChoisi;
                _hardcore = fo.ModeHardcoreChoisi;
            }
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