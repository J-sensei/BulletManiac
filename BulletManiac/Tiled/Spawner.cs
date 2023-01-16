using BulletManiac.Entity;
using BulletManiac.Entity.Enemies;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
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
            Animation smokeAnim = new Animation(ResourcesManager.FindAnimation("Spawn_Smoke_Animation"));
            effect = new TextureEffect(smokeAnim, position, new Vector2(16, 16), new Vector2(1.5f), true);
            GameManager.AddGameObject(effect);
        }

        public Spawn(Enemy enemy, Vector2 position, Vector2 smokeOffset, float spawnDelay = 0.5f)
        {
            this.spawnDelay = spawnDelay;
            this.enemy = enemy;
            Animation smokeAnim = new Animation(ResourcesManager.FindAnimation("Spawn_Smoke_Animation"));
            effect = new TextureEffect(smokeAnim, position + smokeOffset, new Vector2(16, 16), new Vector2(1.5f), true);
            GameManager.AddGameObject(effect);
            this.smokeOffset = smokeOffset;
        }

        public override void Update(GameTime gameTime)
        {
            spawnDelay -= Time.DeltaTime;
            if(spawnDelay <= 0f && !spawn) // Add the enemy only once
            {
                GameManager.AddGameObject(enemy);
                //ResourcesManager.FindSoundEffect("Enemy_Spawn").Play();
                AudioManager.Play("Enemy_Spawn");
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

        private bool active = false;
        public bool IsFinish { get { return currentNumber <= 0; } }

        public Spawner()
        {
            spawnNumber = 5;
            currentNumber = spawnNumber;
            active = true;
        }

        public void Start()
        {
            //spawnCD = 1.1f - (GameManager.Difficulty * 0.1f); // from 1.0f (difficulty 1) to 0.1f (difficulty 10)
            spawnCD = 0.3f;
            spawnNumber = 3 * GameManager.Difficulty; // from 3 to 30 enemy 
            currentNumber = spawnNumber;
            currentSpawnCD = spawnCD;
            active = true;
        }

        public void Stop() => active = false;


        public void Update(GameTime gameTime)
        {
            if (!active) return;
            // Every X second spawn x enemy from list
            if(currentNumber > 0)
            {
                currentSpawnCD -= Time.DeltaTime;
                if (currentSpawnCD <= 0f)
                {
                    Vector2 pos = GameManager.CurrentLevel.TileGraph.RandomPositionAwayFromDistance(100f);

                    // Difficulty 1 - 2 (Bat) 3 - 5 (Shadow) 6 - 7 (Suicide hadow) 8 - 10 (Summoner) 
                    int difficulty = GameManager.Difficulty;
                    int r = Extensions.Random.Next(1, difficulty + 1);
                    switch (r)
                    {
                        case 1:
                            Spawn(new Bat(pos), pos);
                            break;
                        case 2:
                            Spawn(new Shadow(pos), pos);
                            break;
                        case 3:
                            Spawn(new SuicideShadow(pos), pos);
                            break;
                        case 4:
                            Spawn(new Summoner(pos), pos);
                            break;
                        default:
                            int n = Extensions.Random.Next(1, 5);
                            switch (n)
                            {
                                case 1:
                                    Spawn(new Bat(pos), pos);
                                    break;
                                case 2:
                                    Spawn(new Shadow(pos), pos);
                                    break;
                                case 3:
                                    Spawn(new SuicideShadow(pos), pos);
                                    break;
                                case 4:
                                    Spawn(new Summoner(pos), pos);
                                    break;
                                default:
                                    GameManager.Log("Spawner", "The random number is out of bound r = " + r);
                                    throw new Exception("Random enemy spawn is out of bound");
                            }
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
