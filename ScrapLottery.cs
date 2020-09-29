using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Scrap Lottery", "Kopter", "1.0.1")]
    [Description("A plugin to gamble scrap in a lottery-style anywhere on the map using commands!")]
    public class ScrapLottery : RustPlugin
    {

        int WinningNumber;
        bool LotteryRunning;
        List<ulong> GuessedPlayerIds;

        #region Oxide Hooks

        void Init()
        {
            GuessedPlayerIds = new List<ulong>();
        }

        void Loaded()
        {
            timer.Every(config.LotteryRate, () =>
            {
                StartLottery();

            });
        }

        void StartLottery()
        {
            LotteryRunning = true;
            WinningNumber = Random.Range(config.MinNumber, config.MaxNumber + 1);
            PrintToChat("Lottery Time! Guess a number between " + config.MinNumber + " and " + config.MaxNumber + " (Example: /lottery 8)!");
            timer.Once(config.LotteryLength, () =>
            {
          
                if(LotteryRunning) LotteryExpired();

            });
        }

        void LotteryExpired()
        {
            PrintToChat("The lottery is over! There were no winners, better luck next time! Correct anwser: " + WinningNumber);
            LotteryRunning = false;
            GuessedPlayerIds.Clear();
        }

        [ChatCommand("lottery")]
        void LotteryCommand(BasePlayer player, string command, string[] args)
        {
            if(!LotteryRunning)
            {
                player.ChatMessage("The lottery has not started yet!");
                return;
            }

            if(GuessedPlayerIds.Contains(player.userID))
            {
                player.ChatMessage("You've already played, you can try again next lottery!");
                return;
            }

            int GuessedNumber;
            bool isNumeric = int.TryParse(args[0], out GuessedNumber);

            if(args.Length != 1 || !isNumeric)
            {
                player.ChatMessage("Invalid guess (Try something like /lottery 6)!");
                return;
            }

            if(player.inventory.GetAmount(-932201673) < config.ScrapNeeded)
            {
                player.ChatMessage("You don't have enough scrap to play! Scrap needed: " + config.ScrapNeeded);
                return;
            }

            if(GuessedNumber > config.MaxNumber)
            {
                player.ChatMessage("Guess a number between " + config.MinNumber + " and " + config.MaxNumber + "!");
                return;
            }

            player.inventory.Take(null, -932201673, config.ScrapNeeded);

            if(LotteryRunning && GuessedNumber == WinningNumber)
            {
                PrintToChat(player.displayName + " has won the lottery! Correct anwser: " + WinningNumber);
                player.inventory.GiveItem(ItemManager.CreateByItemID(-932201673, config.ScrapReward, 0), player.inventory.containerMain);
                LotteryRunning = false;
                GuessedPlayerIds.Clear();
            }

            else
            {
                player.ChatMessage("You didn't win, you can try again next lottery!");
                GuessedPlayerIds.Add(player.userID);
            }
        }

        void Unload()
        {
            config = null;
        }

        #endregion

        #region Config File

        private static ConfigData config = new ConfigData();

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Lottery Rate (In Seconds)")]
            public float LotteryRate = 1800f;

            [JsonProperty(PropertyName = "Lottery Duration (In Seconds)")]
            public float LotteryLength = 30f;

            [JsonProperty(PropertyName = "Min Number To Bet On")]
            public int MinNumber = 1;

            [JsonProperty(PropertyName = "Max Number To Bet On")]
            public int MaxNumber = 50;

            [JsonProperty(PropertyName = "Scrap Needed To Bet")]
            public int ScrapNeeded = 10;

            [JsonProperty(PropertyName = "Scrap Reward")]
            public int ScrapReward = 100;
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<ConfigData>();

                if (config == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch
            {
                
                PrintError("Configuration file is corrupt, check your config file at https://jsonlint.com/!");
                LoadDefaultConfig();
                return;
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            config = new ConfigData();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(config);
        }

        #endregion
    }
}