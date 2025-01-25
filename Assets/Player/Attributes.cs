using UnityEngine;
using System;

public class Attributes : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _health;
    private float _prevHealth;
    [SerializeField] private GameObject _playerAirBubble;
    private SpriteRenderer _playerAirBubbleSprite;
    [Range(0f, 0.5f)]
    [SerializeField] private float _airLerpScalar;
    void Start()
    {
        // _playerAirBubbleSprite = _playerAirBubble.GetComponentInChildren<SpriteRenderer>();
        _health = _maxHealth;
        _prevHealth = _health;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
        LerpSpriteToHealth();
    }

    public void TakeDamage(float damage)
    {
        _prevHealth = _health;
        _health -= damage;
        if (_health <= 0)
        {
            print("lol!");
        }
    }

    public void AddHealth(float health)
    {
        _prevHealth = _health;
        _health += health;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }

    void LerpSpriteToHealth()
    {
        var currentScale = _playerAirBubble.transform.localScale.x;
        var newScale = _health / _maxHealth;

        var response = 1f - Mathf.Exp(-_airLerpScalar * Time.deltaTime);
        var scale = Mathf.Lerp(currentScale, newScale, response);

        _playerAirBubble.transform.localScale = new Vector3(scale, scale, 0);
    }
}
