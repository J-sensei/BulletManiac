using BulletManiac.Entity;
using BulletManiac.Entity.Enemy;
using BulletManiac.Managers;
using BulletManiac.Particle;
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
        public Spawn(Enemy enemy, Vector2 position, float spawnDelay = 0.5f)
        {
            this.spawnDelay = spawnDelay;
            this.enemy = enemy;
            Animation smokeAnim = new Animation(GameManager.Resources.FindAnimation("Spawn_Smoke_Animation"));
            effect = new TextureEffect(smokeAnim, position, new Vector2(16, 16), new Vector2(1f), true);
            GameManager.AddGameObject(effect);

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
            throw new NotImplementedException();
        }
    }
    public class Spawner
    {
        private float spawnCD = 2f;
        private float currentSpawnCD = 2f;
        private int spawnNumber = 2;
        private List<Spawn> spawns = new();

        int batCount = 0;
        int shadowCount = 0;
        int suicideShadowCount = 0;
        int summnerCount = 0;

        public Spawner()
        {
            batCount = 10;
            shadowCount = 5;
            suicideShadowCount = 3;
            summnerCount = 1;
        }

        public void Update(GameTime gameTime)
        {
            // Every X second spawn x enemy from list
            
        }

        public void Spawn(Enemy enemy, Vector2 position)
        {
            GameManager.AddGameObject(new Spawn(enemy, position));
        }
    }
}
