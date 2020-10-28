using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Scrap Lottery", "Kopter", "1.0.3")]
    [Description("A plugin to gamble scrap in a lottery-style anywhere on the map using commands!")]
    public class ScrapLottery : RustPlugin
    {

        int WinningNumber;
        int ScrapID;
        bool LotteryRunning;
        List<ulong> GuessedPlayerIds = new List<ulong>();

        #region Hooks
        
        void Init()
        {
            permission.RegisterPermission("scraplottery.use", this);
        }

        void OnServerInitialized()
        {
            ScrapID = ItemManager.FindItemDefinition("scrap").itemid;
            timer.Every(config.LotteryRate, () =>
            {
                StartLottery();

            });
        }

        void StartLottery()
        {
            LotteryRunning = true;
            WinningNumber = Random.Range(config.MinNumber, config.MaxNumber + 1);
            PrintToChat($"{Lang("LotteryStarted", null, config.MinNumber, config.MaxNumber)}");
            timer.Once(config.LotteryLength, () =>
            {
                if(LotteryRunning) LotteryExpired();
            });
        }

        void LotteryExpired()
        {
            PrintToChat($"{Lang("LotteryExpired", null)} {WinningNumber}");
            LotteryRunning = false;
            GuessedPlayerIds.Clear();
        }

        [ChatCommand("lottery")]
        void LotteryCommand(BasePlayer player, string command, string[] args)
        {
            if(config.PermissionNeeded && !permission.UserHasPermission(player.UserIDString, "scraplottery.use"))
            {
                player.ChatMessage($"{Lang("NoPermission", player.UserIDString)}");
                return;
            }

            if(!LotteryRunning)
            {
                player.ChatMessage($"{Lang("LotteryNotRunning", player.UserIDString)}");
                return;
            }

            if(GuessedPlayerIds.Contains(player.userID))
            {
                player.ChatMessage($"{Lang("LotteryPlayed", player.UserIDString)}");
                return;
            }

            if(args.Length != 1)
            {
                player.ChatMessage($"{Lang("InvalidGuess", player.UserIDString)}");
                return;
            }

            int GuessedNumber;

            if(!int.TryParse(args[0], out GuessedNumber))
            {
                player.ChatMessage($"{Lang("InvalidGuess", player.UserIDString)}");
                return;
            }

            if (player.inventory.GetAmount(ScrapID) < config.ScrapNeeded)
            {
                player.ChatMessage($"{Lang("NotEnoughScrap", player.UserIDString, config.ScrapNeeded)}");
                return;
            }

            if(GuessedNumber > config.MaxNumber || GuessedNumber < config.MinNumber)
            {
                player.ChatMessage($"{Lang("OverMaxNumber", player.UserIDString, config.MinNumber, config.MaxNumber)}");
                return;
            }

            player.inventory.Take(null, ScrapID, config.ScrapNeeded);

            if(LotteryRunning && GuessedNumber == WinningNumber)
            {
                PrintToChat($"{player.displayName} {Lang("CorrectNumber", player.UserIDString)} {WinningNumber}");
                var reward = ItemManager.CreateByName("scrap", config.ScrapReward, 0);
                if(reward == null) return;
                player.inventory.GiveItem(reward, player.inventory.containerMain);
                LotteryRunning = false;
                GuessedPlayerIds.Clear();
            }

            else
            {
                player.ChatMessage($"{Lang("WrongNumber", player.UserIDString)}");
                GuessedPlayerIds.Add(player.userID);
            }
        }

        #endregion

        #region Config

        private ConfigData config = new ConfigData();
        private class ConfigData
        {
            [JsonProperty(PropertyName = "Permission Needed To Play")]
            public bool PermissionNeeded = false;

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

                if(config == null)
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

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"LotteryStarted", "Lottery Time! Guess a number between {0} and {1}! (Example: /lottery 8)"},
                {"LotteryExpired", "The lottery is over! There were no winners, better luck next time! Correct anwser:"},
                {"NoPermission", "You don't have permission to play!"},
                {"LotteryNotRunning", "The lottery has not started yet!"},
                {"LotteryPlayed", "You've already played, you can try again next lottery!"},
                {"InvalidGuess", "Invalid guess! (Example: /lottery 8)"},
                {"NotEnoughScrap", "You don't have enough scrap to play! Scrap needed: {0}"},
                {"OverMaxNumber", "Guess a number between {0} and {1}!"},
                {"CorrectNumber", "has won the lottery! Correct anwser:"},
                {"WrongNumber", "You didn't win, you can try again next lottery!"}

            }, this);
        }

        #endregion

        #region Helpers
        
            string Lang(string key, string id = null, params object[] args) => string.Format(lang.GetMessage(key, this, id), args);

        #endregion
    }
}
