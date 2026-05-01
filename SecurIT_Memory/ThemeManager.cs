namespace SecurIT_Memory
{
    public static class ThemeManager
    {
        public static (string Emoji, string Nom, bool RedTeam)[] GetIcones(ThemeType theme)
        {
            switch (theme)
            {
                case ThemeType.Materiel:
                    return new[]
                    {
                        ("💾", "Disque", false),
                        ("🧠", "CPU", false),
                        ("🖨", "Imprimante", false),
                        ("📡", "Antenne", false),
                    };

                case ThemeType.Logiciel:
                    return new[]
                    {
                        ("🪟", "OS", false),
                        ("🧩", "Plugin", false),
                        ("🐞", "Bug", true),
                        ("🚫", "Crash", true),
                    };

                case ThemeType.Crypto:
                    return new[]
                    {
                        ("🔑", "Clé", false),
                        ("🔐", "Chiffrement", false),
                        ("📜", "Certificat", false),
                        ("🧮", "Algorithme", true),
                    };

                default:
                    return new[]
                    {
                        ("🛡", "Pare-feu", false),
                        ("🔐", "Chiffrement", false),
                        ("🔒", "Cadenas", false),
                        ("🌐", "VPN", false),
                        ("🎣", "Phishing", true),
                        ("🦠", "Virus", true),
                    };
            }
        }
    }
}
