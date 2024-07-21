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
        public int balance, position, circlesDone;
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
                    Console.WriteLine(i+". type: " + streetsOwned[i].type+"; name: "+ streetsOwned[i].name+"; houses put: " + houses[i]);
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
                    Console.WriteLine(i + ". type: " + streetsMortgaged[i].type + "; name: " + streetsMortgaged[i].name);
                }
            }
        }
    }


    public class Street 
    {
        public string clas, type, name;
        public int cost, number, stepedHere, ownedBy, houses;
        public bool monopolized, mortgaged;

        public Street(string clas, string type,string name,int cost,int i) 
        {
            ownedBy=-1;
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
            for (int i = 0; i < streets.Length; i++)
            {
                Console.WriteLine(i + ". Class = " + streets[i].clas + "; types = " + streets[i].type + "; name = " + streets[i].name + "; cost = " + streets[i].cost + "; owned = " + streets[i].ownedBy+"; mortgaged = " + streets[i].mortgaged + "; houses = " + streets[i].houses + "; monopolized = " + streets[i].monopolized + "; stepped = " + streets[i].stepedHere);
            }
        }


        static void checkDice() 
        {
            Console.WriteLine("Input how much times through a dice: ");
            int n = int.Parse(Console.ReadLine());

            int[] nGets = new int[11];

            for (int i = 0; i < n; i++) 
            {
                nGets[throughDice()+ throughDice() - 2]++;
            }

            for (int i = 0; i < 11; i++) 
            {
                int n2 = i + 2;
                Console.WriteLine(n2+": " + nGets[i]);
            }
        }


        static void checkPlayers() 
        {
            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine(i+". position: "+players[i].position+"; type: "+ players[i].type+"; balance: "+ players[i].balance+"; bankrupted: "+ players[i].bankrupted+"; imprisoned: "+ players[i].imprisoned+"; circles done: "+ players[i].circlesDone);
                Console.WriteLine("Streets has: ");
                players[i].outputStreets();
            }
        }


        static int throughDice() 
        {
            return rand.Next(1, 7);
        }


        static void initializePlayers(int pAmount, int cAmount) 
        {
            int cAdded = 0, mAdded=0;

            players = new Player[pAmount];

            for (int i = 0; i < pAmount; i++) 
            {
                string type = "";

                int who = rand.Next(1,21);

                if (cAdded < cAmount && who <11)
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


        static void withdrawMoney(int amount, int playerNum) 
        {
            players[playerNum].balance = players[playerNum].balance - 160;

            if (players[playerNum].balance < 0)
            {
                while (players[playerNum].balance < 0) 
                {
                    if (players[playerNum].houses.Sum() > 0)
                    {
                        for (int i = 0; i < players[playerNum].houses.Count; i++)
                        {
                            if (players[playerNum].houses[i] > 0)
                            {
                                int level = (players[playerNum].streetsOwned[i].number / 10) + 1;
                                players[playerNum].balance = players[playerNum].balance + (players[playerNum].houses[i] * 50 * level);
                                players[playerNum].houses[i] = 0;
                                players[playerNum].streetsOwned[i].houses = 0;
                                break;
                            }
                        }
                    }
                    else if (players[playerNum].streetsOwned.Count > 0)
                    {
                        players[playerNum].balance = players[playerNum].balance + (players[playerNum].streetsOwned[0].cost / 2);
                        players[playerNum].streetsMortgaged.Add(players[playerNum].streetsOwned[0]);
                        players[playerNum].streetsOwned[0].mortgaged = true;
                        players[playerNum].streetsOwned.RemoveAt(0);
                    }
                    else 
                    {
                        players[playerNum].bankrupted = true;
                    }
                }
            }
        }


        static void unpurchasableCheck(int playerNum) 
        {
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
                double pay=players[playerNum].balance;

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

                if(pay<200)
                    withdrawMoney((int)pay, playerNum);
                else
                    withdrawMoney(200, playerNum);
            }
        }


        static void ComOrMonCheck(int playerNum)
        {
            int dice=throughDice()+throughDice();

            if (players[playerNum].type == "m")
            {
                switch (dice)
                {
                    case 2:
                        players[playerNum].position = 0;
                        players[playerNum].balance = players[playerNum].balance+100;
                        players[playerNum].circlesDone++;
                        break;
                    case 3:
                        players[playerNum].balance = players[playerNum].balance + 75;
                        break;
                    case 4:
                        players[playerNum].position = 24;
                        streetsProcedure(playerNum);
                        break;
                    case 5:
                        withdrawMoney(75, playerNum);
                        break;
                    case 6:
                        players[playerNum].position = 12;
                        streetsProcedure(playerNum);
                        break;
                    case 7:
                        players[playerNum].balance = players[playerNum].balance + 50;
                        break;
                    case 8:
                        players[playerNum].position = 25;
                        streetsProcedure(playerNum);
                        break;
                    case 9:
                        withdrawMoney(50, playerNum);
                        break;
                    case 10:
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
                switch (dice)
                {
                    case 2:
                        players[playerNum].position = 25;
                        streetsProcedure(playerNum);
                        break;
                    case 3:
                        withdrawMoney(75, playerNum);
                        break;
                    case 4:
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
                        streetsProcedure(playerNum);
                        break;
                    case 6:
                        withdrawMoney(25, playerNum);
                        break;
                    case 7:
                        players[playerNum].position = 24;
                        streetsProcedure(playerNum);
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


        static void streetsProcedure(int playerNum) 
        {
            if (!streets[players[playerNum].position].mortgaged) 
            {
                if (streets[players[playerNum].position].ownedBy == -1 && players[playerNum].balance >= streets[players[playerNum].position].cost + shouldHave && streets[players[playerNum].position].cost<340)
                {
                    streets[players[playerNum].position].ownedBy = playerNum;
                    players[playerNum].streetsOwned.Add(streets[players[playerNum].position]);
                    players[playerNum].houses.Add(0);

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
                else if (streets[players[playerNum].position].ownedBy != -1) 
                {
                    if (streets[players[playerNum].position].type == "Transport")
                    {
                        if (players[streets[players[playerNum].position].ownedBy].type == "m" && !players[streets[players[playerNum].position].ownedBy].imprisoned)
                        {
                            if (streets[players[playerNum].position].monopolized)
                            {
                                int have = 0;
                                for (int i = 0; i < players[streets[players[playerNum].position].ownedBy].streetsOwned.Count; i++) 
                                {
                                    if (players[streets[players[playerNum].position].ownedBy].streetsOwned[i].type == "Transport")
                                        have++;
                                }

                                double pay = 40 * have;
                                withdrawMoney((int)pay, playerNum);
                                players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                            }
                            else
                            {
                                withdrawMoney(40, playerNum);
                                players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + 40;
                            }
                        }
                        else
                        {
                            withdrawMoney(20, playerNum);
                            players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + 20;
                        }
                    }
                    else if (streets[players[playerNum].position].type == "Company")
                    {
                        if (players[streets[players[playerNum].position].ownedBy].type == "m" && !players[streets[players[playerNum].position].ownedBy].imprisoned)
                        {
                            if (streets[players[playerNum].position].monopolized)
                            {
                                double pay = (throughDice()+throughDice())*10;
                                withdrawMoney((int)pay, playerNum);
                                players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                            }
                            else
                            {
                                double pay = (throughDice() + throughDice()) * 4;
                                withdrawMoney((int)pay, playerNum);
                                players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                            }
                        }
                        else
                        {
                            double pay = (throughDice() + throughDice()) * 4;
                            withdrawMoney((int)pay, playerNum);
                            players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                        }
                    }
                    else
                    {
                        if (players[streets[players[playerNum].position].ownedBy].type == "m" && !players[streets[players[playerNum].position].ownedBy].imprisoned)
                        {
                            if (streets[players[playerNum].position].monopolized)
                            {
                                double pay = (streets[players[playerNum].position].cost + streets[players[playerNum].position].houses) * 0.2;
                                withdrawMoney((int)pay, playerNum);
                                players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                            }
                            else
                            {
                                double pay = streets[players[playerNum].position].cost * 0.1;
                                withdrawMoney((int)pay, playerNum);
                                players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                            }
                        }
                        else
                        {
                            double pay = (streets[players[playerNum].position].cost + streets[players[playerNum].position].houses) * 0.1;
                            withdrawMoney((int)pay, playerNum);
                            players[streets[players[playerNum].position].ownedBy].balance = players[streets[players[playerNum].position].ownedBy].balance + (int)pay;
                        }
                    }
                }
            }

            streets[players[playerNum].position].stepedHere++;
        }


        static void checkWhatToDo(int playerNum) 
        {
            if (streets[players[playerNum].position].clas == "Unpurchasable")
                unpurchasableCheck(playerNum);
            else if (streets[players[playerNum].position].clas == "Check")
                ComOrMonCheck(playerNum);
            else if (streets[players[playerNum].position].clas == "Street")
                streetsProcedure(playerNum);
            else
                Console.WriteLine("Something went wrong "+ streets[players[playerNum].position].clas+"; "+ players[playerNum].position);
        }


        static void addHouses(int i, int n) 
        {
            int level = (players[i].streetsOwned[n].number / 10) + 1;

            if ((50 * level) + shouldHave <= players[i].balance)
            {
                if (level == 1)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    withdrawMoney(50, i);
                }
                if (level == 2)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    withdrawMoney(100, i);
                }
                if (level == 3)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    withdrawMoney(150, i);
                }
                if (level == 4)
                {
                    players[i].streetsOwned[n].houses = players[i].streetsOwned[n].houses + 1;
                    players[i].houses[n] = players[i].houses[n] + 1;
                    withdrawMoney(200, i);
                }
            }
        }


        static void makeTurn(int pAmount) 
        {
            for (int i = 0; i < pAmount; i++) 
            {
                if (players[i].bankrupted)
                    continue;

                if (players[i].imprisoned)
                    withdrawMoney(50,i);

                int dice1 = throughDice();
                int dice2=throughDice();

                Console.WriteLine("Go "+dice1+dice2+" steps");

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

                checkWhatToDo(i);

                if (dice1 == dice2) 
                {
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

                    checkWhatToDo(i);
                }


                if (players[i].streetsMortgaged.Count > 0) 
                {
                    double min = 500;
                    int n=-1;
                    for (int j = 0; j < players[i].streetsMortgaged.Count; j++) 
                    {
                        if (players[i].streetsMortgaged[j].cost < min) 
                        {
                            min = players[i].streetsMortgaged[j].cost*0.55;
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
                    }
                }


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
                                }
                            }

                            if (n == -1)
                                continue;

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

                            if (n == -1)
                                continue;

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
                                }
                            }

                            if (n == -1)
                                continue;

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

                            if (n == -1)
                                continue;

                            addHouses(i, n);
                        }
                    }
                }

                Console.WriteLine(i + " player done!");
            }
        }


        static void checkWinner() 
        {
            for (int i = 0; i < players.Length; i++) 
            {
                int totalIncome = 0;

                for (int j = 0; j < players[i].streetsOwned.Count; j++) 
                {
                    if (players[i].streetsOwned[j].type == "Transport")
                    {
                        if (players[i].type == "m")
                        {
                            if (players[i].streetsOwned[j].monopolized)
                            {
                                int have = 0;
                                for (int k = 0; k < players[i].streetsOwned.Count; k++)
                                {
                                    if (players[i].streetsOwned[j].type == "Transport" && k!=j)
                                        have++;
                                }

                                totalIncome =totalIncome+ (40 * have);
                            }
                            else
                            {
                                totalIncome = totalIncome + 40;
                            }
                        }
                        else
                        {
                            totalIncome = totalIncome + 20;
                        }
                    }
                    else if (players[i].streetsOwned[j].type == "Company")
                    {
                        if (players[i].type == "m")
                        {
                            if (players[i].streetsOwned[j].monopolized)
                            {
                                totalIncome = totalIncome + ((throughDice() + throughDice()) * 10);
                            }
                            else
                            {
                                totalIncome = totalIncome + ((throughDice() + throughDice()) * 4);
                            }
                        }
                        else
                        {
                            totalIncome = totalIncome + ((throughDice() + throughDice()) * 10);
                        }
                    }
                    else
                    {
                        if (players[i].type == "m")
                        {
                            if (players[i].streetsOwned[j].monopolized)
                            {
                                totalIncome = totalIncome + (int)((players[i].streetsOwned[j].cost + players[i].streetsOwned[j].houses)*0.2);
                            }
                            else
                            {
                                totalIncome = totalIncome + (int)(players[i].streetsOwned[j].cost * 0.1);
                            }
                        }
                        else
                        {
                            totalIncome = totalIncome + (int)((players[i].streetsOwned[j].cost + players[i].streetsOwned[j].houses) * 0.1);
                        }
                    }
                }

                int score = players[i].balance + totalIncome;
                Console.WriteLine("Player " + i + " has " + score + " score");
            }
        }


        static void Main(string[] args)
        {
            rand = new Random();
            shouldHave = 200;

            readFile();

            Console.WriteLine("Input how much players do you want: ");
            int playersAmount=int.Parse(Console.ReadLine());

            Console.WriteLine("Input how much competitors do you want: ");
            int competitorsAmount = int.Parse(Console.ReadLine());

            initializePlayers(playersAmount, competitorsAmount);

            checkPlayers();

            outputStreetData();

            Console.WriteLine("Input how much steps do you want to do: ");
            int steps = int.Parse(Console.ReadLine());

            for (int i = 0; i < steps; i++) 
            {
                makeTurn(playersAmount);

                Console.WriteLine(i+" turn done!");
            }

            checkPlayers();

            outputStreetData();

            checkWinner();

            Console.ReadLine();
        }
    }
}
