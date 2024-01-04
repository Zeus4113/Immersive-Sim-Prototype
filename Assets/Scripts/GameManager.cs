using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	[SerializeField] private UIManager m_UIManager;
	[SerializeField] private Player.Controller m_controller;
	[SerializeField] private LootManager m_lootManager;
	[SerializeField] private CheckpointManager m_checkpointManager;
	[SerializeField] private EnemyManager m_enemyManager;

	private PlayerInput m_input;

	private void Start()
	{
		m_input = GetComponent<PlayerInput>();

		SetupGame();
	}

	void SetupGame()
	{
		m_controller.Init(m_input, this);
		m_lootManager.Init(this);
		m_checkpointManager.Init(this);
		m_enemyManager.Init(this);

		m_UIManager.Init(this);
	}

	public UIManager GetUIManager()
	{
		return m_UIManager;
	}

	public Player.Controller GetController()
	{
		return m_controller;
	}

	public LootManager GetLootManager()
	{
		return m_lootManager;
	}

	public CheckpointManager GetCheckpointManager()
	{
		return m_checkpointManager;
	}

	public EnemyManager GetEnemyManager()
	{
		return m_enemyManager;
	}
}
