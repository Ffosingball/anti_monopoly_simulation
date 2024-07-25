using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace anti_monopoly_simulation
{
    public class Player
    {
        public string type;
        public List<Street> streetsOwned;
        public List<Street> streetsMortgaged;
        public int balance, position, circlesDone, totHousesBought, totHousesSold, inThePrison;
        public List<int> houses;
        public bool imprisoned, bankrupted;

        public Player(string type, int balance)
        {
            streetsOwned = new List<Street>();
            streetsMortgaged = new List<Street>();
            houses = new List<int>();
            this.type = type;
            this.balance = balance;
            position = 0;
            imprisoned = false;
            bankrupted = false;
            circlesDone = 0;
            totHousesBought = 0;
            totHousesSold = 0;
            inThePrison = 0;
        }

        public void outputStreets()
        {
            if (streetsOwned.Count == 0)
            {
                Console.WriteLine("None streets owned");
            }
            else
            {
                for (int i = 0; i < streetsOwned.Count; i++)
                {
                    Console.WriteLine(i + ". type: " + streetsOwned[i].type + "; name: " + streetsOwned[i].name + "; houses put: " + houses[i] + "; monopolized: " + streetsOwned[i].monopolized);
                }
            }

            Console.WriteLine("Streets mortgaged^ ");

            if (streetsMortgaged.Count == 0)
            {
                Console.WriteLine("None streets mortgaged");
            }
            else
            {
                for (int i = 0; i < streetsMortgaged.Count; i++)
                {
                    Console.WriteLine(i + ". type: " + streetsMortgaged[i].type + "; name: " + streetsMortgaged[i].name + "; monopolized: " + streetsMortgaged[i].monopolized);
                }
            }
        }
    }


    public class Street
    {
        public string clas, type, name;
        public int cost, number, stepedHere, ownedBy, houses;
        public bool monopolized, mortgaged;

        public Street(string clas, string type, string name, int cost, int i)
        {
            ownedBy = -1;
            monopolized = false;
            this.clas = clas;
            this.type = type;
            this.name = name;
            this.cost = cost;
            number = i;
            stepedHere = 0;
            houses = 0;
        }
    }


    internal class Program
    {
        static Street[] streets;
        static Random rand;
        static Player[] players;
        static int shouldHave;

        static void readFile()
        {
            Console.WriteLine("readFile");

            streets = new Street[40];

            string[] linesArray = File.ReadAllLines("anti_monopoly_data.txt");

            for (int i = 0; i < linesArray.Length; i++)
            {
                string[] splitedLines = linesArray[i].Split(',');

                Street newStreet = new Street(splitedLines[1], splitedLines[2], splitedLines[3], int.Parse(splitedLines[4]), i);
                streets[i] = newStreet;
            }
        }


        static void outputStreetData()
        {
            Console.WriteLine("outputStreetData");

            for (int i = 0; i < streets.Length; i++)
            {
                Console.WriteLine(i + ". Class = " + streets[i].clas + "; types = " + streets[i].type + "; name = " + streets[i].name + "; cost = " + streets[i].cost + "; owned = " + streets[i].ownedBy + "; mortgaged = " + streets[i].mortgaged + "; houses = " + streets[i].houses + "; monopolized = " + streets[i].monopolized + "; stepped = " + streets[i].stepedHere);
            }
        }


        static void checkDice()
        {
            Console.WriteLine("checkDice");

            Console.WriteLine("Input how much times through a dice: ");
            int n = int.Parse(Console.ReadLine());

            int[] nGets = new int[11];

            for (int i = 0; i < n; i++)
            {
                nGets[throughDice() + throughDice() - 2]++;
            }

            for (int i = 0; i < 11; i++)
            {
                int n2 = i + 2;
                Console.WriteLine(n2 + ": " + nGets[i]);
            }
        }


        static void checkPlayers()
        {
            Console.WriteLine("checkPlayers");

            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine(i + ". position: " + players[i].position + "; type: " + players[i].type + "; balance: " + players[i].balance + "; bankrupted: " + players[i].bankrupted + "; imprisoned: " + players[i].imprisoned + "; circles done: " + players[i].circlesDone + "; totally houses bought: " + players[i].totHousesBought + "; totally houses sold: " + players[i].totHousesSold);
                Console.WriteLine("Streets has: ");
                players[i].outputStreets();
            }
        }


        static int throughDice()
        {
            Console.WriteLine("throughDice");
            return rand.Next(1, 7);
        }


        static void initializePlayers(int pAmount, int cAmount)
        {
            Console.WriteLine("initializePlayer");

            int cAdded = 0, mAdded = 0;

            players = new Player[pAmount];

            for (int i = 0; i < pAmount; i++)
            {
                string type = "";

                int who = rand.Next(1, 21);

                if (cAdded < cAmount && who < 11)
                {
                    cAdded++;
                    type = "c";
                }
                else if (mAdded < pAmount - cAmount)
                {
                    type = "m";
                    mAdded++;
                }
                else
                {
                    type = "c";
                }

                players[i] = new Player(type, 1500);
            }
        }


        /*static void checkTransport()
        {
            
        }*/


        static void withdrawMoney(int amount, int playerNum)
        {
            Console.WriteLine("withdrawMoney " + amount);

            players[playerNum].balance = players[playerNum].balance - amount;

            if (players[playerNum].balance < 0)
            {
                while (players[playerNum].balance < 0)
                {
                    Console.WriteLine("  ");
                    if (players[playerNum].streetsOwned.Count > 0)
                    {
                        int earning = 999;
                        int minN = 0;

                        for (int i = 0; i < players[playerNum].streetsOwned.Count; i++)
                        {
                            int curEarning = countHowMuchPay(players[playerNum].streetsOwned[i].number, playerNum, 0);

                            if (earning > curEarning)
                            {
                                minN = i;
                                earning = curEarning;
                            }
                        }

                        if (players[playerNum].type == "c")
                        {
                            if (players[playerNum].houses[minN] > 0)
                            {
                                Console.WriteLine("Choosed 1 " + minN);
                                int level = (players[playerNum].streetsOwned[minN].number / 10) + 1;
                                players[playerNum].balance = players[playerNum].balance + (players[playerNum].houses[minN] * 50 * level) / 2;
                                players[playerNum].totHousesSold = players[playerNum].houses[minN];
                                players[playerNum].houses[minN] = 0;
                                players[playerNum].streetsOwned[minN].houses = 0;
                            }
                            else
                            {
                                Console.WriteLine("Choosed 2 " + minN);
                                players[playerNum].balance = players[playerNum].balance + (players[playerNum].streetsOwned[minN].cost / 2);
                                players[playerNum].streetsMortgaged.Add(players[playerNum].streetsOwned[minN]);
                                players[playerNum].streetsOwned[minN].mortgaged = true;
                                players[playerNum].streetsOwned.RemoveAt(minN);
                                players[playerNum].houses.RemoveAt(minN);

                            }
                        }
                        else
                        {
                            if (players[playerNum].houses[minN] > 0)
                            {
                                Console.WriteLine("Choosed 3 " + minN);
                                int level = (players[playerNum].streetsOwned[minN].number / 10) + 1;
                                players[playerNum].balance = players[playerNum].balance + (players[playerNum].houses[minN] * 50 * level) / 2;
                                players[playerNum].totHousesSold = players[playerNum].houses[minN];
                                players[playerNum].houses[minN] = 0;
                                players[playerNum].streetsOwned[minN].houses = 0;
                            }
                            else
                            {
                                int sameType = 0, t = -1;
                                for (int i = 0; i < players[playerNum].streetsOwned.Count; i++)
                                {
                                    Console.WriteLine(i + ")  " + players[playerNum].streetsOwned[i].type + "  " + players[playerNum].streetsOwned[minN].type);
                                    if (players[playerNum].streetsOwned[i].type == players[playerNum].streetsOwned[minN].type && i != minN)
                                    {
                                        sameType++;
                                        t = i;

                                    }
                                }
                                Console.WriteLine("sameType: " + sameType);

                                if (sameType == 1)
                                {
                                    Console.WriteLine(minN + "=" + players[playerNum].houses[minN] + "  " + t + "=" + players[playerNum].houses[t]);
                                    if (players[playerNum].houses[t] > 0)
                                    {
                                        Console.WriteLine("Choosed 4 " + t);
                                        int level = (players[playerNum].streetsOwned[t].number / 10) + 1;
                                        players[playerNum].balance = players[playerNum].balance + (players[playerNum].houses[t] * 50 * level) / 2;
                                        players[playerNum].totHousesSold = players[playerNum].houses[t];
                                        players[playerNum].houses[t] = 0;
                                        players[playerNum].streetsOwned[t].houses = 0;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Choosed 5 " + t);
                                        players[playerNum].balance = players[playerNum].balance + (players[playerNum].streetsOwned[minN].cost / 2);
                                        players[playerNum].streetsOwned[minN].monopolized = false;
                                        players[playerNum].streetsOwned[t].monopolized = false;
                                        players[playerNum].streetsMortgaged.Add(players[playerNum].streetsOwned[minN]);
                                        players[playerNum].streetsOwned[minN].mortgaged = true;
                                        players[playerNum].streetsOwned.RemoveAt(minN);
                                        players[playerNum].houses.RemoveAt(minN);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Choosed 6 " + minN);
                                    players[playerNum].balance = players[playerNum].balance + (players[playerNum].streetsOwned[minN].cost / 2);
                                    players[playerNum].streetsOwned[minN].monopolized = false;
                                    players[playerNum].streetsMortgaged.Add(players[playerNum].streetsOwned[minN]);
                                    players[playerNum].streetsOwned[minN].mortgaged = true;
                                    players[playerNum].streetsOwned.RemoveAt(minN);
                                    players[playerNum].houses.RemoveAt(minN);
                                }
                            }
                        }
                    }
                    else
                    {
                        players[playerNum].bankrupted = true;
                        break;
                    }

                    Console.WriteLine("Balance: " + players[playerNum].balance);
                }
            }
        }


        static void unpurchasableCheck(int playerNum)
        {
            Console.WriteLine("unpurchasableCheck");

            if (streets[players[playerNum].position].type == "Anti Monopoly")
            {
                if (players[playerNum].type == "m")
                {
                    withdrawMoney(160, playerNum);
                }
                else
                {
                    int dice = throughDice();

                    if (dice == 1)
                        players[playerNum].balance = players[playerNum].balance + 25;
                    else if (dice == 2)
                        players[playerNum].balance = players[playerNum].balance + 50;
                }
            }
            else if (streets[players[playerNum].position].type == "Go to Prison")
            {
                players[playerNum].position = 10;
                players[playerNum].imprisoned = true;
            }
            else if (streets[players[playerNum].position].type == "Property Tax")
            {
                withdrawMoney(75, playerNum);
            }
            else if (streets[players[playerNum].position].type == "Income Tax")
            {
                double pay = players[playerNum].balance;

                if (players[playerNum].type == "m")
                    pay = pay * 0.2;
                else
                    pay = pay * 0.1;

                double addCost = 0;

                if (players[playerNum].streetsOwned.Count > 0)
                {
                    for (int i = 0; i < players[playerNum].streetsOwned.Count; i++)
                    {
                        addCost = addCost + players[playerNum].streetsOwned[i].cost;
                    }
                    addCost = addCost * 0.1;
                    pay = pay + addCost;
                }

                if (pay < 200)
                    withdrawMoney((int)pay, playerNum);
                else
                    withdrawMoney(200, playerNum);
            }
        }


        static void ComOrMonCheck(int playerNum)
        {
            int dice = throughDice() + throughDice();
            Console.WriteLine("Check!");

            if (players[playerNum].type == "m")
            {
                Console.WriteLine("Got " + dice);

                switch (dice)
                {
                    case 2:
                        players[playerNum].position = 0;
                        players[playerNum].balance = players[playerNum].balance + 100;
                        players[playerNum].circlesDone++;
                        break;
                    case 3:
                        players[playerNum].balance = players[playerNum].balance + 75;
                        break;
                    case 4:
                        players[playerNum].position = 24;
                        streetsProcedure(playerNum, dice);
                        break;
                    case 5:
                        withdrawMoney(75, playerNum);
                        break;
                    case 6:
                        players[playerNum].position = 12;
                        streetsProcedure(playerNum, dice);
                        break;
                    case 7:
                        players[playerNum].balance = players[playerNum].balance + 50;
                        break;
                    case 8:
                        players[playerNum].position = 25;
                        streetsProcedure(playerNum, dice);
                        break;
                    case 9:
                        withdrawMoney(50, playerNum);
                        break;
                    case 10:
                        Console.WriteLine("Choosed give 25 from competitors!");
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i].type == "c")
                            {
                                withdrawMoney(25, i);
                                players[playerNum].balance = players[playerNum].balance + 25;
                            }
                        }
                        break;
                    case 11:
                        players[playerNum].position = 10;
                        players[playerNum].imprisoned = true;
                        break;
                    case 12:
                        withdrawMoney(25, playerNum);
                        break;
                    default:
                        Console.WriteLine("In ComOrMonSomething went wrong " + dice);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Got " + dice);

                switch (dice)
                {
                    case 2:
                        players[playerNum].position = 25;
                        streetsProcedure(playerNum, dice);
                        break;
                    case 3:
                        withdrawMoney(75, playerNum);
                        break;
                    case 4:
                        Console.WriteLine("Choosed give 25 from monopolists!");
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i].type == "m")
                            {
                                withdrawMoney(25, i);
                                players[playerNum].balance = players[playerNum].balance + 25;
                            }
                        }
                        break;
                    case 5:
                        players[playerNum].position = 12;
                        streetsProcedure(playerNum, dice);
                        break;
                    case 6:
                        withdrawMoney(25, playerNum);
                        break;
                    case 7:
                        players[playerNum].position = 24;
                        streetsProcedure(playerNum, dice);
                        break;
                    case 8:
                        players[playerNum].balance = players[playerNum].balance + 75;
                        break;
                    case 9:
                        players[playerNum].position = 0;
                        players[playerNum].balance = players[playerNum].balance + 100;
                        players[playerNum].circlesDone++;
                        break;
                    case 10:
                        withdrawMoney(50, playerNum);
                        break;
                    case 11:
                        players[playerNum].balance = players[playerNum].balance + 50;
                        break;
                    case 12:
                        players[playerNum].position = 10;
                        players[playerNum].imprisoned = true;
                        break;
                    default:
                        Console.WriteLine("In ComOrMonSomething went wrong " + dice);
                        break;
                }
            }
        }


        static int countHowMuchPay(int strNum, int playerNum, int sumOnDice)
        {
            Console.WriteLine("countHowMuchPay");
            Console.WriteLine(streets[strNum].type);

            if (streets[strNum].type == "Transport")
            {
                if (players[playerNum].type == "m" && !players[playerNum].imprisoned)
                {
                    if (streets[strNum].monopolized)
                    {
                        int have = 0;
                        for (int i = 0; i < players[playerNum].streetsOwned.Count; i++)
                        {
                            if (players[playerNum].streetsOwned[i].type == "Transport" && players[playerNum].streetsOwned[i].number != streets[strNum].number)
                                have++;
                        }

                        Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 1 income: ");
                        return (int)(40 * Math.Pow(2, have));
                    }
                    else
                    {
                        Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 2 income: ");
                        return 40;
                    }
                }
                else
                {
                    Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 3 income: ");
                    return 20;
                }
            }
            else if (streets[strNum].type == "Company")
            {
                if (players[playerNum].type == "m" && !players[playerNum].imprisoned)
                {
                    if (streets[players[playerNum].position].monopolized)
                    {
                        Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 4 income: ");
                        return sumOnDice * 10;
                    }
                    else
                    {
                        Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 5 income: ");
                        return sumOnDice * 4;
                    }
                }
                else
                {
                    Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 6 income: ");
                    return sumOnDice * 4;
                }
            }
            else
            {
                int level = (streets[strNum].number / 10) + 1;

                if (players[playerNum].type == "m" && !players[playerNum].imprisoned)
                {
                    if (streets[strNum].monopolized)
                    {
                        Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 7 income: ");
                        return (int)((streets[strNum].cost + (streets[strNum].houses * 50 * level)) * 0.2);
                    }
                    else
                    {
                        Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 8 income: ");
                        return (int)((streets[strNum].cost + (streets[strNum].houses * 50 * level)) * 0.1);
                    }
                }
                else
                {
                    Console.WriteLine("StrNum: " + strNum + "; playerNum: " + playerNum + "; 9 income: ");
                    return (int)((streets[strNum].cost + (streets[strNum].houses * 50 * level)) * 0.1);
                }
            }

            //return 0;
        }


        static void streetsProcedure(int playerNum, int sumOnDice)
        {
            Console.WriteLine("streetsProcedure");

            if (!streets[players[playerNum].position].mortgaged)
            {
                if (streets[players[playerNum].position].ownedBy == -1 && players[playerNum].balance >= streets[players[playerNum].position].cost + shouldHave && streets[players[playerNum].position].cost < 340)
                {
                    streets[players[playerNum].position].ownedBy = playerNum;
                    players[playerNum].streetsOwned.Add(streets[players[playerNum].position]);
                    players[playerNum].houses.Add(0);
                    players[playerNum].balance = players[playerNum].balance - streets[players[playerNum].position].cost;

                    if (players[playerNum].type == "m")
                    {
                        for (int i = 0; i < players[playerNum].streetsOwned.Count - 1; i++)
                        {
                            if (players[playerNum].streetsOwned[i].type == streets[players[playerNum].position].type)
                            {
                                players[playerNum].streetsOwned[i].monopolized = true;
                                streets[players[playerNum].position].monopolized = true;
                            }
                        }
                    }
                }
                else if (streets[players[playerNum].position].ownedBy != -1 && streets[players[playerNum].position].ownedBy != playerNum)
                {
                    int pay = countHowMuchPay(players[playerNum].position, streets[players[playerNum].position].ownedBy, sumOnDice);

                    withdrawMoney(pay, playerNum);
                    players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + pay;
                }
            }

            streets[players[playerNum].position].stepedHere++;
        }


        static void checkWhatToDo(int playerNum, int sumOnDice)
        {
            Console.WriteLine("checkWhatToDo");

            if (streets[players[playerNum].position].clas == "Unpurchasable")
                unpurchasableCheck(playerNum);
            else if (streets[players[playerNum].position].clas == "Check")
                ComOrMonCheck(playerNum);
            else if (streets[players[playerNum].position].clas == "Street")
                streetsProcedure(playerNum, sumOnDice);
            else
                Console.WriteLine("Something went wrong " + streets[players[playerNum].position].clas + "; " + players[playerNum].position);
        }


        static void addHouses(int i, int n)
        {
            Console.WriteLine("addHouses");

            int level = (players[i].streetsOwned[n].number / 10) + 1;

            if ((50 * level) + shouldHave <= players[i].balance)
            {
                if (level == 1)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    players[i].totHousesBought++;
                    withdrawMoney(50, i);
                    Console.WriteLine("Added house 50");
                }
                if (level == 2)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    players[i].totHousesBought++;
                    withdrawMoney(100, i);
                    Console.WriteLine("Added house 100");
                }
                if (level == 3)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    players[i].totHousesBought++;
                    withdrawMoney(150, i);
                    Console.WriteLine("Added house 150");
                }
                if (level == 4)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    players[i].totHousesBought++;
                    withdrawMoney(200, i);
                    Console.WriteLine("Added house 200");
                }
            }
        }


        static void checkImprisoment(int i, int dice1, int dice2)
        {
            Console.WriteLine("checjImrisoment");

            if (players[i].imprisoned)
            {
                Console.WriteLine("Imprisoned!");
                if (players[i].balance < 50 && players[i].inThePrison < 2)
                {
                    if (dice1 == dice2)
                    {
                        players[i].imprisoned = false;
                        players[i].inThePrison = 0;
                    }
                    else
                    {
                        players[i].inThePrison++;
                    }
                }
                else
                {
                    withdrawMoney(50, i);
                    players[i].imprisoned = false;
                    players[i].inThePrison = 0;
                }
            }
        }


        static void proceedMortgages(int i)
        {
            Console.WriteLine("proceedMortgages");

            if (players[i].streetsMortgaged.Count > 0)
            {
                double min = 500;
                int n = -1;
                for (int j = 0; j < players[i].streetsMortgaged.Count; j++)
                {
                    if (players[i].streetsMortgaged[j].cost < min)
                    {
                        min = players[i].streetsMortgaged[j].cost * 0.55;
                        n = j;
                    }
                }

                if (players[i].balance >= min + shouldHave)
                {
                    players[i].balance = players[i].balance - (int)min;
                    players[i].streetsMortgaged[n].mortgaged = false;
                    players[i].streetsOwned.Add(players[i].streetsMortgaged[n]);
                    players[i].houses.Add(0);
                    players[i].streetsMortgaged.RemoveAt(n);

                    if (players[i].type == "m")
                    {
                        int sameType = 0, t = -1, cur = players[i].streetsOwned.Count - 1;
                        for (int j = 1; j < players[i].streetsOwned.Count - 1; j++)
                        {
                            if (players[i].streetsOwned[cur].type == players[i].streetsOwned[j].type)
                            {
                                sameType++;
                                t = j;
                            }
                        }

                        if (sameType > 0)
                        {
                            players[i].streetsOwned[cur].monopolized = true;
                            players[i].streetsOwned[t].monopolized = true;
                        }
                    }
                }
            }
        }


        static void buyHouse(int i)
        {
            Console.WriteLine("buyHouse");


            if (players[i].type == "c")
            {
                if (players[i].streetsOwned.Count > 0)
                {
                    if (players[i].circlesDone < 2)
                    {
                        int n = -1;
                        for (int j = 0; j < players[i].streetsOwned.Count; j++)
                        {
                            if (players[i].streetsOwned[j].houses == 0 && players[i].streetsOwned[j].type != "Transport" && players[i].streetsOwned[j].type != "Company")
                            {
                                n = j;
                                break;
                            }
                        }

                        if (n != -1)
                            addHouses(i, n);
                    }
                    else
                    {
                        int n = -1, visited = 0;
                        for (int j = 0; j < players[i].streetsOwned.Count; j++)
                        {
                            if (players[i].streetsOwned[j].stepedHere > visited && players[i].streetsOwned[j].houses < 5 && players[i].streetsOwned[j].type != "Transport" && players[i].streetsOwned[j].type != "Company")
                            {
                                n = j;
                                visited = players[i].streetsOwned[j].stepedHere;
                            }
                        }

                        if (n != -1)
                            addHouses(i, n);
                    }
                }
            }
            else
            {
                if (players[i].streetsOwned.Count > 0)
                {
                    if (players[i].circlesDone < 2)
                    {
                        int n = -1;
                        for (int j = 0; j < players[i].streetsOwned.Count; j++)
                        {
                            if (players[i].streetsOwned[j].houses == 0 && players[i].streetsOwned[j].monopolized && players[i].streetsOwned[j].type != "Transport" && players[i].streetsOwned[j].type != "Company")
                            {
                                n = j;
                                break;
                            }
                        }

                        if (n != -1)
                            addHouses(i, n);
                    }
                    else
                    {
                        int n = -1, visited = 0;
                        for (int j = 0; j < players[i].streetsOwned.Count; j++)
                        {
                            if (players[i].streetsOwned[j].stepedHere > visited && players[i].streetsOwned[j].houses < 4 && players[i].streetsOwned[j].monopolized && players[i].streetsOwned[j].type != "Transport" && players[i].streetsOwned[j].type != "Company")
                            {
                                n = j;
                                visited = players[i].streetsOwned[j].stepedHere;
                            }
                        }

                        if (n != -1)
                            addHouses(i, n);
                    }
                }
            }
        }


        static void makeTurn(int pAmount)
        {
            Console.WriteLine(" ");
            Console.WriteLine("makeTurn");

            for (int i = 0; i < pAmount; i++)
            {
                if (players[i].bankrupted)
                {
                    Console.WriteLine("Bankrupted!");
                    continue;
                }

                int dice1 = throughDice();
                int dice2 = throughDice();
                int sum = dice1 + dice2;

                checkImprisoment(i, dice1, dice2);

                if (players[i].imprisoned)
                {
                    continue;
                }

                Console.WriteLine("Go " + sum + " steps. Position: " + players[i].position);

                if (players[i].position + dice1 + dice2 < 40)
                {
                    players[i].position = players[i].position + dice1 + dice2;
                }
                else if (players[i].position + dice1 + dice2 == 40)
                {
                    players[i].position = 0;
                    players[i].balance = players[i].balance + 100;
                    players[i].circlesDone++;
                }
                else
                {
                    players[i].position = players[i].position + dice1 + dice2 - 40;
                    players[i].balance = players[i].balance + 100;
                    players[i].circlesDone++;
                }

                checkWhatToDo(i, sum);
                Console.WriteLine("Done streets");

                if (dice1 == dice2)
                {
                    dice1 = throughDice();
                    dice2 = throughDice();
                    sum = dice1 + dice2;

                    Console.WriteLine("Go " + sum + " steps. Position: " + players[i].position);

                    if (players[i].position + dice1 + dice2 < 40)
                    {
                        players[i].position = players[i].position + dice1 + dice2;
                    }
                    else if (players[i].position + dice1 + dice2 == 40)
                    {
                        players[i].position = 0;
                        players[i].balance = players[i].balance + 100;
                        players[i].circlesDone++;
                    }
                    else
                    {
                        players[i].position = players[i].position + dice1 + dice2 - 40;
                        players[i].balance = players[i].balance + 100;
                        players[i].circlesDone++;
                    }

                    checkWhatToDo(i, sum);
                    Console.WriteLine("Done streets");
                }


                proceedMortgages(i);


                buyHouse(i);


                Console.WriteLine(i + ") player done! Position: " + players[i].position + "; balance" + players[i].balance);
            }
        }


        static void checkWinner()
        {
            Console.WriteLine("checkWinner");

            for (int i = 0; i < players.Length; i++)
            {
                int totalIncome = 0;

                if (!players[i].bankrupted)
                {
                    for (int j = 0; j < players[i].streetsOwned.Count; j++)
                    {
                        int inc = countHowMuchPay(players[i].streetsOwned[j].number, i, throughDice() + throughDice());
                        totalIncome = totalIncome + inc;
                        Console.WriteLine("Type: " + players[i].streetsOwned[j].type + "; houses: " + players[i].streetsOwned[j].houses + "; income: " + inc);
                    }

                    Console.WriteLine("totalIncome " + totalIncome);
                    int score = players[i].balance + totalIncome;
                    Console.WriteLine("Player " + i + " " + players[i].type + " has " + score + " score");
                }
                else
                {
                    Console.WriteLine("Player " + i + " " + players[i].type + " bankrupted!");
                }
            }
        }


        static void Main(string[] args)
        {
            rand = new Random();
            shouldHave = 200;
            //DONE! CH! Доделать если монополист выкупает mortgaged улицу
            //и у него 2 штуки одинакового типа то нужно сделать их
            //монополизироваными снова

            //DONE! CH! Также оно не правильно выщитывает цену которую нужно 
            //платить на транспортных компаниях монополистаи

            //DONE! CH! Также оно отдельно кидает кубики для газа и электрикт
            //хотя нужно брать те которые выкинуты для ходьбы

            //DONE! CH! Также нужно чтобы бот продавал не первую улицк
            //на mortgaged а улицу с наименьшим доходом

            //DONE! CH! Также Сделать отдельную процедуру по подсчету сколько нужно платить игроку

            //DONE! CH! Также проверить com or mon

            //DONE! CH! Также выделить покупку домиков и mortgaged улици и 
            // CH! тбрьму в отднльные
            //процедуры

            //DONE! CH! Также добавить вариант выхода из тюрьмы кинувши два одинаковых кубика

            //Также сделать нормальный интерфейс


            readFile();
            //outputStreetData();
            //Console.ReadLine();

            Console.WriteLine("Input how much players do you want: ");
            int playersAmount = int.Parse(Console.ReadLine());
            //string check = Console.ReadLine();
            //check = Console.ReadLine();

            Console.WriteLine("Input how much competitors do you want: ");
            int competitorsAmount = int.Parse(Console.ReadLine());

            initializePlayers(playersAmount, competitorsAmount);

            Console.WriteLine(" ");
            Console.WriteLine("Players data");
            checkPlayers();

            //outputStreetData();

            Console.WriteLine("Input how much steps do you want to do: ");
            int steps = int.Parse(Console.ReadLine());

            for (int i = 0; i < steps; i++)
            {
                Console.WriteLine(" ");
                Console.WriteLine("-----");

                makeTurn(playersAmount);

                Console.WriteLine(i + " turn done!");
            }

            Console.WriteLine(" ");
            Console.WriteLine("Players data");
            checkPlayers();

            //Console.WriteLine(" ");
            //Console.WriteLine("Streets data");
            //outputStreetData();

            // checkTransport();
            Console.WriteLine(" ");
            Console.WriteLine("Who is winner");
            checkWinner();

            Console.ReadLine();
        }
    }
}
