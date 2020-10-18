# ScrapLottery
A Rust plugin to gamble scrap in a lottery-style anywhere on the map using commands!

You can download the plugin by clicking "Download Code", extrating the ZIP file and placing the file ``ScrapLottery.cs`` under ``oxide/plugins``, full info at [uMod](https://umod.org/documentation/plugins/installation),or at [uMod](https://umod.org/plugins/bN5MQYr510) (Awaiting approval)

A simple customizable lottery plugin, every X amount of time a message will be sent saying that the lottery has started and players can guess a number between 2 numbers, a player can guess only 1 time and needs to have a certain amount of scrap. \
If a player guesses the correct number a message is sent saying that the player has won the lottery followed by the correct answer and the reward is given if no one guesses after a certain time a message is sent saying that the lottery is over and that no one won, followed by the correct answer.

# Commands
This plugin provides both chat and console commands using the same syntax. When using a command in chat, prefix it with a forward slash: ``/``.

## Chat

* ``/lottery <Number Guess>`` -- Play on the lottery

## Console

* Coming Soon

# Permissions
This plugin uses Oxide's permission system. To assign a permission, use ``oxide.grant <user or group> <name or steam id> <permission>``. To remove a permission, use ``oxide.revoke <user or group> <name or steam id> <permission>``.

* ``scraplottery.use`` -- Allows players to play on the lottery (Disable by default)

# Configuration

The settings and options for this plugin can be configured in the ``ScrapLottery.json`` file under the ``oxide/config`` directory. The use of a JSON editor or validation site such as [jsonlint.com](https://jsonlint.com/) is recommended to avoid formatting issues and syntax errors.

``` json
{
  "Permission Needed To Play": false,
  "Lottery Rate (In Seconds)": 1800.0,
  "Lottery Duration (In Seconds)": 30.0,
  "Min Number To Bet On": 1,
  "Max Number To Bet On": 50,
  "Scrap Needed To Bet": 10,
  "Scrap Reward": 100
}
```
# Default Values

* Permission Needed To Play: **false**
* Lottery Rate: **1800.0**
* Lottery Lenght: **30.0**
* Min Number To Bet On: **1**
* Max Number To Bet On: **50**
* Scrap Needed to Bet: **10**
* Scrap Reward: **100**
