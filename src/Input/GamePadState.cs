#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2024 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

using System.Numerics;

namespace Microsoft.Xna.Framework.Input
{
	/// <summary>
	/// Represents specific information about the state of a controller,
	/// including the current state of buttons and sticks.
	/// </summary>
	public struct GamePadState
	{
		#region Public Properties

		/// <summary>
		/// Indicates whether the controller is connected.
		/// </summary>
		public bool IsConnected
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the packet number associated with this state.
		/// </summary>
		public int PacketNumber
		{
			get;
			internal set;
		}

		/// <summary>
		/// Returns a structure that identifies which buttons on the controller
		/// are pressed.
		/// </summary>
		public GamePadButtons Buttons
		{
			get;
			internal set;
		}

		/// <summary>
		/// Returns a structure that identifies which directions of the directional pad
		/// on the controller are pressed.
		/// </summary>
		public GamePadDPad DPad
		{
			get;
			internal set;
		}

		/// <summary>
		/// Returns a structure that indicates the position of the controller thumbsticks.
		/// </summary>
		public GamePadThumbSticks ThumbSticks
		{
			get;
			internal set;
		}

		/// <summary>
		/// Returns a structure that identifies the position of triggers on the controller.
		/// </summary>
		public GamePadTriggers Triggers
		{
			get;
			internal set;
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the GamePadState class using the specified
		/// GamePadThumbSticks, GamePadTriggers, GamePadButtons, and GamePadDPad.
		/// </summary>
		/// <param name="thumbSticks">Initial thumbstick state.</param>
		/// <param name="triggers">Initial trigger state.</param>
		/// <param name="buttons">Initial button state.</param>
		/// <param name="dPad">Initial directional pad state.</param>
		public GamePadState(
			GamePadThumbSticks thumbSticks,
			GamePadTriggers triggers,
			GamePadButtons buttons,
			GamePadDPad dPad
		) : this() {
			if (triggers.Left > GamePad.TriggerThreshold)
			{
				buttons.buttons |= Input.Buttons.LeftTrigger;
			}
			if (triggers.Right > GamePad.TriggerThreshold)
			{
				buttons.buttons |= Input.Buttons.RightTrigger;
			}
			buttons.buttons |= StickToButtons(
				thumbSticks.Left,
				Input.Buttons.LeftThumbstickLeft,
				Input.Buttons.LeftThumbstickRight,
				Input.Buttons.LeftThumbstickUp,
				Input.Buttons.LeftThumbstickDown,
				GamePad.LeftDeadZone
			);
			buttons.buttons |= StickToButtons(
				thumbSticks.Right,
				Input.Buttons.RightThumbstickLeft,
				Input.Buttons.RightThumbstickRight,
				Input.Buttons.RightThumbstickUp,
				Input.Buttons.RightThumbstickDown,
				GamePad.RightDeadZone
			);

			ThumbSticks = thumbSticks;
			Triggers = triggers;
			Buttons = buttons;
			DPad = dPad;
			IsConnected = true;
			PacketNumber = 0;
		}

		/// <summary>
		/// Initializes a new instance of the GamePadState class with the specified stick,
		/// trigger, and button values.
		/// </summary>
		/// <param name="leftThumbStick">
		/// Left stick value. Each axis is clamped between 1.0 and 1.0.
		/// </param>
		/// <param name="rightThumbStick">
		/// Right stick value. Each axis is clamped between 1.0 and 1.0.
		/// </param>
		/// <param name="leftTrigger">
		/// Left trigger value. This value is clamped between 0.0 and 1.0.
		/// </param>
		/// <param name="rightTrigger">
		/// Right trigger value. This value is clamped between 0.0 and 1.0.
		/// </param>
		/// <param name="buttons">
		/// Array or parameter list of Buttons to initialize as pressed.
		/// </param>
		public GamePadState(
			Vector2 leftThumbStick,
			Vector2 rightThumbStick,
			float leftTrigger,
			float rightTrigger,
			params Buttons[] buttons
		) : this(
			new GamePadThumbSticks(leftThumbStick, rightThumbStick),
			new GamePadTriggers(leftTrigger, rightTrigger),
			GamePadButtons.FromButtonArray(buttons),
			GamePadDPad.FromButtonArray(buttons)
		) {
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines whether specified input device buttons are pressed in this GamePadState.
		/// </summary>
		/// <param name="button">
		/// Buttons to query. Specify a single button, or combine multiple buttons using
		/// a bitwise OR operation.
		/// </param>
		public bool IsButtonDown(Buttons button)
		{
			return (Buttons.buttons & button) == button;
		}

		/// <summary>
		/// Determines whether specified input device buttons are up (not pressed) in this
		/// GamePadState.
		/// </summary>
		/// <param name="button">
		/// Buttons to query. Specify a single button, or combine multiple buttons using
		/// a bitwise OR operation.
		/// </param>
		public bool IsButtonUp(Buttons button)
		{
			return (Buttons.buttons & button) != button;
		}

		#endregion

		#region Private Static Methods

		private static Buttons StickToButtons(
			Vector2 stick,
			Buttons left,
			Buttons right,
			Buttons up,
			Buttons down,
			float DeadZoneSize
		) {
			Buttons b = (Buttons) 0;

			if (stick.X > DeadZoneSize)
			{
				b |= right;
			}
			if (stick.X < -DeadZoneSize)
			{
				b |= left;
			}
			if (stick.Y > DeadZoneSize)
			{
				b |= up;
			}
			if (stick.Y < -DeadZoneSize)
			{
				b |= down;
			}

			return b;
		}

		#endregion

		#region Public Static Operators and Override Methods

		/// <summary>
		/// Determines whether two GamePadState instances are equal.
		/// </summary>
		/// <param name="left">Object on the left of the equal sign.</param>
		/// <param name="right">Object on the right of the equal sign.</param>
		public static bool operator ==(GamePadState left, GamePadState right)
		{
			return (	(left.IsConnected == right.IsConnected) &&
					(left.PacketNumber == right.PacketNumber) &&
					(left.Buttons == right.Buttons) &&
					(left.DPad == right.DPad) &&
					(left.ThumbSticks == right.ThumbSticks) &&
					(left.Triggers == right.Triggers)	);
		}

		/// <summary>
		/// Determines whether two GamePadState instances are not equal.
		/// </summary>
		/// <param name="left">Object on the left of the equal sign.</param>
		/// <param name="right">Object on the right of the equal sign.</param>
		public static bool operator !=(GamePadState left, GamePadState right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Returns a value that indicates whether the current instance is equal to a
		/// specified object.
		/// </summary>
		/// <param name="obj">Object with which to make the comparison.</param>
		public override bool Equals(object obj)
		{
			return (obj is GamePadState) && (this == (GamePadState) obj);
		}

		/// <summary>
		/// Gets the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Retrieves a string representation of this object.
		/// </summary>
		public override string ToString()
		{
			return base.ToString();
		}

		#endregion
	}
}
