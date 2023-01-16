using BulletManiac.Entity.Enemies;
using BulletManiac.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac
{
    public static class GameResult
    {
        /*
            Floor - each floor add 10 pts
            Level Clear - 100 pts if true
            Time - Every 30 seconds minus 5pts
            Bat - 1pts each
            Shadow - 2pts each
            Suicide - 3pts each
            Summoner - 4pts each
                
        */
        // Enemy eliminated
        public static int Floor = 0;
        public static bool LevelClear = false;
        public static float TimeComplete = 0f;
        public static int Bat = 0;
        public static int Shadow = 0;
        public static int SuicideShadow = 0;
        public static int Summoner = 0;

        public static int FloorScore { get; private set; }
        public static int TimeScore { get; private set; }
        public static int Enemy 
        { 
            get
            {
                return Bat + Shadow + SuicideShadow + Summoner;
            } 
        }

        public static int Score = 0;

        public static void Reset()
        {
            Floor = 0;
            LevelClear = false;
            TimeComplete = 0f;
            Bat = 0;
            Shadow = 0;
            SuicideShadow = 0;
            Summoner = 0;
            Score = 0;
        }

        public static void CountEnemy(Enemy enemy)
        {
            if (enemy is Bat)
                Bat++;
            else if (enemy is Shadow)
                Shadow++;
            else if (enemy is SuicideShadow)
                SuicideShadow++;
            else if(enemy is Summoner)
                Summoner++;
        }

        public static void Calculate()
        {
            Floor = GameManager.Floor;
            TimeComplete = GameManager.TimePass;

            FloorScore = Floor * 10;
            TimeScore = ((int)(TimeComplete / 30) * 5);
            Score = FloorScore - TimeScore + Bat + (Shadow * 2) + (SuicideShadow * 3) + (Summoner * 4);
            if (LevelClear) Score += 100;
        }
    }
}
