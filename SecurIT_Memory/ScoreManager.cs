using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SecurIT_Memory
{
    /// <summary>Représente une entrée du leaderboard.</summary>
    public class EntreeScore
    {
        public string Nom { get; set; }
        public TimeSpan Temps { get; set; }
        public int Essais { get; set; }
        public int Grille { get; set; }
        public DateTime Date { get; set; }

        // le constructeur  qui permet de créer une entrée de score
        public EntreeScore(string nom, TimeSpan temps, int essais, int grille, DateTime date)
        {
            Nom = nom;
            Temps = temps;
            Essais = essais;
            Grille = grille;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Nom} | {Temps.Minutes:00}:{Temps.Seconds:00} | {Essais} essais | {Grille}x{Grille}"; // nous sert a afficher un score proprement 
        }
    }

    
    // Gestionnaire des scores et du leaderboard.
    
    public static class ScoreManager 
    {
        //  Chemins ou sont  stocker les scores 
        private static readonly string DB_PATH = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "scores.db");
        private static readonly string SCORES_TXT = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "scores.txt");

        private static bool _sqlDisponible = false;

        // Initialisation

        // Initialise la base de données ( Le jeu essaie d’utiliser SQLite et si ça échoue il utilisera un fichier texte)
        public static void Initialiser()
        {
            try
            {
                CreerTableSiInexistante();
                _sqlDisponible = true;
            }
            catch
            {
                _sqlDisponible = false; 
            }
        }

        //  Sauvegarde d'un score 

        // Sauvegarde un score dans la base de données ou le fichier texte 
        public static void SauvegarderScore(string nom, TimeSpan temps, int essais, int grille)
        {
            try
            {
                if (_sqlDisponible)
                    SauvegarderSQL(nom, temps, essais, grille);
                else
                    SauvegarderFichier(nom, temps, essais, grille);
            }
            catch { }
        }

        
        // Vérifie si un score bat le record actuel pour une taille de grille
        // Critère principal : temps. Critère secondaire : essais
       
        public static bool EstNouveauRecord(TimeSpan temps, int essais, int grille)
        {
            try
            {
                var meilleur = ObtenirMeilleurScore(grille);
                if (meilleur == null) return true; // Pas encore de record
                if (temps < meilleur.Temps) return true;
                if (temps == meilleur.Temps && essais < meilleur.Essais) return true;
                return false;
            }
            catch { return false; }
        }

        /// <summary>Retourne le meilleur score pour une taille de grille donnée.</summary>
        public static EntreeScore ObtenirMeilleurScore(int grille)
        {
            try
            {
                if (_sqlDisponible)
                    return ObtenirMeilleurSQL(grille);
                else
                    return ObtenirMeilleurFichier(grille);
            }
            catch { return null; }
        }

        /// <summary>Retourne les 10 meilleurs scores (leaderboard).</summary>
        public static List<EntreeScore> ObtenirLeaderboard(int grille = 4)
        {
            try
            {
                if (_sqlDisponible)
                    return ObtenirLeaderboardSQL(grille);
                else
                    return ObtenirLeaderboardFichier(grille);
            }
            catch { return new List<EntreeScore>(); }
        }

        // SQLite 

        private static void CreerTableSiInexistante()
        {
            using (var conn = new SQLiteConnection($"Data Source={DB_PATH};Version=3;"))
            {
                conn.Open(); // creer la table si elle n'existe pas 
                string sql = @"CREATE TABLE IF NOT EXISTS scores ( 
                    id      INTEGER PRIMARY KEY AUTOINCREMENT,
                    nom     TEXT    NOT NULL,
                    temps   INTEGER NOT NULL,
                    essais  INTEGER NOT NULL,
                    grille  INTEGER NOT NULL,
                    date    TEXT    NOT NULL
                );";
                new SQLiteCommand(sql, conn).ExecuteNonQuery();
            }
        }

        private static void SauvegarderSQL(string nom, TimeSpan temps, int essais, int grille)
        {
            using (var conn = new SQLiteConnection($"Data Source={DB_PATH};Version=3;"))
            {
                conn.Open();
                string sql = "INSERT INTO scores (nom, temps, essais, grille, date) VALUES (@nom, @temps, @essais, @grille, @date)";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@temps", (long)temps.TotalSeconds);
                cmd.Parameters.AddWithValue("@essais", essais);
                cmd.Parameters.AddWithValue("@grille", grille);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
            }
        }

        private static EntreeScore ObtenirMeilleurSQL(int grille)
        {
            using (var conn = new SQLiteConnection($"Data Source={DB_PATH};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT nom, temps, essais, grille, date FROM scores WHERE grille=@g ORDER BY temps ASC, essais ASC LIMIT 1";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@g", grille);
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                        return new EntreeScore(
                            r["nom"].ToString(),
                            TimeSpan.FromSeconds(Convert.ToInt64(r["temps"])),
                            Convert.ToInt32(r["essais"]),
                            grille,
                            DateTime.Parse(r["date"].ToString()));
                }
            }
            return null;
        }

        private static List<EntreeScore> ObtenirLeaderboardSQL(int grille)
        {
            var liste = new List<EntreeScore>();
            using (var conn = new SQLiteConnection($"Data Source={DB_PATH};Version=3;"))
            {
                conn.Open();
                string sql = "SELECT nom, temps, essais, grille, date FROM scores WHERE grille=@g ORDER BY temps ASC, essais ASC LIMIT 10";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@g", grille);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        liste.Add(new EntreeScore(
                            r["nom"].ToString(),
                            TimeSpan.FromSeconds(Convert.ToInt64(r["temps"])),
                            Convert.ToInt32(r["essais"]),
                            grille,
                            DateTime.Parse(r["date"].ToString())));
                }
            }
            return liste;
        }

        // ── Fallback fichier texte ─────────────────────────────────────

        private static void SauvegarderFichier(string nom, TimeSpan temps, int essais, int grille)
        {
            string ligne = $"{nom}|{(long)temps.TotalSeconds}|{essais}|{grille}|{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            File.AppendAllText(SCORES_TXT, ligne + Environment.NewLine);
        }

        private static List<EntreeScore> LireScoresFichier()
        {
            var liste = new List<EntreeScore>();
            if (!File.Exists(SCORES_TXT)) return liste;
            foreach (string ligne in File.ReadAllLines(SCORES_TXT))
            {
                string[] parts = ligne.Split('|');
                if (parts.Length < 5) continue;
                try
                {
                    liste.Add(new EntreeScore(
                        parts[0],
                        TimeSpan.FromSeconds(long.Parse(parts[1])),
                        int.Parse(parts[2]),
                        int.Parse(parts[3]),
                        DateTime.Parse(parts[4])));
                }
                catch { }
            }
            return liste;
        }

        private static EntreeScore ObtenirMeilleurFichier(int grille)
        {
            EntreeScore meilleur = null;
            foreach (var s in LireScoresFichier())
            {
                if (s.Grille != grille) continue;
                if (meilleur == null || s.Temps < meilleur.Temps ||
                    (s.Temps == meilleur.Temps && s.Essais < meilleur.Essais))
                    meilleur = s;
            }
            return meilleur;
        }

        private static List<EntreeScore> ObtenirLeaderboardFichier(int grille)
        {
            var liste = LireScoresFichier();
            liste.RemoveAll(s => s.Grille != grille);
            liste.Sort((a, b) => {
                int cmp = a.Temps.CompareTo(b.Temps);
                return cmp != 0 ? cmp : a.Essais.CompareTo(b.Essais);
            });
            if (liste.Count > 10) liste.RemoveRange(10, liste.Count - 10);
            return liste;
        }
    }
}