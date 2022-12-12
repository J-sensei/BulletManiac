using BulletManiac.Entity;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BulletManiac.Utilities
{
    public class Camera
    {
        /// <summary>
        /// Matrix to apply to the sprite batch to draw the correct position of graphics
        /// </summary>
        public Matrix Transform { get; set; }
        /// <summary>
        /// Bounds of the render graphics area
        /// </summary>
        public Rectangle Bounds { get; protected set; }
        /// <summary>
        /// Area that is current visible to the camera
        /// </summary>
        public Rectangle VisibleArea { get; protected set; }
        public float Zoom { get; set; }
        public Vector2 Position { get; set; }

        private float zoom, previousZoom;
        public Camera()
        {
            Zoom = 1f;
            Position = Vector2.Zero;
        }

        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            //// Constraint the camera to move (TEST)
            //float minX = 0f;
            //float maxX = 1280f * 2f;
            //float minY = 0f;
            //float maxY = 720f * 2f;

            //float xLeft = Position.X - Bounds.Width * 0.5f;
            //float xRight = Position.X + Bounds.Width * 0.5f;
            //float yUp = Position.Y - Bounds.Height * 0.5f;
            //float yDown = Position.Y + Bounds.Height * 0.5f;
            ////Console.WriteLine("xLeft: " + xLeft + " xRight: " + xRight + " yUp: " + yUp + " yDown: " + yDown);
            //Vector2 pos = Position;
            //if (xLeft < minX) pos.X = minX + Bounds.Width * 0.5f;
            //if (xRight > maxX) pos.X = maxX - Bounds.Width * 0.5f;
            //if (yUp < minY) pos.Y = minY + Bounds.Height * 0.5f;
            //if (yDown > maxY) pos.Y = maxY - Bounds.Height * 0.5f;

            //Position = pos;

            // Apply Shake
            //if (InputManager.MouseLeftClick && shakeViewport == false)
            //{
            //    shakeViewport = true;
            //}
            UpdateShaking();
            // Update the matrix to make camera move
            Matrix position = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
            Matrix zoom = Matrix.CreateScale(Zoom);
            Matrix offset = Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            Transform = position * zoom * offset;
            UpdateVisibleArea();
        }

        bool shakeViewport = false;
        float shakeStartAngle = 0;
        const float shakeRadiusInit = 1f;
        float shakeRadius = shakeRadiusInit;
        float shakeTime = 0.15f;
        float currentShakeTime =0.15f;

        public void Shake()
        {
            shakeViewport = true;
        }

        private void UpdateShaking()
        {
            Vector2 offset = new Vector2(0, 0);
            Random rand = new Random();
            if (shakeViewport)
            {
                //Console.WriteLine("Radius: " + shakeRadius + " Time: " + shakeTime);
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeStartAngle += (150 + rand.Next(60));
                currentShakeTime -= GameManager.DeltaTime;

                if (currentShakeTime <= 0 || shakeRadius <= 0)
                {
                    shakeViewport = false;
                    shakeRadius = shakeRadiusInit;
                    currentShakeTime = shakeTime;
                }
            }
            Position += offset;
        }

        public void MoveCamera(Vector2 movePosition)
        {
            Vector2 pos;

            // Test If Statament
            if (followPosition != Vector2.Zero)
            {
                pos = followPosition;
            }
            else
            {
                Vector2 newPosition = Position + movePosition;
                pos = newPosition;
            }

            Position = Vector2.Lerp(Position, pos, 5f * GameManager.DeltaTime);
        }

        public void AdjustZoom(float zoomAmount)
        {
            Zoom = zoomAmount;
            //if (Zoom < .35f)
            //{
            //    Zoom = .35f;
            //}
            //if (Zoom > 5f)
            //{
            //    Zoom = 5f;
            //}
        }
        Vector2 followPosition;
        public void Follow(GameObject target)
        {
            //Matrix position = Matrix.CreateTranslation(-target.Position.X - (target.Bound.Width / 2), -target.Position.Y - (target.Bound.Height / 2), 0f);
            //Matrix offset = Matrix.CreateTranslation(GameManager.ScreenSize.X / 2, GameManager.ScreenSize.Y / 2, 0f);
            //Transform = position * offset;
            followPosition = new Vector2(target.Position.X + (target.Bound.Width / 2), target.Position.Y + (target.Bound.Height / 2));
        }

        /// <summary>
        /// Update Camera Logic
        /// </summary>
        /// <param name="bounds"></param>
        public void Update(Viewport bounds)
        {
            Bounds = bounds.Bounds;
            UpdateMatrix();

            Vector2 cameraMovement = Vector2.Zero;
            //int moveSpeed;

            //if (Zoom > .8f)
            //{
            //    moveSpeed = 15;
            //}
            //else if (Zoom < .8f && Zoom >= .6f)
            //{
            //    moveSpeed = 20;
            //}
            //else if (Zoom < .6f && Zoom > .35f)
            //{
            //    moveSpeed = 25;
            //}
            //else if (Zoom <= .35f)
            //{
            //    moveSpeed = 30;
            //}
            //else
            //{
            //    moveSpeed = 10;
            //}

            // Test Code
            int moveSpeed = 0;


            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cameraMovement.Y = -moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cameraMovement.Y = moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                cameraMovement.X = -moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                cameraMovement.X = moveSpeed;
            }

            if (InputManager.MouseScrollUp)
            {
                AdjustZoom(zoom += .05f);
                Console.WriteLine(moveSpeed);
            }

            if (InputManager.MouseScrollDown)
            {
                AdjustZoom(zoom -= .05f);
                Console.WriteLine(moveSpeed);
            }

            // Debug for camera zoom
            previousZoom = zoom;
            zoom = Zoom;
            if (previousZoom != zoom)
            {
                GameManager.Log("Camera", "Current zoom is " + zoom + ".");
            }

            // NEED TO OPTIMISE - as camera movement is not using when follow target is assigned
            MoveCamera(cameraMovement);
        }
    }
}
