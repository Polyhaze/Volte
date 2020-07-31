# BrackeysBot `[ ]`

The official Brackeys Discord bot!  
Contains tools to moderate the server and to create a better user experience overall!

## Important links

[![Discord](https://img.shields.io/discord/243005537342586880?color=7289DA&label=Discord%20Server&style=for-the-badge)](https://discord.gg/brackeys)

[![Travis CI](https://img.shields.io/travis/com/yiliansource/brackeys-bot?color=7289DA&label=Travis%20CI%20Build&style=for-the-badge)](https://travis-ci.com/YilianSource/brackeys-bot)

- [📝 MIT License](https://github.com/YilianSource/brackeys-bot/blob/master/LICENSE)
- [🛡️ Code of Conduct](https://github.com/YilianSource/brackeys-bot/blob/master/.github/CODE_OF_CONDUCT.md)
- [🖋️ Contributing](https://github.com/YilianSource/brackeys-bot/blob/master/.github/CONTRIBUTING.md)

## Building & running

The bot is written with **.NET Core 3.0**, meaning you can build it via `dotnet build`, which will create a binary executeable called `BrackeysBot.exe`. If you are missing packages make sure to run `dotnet restore` prior to building.

When run for the first time, the bot will create a `config.yaml` file next to your executeable. This is where all of the bot configuration will be placed. The only fields that are essential for running the bot are `token` and `prefix`.

```yaml
token: 'Get this from the developer dashboard!'
prefix: '[]'
```

## Versioning

Automatic versioning is planned for the future, but at the moment the version number needs to be manually updated in the file `Version.cs`. The system to update the version numbers goes as follows:

|Number|Description|
|--:|:--|
|Major|Only updated by the repository administrators once a completely new version is deployed.|
|Minor|Updated once a batch of features (or a [project](https://github.com/YilianSource/brackeys-bot/projects)) is fully implemented.|
|Patch|Updated once a new feature is added.|
|Hotfix|Updated if a feature requires a fix.|
