# Scrap-Lottery
A Rust plugin to gamble scrap in a lottery-style anywhere on the map using commands!

You can download the plugin at [uMod](https://umod.org/plugins/bN5MQYr510)

A simple customizable lottery plugin, every X amount of time a message will be sent saying that the lottery has started and players can guess a number between 2 numbers, a player can guess only 1 time and needs to have a certain amount of scrap. \
If a player guesses the correct number a message is sent saying that the player has won the lottery followed by the correct answer and the reward is given if no one guesses after a certain time a message is sent saying that the lottery is over and that no one won, followed by the correct answer.

# Chat Commands
* ``/lottery <Number Guess>`` -- Play on the lottery 
# Configuration
``` json
{
  "Lottery Rate (In Seconds)": 1800.0,
  "Lottery Duration (In Seconds)": 30.0,
  "Min Number To Bet On": 1,
  "Max Number To Bet On": 50,
  "Scrap Needed To Bet": 10,
  "Scrap Reward": 100
}
```
# Default Values
* Lottery Rate: **1800.0**
* Lottery Lenght: **30.0**
* Min Number To Bet On: **1**
* Max Number To Bet On: **50**
* Scrap Needed to Bet: **10**
* Scrap Reward: **100**
