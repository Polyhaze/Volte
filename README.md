# SIVA-Issues
Nightly Build: ![](https://greem.visualstudio.com/_apis/public/build/definitions/a8e3cd4d-6001-43ff-ae22-5a7d66420da7/3/badge)

![SIVA logo](https://raw.githubusercontent.com/Greeem/SIVA-Issues/master/Images/SIVA.png)

## What is the SIVA rewrite?
`The SIVA rewrite is a simple term for creating the bot in an entirely new programming language. Discord.py was getting rewritten, and it was becoming obsolete, so I decided to completely remake the entire bot. It's taken 40+ hours, but it's nearly done.`

## Found an issue with the Public SIVA?
`Create an issue.`

## Want to invite the bots?

### Invite SIVA
> [SIVA-dev](https://discordapp.com/oauth2/authorize?scope=bot&client_id=410547925597421571&permissions=8) **By default, this has the Administrator permission checked. Keep this if you're using the support system. If you're not, feel free to uncheck. It's still advised to keep it checked.**

> [SIVA-public](https://discordapp.com/oauth2/authorize?client_id=320942091049893888&scope=bot&permissions=8) **By default, this has the Administrator permission checked. If you're using moderation, you should keep it checked.**



# Command Documentation

## Moderation / Server Management
> An Admin's dream!

### Welcome
**If you want to setup a welcome message, then run the command `$wc`. Mention a channel afterwards for it to work properly. Example: $wc #welcome - After you run that command, your Guild/Server config has been created. You can then run `$wmsg String Message`. Valid placeholders are {UserMention} and {ServerName}. If you put those in ANY part of the message, the program will replace them.**

### Support
"Oh boy, this is a big one."

If you want to setup support in your Guild, then do the following:

1. Run the command $SupportRole (can be shortened to SR), followed by a Role name. (NOT A MENTION.) So for example, `$sr Support` would allow anyone with the Support role to type in support tickets. This is fully customisable.
2. Go into whatever channel you want to setup as the channel to recieve tickets and create them, and type `SetupSupport`. Doing this will autoset config options into the GuildConfig which you can edit later. **Only the Guild Owner can run SetupSupport**.
3. You're done! Enjoy the support system.

### AutoRole

Autorole has one simple command.

`$autorole RoleName` - Sets the role in the GuildConfig.

### Leveling/Currency Control

If you want to disable leveling then run the following command:

1. `$levels false`
Replacing false with true will enable it.
You cannot disable the Currency as of yet, I will make that in the future.

### Command Prefix Control

Want to use another character than `$`? Run the command `$serverprefix prefix`. This can have spaces (not recommended)

### Literally everything else in the moderation module

  {} = Required Argument
  [] = Optional Argument

  AddRole - Give a user a role. | Aliases: AR\n
  Usage: `$ar {@User} {rolename}` (Can have spaces, gotta love C#)\n\n
  
  RemRole - Remove a role from a user. | Aliases: RR\n
  Usage: `$rr {@User} {rolename}` (Can have spaces)\n\n
  
  Purge - Delete last X messages. | Aliases: None\n
  Usage: `$purge {182}` (Messages cannot be older than 2 weeks, Discord API limitation.)\n\n
  
  Warn - Warn a mentioned user. Requires KickMembers permission. | Aliases: None\n
  Usage: `$warn {@User} {reason}`\n\n
  
  Warns - Gets amount of warns for a specified user.\n 
  Usage: `$warns [@User]`\n
  
  ClearWarns - Clears warns for specified user. Requires Admin permission. | Aliases: CW\n
  Usage: `$cw {@User}`\n\n
  
  Kick - Kicks a user. Requires KickMembers permission.
  Usage: `$kick {@User}`
  
  IdBan - Ban a user by their Discord ID. Requires BanMembers permission.
  Usage: `$idban {UserId}`
  
  Ban - Bans a user. Requires BanMembers permission.
  Usage: `$ban {@User}`
  
  


