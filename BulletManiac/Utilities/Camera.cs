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
        /// Main camera of the game, access anywhere in the code
        /// </summary>
        public static Camera Main { get; private set; } = new();
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
            //float minX = 0;
            //float maxX = 1280f * 2f;
            //float minY = 0f;
            //float maxY = 720f * 2f;
            //float minX = GameManager.CurrentLevel.Bound.Left;
            //float maxX = GameManager.CurrentLevel.Bound.Right;
            //float minY = GameManager.CurrentLevel.Bound.Top;
            //float maxY = GameManager.CurrentLevel.Bound.Bottom;

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
        const float SHAKE_RADIUS = 1f;
        float currentShakeRadius = SHAKE_RADIUS;
        float shakeRadius = SHAKE_RADIUS;
        float shakeTime = 0.15f;
        float currentShakeTime = 0.15f;

        public void Shake(float radius = SHAKE_RADIUS)
        {
            shakeRadius = radius;
            currentShakeRadius = radius;
            shakeViewport = true;
        }

        public bool InViewBound(Vector2 pos)
        {
            return VisibleArea.Contains((int)pos.X, (int)pos.Y);
        }

        private void UpdateShaking()
        {
            Vector2 offset = new Vector2(0, 0);
            Random rand = new Random();
            if (shakeViewport)
            {
                //Console.WriteLine("Radius: " + shakeRadius + " Time: " + shakeTime);
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * currentShakeRadius), (float)(Math.Cos(shakeStartAngle) * currentShakeRadius));
                currentShakeRadius -= 0.25f;
                shakeStartAngle += (150 + rand.Next(60));
                currentShakeTime -= Time.DeltaTime;

                if (currentShakeTime <= 0 || currentShakeRadius <= 0)
                {
                    shakeViewport = false;
                    currentShakeRadius = shakeRadius;
                    currentShakeTime = shakeTime;
                }
            }
            Position += offset;
        }

        public void MoveCamera(Vector2 movePosition)
        {
            Vector2 pos;

            UpdateMouseMove();
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
            //Position = new Vector2((int)pos.X, (int)pos.Y); // No Tearing with int

            // Camera constraint
            // HARDCODED **NEED TO UPDATE**
            float amount = 80f;
            float minX = GameManager.CurrentLevel.Bound.Left;
            float maxX = GameManager.CurrentLevel.Bound.Right;
            float minY = GameManager.CurrentLevel.Bound.Top;
            float maxY = GameManager.CurrentLevel.Bound.Bottom;

            float xLeft = pos.X - (VisibleArea.Width * 0.5f);
            float xRight = pos.X + (VisibleArea.Width * 0.5f);
            float yUp = pos.Y - VisibleArea.Height * 0.5f;
            float yDown = pos.Y + VisibleArea.Height * 0.5f;
            //Console.WriteLine(VisibleArea);
            //Console.WriteLine("xLeft: " + xLeft + " xRight: " + xRight + " yUp: " + yUp + " yDown: " + yDown);
            Vector2 limit = pos;
            if (xLeft < minX) limit.X = minX + amount;
            if (xRight > maxX) limit.X = maxX - amount * 1.3f;
            if (yUp < minY) limit.Y = minY + amount * 0.5f;
            if (yDown > maxY) limit.Y = maxY - amount * 0.8f;
            Position = Vector2.Lerp(Position + offset, limit, 5f * Time.DeltaTime); // Update the camera position
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
        /// Test (Used for moving the camera based on the mouse position)
        /// </summary>
        Vector2 offset;
        private void UpdateMouseMove()
        {
            Vector2 mousePos = InputManager.MousePosition;
            Vector2 screenSize = GameManager.CurrentResolution.ToVector2() / 2f;

            //Console.WriteLine(Vector2.Normalize(mousePos) + " " + screenSize);
            //float amount = 350f;
            offset = new Vector2(0f);
            if (InputManager.GetKeyDown(Keys.LeftShift)) return; // Test

            float x, y;
            //if(mousePos.X > screenSize.X)
            //{
            //    x = mousePos.X - screenSize.X;
            //}
            //else
            //{
            //    x = mousePos.X - screenSize.X;
            //}

            //if (mousePos.Y > screenSize.Y)
            //{
            //    y = amount;
            //}
            //else
            //{
            //    y = -amount;
            //}
            x = mousePos.X - screenSize.X;
            y = mousePos.Y - screenSize.Y;

            Vector2 target = new Vector2(x, y) * 0.2f;
            offset = Vector2.Lerp(offset, target, 2f * Time.DeltaTime);
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

        /// <summary>
        /// Convert on screen positon to the world position along with the entity
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 ScreenToWorld(Vector2 position)
        {
            var matrix = Matrix.Invert(Main.Transform); // Inverted matrix from the main camera
            return Vector2.Transform(position, matrix); // The world position
        }
    }
}
