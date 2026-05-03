using System.Drawing;

namespace SecurIT_Memory
{
    
    // Classe statique centralisant toutes les couleurs et constantes visuelles du thème Cyber Neon
    // Permet de modifier le style en un seul endroit
    public static class ThemeCyber
    {
        // ── Palette de couleurs 
        public static readonly Color NOIR = Color.FromArgb(5, 5, 5);
        public static readonly Color GRIS_DARK = Color.FromArgb(26, 26, 26);
        public static readonly Color VERT_NEON = Color.FromArgb(0, 255, 106);
        public static readonly Color BLEU_NEON = Color.FromArgb(0, 229, 255);
        public static readonly Color ROUGE_NEON = Color.FromArgb(255, 0, 76);
        public static readonly Color VIOLET_NEON = Color.FromArgb(179, 0, 255);
        public static readonly Color BLANC = Color.White;
        public static readonly Color VERT_SOMBRE = Color.FromArgb(0, 80, 40);

        // ── Couleurs spécifiques Blue Team / Red Team
        public static readonly Color FOND_BLUE_TEAM = Color.FromArgb(15, 0, 100, 200);
        public static readonly Color BORDURE_BLUE_TEAM = Color.FromArgb(0, 150, 255);
        public static readonly Color FOND_RED_TEAM = Color.FromArgb(30, 200, 0, 50);
        public static readonly Color BORDURE_RED_TEAM = Color.FromArgb(255, 0, 76);
        public static readonly Color FOND_TROUVEE = Color.FromArgb(40, 179, 0, 255);
        public static readonly Color BORDURE_TROUVEE = Color.FromArgb(179, 0, 255);

        // ── Couleurs grille de fond 
        public static readonly Color GRILLE_FOND = Color.FromArgb(8, 0, 255, 106);
        public static readonly Color SCAN_LIGNE = Color.FromArgb(20, 0, 255, 106);
        public static readonly Color DOS_CARTE = Color.FromArgb(40, 0, 255, 106);
    }

    
    // Enumération des thèmes de cartes disponibles dans les Options.
   
    public enum ThemeCartes
    {
        Cybersecurite,   // Thème par défaut : virus, pare-feu, etc.
        Materiel,        // RAM, CPU, GPU, SSD...
        Logiciel,        // OS, apps, navigateur...
        Cryptographie    // Clés, algorithmes, certificats...
    }

   
    // Données d'une icône de carte : emoji, nom, appartenance Red Team, thème.
    
    public class IconeCarte
    {
        public string Emoji { get; set; } // creato dune proprité ( get = lire la valeur et set = modifier la valeur)
        public string Nom { get; set; }
        public bool RedTeam { get; set; }
        public ThemeCartes Theme { get; set; }

        public IconeCarte(string emoji, string nom, bool redTeam, ThemeCartes theme)
        {
            Emoji = emoji;
            Nom = nom;
            RedTeam = redTeam;
            Theme = theme;
        }
    }

    
    // Catalogue de toutes les icônes disponibles pour chaque thème.
    
