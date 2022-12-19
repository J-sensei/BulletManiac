using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using BulletManiac.Entity.Bullet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using BulletManiac.Tiled;
using MonoGame.Extended.Tiled;

namespace BulletManiac.Entity.Player
{
    public class Player : GameObject
    {
        #region Preserve Code

        //private AnimationManager animationManager; // Manange the animation based on certain action
        //private Vector2 cursorDirection;

        //float moveSpeed = 100f;
        //float animationSpeed = 0.08f;

        //public bool move = true; // test
        //public Player(Vector2 position)
        //{
        //    name = "Player";
        //    this.position = position;
        //    animationManager = new AnimationManager();
        //    scale = new Vector2(1f); // Scale of the player
        //    origin = new Vector2(16f); // Origin (Half of the sprite size)

        //    // Load player sprites
        //    GameManager.Resources.LoadTexture("Player_Down", "Test/TopDownCharacter/Character_Down");
        //    GameManager.Resources.LoadTexture("Player_DownLeft", "Test/TopDownCharacter/Character_DownLeft");
        //    GameManager.Resources.LoadTexture("Player_DownRight", "Test/TopDownCharacter/Character_DownRight");
        //    GameManager.Resources.LoadTexture("Player_Left", "Test/TopDownCharacter/Character_Left");
        //    GameManager.Resources.LoadTexture("Player_Right", "Test/TopDownCharacter/Character_Right");
        //    GameManager.Resources.LoadTexture("Player_Up", "Test/TopDownCharacter/Character_Up");
        //    GameManager.Resources.LoadTexture("Player_UpLeft", "Test/TopDownCharacter/Character_UpLeft");
        //    GameManager.Resources.LoadTexture("Player_UpRight", "Test/TopDownCharacter/Character_UpRight");

        //    // Define the keys and animations
        //    animationManager.AddAnimation(new Vector2(0, 0), new Animation(GameManager.Resources.FindTexture("Player_Down"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(0, 1), new Animation(GameManager.Resources.FindTexture("Player_Down"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(-1, 0), new Animation(GameManager.Resources.FindTexture("Player_Left"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(1, 0), new Animation(GameManager.Resources.FindTexture("Player_Right"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(0, -1), new Animation(GameManager.Resources.FindTexture("Player_Up"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(-1, 1), new Animation(GameManager.Resources.FindTexture("Player_DownLeft"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(-1, -1), new Animation(GameManager.Resources.FindTexture("Player_UpLeft"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(1, 1), new Animation(GameManager.Resources.FindTexture("Player_DownRight"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(1, -1), new Animation(GameManager.Resources.FindTexture("Player_UpRight"), 4, 1, animationSpeed));
        //}

        //protected override Rectangle CalculateBound()
        //{
        //    //float x = 1.75f;
        //    //Vector2 pos = position - origin / x;
        //    //return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width / x), (int)(texture.Height / x));

        //    Vector2 pos = position - (origin * scale / 2f);
        //    return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 2f), (int)(texture.Height * scale.Y / 2f));
        //}

        //public override void Initialize()
        //{
        //    CollisionManager.Add(this, Position.ToString()); // Testing Collision
        //    base.Initialize();
        //}

        //public override void Update(GameTime gameTime)
        //{
        //    //Console.WriteLine(Position);
        //    // Player movement
        //    if (InputManager.Moving && move)
        //    {
        //        position += Vector2.Normalize(InputManager.Direction) * moveSpeed * GameManager.DeltaTime;
        //        animationManager.Start();
        //    }
        //    else
        //    {
        //        animationManager.Stop();
        //    }

        //    UpdateCursorDirection();

        //    // Update the animations
        //    animationManager.Update(cursorDirection, gameTime);
        //    texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation
        //    base.Update(gameTime);
        //}

        //public void UpdateCursorDirection()
        //{
        //    Vector2 halfScreen = GameManager.CurrentResolution.ToVector2() / 2f;
        //    Vector2 mousePos = InputManager.MousePosition;
        //    cursorDirection = Vector2.Zero;
        //    float offset = 25f * GameManager.CurrentCameraZoom;
        //    if(mousePos.X < halfScreen.X - offset)
        //    {
        //        cursorDirection.X = -1;
        //    }
        //    else if(mousePos.X > halfScreen.X + offset)
        //    {
        //        cursorDirection.X = 1;
        //    }

        //    if (mousePos.Y < halfScreen.Y - offset / 2f)
        //    {
        //        cursorDirection.Y = -1;
        //    }
        //    else if (mousePos.Y > halfScreen.Y + offset / 2f)
        //    {
        //        cursorDirection.Y = 1;
        //    }
        //    // Console.WriteLine(halfScreen + " | " + mousePos + " | " + cursorDirection);
        //    //Console.WriteLine(mousePos + " | " + cursorDirection);
        //}

