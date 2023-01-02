﻿using BulletManiac.Entity;
using BulletManiac.Entity.Enemy;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled
{
    public class Spawn : GameObject
    {
        private float spawnDelay;
        private readonly Enemy enemy;
        private readonly TextureEffect effect;
        private bool spawn = false;
        private Vector2 smokeOffset = Vector2.Zero;

        public Spawn(Enemy enemy, Vector2 position, float spawnDelay = 0.5f)
        {
            this.spawnDelay = spawnDelay;
            this.enemy = enemy;
            Animation smokeAnim = new Animation(GameManager.Resources.FindAnimation("Spawn_Smoke_Animation"));
            effect = new TextureEffect(smokeAnim, position, new Vector2(16, 16), new Vector2(1.5f), true);
            GameManager.AddGameObject(effect);
        }

        public Spawn(Enemy enemy, Vector2 position, Vector2 smokeOffset, float spawnDelay = 0.5f)
        {
            this.spawnDelay = spawnDelay;
            this.enemy = enemy;
            Animation smokeAnim = new Animation(GameManager.Resources.FindAnimation("Spawn_Smoke_Animation"));
            effect = new TextureEffect(smokeAnim, position + smokeOffset, new Vector2(16, 16), new Vector2(1.5f), true);
            GameManager.AddGameObject(effect);
            this.smokeOffset = smokeOffset;
        }

        public override void Update(GameTime gameTime)
        {
            spawnDelay -= GameManager.DeltaTime;
            if(spawnDelay <= 0f && !spawn) // Add the enemy only once
            {
                GameManager.AddGameObject(enemy);
                spawn = true;
            }

            if(spawnDelay <= 0f && GameManager.FindGameObject(effect) == null)
            {
                Destroy(this); // Delete the spawner
            }
            base.Update(gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }
    }
    public class Spawner
    {
        private float spawnCD = 1f;
        private float currentSpawnCD = 1f;
        private int spawnNumber = 2;
        private int currentNumber;

        int batCount = 0;
        int shadowCount = 0;
        int suicideShadowCount = 0;
        int summnerCount = 0;

        public Spawner()
        {
            //batCount = 10;
            //shadowCount = 5;
            //suicideShadowCount = 3;
            //summnerCount = 1;

            spawnNumber = 5;
            currentNumber = spawnNumber;
        }

        public void Update(GameTime gameTime)
        {
            // Every X second spawn x enemy from list
            if(currentNumber > 0)
            {
                currentSpawnCD -= GameManager.DeltaTime;
                if (currentSpawnCD <= 0f)
                {
                    Vector2 pos = GameManager.CurrentLevel.TileGraph.RandomPositionAwayFromDistance(100f);
                    int r = Extensions.Random.Next(0, 4);
                    switch (r)
                    {
                        case 0:
                            Spawn(new Bat(pos), pos);
                            break;
                        case 1:
                            Spawn(new Shadow(pos), pos);
                            break;
                        case 2:
                            Spawn(new SuicideShadow(pos), pos);
                            break;
                        case 3:
                            Spawn(new Summoner(pos), pos);
                            break;
                        default:
                            GameManager.Log("Spawner", "The random number is out of bound r = " + r);
                            break;
                    }
                    currentSpawnCD = spawnCD;
                    currentNumber--;
                }
            }
        }

        public void Reset()
        {
            currentNumber = spawnNumber;
        }

        public void Spawn(Enemy enemy, Vector2 position)
        {
            // Dump fix for the smoke position correct with the enemy spawn
            if(enemy is Summoner)
                GameManager.AddGameObject(new Spawn(enemy, position, new Vector2(0f, 5f)));
            else
                GameManager.AddGameObject(new Spawn(enemy, position, new Vector2(0f, -12f)));
        }
    }
}