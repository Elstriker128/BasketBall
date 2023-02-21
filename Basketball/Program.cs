using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Basketball
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Team firstTeam = InOutUtils.Read("Basket33.txt");
            Team secondTeam = InOutUtils.Read("Basket34.txt");

            if (File.Exists("Rez.txt"))
            {
                File.Delete("Rez.txt");
            }

            InOutUtils.Print(firstTeam, "Rez.txt", firstTeam.TeamName);
            InOutUtils.Print(secondTeam, "Rez.txt", secondTeam.TeamName);

            TaskUtils.Remove(firstTeam);
            TaskUtils.Remove(secondTeam);

            firstTeam.Sort();
            secondTeam.Sort();

            InOutUtils.Print(firstTeam, "Rez.txt", firstTeam.TeamName);
            InOutUtils.Print(secondTeam, "Rez.txt", secondTeam.TeamName);

            if(firstTeam>secondTeam)
            {
                File.AppendAllText("Rez.txt", firstTeam.TeamName + " yra daugiau žaidėjų pelniusių 0 taškų");
            }
            else if(secondTeam>firstTeam)
            {
                File.AppendAllText("Rez.txt", secondTeam.TeamName + " yra daugiau žaidėjų pelniusių 0 taškų");
            }
            else if(firstTeam==secondTeam) 
            {
                File.AppendAllText("Rez.txt", "Abiejuose komandos yra vienodai žaidėjų pelniusių 0 taškų");
            }
        }
    }
    class BasketPlayer
    {
        public string Surname { get; private set; }
        public string Name { get; private set; }
        private int[] Scores { get; set; }

        public BasketPlayer(string surname, string name, int[] scores)
        {
            Surname = surname;
            Name = name;
            Scores = scores;
        }
        public int[] GetScores() 
        { 
            return this.Scores; 
        }
        public int CountC00(int ii)
        {
            if(ii < Scores.Count())
            {
                if (Scores[ii] == 0)
                {
                    return 1 + CountC00(ii + 1);
                }
               return CountC00(ii + 1);
            }
            else
            {
                return 0;
            }
        }
        
        public static bool operator >(BasketPlayer a, BasketPlayer b)
        {
            if(a.Surname!=b.Surname)
            {
                return string.Compare(a.Surname, b.Surname) > 0;
            }
            else if(a.Name!=b.Name)
            {
                return string.Compare(a.Name,b.Name) > 0;
            }
            else
            {
                return false;
            }
        }
        public static bool operator <(BasketPlayer a, BasketPlayer b)
        {
            if (a.Surname != b.Surname)
            {
                return string.Compare(a.Surname, b.Surname) < 0;
            }
            else if (a.Name != b.Name)
            {
                return string.Compare(a.Name, b.Name) < 0;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return String.Format($"{this.Surname,-20} | {this.Name,-20} |");
        }
    }
    //
    class Team
    {
        private List<BasketPlayer> allPlayers;
        public string TeamName { get; private set; }
        public Team(string teamName)
        {
            allPlayers= new List<BasketPlayer>();
            this.TeamName = teamName;
        }

        public static bool operator >(Team a, Team b)
        {
            return a.Count() > b.Count();
        }
        public static bool operator <(Team a, Team b)
        {
            return a.Count() < b.Count();
        }
        public static bool operator ==(Team a, Team b)
        {
            return a.Count() == b.Count();
        }
        public static bool operator !=(Team a, Team b)
        {
            return a.Count() != b.Count();
        }
        public BasketPlayer Get(int index)
        {
            return this.allPlayers[index];
        }
        public int Count()
        {
            return allPlayers.Count();
        }
        public void Add(BasketPlayer player)
        {
            allPlayers.Add(player);
        }
        public override bool Equals(object obj)
        {
            return obj is Team team &&
                   EqualityComparer<List<BasketPlayer>>.Default.Equals(allPlayers, team.allPlayers);
        }

        public override int GetHashCode()
        {
            return -1864071250 + EqualityComparer<List<BasketPlayer>>.Default.GetHashCode(allPlayers);
        }
        public void Sort()
        {
            int i = 0;
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int j = allPlayers.Count()-1; j > i; j--)
                {
                    if (allPlayers[j] < allPlayers[j - 1])
                    {
                        flag = true;
                        var current = allPlayers[j];
                        allPlayers[j] = allPlayers[j - 1];
                        allPlayers[j - 1] = current;
                    }
                }
                i++;
            }
        }
        public void Remove(int index)
        {
            allPlayers.Remove(Get(index));
        }
    }
    class TaskUtils
    {
        public static void Remove(Team T)
        {
            for (int i = 0; i < T.Count(); i++)
            {
                BasketPlayer current = T.Get(i);
                if (current.CountC00(0) == 0)
                {
                    T.Remove(i);
                    i--;
                }
            }
        }
    }
    class InOutUtils
    {
        public static Team Read(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            string teamName = Regex.Match(lines[0],$@"([\w]+)").Value;
            Team allPlayers = new Team(teamName);
            foreach(string line in lines.Skip(1))
            {
                string[] values = Regex.Split(line, ", ");
                string surname  = values[0];
                string name = values[1];
                int[] scores = new int[values.Length-2];
                for (int i = 0; i < values.Count()-2; i++)
                {
                    scores[i] = int.Parse(values[2+i]);
                }
                BasketPlayer current = new BasketPlayer(surname,name,scores);
                allPlayers.Add(current);
            }
            return allPlayers;
        }
        public static void Print(Team T, string Filename, string Header)
        {
            string[] lines = new string[T.Count() + 7];
            lines[0] = String.Format(new string('-', 75));
            lines[1] = String.Format($"{Header,-20}");
            lines[2] = String.Format(new string('-', 75));
            for (int i = 0; i < T.Count(); i++)
            {
                BasketPlayer current = T.Get(i);
                lines[2 + i] = String.Format($"{current.ToString()}");
                int[] curScores = current.GetScores();

                for (int j = 0; j < curScores.Count(); j++)
                {
                    lines[2 + i] += String.Format($"{curScores[j],-2} ");
                }
            }
            lines[T.Count() + 2] = String.Format(new string('-', 75));
            File.AppendAllLines(Filename, lines, Encoding.UTF8);
        }
    }
}
