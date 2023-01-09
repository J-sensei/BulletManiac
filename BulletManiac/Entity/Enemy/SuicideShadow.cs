using BulletManiac.AI;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletManiac.Entity.Enemy
{
    public class SuicideShadow : Enemy
    {
        private NavigationAgent navigationAgent;
        const float DISTANCE_TO_EXPLODE = 35f;
        private float animationSpeed = 0.1f;
        private bool attackSoundPlay = false;
        private SoundEffectInstance attackingSound;
        private float pathfindCD = 2f;
        private float currentPathfindCD = 2f;
        public SuicideShadow(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Suicide Shadow";
            hp = 20f;
            currentAction = EnemyAction.Idle;
            
            scale = new Vector2(0.5f);
            navigationAgent = new NavigationAgent(this);
        }

        public override void Initialize()
        {
            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Idle"), 8, 1, 0.1f));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Move"), 8, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("SuicideShadow_Attack"), 15, 1, 0.15f, looping: false));
            
            // Shadow Visual
            shadowEffect = new TextureEffect(GameManager.Resources.FindTexture("Shadow"),
                    new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                    this,
                    new Vector2(32f), new Vector2(0.5f), new Vector2(0f, -5f));

            origin = animationManager.CurrentAnimation.TextureBound / 2f;
            attackingSound = GameManager.Resources.FindSoundEffect("SuicideShadow_Attacking").CreateInstance();
            attackingSound.IsLooped = true;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Recover from hit state
            if (currentAction == EnemyAction.Hit)
            {
                if (!attackSoundPlay)
                {
                    if (navigationAgent.CurrentState == NavigationState.MOVING)
                        currentAction = EnemyAction.Move;
                    else
                        currentAction = EnemyAction.Idle;
                }
                else
                    currentAction = EnemyAction.Attack;
            }

            // Bomb logic
            float distance = (GameManager.Player.Position - Position).Length();
            if(distance < DISTANCE_TO_EXPLODE)
            {
                currentAction = EnemyAction.Attack;
                //if (!attackSoundPlay)
                //{
                //    GameManager.Resources.FindSoundEffect("SuicideShadow_AttackStart").Play();
                //    attackSoundPlay = true;
                //}
            }

            if(currentAction == EnemyAction.Attack)
            {
                attackingSound.Play();
            }
            else
            {
                attackingSound.Stop();
            }

            if(currentAction == EnemyAction.Attack && animationManager.CurrentAnimation.Finish)
            {
                // Explode and delete enemy
                Destroy(this);
                HitBox hitBox = new HitBox(new Animation(GameManager.Resources.FindTexture("SuicideShadow_Explode"), 8, 1, animationSpeed, looping: false),
                            Position, new Vector2(1f), new List<int>() { 3, 4 }, enableEnemyDamage: true, enablePlayerDamage: true);
                hitBox.AddSoundEffect(GameManager.Resources.FindSoundEffect("SuicideShadow_Explosion"), 1);
                GameManager.AddGameObject(hitBox);

                // Shake the camera is the explosion is happen inside the visible area
                if (GameManager.MainCamera.InViewBound(Position))
                    GameManager.MainCamera.Shake(2f);
            }
            base.Update(gameTime);

            if (currentAction == EnemyAction.Attack) return;
            // Movement
            if (navigationAgent.CurrentState == NavigationState.STOP)
            {
                if (currentAction != EnemyAction.Hit)
                    currentAction = EnemyAction.Idle; // Make enemy idle when its not in hit state
                currentPathfindCD -= Time.DeltaTime;
                if (currentPathfindCD <= 0f)
                {
                    var nodes = GameManager.CurrentLevel.TileGraph.Nodes;
                    navigationAgent.Pathfind(GameManager.Player.Position); // Execute pathfinding
                    currentPathfindCD = pathfindCD;
                    currentAction = EnemyAction.Move;
                }
            }

            // Navigate the enemy when its moving state
            if (currentAction == EnemyAction.Move)
            {
                navigationAgent.Update(gameTime);
            }

            // Flip sprite
            if (navigationAgent.CurrentXDir == XDirection.Right)
                spriteEffects = SpriteEffects.None;
            else
                spriteEffects = SpriteEffects.FlipHorizontally;
        }


        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X - 2, (int)pos.Y + 2, (int)((origin.X * 2) * scale.X / 2.2f), (int)((origin.Y * 2) * scale.Y / 2.2f));
        }

        public override void DeleteEvent()
        {
            HitBox hitBox = new HitBox(new Animation(GameManager.Resources.FindTexture("SuicideShadow_Explode"), 8, 1, animationSpeed, looping: false),
                                        Position, new Vector2(1f), new List<int>() { 3, 4 }, enableEnemyDamage: true);
            hitBox.AddSoundEffect(GameManager.Resources.FindSoundEffect("SuicideShadow_Explosion"), 1);
            GameManager.AddGameObject(hitBox);

            // Shake the camera is the explosion is happen inside the visible area
            if (GameManager.MainCamera.InViewBound(Position))
                GameManager.MainCamera.Shake(2f);

            base.DeleteEvent();
        }

        public override void Dispose()
        {
            attackingSound.Dispose();
            base.Dispose();
        }
    }
}
