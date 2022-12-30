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
        Seek, Flee, Arrival, Wander
    }

    public struct SteeringSetting
    {
        float? distanceToChase;
        public float DistanceToChase { get { return distanceToChase ?? 1000f; } set { distanceToChase = value; } }
        float? distanceToFlee;
        public float DistanceToFlee { get { return distanceToFlee ?? 1000f; } set { distanceToFlee = value; } }
    }

    /// <summary>
    /// Use to flip the user sprite
    /// </summary>
    public enum XDirection
    {
        Left, Right
    }

    public class SteeringAgent : IDisposable
    {
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
        
        private Flock currentFlock; // Current Flock of the user (Create when enable flock)
        private bool enableFlock; // Determine if this steering agent need to enable flock behavior
        private SteeringSetting? setting;

        public SteeringAgent(GameObject user, SteeringSetting? steeringSetting, FlockSetting flockSetting, float speed = 50f, float arrivalRadius = 10f, bool enableFlock = false)
        {
            this.user = user;
            this.currentSpeed = speed;
            this.arrivalRadius = arrivalRadius;
            this.enableFlock = enableFlock;
            if (enableFlock)
            {
                currentFlock = new Flock(user, flockSetting);
                FlockManager.Add(user.Name, currentFlock); // Add to flock manager
            }

            if(steeringSetting == null)
                setting = new SteeringSetting();
            else
                setting = steeringSetting;
        }

        private bool newAgent = true;
        static Queue<GameObject> targetQueue = new();
        static Queue<SteeringAgent> agentQueue = new();
        static int executeCount = 0;
        const int MAX_EXECUTE_COUNT = 60;
        public static void GlobalUpdate()
        {
            executeCount = 0;
            while (agentQueue.Count > 0 && executeCount <= MAX_EXECUTE_COUNT)
            {
                var agent = agentQueue.Dequeue();
                var target = targetQueue.Dequeue();

                if (GameManager.FindGameObject(agent.user) == null) continue; // If user the destroyed, skip
                agent.UpdateVelocity(target); // Execute the steering behavior

                // Put back to the queue
                agentQueue.Enqueue(agent);
                targetQueue.Enqueue(target);

                executeCount++;
            }
        }

        public void Update(GameTime gameTime, GameObject target)
        {
            if (newAgent)
            {
                UpdateVelocity(target);
                newAgent = false;
            }
            agentQueue.Enqueue(this);
            targetQueue.Enqueue(target);

            //switch (SteeringBehavior)
            //{
            //    case SteeringBehavior.Seek:
            //        currentVelocity = Seek(target.Position, setting.Value.DistanceToChase);
            //        break;
            //    case SteeringBehavior.Flee:
            //        currentVelocity = Flee(target.Position, setting.Value.DistanceToFlee);
            //        break;
            //    case SteeringBehavior.Arrival:
            //        currentVelocity = Arrive(target.Position, currentSpeed, arrivalRadius, setting.Value.DistanceToChase);
            //        break;
            //    default:
            //        currentVelocity = Vector2.Zero;
            //        GameManager.Log("Steering Agent", "No Steering Behavior is selected.");
            //        break;
            //}

            //if (enableFlock)
            //{
            //    // Make sure agent will not move the user out of the map
            //    Vector2 velocity = Extensions.Truncate(CurrentVelocity + currentFlock.Acceleration, currentFlock.MaxSpeed);
            //    currentFlock.CurrentVelocity = velocity;
            //    if (user.Position.X < GameManager.CurrentLevel.Bound.X && velocity.X < 0f)
            //        velocity.X = 0f;
            //    if (user.Position.X > GameManager.CurrentLevel.Bound.Width && velocity.X > 0f)
            //        velocity.X = 0f;
            //    if (user.Position.Y < GameManager.CurrentLevel.Bound.Y && velocity.Y < 0f)
            //        velocity.Y = 0f;
            //    if (user.Position.Y > GameManager.CurrentLevel.Bound.Height && velocity.Y > 0f)
            //        velocity.Y = 0f;

            //    CurrentFinalVelocity = velocity;
            //    currentFlock.ResetAcceleration(); // Reset Acceleration to zero for next frame.
            //}
            //else
            //{
            //    CurrentFinalVelocity = currentVelocity;
            //}


            //if (CurrentFinalVelocity.X >= 0) CurrentXDir = XDirection.Right;
            //else CurrentXDir = XDirection.Left;

            //if (enableFlock)
            //{
            //    var flocks = FlockManager.Find(user.Name);
            //    foreach (var flock in flocks)
            //    {
            //        flock.Process(flocks);
            //    }
            //}
        }

        void UpdateVelocity(GameObject target)
        {
            switch (SteeringBehavior)
            {
                case SteeringBehavior.Seek:
                    currentVelocity = Seek(target.Position, setting.Value.DistanceToChase);
                    break;
                case SteeringBehavior.Flee:
                    currentVelocity = Flee(target.Position, setting.Value.DistanceToFlee);
                    break;
                case SteeringBehavior.Arrival:
                    currentVelocity = Arrive(target.Position, currentSpeed, arrivalRadius, setting.Value.DistanceToChase);
                    break;
                default:
                    currentVelocity = Vector2.Zero;
                    GameManager.Log("Steering Agent", "No Steering Behavior is selected.");
                    break;
            }

            if (enableFlock)
            {
                // Make sure agent will not move the user out of the map
                Vector2 velocity = Extensions.Truncate(CurrentVelocity + currentFlock.Acceleration, currentFlock.MaxSpeed);
                currentFlock.CurrentVelocity = velocity;


                CurrentFinalVelocity = velocity;
                currentFlock.ResetAcceleration(); // Reset Acceleration to zero for next frame.
            }
            else
            {
                CurrentFinalVelocity = currentVelocity;
            }


            if (CurrentFinalVelocity.X >= 0) CurrentXDir = XDirection.Right;
            else CurrentXDir = XDirection.Left;

            if (enableFlock)
            {
                var flocks = FlockManager.Find(user.Name);
                foreach (var flock in flocks)
                {
                    flock.Process(flocks);
                }
            }
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

        private Vector2 Arrive(Vector2 target, float maxSpeed, float radius, float distanceToChase = 100f)
        {
            Vector2 velocity = target - user.Position;
            float distance = velocity.Length();
            if (Extensions.Approximately(distance, 0.0f) || distance > distanceToChase)
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

        private Vector2 Seek(Vector2 target, float distanceToChase = 100f)
        {
            if((target - user.Position).Length() < distanceToChase)
            {
                return Vector2.Normalize(target - user.Position) * currentSpeed;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        private Vector2 Flee(Vector2 target, float distanceToFlee = 100f)
        {
            return -Seek(target, distanceToFlee);
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

        private Vector2 ApplyMove(Vector2 velocity)
        {
            Vector2 push = Vector2.Zero;
            bool pushX = false; // Determine if the player can move left / right
            bool pushY = false; // Determine if the player can move up / down

            Vector2 moveAmount = velocity; // Amount of move in this frmae
            Vector2 moveAmountX = moveAmount; // Amount of move for x axis
            moveAmountX.Y = 0;
            Vector2 moveAmountY = moveAmount; // Amount of move for y axis
            moveAmountY.X = 0;

            pushX = CollisionManager.CheckTileCollision(user, moveAmountX); // If user will collide X axis
            pushY = CollisionManager.CheckTileCollision(user, moveAmountY); // If user will collide y axis

            if (pushX)
            {
                push.X = velocity.X > 0f ? -1f : 1f;
                velocity.X = 0f;
            }

            if (pushY)
            {
                push.Y = velocity.Y > 0f ? -1f : 1f;
                velocity.Y = 0f;
            }


            push *= 2f;
            return velocity + push;
        }

        public void Dispose()
        {
            if (currentFlock != null)
                FlockManager.Remove(user.Name, currentFlock);
        }
    }
}
