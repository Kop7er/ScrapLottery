﻿using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Scrap Lottery", "Kopter", "1.0.2")]
    [Description("A plugin to gamble scrap in a lottery-style anywhere on the map using commands!")]
    public class ScrapLottery : RustPlugin
    {

        int WinningNumber;
        bool LotteryRunning;
        List<ulong> GuessedPlayerIds = new List<ulong>();

        #region Hooks
        
        void Loaded()
        {
            permission.RegisterPermission("scraplottery.use", this);

            timer.Every(config.LotteryRate, () =>
            {
                StartLottery();

            });
        }

        void StartLottery()
        {
            LotteryRunning = true;
            WinningNumber = Random.Range(config.MinNumber, config.MaxNumber + 1);
            PrintToChat(lang.GetMessage("LotteryStarted", this));
            timer.Once(config.LotteryLength, () =>
            {
          
                if(LotteryRunning) LotteryExpired();

            });
        }

        void LotteryExpired()
        {
            PrintToChat(lang.GetMessage("LotteryExpired", this) + WinningNumber);
            LotteryRunning = false;
            GuessedPlayerIds.Clear();
        }

        [ChatCommand("lottery")]
        void LotteryCommand(BasePlayer player, string command, string[] args)
        {
            if(config.PermissionNeeded && !(permission.UserHasPermission(player.userID.ToString(), "scraplottery.use")))
            {
                player.ChatMessage(lang.GetMessage("NoPermission", this));
                return;
            }

            if(!LotteryRunning)
            {
                player.ChatMessage(lang.GetMessage("LotteryNotRunning", this));
                return;
            }

            if(GuessedPlayerIds.Contains(player.userID))
            {
                player.ChatMessage(lang.GetMessage("LotteryPlayed", this));
                return;
            }

            if(args.Length != 1)
            {
                player.ChatMessage(lang.GetMessage("InvalidGuess", this));
                return;
            }

            int GuessedNumber;

            if(!int.TryParse(args[0], out GuessedNumber))
            {
                player.ChatMessage(lang.GetMessage("InvalidGuess", this));
                return;
            }

            if(player.inventory.GetAmount(ItemManager.FindItemDefinition("scrap").itemid) < config.ScrapNeeded)
            {
                player.ChatMessage(lang.GetMessage("NotEnoughScrap", this));
                return;
            }

            if(GuessedNumber > config.MaxNumber || GuessedNumber < config.MinNumber)
            {
                player.ChatMessage(lang.GetMessage("OverMaxNumber", this));
                return;
            }

            player.inventory.Take(null, ItemManager.FindItemDefinition("scrap").itemid, config.ScrapNeeded);

            if(LotteryRunning && GuessedNumber == WinningNumber)
            {
                PrintToChat(player.displayName + lang.GetMessage("CorrectNumber", this) + WinningNumber);
                player.inventory.GiveItem(ItemManager.CreateByName("scrap", config.ScrapReward, 0), player.inventory.containerMain);
                LotteryRunning = false;
                GuessedPlayerIds.Clear();
            }

            else
            {
                player.ChatMessage(lang.GetMessage("WrongNumber", this));
                GuessedPlayerIds.Add(player.userID);
            }
        }

        void Unload()
        {
            config = null;
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
                {"LotteryStarted", "Lottery Time! Guess a number between " + config.MinNumber + " and " + config.MaxNumber + "! (Example: /lottery 8)"},
                {"LotteryExpired", "The lottery is over! There were no winners, better luck next time! Correct anwser: "},
                {"NoPermission", "You don't have permission to play!"},
                {"LotteryNotRunning", "The lottery has not started yet!"},
                {"LotteryPlayed", "You've already played, you can try again next lottery!"},
                {"InvalidGuess", "Invalid guess! (Example: /lottery 8)"},
                {"NotEnoughScrap", "You don't have enough scrap to play! Scrap needed: " + config.ScrapNeeded},
                {"OverMaxNumber", "Guess a number between " + config.MinNumber + " and " + config.MaxNumber + "!"},
                {"CorrectNumber", " has won the lottery! Correct anwser: "},
                {"WrongNumber", "You didn't win, you can try again next lottery!"}

            }, this);
        }

        #endregion
    }
}