using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.View;

namespace FirstGame.Model
{
	public class Player
	{
		#region Declaration Section
		private Animation playerAnimation;
		private Vector2 origin;
		private int score;
		private bool active;
		private int health;

		// Animation representing the player
		public Texture2D PlayerTexture;

		// Position of the Player relative to the upper left side of the screen
		public Vector2 Position;
		#endregion

		public Animation PlayerAnimation
		{
			get{ return playerAnimation; }
			set{ playerAnimation = value; }
		}

		#region Variable Properties (Getters/Setters)
		//Properties for Variables
		public bool Active
		{
			get{ return active; }
			set{ active = value; }
		}

		public int Health 
		{
			get{ return health; }
			set{ health = value; }
		}

		// Get the width of the player ship
		public int Width
		{
		get { return playerAnimation.FrameWidth; }
		}

		// Get the height of the player ship
		public int Height
		{
		get { return playerAnimation.FrameHeight; }
		}

		public int Score
		{
			get{ return score; }
			set{ score = value; }
		}
		#endregion

		#region Initialize
		public void Initialize(Animation animation, Vector2 position)
		{
			PlayerAnimation = animation;

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			active = true;

			// Set the player health
			health = 100;

			origin.X = animation.FrameWidth / 2;
			origin.Y = animation.frameHeight / 2;
			// Set the player to be active
			active = true;

			// Set the player health
			health = 100;

			//Set the player score
			score = 0;
		}
		#endregion


		// Update the player animation
		public void Update(GameTime gameTime)
		{
			PlayerAnimation.Position = Position;
			PlayerAnimation.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			PlayerAnimation.Draw(spriteBatch);
		}
	}
}