        //public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    base.Draw(spriteBatch, gameTime);
        //    //animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(3f, 3f), SpriteEffects.None, 0f);
        //}

        //public override void CollisionEvent(GameObject gameObject)
        //{

        //}
        #endregion
        enum PlayerAction
        {
            Idle, Run, Walk, Death, Throw
        }

        private AnimationManager animationManager; // Manange the animation based on certain action

        float moveSpeed = 80f;
        float animationSpeed = 0.1f;
        float runAnimationSpeed = 0.1f;
        float attackAnimationSpeed = 0.1f;
        float shootSpeed = 1f;

        private bool shooting = false;

        PlayerAction currentAction = PlayerAction.Idle;
        public Player(Vector2 position)
        {
            name = "Player";
            this.position = position;
            animationManager = new AnimationManager();
            scale = new Vector2(0.7f); // Scale of the player
            origin = new Vector2(16f); // Origin (Half of the sprite size)
        }

        protected override Rectangle CalculateBound()
        {
            if(spriteEffects == SpriteEffects.None)
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f); ;
                return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            }
            else
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f);
                return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            }
        }

        public override void Initialize()
        {
            CollisionManager.Add(this, Position.ToString()); // Testing Collision

            // Load player sprites
            GameManager.Resources.LoadTexture("Player_Death", "SpriteSheet/Player/Owlet_Monster_Death_8");
            GameManager.Resources.LoadTexture("Player_Idle", "SpriteSheet/Player/Owlet_Monster_Idle_4");
            GameManager.Resources.LoadTexture("Player_Walk", "SpriteSheet/Player/Owlet_Monster_Walk_6");
            GameManager.Resources.LoadTexture("Player_Run", "SpriteSheet/Player/Owlet_Monster_Run_6");
            GameManager.Resources.LoadTexture("Player_Throw", "SpriteSheet/Player/Owlet_Monster_Throw_4");

            // Define the keys and animations
            animationManager.AddAnimation(PlayerAction.Idle, new Animation(GameManager.Resources.FindTexture("Player_Idle"), 4, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Run, new Animation(GameManager.Resources.FindTexture("Player_Run"), 6, 1, runAnimationSpeed));
            animationManager.AddAnimation(PlayerAction.Walk, new Animation(GameManager.Resources.FindTexture("Player_Walk"), 6, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Death, new Animation(GameManager.Resources.FindTexture("Player_Death"), 8, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Throw, new Animation(GameManager.Resources.FindTexture("Player_Throw"), 4, 1, attackAnimationSpeed * shootSpeed, looping: false));

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //MoveVector();
            PlayerMovement();
            PlayerAttack();
            // Update the animations
            animationManager.Update(currentAction, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation

            base.Update(gameTime);
        }

        private void PlayerAttack()
        {
            if (shooting && animationManager.GetAnimation(PlayerAction.Throw).Finish)
            {
                shooting = false;
                animationManager.GetAnimation(PlayerAction.Throw).Reset(); // Reset the animation once its finish playing
            }

            if (InputManager.MouseLeftHold && !shooting)
            {
                shooting = true;
                currentAction = PlayerAction.Throw;
                GameManager.MainCamera.Shake();

                // Spawn Bullet
                Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition); // Convert mouse screen position to the world position
                Vector2 bulletDirection = mousePos - position;

                // Fire Bullet
                DefaultBullet bullet = new DefaultBullet(position, bulletDirection, 100f);
                GameManager.AddGameObject(bullet); // Straight away add bullet to entity manager to run it
            }
        }

        private void PlayerMovement()
        {
            // Flip the sprite based on the mouse X position
            Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition);
            if (position.X <= mousePos.X)
            {
                spriteEffects = SpriteEffects.None;
            }
            else if (position.X > mousePos.X)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            if (shooting) return; // No movement when player is shooting

            // Updating the speed
            if (InputManager.Moving)
            {
                float currentSpeed = 0f;

                // Player moving backward, set animation reverse and make the player move slower
                if (spriteEffects == SpriteEffects.None && InputManager.Direction.X < 0 ||
                   spriteEffects == SpriteEffects.FlipHorizontally && InputManager.Direction.X > 0)
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(true);
                    // currentSpeed = moveSpeed * 0.5f; // 50% slower if move backward
                }
                else
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(false);

                }
                currentSpeed = moveSpeed;
                // Player move
                if (InputManager.GetKeyDown(Keys.LeftShift))
                {
                    // position += Vector2.Normalize(InputManager.Direction) * (50f) * GameManager.DeltaTime;
                    currentSpeed = moveSpeed * 0.5f;
                    currentAction = PlayerAction.Walk;
                }
                else
                {
                    currentAction = PlayerAction.Run;
                }

                //Vector2 moveAmount = Vector2.Normalize(InputManager.Direction) * currentSpeed * GameManager.DeltaTime;

                //if (!Tile.Collision(position + moveAmount, 16))
                //{
                //    position += moveAmount;
                //}

                ApplyMove(InputManager.Direction, currentSpeed);

                //Console.WriteLine(Bound + " " + Tile.GetTileBound(position + moveAmount, 16));
                //if (!CollisionManager.IsCollided_AABB(Bound, Tile.GetTileBound(position + moveAmount, 16)))
                //{
                //    Console.WriteLine("Colliding with tile");
                //    position += moveAmount;
                //}
            }
            else
            {
                currentAction = PlayerAction.Idle; // Player is Idle when not moving
            }
        }
        
        Tile leftTile = new Tile(0, 0, 16, 16); // Test Code
        Tile topTile = new Tile(0, 0, 16, 16); // Test Code
        Tile rightTile = new Tile(0, 0, 16, 16); // Test Code
        Tile bottomTile = new Tile(0, 0, 16, 16); // Test Code
        private void ApplyMove(Vector2 direction, float moveSpeed)
        {
            leftTile.Col = leftTile.Row = 0;
            topTile.Col = topTile.Row = 0;
            rightTile.Col = rightTile.Row = 0;
            bottomTile.Col = bottomTile.Row = 0;

            bool moveX = true;
            bool moveY = true;

            // Check collision to tiles
            if (direction.X < 0)
            {
                ushort x = (ushort)(Bound.Left / 16);
                ushort y = (ushort)((Bound.Center.Y / 16));
                leftTile.Col = x;
                leftTile.Row = y;

                if (Tile.IsCollided(Bound, leftTile)) moveX = false;
            }

            if (direction.X > 0)
            {
                ushort x = (ushort)(Bound.Right / 16);
                ushort y = (ushort)((Bound.Center.Y / 16));
                rightTile.Col = x;
                rightTile.Row = y;

                if (Tile.IsCollided(Bound, rightTile)) moveX = false;
            }

            if (direction.Y > 0)
            {
                ushort x = (ushort)(Bound.Center.X / 16);
                ushort y = (ushort)((Bound.Bottom / 16));
                bottomTile.Col = x;
                bottomTile.Row = y;

                if (Tile.IsCollided(Bound, bottomTile)) moveY = false;
            }

            if (direction.Y < 0)
            {
                ushort x = (ushort)(Bound.Center.X / 16);
                ushort y = (ushort)((Bound.Top / 16));
                topTile.Col = x;
                topTile.Row = y;

                if (Tile.IsCollided(Bound, topTile)) moveY = false;
            }

            Vector2 moveAmount = Vector2.Normalize(InputManager.Direction) * moveSpeed * GameManager.DeltaTime;
            if (moveX)
            {
                position.X += moveAmount.X;
            }

            if (moveY)
            {
                position.Y += moveAmount.Y;
            }
        }

        private void MoveVector()
        {
            Vector2 result = Vector2.Zero;
            int tileSize = 16;

            // TiledMap map
            TiledMapTileLayer layer = GameManager.CurrentLevel.Map.GetLayer<TiledMapTileLayer>("Wall");
            TiledMapTile? tile = null;

            //ushort x = (ushort)(playerPosX / tileSize);
            //ushort y = (ushort)(playerPosY / tileSize);
            Tile t = Tile.ToTile(Position, tileSize, tileSize);

            // Get tile based on player position
            layer.TryGetTile((ushort)t.Col, (ushort)t.Row, out tile);

            if (tile.HasValue && tile.Value.GlobalIdentifier != 0)
            {
                // collided!
                // you can also compute the tile's position using the X, Y and tileWidth if needed.
                Console.WriteLine(t.Col + " " + t.Row);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Debug test
            topTile.Draw(spriteBatch, gameTime);
            leftTile.Draw(spriteBatch, gameTime);
            bottomTile.Draw(spriteBatch, gameTime);
            rightTile.Draw(spriteBatch, gameTime);

            base.Draw(spriteBatch, gameTime);
            //animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(3f, 3f), SpriteEffects.None, 0f);
        }

        public override void CollisionEvent(GameObject gameObject)
        {

        }
    }
}
