using UnityEngine;

public class Attributes
{
    public float CurrentHealth { get; private set; }

    private readonly Transform _airBubbleTransform;
    private readonly float _scaleFactor;
    private readonly float _response;
    private readonly float _maximumHealth;

    public Attributes(Transform airBubbleTransform, float maximumHealth, float response)
    {
        _airBubbleTransform = airBubbleTransform;
        _maximumHealth = maximumHealth;
        _response = response;

        _scaleFactor = (airBubbleTransform.localScale.x + airBubbleTransform.localScale.y) / 2;
        Attributes_ResetGameState();

        LevelManager.ResetGameState += Attributes_ResetGameState;
    }

    public void LateTick(float deltaTime)
    {
        var currentScale = _airBubbleTransform.localScale.x;
        var targetScale = CurrentHealth / _maximumHealth * _scaleFactor;

        var response = 1f - Mathf.Exp(-_response * deltaTime);
        var scale = Mathf.Lerp(currentScale, targetScale, response);
        _airBubbleTransform.localScale = new Vector3(scale, scale, 0);
    }

    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }

    public void RemoveHealth(float damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
    }

    public void AddHealth(float health)
    {
        CurrentHealth += health;
        CurrentHealth = Mathf.Min(CurrentHealth, _maximumHealth);
    }

    private void Attributes_ResetGameState() => CurrentHealth = _maximumHealth;
}