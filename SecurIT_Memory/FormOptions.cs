using System;
using System.Drawing;
using System.Windows.Forms;

namespace SecurIT_Memory
{
    public class FormOptions : Form
    {
        private Label lblTitre; // choix de la grille 
        private RadioButton rb4x4;
        private RadioButton rb6x6;
        private Button btnValider;
        private Button btnAnnuler;
        private Timer _timerAnim; // anime lle fond du formulaire avec des lignes et un scan lumineux
        private int _animTick = 0;

        public int TailleGrilleChoisie { get; private set; } // 4 ou 6 selon le choix de l'utilisateur

        // constructeur
        public FormOptions(int tailleActuelle) // il recupere la taille actuelle pour precocher le bon bouton
        {
            TailleGrilleChoisie = tailleActuelle;
            InitialiserComposants(); // il construil l'interface avec iniatilisercomposants et il demarre l'animation avec initialisertimeranim
            if (tailleActuelle == 6) rb6x6.Checked = true;
            else rb4x4.Checked = true;

            _timerAnim = new Timer();
            _timerAnim.Interval = 30;
            _timerAnim.Tick += (s, e) => { _animTick++; this.Invalidate(); };
            _timerAnim.Start();
        }

        private void InitialiserComposants()
        {
            this.Text = "Options — SecurIT Memory"; // taille fixe , position centrée, fond noir, bordure fixe, pas de redimensionnement, double buffering pour eviter les scintillements
            this.Size = new Size(400, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = FormMenu.NOIR;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;
            this.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (var pen = new Pen(Color.FromArgb(10, 0, 255, 106), 1))
                {
                    for (int x = 0; x < Width; x += 40) g.DrawLine(pen, x, 0, x, Height);
                    for (int y = 0; y < Height; y += 40) g.DrawLine(pen, 0, y, Width, y);
                }
                int scanY = (_animTick * 3) % Height;
                using (var sp = new Pen(Color.FromArgb(30, 0, 255, 106), 1))
                    g.DrawLine(sp, 0, scanY, Width, scanY);
            };

            lblTitre = new Label();
            lblTitre.Text = "OPTIONS";
            lblTitre.Font = new Font("Courier New", 20, FontStyle.Bold);
            lblTitre.ForeColor = FormMenu.VERT_NEON;
            lblTitre.BackColor = Color.Transparent;
            lblTitre.AutoSize = true;
            lblTitre.Location = new Point(130, 20);

            var lblGrille = new Label();
            lblGrille.Text = "// TAILLE DE LA GRILLE";
            lblGrille.Font = new Font("Courier New", 9);
            lblGrille.ForeColor = Color.FromArgb(0, 160, 80);
            lblGrille.BackColor = Color.Transparent;
            lblGrille.AutoSize = true;
            lblGrille.Location = new Point(40, 80);

            rb4x4 = CreerRadio("4 x 4  —  8 paires   [ FACILE ]", 115);
            rb6x6 = CreerRadio("6 x 6  —  18 paires  [ DIFFICILE ]", 155);

            btnValider = CreerBouton("✓  VALIDER", 220, FormMenu.VERT_NEON);
            btnValider.DialogResult = DialogResult.OK; // qd on clique la fenetre renvoie ok , la taille chois est enregistre et lanimation s'arrete 
            btnValider.Click += (s, e) =>
            {
                TailleGrilleChoisie = rb6x6.Checked ? 6 : 4;
                _timerAnim?.Stop();
            };

            btnAnnuler = CreerBouton("✕  ANNULER", 220, FormMenu.ROUGE_NEON);
            btnAnnuler.Location = new Point(220, 220);
            btnAnnuler.DialogResult = DialogResult.Cancel;
            btnAnnuler.Click += (s, e) => _timerAnim?.Stop();

            this.Controls.Add(lblTitre);
            this.Controls.Add(lblGrille);
            this.Controls.Add(rb4x4);
            this.Controls.Add(rb6x6);
            this.Controls.Add(btnValider);
            this.Controls.Add(btnAnnuler);
        }

        private RadioButton CreerRadio(string texte, int posY)
        {
            var rb = new RadioButton();
            rb.Text = texte;
            rb.Font = new Font("Courier New", 10);
            rb.ForeColor = Color.FromArgb(200, 255, 255, 255);
            rb.BackColor = Color.Transparent;
            rb.AutoSize = true;
            rb.Location = new Point(60, posY);
            return rb;
        }

        private Button CreerBouton(string texte, int posY, Color couleur) //CreerBouton() est une méthode utilitaire qui crée un bouton stylé Cyber Neon sans reecrire l code plusieurs fois 
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
            btn.Size = new Size(150, 45);
            btn.Location = new Point(40, posY);
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}