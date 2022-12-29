using BulletManiac.Entity;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.AI
{
    public struct FlockSetting
    {
        /// <summary>
        /// Default Flock Setting
        /// </summary>
        public static FlockSetting Default = new FlockSetting(); 

        bool? seperate;
        public bool Seperate { get { return seperate ?? true; } set { seperate = value; } }
        bool? alignment;
        public bool Alignment { get { return alignment ?? true; } set { alignment = value; } }
        bool? cohesion;
        public bool Cohesion { get { return cohesion ?? true; } set { cohesion = value; } }

        float? neighbourRadius;
        public float NeighbourRadius { get { return neighbourRadius ?? 100f; } set { neighbourRadius = value; } }
        float? agentRadius;
        public float AgentRadius { get { return agentRadius ?? 16f; } set { agentRadius = value; } }

        float? separationWeight;
        public float SeparationWeight { get { return separationWeight ?? 1.2f; } set { separationWeight = value; } }
        float? alignmentWeight;
        public float AlignmentWeight { get { return alignmentWeight ?? 1.0f; } set { alignmentWeight = value; } }
        float? cohesionWeight;
        public float CohesionWeight { get { return cohesionWeight ?? 1.0f; } set { cohesionWeight = value; } }
    }

    /// <summary>
    /// Flock behavior
    /// </summary>
    public class Flock
    {
        private GameObject user;
        private FlockSetting setting;
        public float MaxSpeed { get; private set; }

        public float NeighbourRadius { get; private set; } = 100.0f;
        public float AgentRadius { get; private set; } = 16.0f;

        public float SeparationWeight { get; private set; } = 1.2f;
        public float AlignmentWeight { get; private set; } = 1.0f;
        public float CohesionWeight { get; private set; } = 1.0f;

        public Vector2 Acceleration { get; private set; } = Vector2.Zero;
        public Vector2 CurrentVelocity { get; set; }

        public Flock(GameObject user, FlockSetting flockSetting, float speed = 50f)
        {
            this.user = user;
            setting = flockSetting;
            MaxSpeed = speed;

            NeighbourRadius = setting.NeighbourRadius;
            AgentRadius = setting.AgentRadius;

            SeparationWeight = setting.SeparationWeight;
            AlignmentWeight = setting.AlignmentWeight;
            CohesionWeight = setting.CohesionWeight;
        }

        public void ResetAcceleration()
        {
            Acceleration *= 0f;
        }

        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Process the flock
        /// </summary>
        /// <param name="boids"></param>
        public void Process(List<Flock> flocks)
        {
            Vector2 sumForce = new Vector2(0f);

            if(setting.Seperate)
                sumForce += Separate(flocks) * SeparationWeight;
            if (setting.Alignment)
                sumForce += Align(flocks) * AlignmentWeight;
            if (setting.Cohesion)
                sumForce += Cohesion(flocks) * CohesionWeight;

            Acceleration = sumForce;
        }

        /// <summary>
        /// Avoid crowding neighbours (short range repulsion)
        /// </summary>
        /// <param name="flocks"></param>
        /// <returns></returns>
        private Vector2 Separate(List<Flock> flocks)
        {
            Vector2 sum = new();
            int count = 0;
            float personalCircle = AgentRadius * 2;
            // Algorithm:
            // Part 1:
            // 1. Iterate boids, for each boid
            // 2. If the boid is this agent, skip
            // 3. Get distance of from boid to this agent.
            // 4. If distance is nonzero, and lesser than personalCircle:
            //    a. get direction from boid to agent.
            //    b. inversely proportion the direction vector with distance. (Hint: proportional: a = b, inversely proportional: ab = 1, a ?)
            //    c. add that to sum, and increase count.
            foreach (Flock flock in flocks)
            {
                if (this != flock)
                {
                    float distance = Vector2.Distance(user.Position, flock.user.Position);
                    if (distance != 0f && distance < personalCircle)
                    {
                        Vector2 direction = user.Position - flock.user.Position;
                        direction = direction / distance;
                        sum += direction;
                        count++;
                    }
                }
            }

            // Part 2
            // return Vector2.Zero IF count is 0.
            if (count == 0) return Vector2.Zero;

            // Part 3
            // 1. get the average of sum, using count.
            // 2. normalize sum and multiply with maxSpeed, that is your desiredVelocity
            // 3. calculate force using desiredVelocity and return;
            sum = sum / count;
            sum.Normalize();
            Vector2 desiredVelocity = sum * MaxSpeed;

            return desiredVelocity;
            //return CalculateForce(desiredVelocity);
        }

        private Vector2 Align(List<Flock> flocks)
        {
            Vector2 sum = new();
            int count = 0;
            // Algorithm:
            // Part 1:
            // 1. Iterate boids, for each boid
            // 2. If the boid is this agent, skip
            // 3. If distance between boid and agent is lesser than neighbourRadius:
            //    a. add the boid's velocity to sum, and increase count.
            foreach (Flock flock in flocks)
            {
                if (this != flock)
                {
                    float distance = Vector2.Distance(user.Position, flock.user.Position);
                    if (distance < NeighbourRadius)
                    {
                        sum += flock.CurrentVelocity;
                        count++;
                    }
                }
            }

            // Part 2
            // return Vector2.Zero IF count is 0 OR magnitude of sum is 0.
            if (count == 0 || sum.Length() == 0) return Vector2.Zero;

            // Part 3
            // 1. normalize sum and multiply with maxSpeed, that is your desiredVelocity
            // 2. calculate force using desiredVelocity and return;
            Vector2 desiredVelocity = Vector2.Normalize(sum) * MaxSpeed;

            return desiredVelocity;
            //return CalculateForce(desiredVelocity);
        }

        private Vector2 Cohesion(List<Flock> flocks)
        {
            Vector2 sum = new();
            int count = 0;
            // Algorithm:
            // Part 1:
            // 1. Iterate boids, for each boid
            // 2. If the boid is this agent, skip
            // 3. If distance between boid and agent is lesser than neighbourRadius:
            //    a. add the boid's position to sum, and increase count.
            foreach (Flock flock in flocks)
            {
                if (this != flock)
                {
                    float distance = Vector2.Distance(user.Position, flock.user.Position);
                    if (distance < NeighbourRadius)
                    {
                        sum += flock.user.Position;
                        count++;
                    }
                }
            }

            // Part 2
            // return Vector2.Zero IF count is 0 OR magnitude of sum is 0.
            if (count == 0 || sum.Length() == 0) return Vector2.Zero;

            // Part 3
            // 1. get the average of sum, using count.
            // 2. apply Seek to the average sum.
            Vector2 average = sum / count;

            Vector2 desiredDirection = Vector2.Normalize(average - user.Position);
            Vector2 desiredVelocity = desiredDirection * MaxSpeed;
            
            return CalculateForce(desiredVelocity);
        }

        private Vector2 CalculateForce(Vector2 desiredVelocity)
        {
            // Instead of returning Desired Velocity
            // Calculate SteerVector, this quantity is the amount needed to instantly change
            // CurrentVelocity to head to the target
            Vector2 steerVector = desiredVelocity - CurrentVelocity;

            // Instead of using SteerVector, we limit the magnitude to maxForce
            // If amount of maxForce is low, then it needs longer time to turn.
            Vector2 steerForce = Extensions.Truncate(steerVector, 5.0f);
            return steerForce;
        }
    }
}
