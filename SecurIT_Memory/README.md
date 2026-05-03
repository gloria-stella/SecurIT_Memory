# SecurIT Memory

Un jeu de cartes Memory sur le thème de la cybersécurité, développé en C# avec WinForms dans le cadre du Salon de l'Innovation Tech.

---

## C'est quoi ce projet ?

On travaille pour **SecurIT**, une start-up en cybersécurité. Leur équipe marketing voulait un mini-jeu pour animer leur stand au salon tech. On a développé un jeu Memory où toutes les cartes représentent des concepts de cybersécurité : virus, pare-feu, ransomware, VPN...

Le jeu a été réalisé en binôme avec Visual Studio et C#.

---

## Comment jouer ?

1. Lance le jeu depuis Visual Studio **(F5)** **ctrl + Shift + B** pour compiler puis exécuter
2. Sur le menu, clique sur **JOUER**
3. Toutes les cartes sont retournées face cachée
4. Clique sur une carte pour la retourner
5. Clique sur une deuxième carte
   - Si c'est la même → elles restent visibles 
   - Si c'est différent → elles se retournent après 1 seconde 
6. Continue jusqu'à avoir trouvé toutes les paires
7. À la fin, ton temps et ton nombre d'essais sont enregistrés

---

## Fonctionnalités

**Menu**
- Jouer, Options, Leaderboard, Quitter

**Options**
- Choisir la taille de la grille : 4×4 (facile) ou 6×6 (difficile)
- Choisir le thème des cartes : Cybersécurité, Matériel, Logiciel ou Cryptographie
- Activer le mode Hardcore (les cartes bougent toutes les 30 secondes !)

**Pendant la partie**
- Chronomètre en temps réel
- Compteur d'essais
- Compteur de paires trouvées
- Sons à chaque action (clic, paire trouvée, erreur, victoire)
- Blue Team en bleu, Red Team en rouge, paires trouvées en violet

**Leaderboard**
- Les scores sont sauvegardés automatiquement
- Classement des 10 meilleurs temps par taille de grille
- Détection du nouveau record à la fin d'une partie

---

## Structure du projet

```
SecurIT_Memory/
│
├── Carte.cs              → La classe Carte (id, image, état)
├── JeuMemory.cs          → Toute la logique du jeu + timers
├── ThemeCyber.cs         → Les couleurs et les icônes par thème
├── ScoreManager.cs       → Sauvegarde et lecture des scores
│
├── FormMenu.cs           → Fenêtre du menu principal
├── FormJeu.cs            → Fenêtre de la partie (grille de PictureBox)
├── FormOptions.cs        → Fenêtre des options
├── FormLeaderboard.cs    → Fenêtre du classement
│
├── SonManager.cs         → Gestion des sons .wav
├── Program.cs            → Point de démarrage de l'application
│
└── Sounds/
    ├── clic.wav
    ├── paire.wav
    ├── erreur.wav
    └── victoire.wav
```

---

## Ce qu'on a utilisé

| Notion | Où on l'a utilisée |
|---|---|
| Classes & Encapsulation | `Carte.cs` avec `get` / `private set` |
| Enumération | `EtatCarte` : Cachee, Revelee, Trouvee |
| Héritage | `FormJeu`, `FormMenu` héritent de `Form` |
| List\<T\> | `JeuMemory` stocke les cartes dans une `List<Carte>` |
| Dictionary | `FormJeu` lie chaque `PictureBox` à sa `Carte` |
| Timers | Chronomètre + délai de retournement + timer Hardcore |
| Bitmap & Graphics | Dessin des cartes avec `System.Drawing` |
| WinForms | `PictureBox`, `Label`, `Button`, `Panel` |
| SQLite | Sauvegarde des scores dans une base de données locale |

---

## Répartition du travail

| Membre | Partie |
|---|---|
| **[GLORIA]** | Interface WinForms : FormMenu, FormJeu, FormOptions, FormLeaderboard, SonManager |
| **[KENZA'S]** | Logique du jeu : Carte, JeuMemory, ScoreManager, ThemeCyber |

---

## Installation

1. Cloner le repo :
```
git clone https://github.com/gloria-stella/SecurIT_Memory
```

2. Ouvrir `SecurIT_Memory.sln` dans Visual Studio

3. Installer le package SQLite via la console NuGet :
```
Install-Package System.Data.SQLite
```

4. Ajouter la référence **Microsoft.VisualBasic** dans les références du projet

5. Lancer avec **F5**

> Les sons sont optionnels. Si le dossier `Sounds/` est vide, le jeu fonctionne quand même sans problème.

---

## Bonus réalisés

- ✅ Effets sonores
- ✅ Thèmes de cartes (4 thèmes différents)
- ✅ Leaderboard avec base de données SQLite
- ✅ Mode Hardcore