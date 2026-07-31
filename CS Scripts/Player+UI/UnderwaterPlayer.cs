using Godot;
using System;

public partial class UnderwaterPlayer : Player
{
	//Stores what type of coral player is stuck in
	public enum InCoral {
		VERTICAL, HORIZONTAL, NONE
	}
	
	public InCoral CoralStatus = InCoral.NONE;
	private AnimatedSprite2D animatedSprite;
	private Godot.Timer hurtTimer;
	
	public override void _Ready() {
		base._Ready();
		base.Speed = 120;
		base.Gravity = 0.001F;

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite.Animation = "sit-helmet";
		hurtTimer = GetNode<Godot.Timer>("HurtTimer");
	}
	
	public override void _PhysicsProcess(double delta) {
		var velocity = Vector2.Zero; //(0, 0)

		if (!MovementIsDisabled())
		{
			if (InputEnabled) {
				if (CoralStatus != InCoral.VERTICAL) {
					if (Input.IsActionPressed("move_right")) {
						velocity.X += 1;
					}
					if (Input.IsActionPressed("move_left")) {
						velocity.X -= 1;
					}
				}
				if (CoralStatus != InCoral.HORIZONTAL) {
					if (Input.IsActionPressed("move_down")) {
						velocity.Y += 1;
					}
					if (Input.IsActionPressed("move_up")) {
						velocity.Y -= 1;
					}
				}
			}

			var originalY = velocity.Y;
			//gravity and velocity modifier
			if (CoralStatus != InCoral.HORIZONTAL) {
				velocity.Y += Gravity;
			}
		
			velocity = velocity.Normalized() + VelocityModifier;
		
			if (velocity.Length() > 0) {
				velocity = velocity * Speed;
				
				animatedSprite.Play();
			}
			else {
				animatedSprite.Stop();
			}

			//setting the animation
			if (velocity.X < 0)
			{
				animatedSprite.Animation = "swim-left";
				FacingRight = false;
			}
			else if (velocity.X > 0)
			{
				animatedSprite.Animation = "swim-right";
				FacingRight = true;
			}
			else
			{
				if (FacingRight) {
					animatedSprite.Animation = "sit-helmet";
				}
				else {
					animatedSprite.Animation = "left_sit";
				}
				
			}
			if (originalY != 0) {
				if (FacingRight) {
					animatedSprite.Animation = "swim-right";
				}
				else {
					animatedSprite.Animation = "swim-left";
				}
			}
			//Position += velocity * (float)delta;
			//MoveAndCollide(velocity * (float)delta); //character2d movement
			
			Velocity = velocity;
		} //end of if (movement not disabled)
		
		MoveAndSlide();
		//flashing when hurt
		if (Flash)
		{
			if (hurtTimer.TimeLeft % 0.2 < 0.1)
			{
				animatedSprite.SelfModulate = new Color(1, 1, 1, 1);
			}
			else
			{
				animatedSprite.SelfModulate = new Color(100, 1, 1, 1);
			}
		}

		int collisionCount = GetSlideCollisionCount();
		for (int i = 0; i < collisionCount; i++) {
			//get info returned from MoveAndCollide about collisions
			var collisionInfo = GetSlideCollision(i);
			var collider = collisionInfo.GetCollider();
			if (collider is RollingBomb bomb) {
				float massRatio = Mass / (Mass + bomb.bombMass);
				//GetNormal returns Vector2 pointing where it was hit, - flips it to point the other way
				Vector2 impulse = -collisionInfo.GetNormal() * Velocity.Length() * massRatio;

				//Gets position of collision in global coordinates, convert to local coordinates
				//Vector2 positionHit = ToLocal(collisionInfo.GetPosition());
				
				//checks that player didn't hit from above (which makes it do wierd things)
				//GetNormal returns normal vector of collision (points towards what hit it)
				if (!(collisionInfo.GetNormal().Y < -0.7f)) {
					bomb.ApplyCentralImpulse(impulse);
				}
			}
			else if (collider is RigidBody2D jellyfish) { //bouncy jellyfish bounce
				globalSound.PlaySound("bounce");
				Velocity = collisionInfo.GetNormal() * new Vector2(300, 300);
				
				//glances off and goes right if x of normal vector >= 0
				if (collisionInfo.GetNormal().X >= 0) {
					animatedSprite.Animation = "swim-right";
				}
				else {
					animatedSprite.Animation = "swim-left";
				}
				animatedSprite.Play();
				SetDisableControl(true);
				StunTimer();
			}
		}
	}
	
	private async void StunTimer() {
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
		SetDisableControl(false);
	}

	//change player back to normal after flashing animation
	new public void OnHurtTimerTimeout()
	{
		base.OnHurtTimerTimeout();
		animatedSprite.Modulate = new Color(1, 1, 1, 1);

	}

	private void OnPlayerDied()
	{
		animatedSprite.Animation = "died";
	}
	
	public void OnTubeCoralPull(Vector2 tubeVelocity)
	{
		SetVelocityModifier(tubeVelocity);
		
		if (tubeVelocity.X != 0) {
			CoralStatus = InCoral.HORIZONTAL;
		}
		else {
			CoralStatus = InCoral.VERTICAL;
		}
		
	}

	//stop pulling the character when it leaves the AOE
	public void OnTubeCoralUnpull()
	{
		SetVelocityModifier(Vector2.Zero);
		CoralStatus = InCoral.NONE;
	}

}
