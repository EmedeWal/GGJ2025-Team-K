using UnityEngine;

public class Attributes
{
    public int CurrentHealth { get; private set; }

    private readonly Transform _airBubbleTransform;
    private readonly float _scaleFactor;
    private readonly float _response;
    private readonly int _maximumHealth;

    public Attributes(Transform airBubbleTransform, int maximumHealth, float response)
    {
        _airBubbleTransform = airBubbleTransform;
        _maximumHealth = maximumHealth;
        _response = response;

        _scaleFactor = (airBubbleTransform.localScale.x + airBubbleTransform.localScale.y) / 2;
        SetCurrentToMaxHealth();
    }

    public void SetCurrentToMaxHealth() => CurrentHealth = _maximumHealth;

    public void LateTick(float deltaTime)
    {
        var currentScale = _airBubbleTransform.localScale.x;
        var targetScale = (float)CurrentHealth / (float)_maximumHealth * _scaleFactor;

        var response = 1f - Mathf.Exp(-_response * deltaTime);
        var scale = Mathf.Lerp(currentScale, targetScale, response);
        _airBubbleTransform.localScale = new Vector3(scale, scale, 0);
    }

    public void RemoveHealth(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
    }

    public void AddHealth(int health)
    {
        CurrentHealth += health;
        CurrentHealth = Mathf.Min(CurrentHealth, _maximumHealth);
    }
}