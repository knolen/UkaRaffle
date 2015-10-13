/**
 * UkaRaffelCommand
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
using UnityEngine;
using SDG.Unturned;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace UkaRaffel
{
    public class UakRaffelCommand:IRocketCommand
    {
        public string Name
        {
            get { return "rticket";  }
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Help
        {
            get { return "Buy raffel ticket(s)"; }
        }

        public string Syntax
        {
            get { return "rticket <quanity>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "UkaRaffel.rticket" }; }
        }

        public void Execute(IRocketPlayer caller, string[] amount)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Raffel.Instance.BuyTicket(player, amount);
            return;
        }

    }
}
