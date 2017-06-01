using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using FirstGame.View;
using FirstGame.Model;
using System.Collections.Generic;

namespace FirstGame.Controller
{
	/// This is the main type for your game.
	public class FirstGame : Game
	{
		#region Declaration Section
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Player player;

		// Image used to display the static background
		Texture2D mainBackground;

		// Parallaxing Layers
		ParallaxingBackground bgLayer1;
		ParallaxingBackground bgLayer2;

		Texture2D projectileTexture;
		List<Projectile> projectiles;

		// The rate of fire of the player laser
		TimeSpan fireTime;
		TimeSpan previousFireTime;

		// Enemies
		Texture2D enemyTexture;
		List<Enemy> enemies;

		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		// A random number generator
		Random random;

		// Keyboard states used to determine key presses
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState; 

		// A movement speed for the player
		private float playerMoveSpeed;
		#endregion

		#region Constructor
		public FirstGame ()
		{
			graphics = new GraphicsDeviceManager (this);

			Content.RootDirectory = "Content";
		}
		#endregion

		#region Initialize (For Game Engine)
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		protected override void Initialize ()
		{
			player = new Player ();

			// Initialize the enemies list
			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();

			projectiles = new List<Projectile>();

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);

			// Set a constant player move speed
			playerMoveSpeed = 8.0f;

			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();

			mainBackground = Content.Load<Texture2D>("Textures/mainbackground");

			base.Initialize ();
		}
		#endregion

		#region Load Content (Pictures)
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// Load the player resources 
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animations/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
			+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Textures/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Textures/bgLayer2", GraphicsDevice.Viewport.Width, -2);
			enemyTexture = Content.Load<Texture2D>("Animations/mineAnimation");
			mainBackground = Content.Load<Texture2D>("Textures/mainbackground");

			player.Initialize(playerAnimation, playerPosition);
		}
		#endregion

		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile(); 
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture,position); 
			projectiles.Add(projectile);
		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--) 
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				} 
			}
		}

		private void UpdateCollision()
		{
		// Use the Rectangle's built-in intersect function to 
		// determine if two objects are overlapping
		Rectangle rectangle1;
		Rectangle rectangle2;

		// Only create the rectangle once for the player
		rectangle1 = new Rectangle((int)player.Position.X,
		(int)player.Position.Y,
		player.Width,
		player.Height);

		// Do the collision between the player and the enemies
		for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
				(int)enemies[i].Position.Y,
				enemies[i].Width,
				enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
				// Subtract the health from the player based on
				// the enemy damage
				player.Health -= enemies[i].Damage;

				// Since the enemy collided with the player
				// destroy it
				enemies[i].Health = 0;

				// If the player health is less than zero we died
				if (player.Health <= 0)
				player.Active = false; 
				}

			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - 
						projectiles[i].Width / 2,(int)projectiles[i].Position.Y - 
						projectiles[i].Height / 2,projectiles[i].Width, projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}
		}

		#region Update Player (KeyStrokes)
		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update(gameTime);
			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A) ||
				currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D) ||
				currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W) ||
				currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S) ||
				currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}
			setPlayerBounds ();

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
			}
		}
		#endregion

		#region Player Region
		// Sets the player bounds position
		private void setPlayerBounds()
		{
			player.Position.X = MathHelper.Clamp(player.Position.X, 60, GraphicsDevice.Viewport.Width - 60);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 60, GraphicsDevice.Viewport.Height - player.Height);
		}
		#endregion

		#region Updates the Game (Every 60 Seconds)
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		protected override void Update (GameTime gameTime)
		{
			//Exits the game if Escape is Pressed
			if (Keyboard.GetState ().IsKeyDown (Keys.Escape)) 
			{	Exit ();	}
            
			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);

			//Update the player
			UpdatePlayer(gameTime);

			bgLayer1.Update();
			bgLayer2.Update();

			// Update the enemies
			UpdateEnemies(gameTime);
			UpdateCollision();
			UpdateProjectiles();
			base.Update (gameTime);
		}
		#endregion

		private void AddEnemy()
		{ 
		// Create the animation object
		Animation enemyAnimation = new Animation();

		// Initialize the animation with the correct animation information
		enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30,Color.White, 1f, true);

		// Randomly generate the position of the enemy
		Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

		// Create an enemy
		Enemy enemy = new Enemy();

		// Initialize the enemy
		enemy.Initialize(enemyAnimation, position); 

		// Add the enemy to the active enemies list
		enemies.Add(enemy);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
		// Spawn a new enemy enemy every 1.5 seconds
		if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) 
			{
			previousSpawnTime = gameTime.TotalGameTime;

			// Add an Enemy
			AddEnemy();
			}

		// Update the Enemies
		for (int i = enemies.Count - 1; i >= 0; i--) 
			{
			enemies[i].Update(gameTime);

			if (enemies[i].Active == false)
				{
				enemies.RemoveAt(i);
				} 
			}
		}

		#region Puts everything on display (Last Method Call)
		/// This is called when the game should draw itself.
		protected override void Draw (GameTime gameTime)
		{
			//Sets the background color
			graphics.GraphicsDevice.Clear (Color.AntiqueWhite);

			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for (int i = 0; i < enemies.Count; i++)
			{
			enemies[i].Draw(spriteBatch);
				for (int iz = 0; iz < projectiles.Count; i++)
				{
					projectiles[i].Draw(spriteBatch);
				}
			}

			// Draw the Player
			player.Draw(spriteBatch);

			// Stop drawing
			spriteBatch.End();
            
			//Re-draws the screen every time it updates
			base.Draw (gameTime);
		}
		#endregion
	}
}

