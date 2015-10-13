/**
 * UkaRaffleConfiguration
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
using System.Xml.Serialization;
using Rocket.API;

namespace UkaRaffle
{
    public sealed class Vehicle //Create our vehicle object.
    {
        [XmlAttribute("Id")]
        public ushort Id;

        [XmlAttribute("Name")]
        public string Name;

        public Vehicle(ushort id, string name)
        {
            Id = id;
            Name = name;
        }

        public Vehicle()
        {
            Id = 0;
            Name = "";
        }

    }

    public sealed class Money
    {
        [XmlAttribute("Amount")]
        public double Amount;

        public Money(double amount)
        {
            Amount = amount;
        }
        public Money()
        {
            Amount = 0.00;
        }
    }

    
    public class UkaRaffleConfiguration:IRocketPluginConfiguration
    {
        public bool Enabled;
        public int Interval;
        public decimal TicketCost;
        public string MessageColor;
        [XmlArrayItem("Vehicle")]
        [XmlArray(ElementName = "Vehicles")]
        public Vehicle[] Vehicles;
        [XmlArrayItem("Money")]
        [XmlArray(ElementName = "MoneyPrizes")]
        public Money[] MoneyPrizes;

        public void LoadDefaults()
        {
            Enabled = true;
            Interval = 1800;
            TicketCost = 5;
            MessageColor = "blue";
            MoneyPrizes = new Money[] {
                new Money(100.00),
                new Money(1000.00),
                new Money(10000.00),
                new Money(100000.00)
            };
            Vehicles = new Vehicle[]{
                new Vehicle(1, "Black Offroader"),
                new Vehicle(2, "Blue Offroader"),
                new Vehicle(11, "Green Hatchback"),
                new Vehicle(12, "Orange Hatchback"),
                new Vehicle(21, "Purple Truck"),
                new Vehicle(22, "Red Truck"),
                new Vehicle(31, "White Sedan"),
                new Vehicle(32, "Yellow Sedan"),
                new Vehicle(33, "Police Car"),
                new Vehicle(34, "Fire Truck"),
                new Vehicle(35, "Black Van"),
                new Vehicle(36, "Blue Van"),
                new Vehicle(49, "White Roadster"),
                new Vehicle(50, "Yellow Roadster"),
                new Vehicle(52, "Humvee"),
                new Vehicle(54, "Ambulance"),
                new Vehicle(69, "Green Quad"),
                new Vehicle(73, "White Quad"),
                new Vehicle(76, "Taxi"),
                new Vehicle(77, "Racecar"),
                new Vehicle(85, "Tractor"),
                new Vehicle(87, "Forest Jeep"),
                new Vehicle(88, "Desert Jeep")
            };
        }
    }

}
