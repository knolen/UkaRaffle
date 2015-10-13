/**
 * UkaRaffle
 * 
 * Copyright (C) 2015 NerdDad
 * Contact: NerdDad79@gmail.com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, Get it here: https://www.gnu.org/licenses/gpl-2.0.html
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml.Serialization;
using Rocket;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Effects;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Rocket.Unturned.Items;
using SDG.Unturned;
using UnityEngine;
using unturned.ROCKS.Uconomy;

namespace UkaRaffle
{
    public class Raffle:RocketPlugin<UkaRaffleConfiguration>
    {
        public static Raffle Instance;
        public bool firstRun = true;
        public DateTime lastran = DateTime.Now;
        public DateTime nextrun = DateTime.Now.AddMinutes(30);
        public DateTime lastmsg1 = DateTime.Now;
        public DateTime lastmsg2 = DateTime.Now;
        public int myMaxNumCars = 0;
        public int myCarPick = 0;
        public System.Random myRandom = new System.Random();
        public Vehicle vehicle;
        public Money money;
        public byte msgNum = 5;
        public List<string> currentTickets = new List<string>();
        public string WinnerName = "";
        public bool PrizedClaimed = true;
        public int PrizeID = 0;
        public decimal TicketCost = 5;
        public string PrizeType = "";
        public string WinnerPrizeType = "";
        public double PrizeAmount;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"Raffle_announce","The next Raffle will be in {0} minutes!"},
                    {"Raffle_prize","The next Raffle will be for {0}"},
                    {"Raffle_msg1","Next raffle prize is {0}. Raffle in {1} minutes."},
                    {"Raffle_now","The Raffle is now! Let's see who wins!"}
                };
            }
        }

        protected override void Load()
        {
            Logger.Log("UkaRaffle Loaded");
            Raffle.Instance = this;
            UkaRaffePickPrize();
        }
        public void UkaRaffePickPrize()
        {
            System.Random myPrizeRan = new System.Random();
            int MyPrizeType = myPrizeRan.Next(0, 2);
            if (MyPrizeType == 1)
            {
                //Pick the prize
                PrizeType = "Vehicle";
                myMaxNumCars = (Configuration.Instance.Vehicles.Length);
                myCarPick = myRandom.Next(myMaxNumCars);
                vehicle = Configuration.Instance.Vehicles[myCarPick];
                UnturnedChat.Say(Raffle.Instance.Translate("Raffle_prize", vehicle.Name));
                UnturnedChat.Say(Raffle.Instance.Translate("Raffle_announce", Math.Round((nextrun - DateTime.Now).TotalMinutes)));
            }
            else if (MyPrizeType == 0)
            {
                PrizeType = "Money";
                myMaxNumCars = (Configuration.Instance.MoneyPrizes.Length);
                myCarPick = myRandom.Next(myMaxNumCars);
                money = Configuration.Instance.MoneyPrizes[myCarPick];
                UnturnedChat.Say(Raffle.Instance.Translate("Raffle_prize", money.Amount.ToString()));
                UnturnedChat.Say(Raffle.Instance.Translate("Raffle_announce", Math.Round((nextrun - DateTime.Now).TotalMinutes)));
            }
            Logger.Log("Random pick was: " + myPrizeRan.ToString());
        }


        protected override void Unload()
        {
            Logger.Log("UkaRaffle Unloaded");
        }
    
        void FixedUpdate()
        {
            UkaRaffleCheck();
        }

        public void UkaRaffleCheck()
        {
            try
            {
                if ((Raffle.Instance.nextrun - DateTime.Now).TotalSeconds - 300.0 <= 0.0 && (DateTime.Now - Raffle.Instance.lastmsg1).TotalSeconds >= 60.0) 
                {
                 
                    byte b = Raffle.Instance.msgNum;
                    if (b != 0) {
                        if (PrizeType == "Vehicle")
                        {
                            UnturnedChat.Say(Raffle.Instance.Translate("Raffle_announce", Math.Round((nextrun - DateTime.Now).TotalMinutes)));
                            UnturnedChat.Say(Raffle.Instance.Translate("Raffle_prize", vehicle.Name));
                            Raffle.Instance.lastmsg1 = DateTime.Now;
                            Raffle.Instance.msgNum -= 1;
                        }
                        else if (PrizeType == "Money")
                        {
                            UnturnedChat.Say(Raffle.Instance.Translate("Raffle_announce", Math.Round((nextrun - DateTime.Now).TotalMinutes)));
                            UnturnedChat.Say(Raffle.Instance.Translate("Raffle_prize", money.Amount.ToString()));
                            Raffle.Instance.lastmsg1 = DateTime.Now;
                            Raffle.Instance.msgNum -= 1;
                        }
                    }
                    else
                    {
                        Raffle.Instance.lastmsg1 = DateTime.Now;
                        UnturnedChat.Say(Raffle.Instance.Translate("Raffle_now"));
                        Raffle.Instance.UkaRaffleRun();
                    }
                }
                    
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UkaRaffleRun()
        {
            if (currentTickets.Count > 0)
            {
                //Run the thing!
                UnturnedChat.Say("Lets Raffle!");
                //Convert our list to an array.
                var TicketArray = currentTickets.ToArray();
                //Get the high end for max.
                int maxTicket = TicketArray.Length;
                //Pick random winner.
                int myWinnerNum = myRandom.Next(maxTicket);
                WinnerName = TicketArray[myWinnerNum];
                PrizedClaimed = false;
                WinnerPrizeType = PrizeType;
                if (WinnerPrizeType == "Vehicle")
                {
                    PrizeID = vehicle.Id;
                    Logger.Log("Raffel winner is: " + WinnerName + " and the prize is: "+vehicle.Name);
                }
                else if (WinnerPrizeType == "Money")
                {
                    PrizeAmount = money.Amount;
                    Logger.Log("Raffel winner is: " + WinnerName + " and the prize is: "+money.Amount.ToString());
                }
                
                UnturnedChat.Say("And the winner is... " + WinnerName + "! Use the rclaim command to get your prize.");
                UkaRaffleReset();
            }
            else
            {
                UnturnedChat.Say("No raffle tickets were sold, let's try again! Use /rticket to buy tickets!");
                Logger.Log("No tickets sold. Resetting without running.");
                UkaRaffleReset();
            }

        }

        public void UkaRaffleReset()
        {
            //Update the last time we ran a Raffle.
            lastran = DateTime.Now;           
            //Set the next time we Raffle.
            nextrun = DateTime.Now.AddMinutes(30);
            //Pick the next prize.
            UkaRaffePickPrize();
            //Reset the message count.
            Raffle.Instance.msgNum = 5;
            //Get rid of all the old tickets.
            currentTickets.Clear();
            Logger.Log(currentTickets.Count + " current tickets.");

        }

        public void BuyTicket(IRocketPlayer caller, string[] amount)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)caller;
            int int_amount;

            try {
                int.TryParse(amount[0], out int_amount);
            }
            catch {
                 int_amount = 1;
            }

            //get the balance
            decimal balance = Uconomy.Instance.Database.GetBalance(uCaller.CSteamID.ToString());
            //figure the cost of the tickets to buy
            decimal tcost = decimal.Round(int_amount * TicketCost,2);
            Logger.Log("int_amount=" + int_amount.ToString() + "TicketCost=" + TicketCost.ToString() + " tcost=" + tcost.ToString());

            if (balance < tcost)
            {
                UnturnedChat.Say(caller, "You do not have enough to purchase the tickets.");
                return;
            }
            if (balance > tcost)
            {
                decimal newbal = Uconomy.Instance.Database.IncreaseBalance(uCaller.CSteamID.ToString(), (tcost * -1));
                Logger.Log(uCaller.CharacterName + " buying " + int_amount.ToString() + " for " + tcost.ToString());
                UnturnedChat.Say(caller, "You purchased " + int_amount.ToString() + " ticket(s). Your new balance is " + newbal.ToString());
                int numTickets = int_amount;
                do
                {
                    currentTickets.Add(uCaller.CharacterName);
                    numTickets--;
                } while (numTickets > 0);

                Logger.Log(currentTickets.Count + " tickets sold!");
            }

        }

        public void ClaimPrize(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uCaller = (UnturnedPlayer)caller;
            if (uCaller.CharacterName == WinnerName && PrizedClaimed == false)
            {
                if (WinnerPrizeType == "Vehicle")
                {
                    try
                    {
                        UnturnedChat.Say(caller, "Congrats! Enjoy your prize!");
                        ushort uPrizeId = (ushort)PrizeID;
                        uCaller.GiveVehicle(uPrizeId);
                        PrizedClaimed = true;
                        Logger.Log(caller.DisplayName + "has claimed their " + vehicle.Name);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
                if (WinnerPrizeType == "Money")
                {
                    try
                    {
                        UnturnedChat.Say(caller, "Enjoy! Depositing "+ PrizeAmount.ToString() + " to your account!");
                        decimal PrizeDeposit = (Decimal)PrizeAmount;
                        decimal newbal = Uconomy.Instance.Database.IncreaseBalance(uCaller.CSteamID.ToString(), (PrizeDeposit));
                        UnturnedChat.Say(caller, "Your new balance is: " + newbal.ToString());
                        PrizedClaimed = true;
                        Logger.Log(caller.DisplayName + "'s account has been increased by " + PrizeDeposit.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            else if (uCaller.CharacterName == WinnerName && PrizedClaimed == true)
            {
                UnturnedChat.Say(caller, "You can only claim your prize once.");
            }
            else
            {
                UnturnedChat.Say(caller, "You are not the winner.");
            }
        }

        public void ShowRaffle(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer myPlayer = (UnturnedPlayer)caller;
            int myPlayerTickets = currentTickets.Count(x => x == myPlayer.CharacterName);
            try
            {
                UnturnedChat.Say(myPlayer, "You have purchased "+ myPlayerTickets +" tickets.");
                if (PrizeType == "Vehicle")
                {
                    UnturnedChat.Say(myPlayer, "The current prize is " + vehicle.Name);
                }
                if (PrizeType == "Money")
                {
                    UnturnedChat.Say(myPlayer, "The current prize is " + PrizeAmount.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void RunNow(IRocketPlayer caller, string[] command)
        {
            if (caller.IsAdmin)
            {
                UkaRaffleRun();
            }
            else
            {
                UnturnedChat.Say(caller, "Must be admin to run this command.");
            }
        }

        public void ShowStats(IRocketPlayer caller, string[] command)
        {
                int totaltickets = currentTickets.Count;
                UnturnedChat.Say(caller, "Ticket count: " + totaltickets + ".");
            if (PrizeType == "Vehicle")
            {
                UnturnedChat.Say(caller, "Current prize: " + vehicle.Name);
            }
            else if (PrizeType == "Money")
            {
                UnturnedChat.Say(caller, "Crrent prize: " + PrizeAmount.ToString());
            }
            
        }
    }
}
