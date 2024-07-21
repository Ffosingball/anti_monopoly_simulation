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
        public int balance, position;
        public List<int> houses;

        public Player(string type, int balance) 
        {
            streetsOwned = new List<Street>();
            houses = new List<int>();
            this.type = type;
            this.balance = balance;
            position = 0;
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
                    Console.WriteLine(i+". type: " + streetsOwned[i].type+"; name: "+ streetsOwned[i].name+"; houses puted" + houses[i]);
                }
            }
        }
    }


    public class Street 
    {
        public string clas, type, name;
        public int cost, number, stepedHere;
        public bool owned, monopolized;

        public Street(string clas, string type,string name,int cost,int i) 
        {
            owned=false;
            monopolized = false;
            this.clas = clas;
            this.type = type;
            this.name = name;
            this.cost = cost;
            number = i;
            stepedHere = 0;
        } 
    }


    internal class Program
    {
        static Street[] streets;
        static Random rand;
        static Player[] players;

        static void checkFileReading()
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
                Console.WriteLine(i + ". Class = " + streets[i].clas + "; types = " + streets[i].type + "; name = " + streets[i].name + "; cost = " + streets[i].cost + "; owned = " + streets[i].owned);
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
                Console.WriteLine(i+". position: "+players[i].position+"; type: "+ players[i].type+"; balance: "+ players[i].balance);
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


        static void makeTurn() 
        {
            
        }


        static void Main(string[] args)
        {
            rand = new Random();

            Console.WriteLine("Input how much players do you want: ");
            int playersAmount=int.Parse(Console.ReadLine());

            Console.WriteLine("Input how much competitors do you want: ");
            int competitorsAmount = int.Parse(Console.ReadLine());

            initializePlayers(playersAmount, competitorsAmount);

            checkPlayers();

            Console.WriteLine("Input how much steps do you want to do: ");
            int steps = int.Parse(Console.ReadLine());

            for (int i = 0; i < steps; i++) 
            {
                makeTurn();
            }

            Console.ReadLine();
        }
    }
}