    public static class CatalogueIcones
    {
        public static readonly IconeCarte[] Toutes = new IconeCarte[]
        {
            // ── Thème Cybersécurité (défaut) , la methode parTheme() filtre 
            // ce catalogue pour ne retourner que les icones du thème sélectionné dans les options
            new IconeCarte("🛡",  "Pare-feu",    false, ThemeCartes.Cybersecurite),
            new IconeCarte("🔐", "Chiffrement",  false, ThemeCartes.Cybersecurite),
            new IconeCarte("🔒", "Cadenas",      false, ThemeCartes.Cybersecurite),
            new IconeCarte("🌐", "VPN",          false, ThemeCartes.Cybersecurite),
            new IconeCarte("🔑", "Mot de passe", false, ThemeCartes.Cybersecurite),
            new IconeCarte("📱", "MFA",          false, ThemeCartes.Cybersecurite),
            new IconeCarte("🔍", "Audit",        false, ThemeCartes.Cybersecurite),
            new IconeCarte("📜", "Certificat",   false, ThemeCartes.Cybersecurite),
            new IconeCarte("💾", "Backup",       false, ThemeCartes.Cybersecurite),
            new IconeCarte("🎣", "Phishing",     true,  ThemeCartes.Cybersecurite),
            new IconeCarte("🦠", "Virus",        true,  ThemeCartes.Cybersecurite),
            new IconeCarte("👾", "Hacker",       true,  ThemeCartes.Cybersecurite),
            new IconeCarte("💀", "Ransomware",   true,  ThemeCartes.Cybersecurite),
            new IconeCarte("🐛", "Malware",      true,  ThemeCartes.Cybersecurite),
            new IconeCarte("⚡", "DDoS",         true,  ThemeCartes.Cybersecurite),
            new IconeCarte("🐴", "Trojan",       true,  ThemeCartes.Cybersecurite),
            new IconeCarte("📡", "Botnet",       true,  ThemeCartes.Cybersecurite),
            new IconeCarte("🧬", "Zero-Day",     true,  ThemeCartes.Cybersecurite),

            // ── Thème Matériel ─────────────────────────────────────────
            new IconeCarte("🖥",  "CPU",         false, ThemeCartes.Materiel),
            new IconeCarte("💾", "RAM",          false, ThemeCartes.Materiel),
            new IconeCarte("🖨",  "GPU",         false, ThemeCartes.Materiel),
            new IconeCarte("💿", "SSD",          false, ThemeCartes.Materiel),
            new IconeCarte("🖱",  "Souris",      false, ThemeCartes.Materiel),
            new IconeCarte("⌨",  "Clavier",     false, ThemeCartes.Materiel),
            new IconeCarte("📡", "Wi-Fi",        false, ThemeCartes.Materiel),
            new IconeCarte("🔌", "USB",          false, ThemeCartes.Materiel),
            new IconeCarte("🖲",  "Routeur",     true,  ThemeCartes.Materiel),
            new IconeCarte("📟", "Serveur",      true,  ThemeCartes.Materiel),
            new IconeCarte("🔋", "Batterie",     false, ThemeCartes.Materiel),
            new IconeCarte("📺", "Ecran",        false, ThemeCartes.Materiel),
            new IconeCarte("📷", "Webcam",       true,  ThemeCartes.Materiel),
            new IconeCarte("🎙", "Micro",        true,  ThemeCartes.Materiel),
            new IconeCarte("🔧", "Carte mère",   false, ThemeCartes.Materiel),
            new IconeCarte("💡", "LED",          false, ThemeCartes.Materiel),
            new IconeCarte("🧲", "Disque dur",   false, ThemeCartes.Materiel),
            new IconeCarte("📠", "Imprimante",   true,  ThemeCartes.Materiel),

            // ── Thème Logiciel ─────────────────────────────────────────
            new IconeCarte("🪟", "Windows",      false, ThemeCartes.Logiciel),
            new IconeCarte("🐧", "Linux",        false, ThemeCartes.Logiciel),
            new IconeCarte("🍎", "MacOS",        false, ThemeCartes.Logiciel),
            new IconeCarte("🌍", "Navigateur",   false, ThemeCartes.Logiciel),
            new IconeCarte("📧", "Email",        false, ThemeCartes.Logiciel),
            new IconeCarte("📁", "Fichier",      false, ThemeCartes.Logiciel),
            new IconeCarte("🗄",  "Base données", false, ThemeCartes.Logiciel),
            new IconeCarte("⚙",  "Paramètres",  false, ThemeCartes.Logiciel),
            new IconeCarte("🐞", "Bug",          true,  ThemeCartes.Logiciel),
            new IconeCarte("🔄", "Update",       false, ThemeCartes.Logiciel),
            new IconeCarte("📦", "Package",      false, ThemeCartes.Logiciel),
            new IconeCarte("🐋", "Docker",       false, ThemeCartes.Logiciel),
            new IconeCarte("🐙", "Git",          false, ThemeCartes.Logiciel),
            new IconeCarte("☁",  "Cloud",        false, ThemeCartes.Logiciel),
            new IconeCarte("🤖", "IA",           true,  ThemeCartes.Logiciel),
            new IconeCarte("📊", "Dashboard",    false, ThemeCartes.Logiciel),
            new IconeCarte("🔑", "API Key",      true,  ThemeCartes.Logiciel),
            new IconeCarte("💬", "Chat",         false, ThemeCartes.Logiciel),

            // ── Thème Cryptographie ────────────────────────────────────
            new IconeCarte("🗝",  "Clé privée",  false, ThemeCartes.Cryptographie),
            new IconeCarte("🔓", "Clé publique", false, ThemeCartes.Cryptographie),
            new IconeCarte("📜", "Certificat",   false, ThemeCartes.Cryptographie),
            new IconeCarte("🔐", "AES",          false, ThemeCartes.Cryptographie),
            new IconeCarte("🔏", "RSA",          false, ThemeCartes.Cryptographie),
            new IconeCarte("#️⃣",  "Hash",         false, ThemeCartes.Cryptographie),
            new IconeCarte("✍",  "Signature",   false, ThemeCartes.Cryptographie),
            new IconeCarte("🔢", "SHA-256",      false, ThemeCartes.Cryptographie),
            new IconeCarte("🧮", "Algorithme",   false, ThemeCartes.Cryptographie),
            new IconeCarte("🔀", "Chiffrement",  false, ThemeCartes.Cryptographie),
            new IconeCarte("🪙", "Token",        true,  ThemeCartes.Cryptographie),
            new IconeCarte("🧩", "XOR",          false, ThemeCartes.Cryptographie),
            new IconeCarte("🌀", "SSL/TLS",      false, ThemeCartes.Cryptographie),
            new IconeCarte("🛂", "PKI",          false, ThemeCartes.Cryptographie),
            new IconeCarte("🪪", "Identité",     false, ThemeCartes.Cryptographie),
            new IconeCarte("🔭", "Blockchain",   false, ThemeCartes.Cryptographie),
            new IconeCarte("💎", "Bitcoin",      true,  ThemeCartes.Cryptographie),
            new IconeCarte("🧬", "DNA-key",      true,  ThemeCartes.Cryptographie),
        };

        // Retourne les icônes filtrées par thème
        public static IconeCarte[] ParTheme(ThemeCartes theme) 
        {
            var liste = new System.Collections.Generic.List<IconeCarte>(); // creation d'une liste vide qui va contenir les icones filtrées 
            foreach (var ic in Toutes)
                if (ic.Theme == theme) liste.Add(ic);
            return liste.ToArray();
        }
    }
}