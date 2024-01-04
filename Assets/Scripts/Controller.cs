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

		private PlayerInput m_input;
		private GameManager m_gameManager;

		public void Init(PlayerInput playerInput, GameManager gm)
		{
			m_input = playerInput;
			m_gameManager = gm;

			SetupPlayer();
		}

		public void SetupPlayer()
		{
			m_movement.Init(m_input, this);
			m_visibilityCalculator.Init(this);
			m_interact.Init(m_input, this);
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
	}
}
