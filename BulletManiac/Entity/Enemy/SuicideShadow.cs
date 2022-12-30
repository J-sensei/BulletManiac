using BulletManiac.Managers;
using BulletManiac.Particle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Enemy
{
    public class SuicideShadow : Enemy
    {
        private float animationSpeed = 0.1f;
        public SuicideShadow(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Suicide Shadow";
            hp = 20f;
            currentAction = EnemyAction.Idle;
            
            scale = new Vector2(0.5f);
        }

        public override void Initialize()
        {
            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Idle"), 8, 1, 0.1f));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Move"), 8, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Attack"), 58, 1, 0.05f, looping: false));
            //animationManager.AddAnimation(EnemyAction.Die, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Attack"), 58, 1, 0.08f, looping: false));

            //deathSoundEffect = GameManager.Resources.FindSoundEffect("Shadow_Death");
            // Shadow Visual
            shadowEffect = new TextureEffect(GameManager.Resources.FindTexture("Shadow"),
                    new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                    this,
                    new Vector2(32f), new Vector2(0.5f), new Vector2(0f, -5f));

            origin = animationManager.CurrentAnimation.TextureBound / 2f;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (currentAction == EnemyAction.Hit) currentAction = EnemyAction.Idle;
            base.Update(gameTime);
        }


        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)((origin.X * 2) * scale.X / 2.2f), (int)((origin.Y * 2) * scale.Y / 2.2f));
        }

        public override void DeleteEvent()
        {
            HitBox hitBox = new HitBox(new Animation(GameManager.Resources.FindTexture("SuicideShadow_Explode"), 8, 1, animationSpeed, looping: false),
                                        Position, new Vector2(1f), new List<int>() { 3, 4 });
            hitBox.AddSoundEffect(GameManager.Resources.FindSoundEffect("SuicideShadow_Explosion"), 1);
            GameManager.AddGameObject(hitBox);

            base.DeleteEvent();
        }
    }
}
