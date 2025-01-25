using UnityEngine;

public class Attributes
{
    private int _health;
    private float _originalScale;

    public Attributes()
    {

    }
}

    //private void Start()
    //{
    //    _originalScale = ((Vector2)_playerAirBubble.localScale).magnitude;

    //    _health = _maxHealth;
    //}

    //private void Update()
    //{
    //    LerpSpriteToHealth();
    //}

    //public void TakeDamage(float damage)
    //{
    //    _health -= damage;
    //    if (_health <= 0)
    //    {
    //        print("lol!");
    //    }
    //}

    //public void AddHealth(float health)
    //{
    //    _health += health;
    //    if (_health > _maxHealth)
    //    {
    //        _health = _maxHealth;
    //    }
    //}

    //void LerpSpriteToHealth()
    //{
    //    var currentScale = _playerAirBubble.localScale.x;
    //    var newScale = _health / _maxHealth * _originalScale;       
        
    //    var response = 1f - Mathf.Exp(-_lerpSpeed * Time.deltaTime);
    //    var scale = Mathf.Lerp(currentScale, newScale, response);
    //    _playerAirBubble.localScale = new Vector3(scale, scale, 0);
    //}

