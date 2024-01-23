using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Toolset : MonoBehaviour
	{
		public delegate void ToolChange(Tools tool);

		public event ToolChange openMenu;
		public event ToolChange closeMenu;
		public event ToolChange changingTools;

		private PlayerInput m_input;
		private Controller m_controller;

		private Flashlight m_flashlight;
		private Lockpick m_lockpick;
		private Keychain m_keychain;
		private Whacker m_whacker;

		public Controller GetController()
		{
			return m_controller;
		}

		public enum Tools
		{
			lockpick,
			keychain,
			flashlight,
			whacker,
			dartgun,
			flashbang
		}

		Tools m_currentTool;

		public void Init(PlayerInput playerInput, Controller controller)
		{
			m_input = playerInput;
			m_controller = controller;

			if (m_input != null)
			{
				m_input.actions.FindAction("Tool Menu").performed += StartSelecting;
				m_input.actions.FindAction("Tool Menu").canceled += StopSelecting;
				m_input.actions.FindAction("Use Tool").performed += UseTool;
				m_input.actions.FindAction("Use Tool").canceled += CancelTool;
			}

			m_whacker = GetComponentInChildren<Whacker>();
			m_whacker.Init(this);

			m_flashlight = GetComponentInChildren<Flashlight>();
			m_flashlight.Init(this);

			m_lockpick = GetComponentInChildren<Lockpick>();
			m_lockpick.Init(this);

			m_keychain = GetComponentInChildren<Keychain>();
			m_keychain.Init(this);

			m_currentTool = Tools.keychain;

		}
		
		public Lockpick GetLockpick()
		{
			return m_lockpick;
		}

		public Flashlight GetFlashlight()
		{
			return m_flashlight;
		}

		public Keychain GetKeychain()
		{
			return m_keychain;
		}

		public Whacker GetWhacker()
		{
			return m_whacker;
		}

		void UseTool(InputAction.CallbackContext ctx)
		{
			switch (m_currentTool)
			{
				case Tools.lockpick:
					m_lockpick.StartLockpicking();
					break;

				case Tools.keychain:
					m_keychain.UseKeychain();
					break;

				case Tools.flashlight:
					m_flashlight.ToggleFlashlight();
					break;
				case Tools.whacker:
					m_whacker.StartCharge();
					break;
			}
		}

		void CancelTool(InputAction.CallbackContext ctx)
		{
			switch (m_currentTool)
			{
				case Tools.lockpick:
					m_lockpick.StopLockpicking();
					break;
				case Tools.whacker:
					m_whacker.StopCharge();
					m_whacker.Attack();
					break;
				default:
					break;
			}
		}

		// Tool menu selection

		bool c_isSelecting = false;
		Coroutine c_selecting;

		void StartSelecting(InputAction.CallbackContext ctx)
		{
			if (c_isSelecting) return;
			c_isSelecting = true;

			if (c_selecting != null) return;
			c_selecting = StartCoroutine(Selecting(ctx));

			Time.timeScale = 0.5f;

			openMenu?.Invoke(m_currentTool);
		}

		void StopSelecting(InputAction.CallbackContext ctx)
		{
			if (!c_isSelecting) return;
			c_isSelecting = false;

			if (c_selecting == null) return;
			StopCoroutine(c_selecting);
			c_selecting = null;

			Time.timeScale = 1f;

			closeMenu?.Invoke(m_currentTool);
		}

		IEnumerator Selecting(InputAction.CallbackContext ctx)
		{
			float angle = 0f;

			while (c_isSelecting)
			{
				Vector2 mouseDelta = m_input.actions.FindAction("Rotation").ReadValue<Vector2>();

				if(mouseDelta.magnitude > 1.25f)
				{
					angle = Vector2.SignedAngle(Vector2.up, mouseDelta);
					angle = MathTools.NormalizeAngle(angle);

					Debug.LogWarning("Current Angle: " + angle);

					int toolAmount = 6;

					switch (angle)
					{
						case float x when (x >= 0 && x < (360 / toolAmount) * 1):
							m_currentTool = Tools.flashbang;
							break;
						case float x when (x >= (360 / toolAmount) * 1 && x < (360 / toolAmount) * 2):
							m_currentTool = Tools.dartgun;
							break;
						case float x when (x >= (360 / toolAmount) * 2 && x < (360 / toolAmount) * 3):
							m_currentTool = Tools.whacker;
							break;
						case float x when (x >= (360 / toolAmount) * 3 && x < (360 / toolAmount) * 4):
							m_currentTool = Tools.lockpick;
							break;
						case float x when (x >= (360 / toolAmount) * 4 && x < (360 / toolAmount) * 5):
							m_currentTool = Tools.keychain;
							break;
						case float x when (x >= (360 / toolAmount) * 5 && x <= (360 / toolAmount) * 6):
							m_currentTool = Tools.flashlight;
							break;
						default:
							break;
					}
				}

				changingTools?.Invoke(m_currentTool);

				yield return new WaitForFixedUpdate();
			}

			StopSelecting(ctx);
		}

	}
}
