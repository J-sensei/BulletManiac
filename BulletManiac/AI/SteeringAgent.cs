using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Managers;
using BulletManiac.Tiled;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.AI
{
    public enum SteeringBehavior
    {
        Seek, Flee, Arrival, Wander, 
    }
    public enum XDirection
    {
        Left, Right
    }

    public class SteeringAgent : IDisposable
    {
        private Flock boid; // Boid behavior
        private GameObject user;

        private const float DEFAULT_COOLDOWN = 0.5f;
        private float currentCD = DEFAULT_COOLDOWN;
        private float currentSpeed;
        private float arrivalRadius;

        private Vector2 currentVelocity;

        const float DEFAULT_WANDER_CD = 2f;
        private float currentWanderCD = DEFAULT_WANDER_CD;

        /// <summary>
        /// Velocity calculated by agent to move
        /// </summary>
        public Vector2 CurrentVelocity { get { return currentVelocity; } }
        public Vector2 CurrentFinalVelocity { get; private set; }
        /// <summary>
        /// Current steer behavior of the agent
        /// </summary>
        public SteeringBehavior SteeringBehavior { get; set; } = SteeringBehavior.Seek;
        /// <summary>
        /// Determine the agent is going left or right currently
        /// </summary>
        public XDirection CurrentXDir { get; private set; }

        static List<Flock> flocks = new();
        private Flock currentFlock;

        public SteeringAgent(GameObject user, float speed = 50f, float arrivalRadius = 10f)
        {
            this.user = user;
            this.currentSpeed = speed;
            this.arrivalRadius = arrivalRadius;

            currentFlock = new Flock(user);
            flocks.Add(currentFlock);
        }

        public void Update(GameTime gameTime, GameObject target)
        {
            switch (SteeringBehavior)
            {
                case SteeringBehavior.Seek:
                    currentVelocity = Seek(target.Position);
                    break;
                case SteeringBehavior.Flee:
                    currentVelocity = Flee(target.Position);
                    break;
                case SteeringBehavior.Arrival:
                    currentVelocity = Arrive(target.Position, currentSpeed, arrivalRadius);
                    break;
                default:
                    currentVelocity = Vector2.Zero;
                    GameManager.Log("Steering Agent", "No Steering Behavior is selected.");
                    break;
            }
            //MoveAvoid(user, target.Position);
            //currentVelocity = Arrive(target.Position, currentSpeed, 10f);
            //currentVelocity = Flee(target.Position);

            CurrentFinalVelocity = Extensions.Truncate(CurrentVelocity + currentFlock.Acceleration, currentFlock.MaxSpeed);
            currentFlock.ResetAcceleration(); // Reset Acceleration to zero for next frame.

            if (currentVelocity.X >= 0) CurrentXDir = XDirection.Right;
            else CurrentXDir = XDirection.Left;

            //user.Position += currentVelocity * GameManager.DeltaTime;
            //ApplyMove(currentVelocity, currentSpeed);
            //MoveAvoid(user, target.Position);

            foreach (var f in flocks)
                f.Process(flocks);
        }

        private void MoveAvoid(GameObject source, Vector2 target)
        {
            float pullDistance = Vector2.Distance(target, source.Position);

            if (pullDistance > 1)
            {
                Vector2 pull = (target - source.Position) * (1 / pullDistance); //the target tries to 'pull us in'
                Vector2 totalPush = Vector2.Zero;

                int contenders = 0;
                List<Tile> obstacles = CollisionManager.TileBounds;
                for (int i = 0; i < CollisionManager.TileBounds.Count; ++i)
                {

                    //draw a vector from the obstacle to the ship, that 'pushes the ship away'
                    Vector2 push = source.Position - obstacles[i].Position;

                    //calculate how much we are pushed away from this obstacle, the closer, the more push
                    float distance = (Vector2.Distance(source.Position, obstacles[i].Position) - 16f) - 64f;
                    //only use push force if this object is close enough such that an effect is needed
                    if (distance < 64f)
                    {
                        ++contenders; //note that this object is actively pushing

                        if (distance < 0.0001f) //prevent division by zero errors and extreme pushes
                        {
                            distance = 0.0001f;
                        }
                        float weight = 1 / distance;

                        totalPush += push * weight;
                    }
                }

                pull *= Math.Max(1, 4 * contenders); //4 * contenders gives the pull enough force to pull stuff trough (tweak this setting for your game!)
                pull += totalPush;

                //Normalize the vector so that we get a vector that points in a certain direction, which we van multiply by our desired speed
                pull.Normalize();
                //Set the ships new position;
                source.Position += (pull * currentSpeed) * GameManager.DeltaTime;
            }
        }

        private Vector2 Arrive(Vector2 target, float maxSpeed, float radius)
        {
            Vector2 velocity = target - user.Position;
            float distance = velocity.Length();
            if (Extensions.Approximately(distance, 0.0f))
            {
                return Vector2.Zero;
            }
            else
            {
                velocity.Normalize();
                velocity *= maxSpeed * MathF.Min(distance / radius, 1);
                return velocity;
            }
        }

        private Vector2 Seek(Vector2 target)
        {
            return Vector2.Normalize(target - user.Position) * currentSpeed;
        }

        private Vector2 Flee(Vector2 target)
        {
            return -Seek(target);
        }

        private Vector2 Wander(float radius, float distance)
        {
            currentWanderCD -= GameManager.DeltaTime;
            if (currentWanderCD <= 0f)
            {
                Vector2 circle = new Vector2(Extensions.NextFloat() * 2 - 1, Extensions.NextFloat() * 2 - 1);
                circle.Normalize();
                circle = circle * radius;
                Vector2 circlePos = user.Position * user.Direction * distance;
                Vector2 result = circlePos + (circle * radius);

                result = Vector2.Normalize(result) * currentSpeed;

                currentWanderCD = DEFAULT_WANDER_CD;
                return result;
            }
            else
            {
                return currentVelocity;
            }
        }

        /// <summary>
        /// Change the rotation of the user
        /// </summary>
        /// <param name="currentVelocity"></param>
        /// <returns></returns>
        private Vector2 GetHeading(Vector2 currentVelocity)
        {
            float speed = currentVelocity.Length();
            Vector2 turnVelocity = Extensions.Approximately(speed, 0.0f) ? Vector2.Zero : currentVelocity / speed;
            return Extensions.Approximately(turnVelocity.Length(), 0.0f) ? user.Direction : turnVelocity;
        }

        private void ApplyMove(Vector2 direction, float moveSpeed)
        {
            bool moveX = false; // Determine if the player can move left / right
            bool moveY = false; // Determine if the player can move up / down

            Vector2 moveAmount = Vector2.Normalize(direction) * moveSpeed * GameManager.DeltaTime; // Amount of move in this frmae
            Vector2 moveAmountX = moveAmount; // Amount of move for x axis
            moveAmountX.Y = 0;
            Vector2 moveAmountY = moveAmount; // Amount of move for y axis
            moveAmountY.X = 0;

            // Check the collisioni for x and y axis
            if (direction.X >= 0)
                moveX = !CollisionManager.CheckTileCollision(user, moveAmountX * 1.5f);
            else
                moveX = !CollisionManager.CheckTileCollision(user, moveAmountX * 1f);

            if (direction.Y >= 0)
                moveY = !CollisionManager.CheckTileCollision(user, moveAmountY * 1.5f);
            else
                moveY = !CollisionManager.CheckTileCollision(user, moveAmountY * 1f);

            // Move the character according to the result
            Vector2 result = Vector2.Zero;
            if (moveX)
                result.X += moveAmountX.X;

            if (moveY)
                result.Y += moveAmountY.Y;

            user.Position += result;
        }

        public void Dispose()
        {
            flocks.Remove(currentFlock);
        }
    }
}
