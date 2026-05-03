using System;
using System.Drawing;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    // Formulaire d'options : taille de grille, thème des cartes, mode Hardcore
    public class FormOptions : Form
    {
        // Contrôles
        private Label lblTitre;
        private Label lblGrille;
        private RadioButton rb4x4;
        private RadioButton rb6x6;
        private Label lblTheme;
        private RadioButton rbCyber;
        private RadioButton rbMateriel;
        private RadioButton rbLogiciel;
        private RadioButton rbCrypto;
        private Label lblHardcore;
        private CheckBox chkHardcore;
        private Label lblHardcoreInfo;
        private Button btnValider;
        private Button btnAnnuler;
        private Timer _timerAnim;
        private int _animTick = 0;

        // Résultats 
        public int TailleGrilleChoisie { get; private set; } // quand on valide formOption renvoie ces valeurs a formMenu
        public ThemeCartes ThemeChoisi { get; private set; }
        public bool ModeHardcoreChoisi { get; private set; }

        // constructeur du formOption qui prend en paramètre les valeurs actuelles pour les pré-sélectionner dans le formulaire
        public FormOptions(int tailleActuelle, ThemeCartes themeActuel, bool hardcoreActuel)
        {
            TailleGrilleChoisie = tailleActuelle;
            ThemeChoisi = themeActuel;
            ModeHardcoreChoisi = hardcoreActuel;

            InitialiserComposants();
            PreSelectionner(tailleActuelle, themeActuel, hardcoreActuel);
            InitialiserAnim();
        }

        private void InitialiserComposants()
        {
            this.Text = "Options — SecurIT Memory"; // config de la fenetre 
            this.Size = new Size(440, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeCyber.NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;
            this.Paint += FormOptions_Paint;

            // Titre 
            lblTitre = CreerLabel("OPTIONS", 20, 18, ThemeCyber.VERT_NEON, true);
            lblTitre.Font = new Font("Courier New", 20, FontStyle.Bold);

            // Section Grille 
            lblGrille = CreerLabel("// TAILLE DE LA GRILLE", 70, 10, Color.FromArgb(0, 160, 80));
            rb4x4 = CreerRadio("4 × 4  —  8 paires   [ FACILE ]", 100);
            rb6x6 = CreerRadio("6 × 6  —  18 paires  [ DIFFICILE ]", 130);

            // Section Thème 
            lblTheme = CreerLabel("// THÈME DES CARTES", 175, 10, Color.FromArgb(0, 160, 80));
            rbCyber = CreerRadio("🔐  Cybersécurité  (par défaut)", 205);
            rbMateriel = CreerRadio("💾  Matériel         (RAM, CPU, GPU...)", 235);
            rbLogiciel = CreerRadio("🖥  Logiciel          (OS, Apps, Cloud...)", 265);
            rbCrypto = CreerRadio("🗝  Cryptographie  (Clés, Hash, TLS...)", 295);

            //  Section Hardcore 
            lblHardcore = CreerLabel("// MODE HARDCORE", 340, 10, Color.FromArgb(180, 0, 40));

            chkHardcore = new CheckBox();
            chkHardcore.Text = "Activer le mode HARDCORE";
            chkHardcore.Font = new Font("Courier New", 10, FontStyle.Bold);
            chkHardcore.ForeColor = ThemeCyber.ROUGE_NEON;
            chkHardcore.BackColor = Color.Transparent;
            chkHardcore.AutoSize = true;
            chkHardcore.Location = new Point(50, 368);

            lblHardcoreInfo = new Label();
            lblHardcoreInfo.Text = "⚠  Les cartes non-trouvées bougent toutes les 30s !";
            lblHardcoreInfo.Font = new Font("Courier New", 8);
            lblHardcoreInfo.ForeColor = Color.FromArgb(180, 60, 60);
            lblHardcoreInfo.BackColor = Color.Transparent;
            lblHardcoreInfo.AutoSize = true;
            lblHardcoreInfo.Location = new Point(50, 393);

            //  Boutons 
            btnValider = CreerBouton("✓  VALIDER", ThemeCyber.VERT_NEON, 40, 428);
            btnAnnuler = CreerBouton("✕  ANNULER", ThemeCyber.ROUGE_NEON, 230, 428);
            btnValider.DialogResult = DialogResult.OK;
            btnAnnuler.DialogResult = DialogResult.Cancel;
            btnValider.Click += BtnValider_Click;

            //  Ajout des contrôles 
            this.Controls.AddRange(new Control[] {
                lblTitre, lblGrille, rb4x4, rb6x6,
                lblTheme, rbCyber, rbMateriel, rbLogiciel, rbCrypto,
                lblHardcore, chkHardcore, lblHardcoreInfo,
                btnValider, btnAnnuler
            });
        }

        //  Helpers création contrôles 

        private Label CreerLabel(string texte, int y, int x, Color couleur, bool grand = false)
        {
            var l = new Label();
            l.Text = texte;
            l.Font = new Font("Courier New", grand ? 20 : 9);
            l.ForeColor = couleur;
            l.BackColor = Color.Transparent;
            l.AutoSize = true;
            l.Location = new Point(x == 20 ? 140 : x, y);
            return l;
        }

        private RadioButton CreerRadio(string texte, int y)
        {
            var rb = new RadioButton(); // creation d'un radio button pour les options de taille de grille et de thème
            rb.Text = texte;
            rb.Font = new Font("Courier New", 10);
            rb.ForeColor = Color.FromArgb(200, 200, 220);
            rb.BackColor = Color.Transparent;
            rb.AutoSize = true;
            rb.Location = new Point(50, y);
            return rb;
        }

        private Button CreerBouton(string texte, Color couleur, int x, int y)
        {
            var btn = new Button();
            btn.Text = texte;
            btn.Font = new Font("Courier New", 10, FontStyle.Bold);
            btn.ForeColor = couleur;
            btn.BackColor = Color.FromArgb(8, couleur.R, couleur.G, couleur.B);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = couleur;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, couleur.R, couleur.G, couleur.B);
            btn.Size = new Size(155, 42);
            btn.Location = new Point(x, y);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        // Présélection des valeurs actuelles

        private void PreSelectionner(int grille, ThemeCartes theme, bool hardcore)
        {
            if (grille == 6) rb6x6.Checked = true; else rb4x4.Checked = true;

            switch (theme)
            {
                case ThemeCartes.Materiel: rbMateriel.Checked = true; break;
                case ThemeCartes.Logiciel: rbLogiciel.Checked = true; break;
                case ThemeCartes.Cryptographie: rbCrypto.Checked = true; break;
                default: rbCyber.Checked = true; break;
            }

            chkHardcore.Checked = hardcore;
        }

        //  Animation fond 

        private void InitialiserAnim()
        {
            _timerAnim = new Timer();
            _timerAnim.Interval = 30;
            _timerAnim.Tick += (s, e) => { _animTick++; this.Invalidate(false); };
            _timerAnim.Start();
        }

        private void FormOptions_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.FromArgb(10, 0, 255, 106), 1))
            {
                for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
            }
            int scanY = (_animTick * 3) % Height;
            using (var sp = new Pen(Color.FromArgb(25, 0, 255, 106), 1))
                g.DrawLine(sp, 0, scanY, Width, scanY);

            // Lignes séparatrices entre sections
            using (var lp = new Pen(Color.FromArgb(40, 0, 255, 106), 1))
            {
                g.DrawLine(lp, 20, 160, Width - 20, 160);
                g.DrawLine(lp, 20, 330, Width - 20, 330);
            }
        }

        // Validation (recupere les choix du joueur et ferm la fenetre avc DialogResult.OK)
        private void BtnValider_Click(object sender, EventArgs e)
        {
            TailleGrilleChoisie = rb6x6.Checked ? 6 : 4;
            ModeHardcoreChoisi = chkHardcore.Checked;

            if (rbMateriel.Checked) ThemeChoisi = ThemeCartes.Materiel;
            else if (rbLogiciel.Checked) ThemeChoisi = ThemeCartes.Logiciel;
            else if (rbCrypto.Checked) ThemeChoisi = ThemeCartes.Cryptographie;
            else ThemeChoisi = ThemeCartes.Cybersecurite;

            _timerAnim?.Stop();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) // fermeture ( stoppe lanimation proprement)
        {
            _timerAnim?.Stop();
            base.OnFormClosing(e);
        }
    }
}