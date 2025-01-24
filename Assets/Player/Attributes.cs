using UnityEngine;

public class Attributes : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _health;
    [SerializeField] private GameObject _playerAirBubble;
    private SpriteRenderer _playerAirBubbleSprite;
    void Start()
    {
        // _playerAirBubbleSprite = _playerAirBubble.GetComponentInChildren<SpriteRenderer>();
        _health = _maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _playerAirBubble.transform.localScale = new Vector3(_health / _maxHealth, _health / _maxHealth, 1);
        if (_health <= 0)
        {
            print("lol!");
        }
    }

    public void AddHealth(float health)
    {
        _health += health;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        _playerAirBubble.transform.localScale = new Vector3(_health / _maxHealth, _health / _maxHealth, 1);
    }
}
