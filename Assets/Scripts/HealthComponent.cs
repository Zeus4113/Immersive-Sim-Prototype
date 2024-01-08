using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private Player.Controller m_controller;

    public delegate void OnHealthChange(float health);
    public event OnHealthChange healthChange;

    [SerializeField] private float m_maxHealth = 100f;

    private float m_currentHealth;

	public void Init(Player.Controller con)
	{
        m_controller = con;
        m_currentHealth = m_maxHealth;
        healthChange?.Invoke(m_currentHealth);
    }

	public void Damage(float damage)
    {
        m_currentHealth -= damage;
        Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
        healthChange?.Invoke(m_currentHealth);
    }

    public void SetHealth(float health)
    {
        m_currentHealth = health;
        Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
        healthChange?.Invoke(m_currentHealth);
    }

    public bool IsAlive()
    {
        return m_currentHealth > 0;
    }
}
