# SecurIT Memory — Cyber Edition

> Jeu de cartes Memory sur le thème de la cybersécurité, développé en C# WinForms pour le **Salon de l'Innovation Tech** 
par les étudiantes : **NJUNDOM STELLA ** et **SIRAG KENZA'S**

![C#](https://img.shields.io/badge/C%23-WinForms-.NET-blue)
![Visual Studio](https://img.shields.io/badge/IDE-Visual%20Studio-purple)
![Status](https://img.shields.io/badge/Status-Terminé-brightgreen)

---

##  Contexte

Projet réalisé pour **SecurIT**, une start-up spécialisée en cybersécurité.  
L'objectif : créer un mini-jeu interactif pour le stand du **Salon de l'Innovation Tech**, permettant aux visiteurs de tester leur mémoire autour d'icônes de cybersécurité
SecurIT Memory est un jeu de Memory revisité dans un univers Cyberpunk & Cybersécurité,le joueur doit retrouver des paires de cartes le plus rapidement possible, tout en gérant :
un chronomètre en temps réel ; un compteur d’essais ; une grille dynamique ; un mode Hardcore où les cartes bougent toutes les 30 secondes

---

##  Règles du jeu

1. Toutes les cartes sont **face cachée** au départ
2. Le joueur **clique sur une carte** pour la retourner
3. Il **clique sur une 2ème carte** pour tenter de trouver la paire
4. Si les deux cartes sont **identiques** → elles restent visibles 
5. Si elles sont **différentes** → elles se retournent après 1,2 seconde 
6. Le joueur **gagne** quand toutes les paires sont trouvées 
7. Le mode Harcord ajoute un **chronomètre** et un **compteur d'essais** pour plus de challenge


---

##  Fonctionnalités

### Menu Principal
- **Jouer** — Lance une nouvelle partie
- **LEADERBOARD** - Enregistre automatiquement les scores , tri par meilleur temps 
- **Options** — Choix de la taille de grille (4×4 ou 6×6)
- **Quitter** — Ferme l'application


### Interface de Jeu
- Grille générée **dynamiquement** avec des `PictureBox`
- Chaque `PictureBox` est liée à un objet `Carte`
- **Chronomètre** en temps réel (HUD)
- **Compteur d'essais** (HUD)
- **Compteur de paires** trouvées / total (HUD)
- Bouton **Rejouer** pour relancer une partie

### Cartes
- **Blue Team** — Pare-feu, Chiffrement, Cadenas, VPN, Clé USB, MFA...
- **Red Team** — Virus, Phishing, Ransomware, Hacker, Malware, DDoS...
- Dos de carte uniforme avec symbole **SECURIT**

### Sons
| Événement | Son |

| Retournement d'une carte | `clic.wav` |
| Paire trouvée | `paire.wav` |
| Mauvaise paire | `erreur.wav` |
| Victoire | `victoire.wav` |

---

## Architecture du projet

```
SecurIT_Memory/
│
├── Carte.cs              # Classe Carte + énumération EtatCarte
├── JeuMemory.cs          # Logique du jeu, List<Carte>, mélange Fisher-Yates
|── ThemeCyber.cs
├── FormMenu.cs           # Menu principal (Jouer / Options / Quitter)
├── FormJeu.cs            # Grille PictureBox, HUD, Timers, rendu Bitmap
├── FormOptions.cs        # Choix taille de grille
├── SonManager.cs         # Gestionnaire de sons (.wav)
├── Program.cs            # Point d'entrée de l'application
│
└── Sounds/
    ├── clic.wav
    ├── paire.wav
    ├── erreur.wav
    └── victoire.wav
```

---

## Notions C# utilisées

| Notion | Où |
|---|---|
| **Classes & Encapsulation** | `Carte.cs` — propriétés `get`/`set` privés |
| **Énumération** | `EtatCarte` — Cachee / Revelee / Trouvee |
| **Héritage** | `FormJeu`, `FormMenu` héritent de `Form` |
| **List\<T\>** | `JeuMemory` — `List<Carte>` |
| **Dictionary\<K,V\>** | `FormJeu` — `Dictionary<PictureBox, Carte>` |
| **Timers** | Chronomètre + délai de retournement |
| **Bitmap & Graphics** | Rendu des cartes avec `System.Drawing` |
| **WinForms** | `PictureBox`, `Label`, `Button`, `Panel` |

---

## Répartition du travail

| Membre | Partie |
|---|---|
| **[Gloria]** | Interface WinForms — `FormMenu`, `FormJeu`, `FormOptions`, `SonManager` |
| **[Kenza's]** | Logique de jeu — `Carte`, `JeuMemory`, algorithme de mélange, Timers |

---

## Installation & Lancement

### Prérequis
- Visual Studio 2019 ou supérieur
- .NET Framework 4.7.2 ou supérieur

### Étapes
1. Cloner le dépôt :
```bash
git clone https://github.com/gloria-stella/SecurIT_Memory
```
2. Ouvrir `SecurIT_Memory.sln` dans Visual Studio
3. Appuyer sur **F5** pour lancer

### Sons (optionnel)
Placer les fichiers `.wav` dans le dossier `Sounds/` du projet.  
Le jeu fonctionne sans les sons si les fichiers sont absents.

---

## Style visuel

Thème **Cyber Neon Extraterrestre** :

| Couleur | Code | Usage |
|---|---|---|
| Noir profond | `#050505` | Fond général |
| Vert néon | `#00FF6A` | Titres, bordures, Blue Team |
| Bleu cyber | `#00E5FF` | Boutons, cartes révélées |
| Rouge néon | `#FF004C` | Red Team, bouton Quitter |

---

## 📄 Licence

Projet scolaire — SecurIT Memory © 2026