# 🦬 Buffalo App

Application mobile Android pour jouer au jeu du Buffalo avec détection Bluetooth des autres joueurs.

![.NET 10](https://img.shields.io/badge/.NET-10.0-purple)
![MAUI](https://img.shields.io/badge/MAUI-Latest-blue)
![Platform](https://img.shields.io/badge/Platform-Android-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

## 📥 Télécharger l'APK

[![Télécharger APK](https://img.shields.io/badge/Télécharger-APK-brightgreen?style=for-the-badge&logo=android)](https://github.com/Tomtoxi44/Buffalo/releases/latest)

Clique sur le badge ci-dessus pour télécharger la dernière version de l'application !

## 📱 Description

**Buffalo** est un jeu de bar légendaire où tu dois toujours boire avec ta main non-dominante. Cette application te permet de :

- 🔍 **Détecter** automatiquement les autres joueurs à proximité via Bluetooth
- 🦬 **Donner des Buffalo** quand tu surprends quelqu'un en train de boire avec la mauvaise main
- ✅ **Accepter ou refuser** - Tu peux mettre un Buffalo sur l'ardoise si tu ne peux pas boire
- 📝 **Gérer ton ardoise** - Les Buffalo refusés peuvent être réclamés à tout moment
- 📊 **Suivre tes stats** - Buffalo donnés, reçus, ton classement
- 🏆 **Classement** - Qui est le meilleur chasseur de Buffalo ?
- 📖 **Consulter les règles** - Toutes les règles officielles du jeu

## 🎮 Les Règles du Buffalo

1. **Règle d'Or** : Boire TOUJOURS avec sa main NON-DOMINANTE
2. **Le Cri** : Si quelqu'un se trompe, crie "BUFFALO !"
3. **La Sanction** : Finir son verre cul-sec
4. **C'est pour la vie** : Une fois Buffalo, toujours Buffalo !
5. **L'Ardoise** : Tu peux refuser, mais le Buffalo reste dû

## 🔧 Technologies

- **.NET MAUI 10** - Framework cross-platform
- **SQLite** - Base de données locale
- **Bluetooth Low Energy (BLE)** - Détection des joueurs
- **MVVM** avec CommunityToolkit
- **Architecture propre** - Models, Services, ViewModels, Views

## 📦 Installation

### Prérequis
- Visual Studio 2022+ ou VS Code avec extension C# Dev Kit
- .NET 10 SDK
- Android SDK (API 21+)

### Cloner et lancer
```bash
git clone https://github.com/TommyANGIBAUD/Buffalo.git
cd Buffalo/BuffaloApp
dotnet restore
dotnet build -f net10.0-android
dotnet build -t:Run -f net10.0-android
```

### Sur appareil Android
1. Active le mode développeur sur ton téléphone
2. Active le débogage USB
3. Connecte ton téléphone
4. Lance l'app avec la commande ci-dessus

## 🏗️ Structure du Projet

```
BuffaloApp/
├── Models/              # Modèles de données (Player, BuffaloEvent, SlateEntry)
├── Data/                # Base de données SQLite
├── Services/            # Services (Bluetooth, Buffalo logic)
├── ViewModels/          # ViewModels MVVM
├── Views/               # Pages XAML
├── Converters/          # Convertisseurs pour l'UI
└── Platforms/           # Code spécifique à chaque plateforme
```

## 🚀 Fonctionnalités

### Page Principale
- Switch ON/OFF pour activer le mode Buffalo
- Liste des joueurs détectés à proximité
- Distance estimée
- Bouton "BUFFALO!" pour chaque joueur
- Affichage des ardoises en attente

### Ardoise
- Buffalo que tu dois (à payer)
- Buffalo qu'on te doit (à récupérer)
- Bouton pour régler une ardoise

### Classement
- Top des meilleurs donneurs de Buffalo
- Ta position dans le classement
- Nombre de Buffalo donnés par chaque joueur

### Profil
- Ton pseudo
- Ta main dominante (droitier/gaucher)
- Tes statistiques complètes
- Date de création du compte

### Règles
- Toutes les règles du Buffalo
- Explications détaillées
- Rappel de consommation responsable

## 🔐 Permissions Android

L'app nécessite les permissions suivantes :
- `BLUETOOTH` / `BLUETOOTH_ADMIN` - Pour la détection BLE
- `BLUETOOTH_SCAN` / `BLUETOOTH_ADVERTISE` / `BLUETOOTH_CONNECT` - Android 12+
- `ACCESS_FINE_LOCATION` / `ACCESS_COARSE_LOCATION` - Requis pour BLE sur Android

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésite pas à :
1. Fork le projet
2. Créer une branche (`git checkout -b feature/AmazingFeature`)
3. Commit tes changements (`git commit -m 'Add AmazingFeature'`)
4. Push sur la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📝 TODO / Améliorations futures

- [ ] Implémenter la vraie détection BLE (actuellement en mode démo)
- [ ] Ajouter des notifications push
- [ ] Mode "Soirée" avec plusieurs joueurs en simultané
- [ ] Historique détaillé des Buffalo
- [ ] Système d'achievements/badges
- [ ] Support multi-langue
- [ ] Mode sombre
- [ ] Export des stats en PDF

## ⚠️ Avertissement

Cette application est un jeu. **Buvez responsablement** et **ne conduisez jamais** après avoir bu. L'abus d'alcool est dangereux pour la santé.

## 📄 License

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 👤 Auteur

**Tommy ANGIBAUB**

---

🦬 **Fait avec passion pour tous les fans de Buffalo !**
