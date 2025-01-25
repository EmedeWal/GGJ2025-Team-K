using UnityEngine;

public class Attributes : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _health;
    private float _prevHealth;
    [SerializeField] private Transform _playerAirBubble;
    [SerializeField] private float _lerpSpeed = 1f;
    private SpriteRenderer _playerAirBubbleSprite;
    private float _originalScale;

    void Start()
    {
        _originalScale = ((Vector2)_playerAirBubble.localScale).magnitude;

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
        var currentScale = _playerAirBubble.localScale.x;
        var newScale = _health / _maxHealth * _originalScale;       
        
        var response = 1f - Mathf.Exp(-_lerpSpeed * Time.deltaTime);
        var scale = Mathf.Lerp(currentScale, newScale, response);
        _playerAirBubble.localScale = new Vector3(scale, scale, 0);
    }
}
