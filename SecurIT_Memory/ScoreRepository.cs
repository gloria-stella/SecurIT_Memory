using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace SecurIT_Memory
{
    public static class ScoreRepository
    {
        private static readonly string CheminDb =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scores.db");
        private static readonly string ConnStr = $"Data Source={CheminDb};Version=3;";

        public static void Initialiser()
        {
            if (!File.Exists(CheminDb))
                SQLiteConnection.CreateFile(CheminDb);

            using (var cn = new SQLiteConnection(ConnStr))
            {
                cn.Open();
                string sql = @"
CREATE TABLE IF NOT EXISTS Scores (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Joueur TEXT NOT NULL,
    Grille INTEGER NOT NULL,
    Essais INTEGER NOT NULL,
    TempsSecondes INTEGER NOT NULL,
    DatePartie TEXT NOT NULL
);";
                using (var cmd = new SQLiteCommand(sql, cn))
                    cmd.ExecuteNonQuery();
            }
        }

        public static void AjouterScore(ScoreEntry s)
        {
            using (var cn = new SQLiteConnection(ConnStr))
            {
                cn.Open();
                string sql = @"INSERT INTO Scores (Joueur, Grille, Essais, TempsSecondes, DatePartie)
                               VALUES (@j, @g, @e, @t, @d)";
                using (var cmd = new SQLiteCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@j", s.Joueur);
                    cmd.Parameters.AddWithValue("@g", s.Grille);
                    cmd.Parameters.AddWithValue("@e", s.Essais);
                    cmd.Parameters.AddWithValue("@t", s.TempsSecondes);
                    cmd.Parameters.AddWithValue("@d", s.DatePartie.ToString("s"));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
