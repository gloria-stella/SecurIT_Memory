using System;

namespace SecurIT_Memory
{
    public class ScoreEntry
    {
        public int Id { get; set; }
        public string Joueur { get; set; }
        public int Grille { get; set; }
        public int Essais { get; set; }
        public int TempsSecondes { get; set; }
        public DateTime DatePartie { get; set; }
    }
}
