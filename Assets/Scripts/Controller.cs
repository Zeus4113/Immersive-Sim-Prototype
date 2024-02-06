using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Controller : MonoBehaviour
	{
		[SerializeField] private Movement m_movement;
		[SerializeField] private VisibilityCalculator m_visibilityCalculator;
		[SerializeField] private Interaction m_interact;
		[SerializeField] private HealthComponent m_healthComponent;
		[SerializeField] private PlayerAudioManager m_audioManager;
		[SerializeField] private Toolset m_toolset;
		[SerializeField] private Footsteps m_footsteps;

		private PlayerInput m_input;
		private GameManager m_gameManager;

		public void Init(PlayerInput playerInput, GameManager gm)
		{
			m_input = playerInput;
			m_gameManager = gm;

			SetupPlayer();
			BindEvents();
		}

		public void SetupPlayer()
		{
			m_movement.Init(m_input, this);
			m_visibilityCalculator.Init(this);
			m_interact.Init(m_input, this);
			m_toolset.Init(m_input, this);
			m_healthComponent.Init(this);
			m_footsteps.Init(m_input, this);

			m_healthComponent.playerDead += m_gameManager.EnableInputEvents;
			m_healthComponent.playerDead += m_gameManager.GetUIManager().PlayerDead;

		}

		private void OnDestroy()
		{
			UnbindEvents();
		}

		void BindEvents()
		{
			if (m_input == null) return;
			m_input.actions.FindAction("Pause Menu").performed += m_gameManager.GetUIManager().GamePaused;
		}

		void UnbindEvents()
		{
			if (m_input == null) return;
			m_input.actions.FindAction("Pause Menu").performed -= m_gameManager.GetUIManager().GamePaused;
		}

		public Movement GetMovement()
		{
			return m_movement;
		}

		public Interaction GetInteract()
		{
			return m_interact;
		}

		public VisibilityCalculator GetVisibilityCalculator()
		{
			return m_visibilityCalculator;
		}

		public GameManager GetManager()
		{
			return m_gameManager;
		}

		public HealthComponent GetHealthComponent()
		{
			return m_healthComponent;
		}

		public PlayerAudioManager GetAudioManager()
		{
			return m_audioManager;
		}

		public Toolset GetToolset()
		{
			return m_toolset;
		}

		public Footsteps GetFootsteps()
		{
			return m_footsteps;
		}
	}
}
