using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Behaviour[] componentsToDisableOnDeath;

    [SyncVar]
    string playerName;
    [SyncVar]
    float currentHealth;
    [SyncVar]
    bool _isDead;

    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    
    bool[] wasComponentEnabled;

    public void Setup()
    {
        wasComponentEnabled = new bool[componentsToDisableOnDeath.Length];
        for (int i = 0; i < componentsToDisableOnDeath.Length; i++)
        {
            wasComponentEnabled[i] = componentsToDisableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [ClientRpc]
    public void RpcChangeName(string name)
    {
        playerName = name;
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < componentsToDisableOnDeath.Length; i++)
        {
            componentsToDisableOnDeath[i].enabled = wasComponentEnabled[i];
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;

        for (int i = 0; i < componentsToDisableOnDeath.Length; i++)
        {
            componentsToDisableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

}
