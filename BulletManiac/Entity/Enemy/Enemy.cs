﻿using BulletManiac.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Enemy
{
    /// <summary>
    /// Base class for enemy
    /// </summary>
    public abstract class Enemy : GameObject
    {
        /// <summary>
        /// Enemy actions, used to determine animation
        /// </summary>
        enum EnemyAction
        {
            Idle, Move, Attack, Die
        }

        private AnimationManager animationManager; // Manange the animation based on certain action

        public Enemy(Vector2 position)
        {
            name = "Enemy";
            this.position = position;
            animationManager = new AnimationManager();
            CollisionManager.Add(this, "Enemy"); // Add enemy into the collision manager
        }
    }
}