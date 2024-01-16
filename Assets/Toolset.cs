using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Toolset : MonoBehaviour
	{
		private PlayerInput m_input;
		private Controller m_controller;

		private Flashlight m_flashlight;
		private Lockpick m_lockpick;
		private Keychain m_keychain;

		public Controller GetController()
		{
			return m_controller;
		}

		enum Tools
		{
			lockpick,
			keychain,
			flashlight,
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

			//m_flashlight = GetComponentInChildren<Flashlight>();
			//m_flashlight.Init(this);

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
			}
		}

		void CancelTool(InputAction.CallbackContext ctx)
		{
			switch (m_currentTool)
			{
				case Tools.lockpick:
					m_lockpick.StopLockpicking();
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
			c_selecting = StartCoroutine(Selecting());
		}

		void StopSelecting(InputAction.CallbackContext ctx)
		{
			if (!c_isSelecting) return;
			c_isSelecting = false;

			if (c_selecting == null) return;
			StopCoroutine(c_selecting);
			c_selecting = null;
		}

		IEnumerator Selecting()
		{
			while (c_isSelecting)
			{
				Vector2 mouseDelta = m_input.actions.FindAction("Rotation").ReadValue<Vector2>();

				Debug.Log("Current Delta: " + mouseDelta);

				float angle = Vector2.SignedAngle(Vector2.up, mouseDelta);

				Debug.Log("Current Angle: " + angle);

				yield return new WaitForFixedUpdate();
			}
		}

	}
}
