using UnityEngine;
using System;

public class Attributes
{
    public float CurrentHealth { get; private set; }

    private readonly Transform _airBubbleTransform;
    private readonly float _scaleFactor;
    private readonly float _response;
    private readonly float _maximumHealth;

    public static event Action<float> HealthUpdated;

    public Attributes(Transform airBubbleTransform, float maximumHealth, float response)
    {
        _airBubbleTransform = airBubbleTransform;
        _maximumHealth = maximumHealth;
        _response = response;

        _scaleFactor = (airBubbleTransform.localScale.x + airBubbleTransform.localScale.y) / 2;
        Attributes_ResetGameState();

        LevelManager.ResetGameState += Attributes_ResetGameState;
    }

    public void Cleanup()
    {
        HealthUpdated = null;
    }

    public void LateTick(float deltaTime)
    {
        var currentScale = _airBubbleTransform.localScale.x;
        var targetScale = CurrentHealth / _maximumHealth * _scaleFactor;

        var response = 1f - Mathf.Exp(-_response * deltaTime);
        var scale = Mathf.Lerp(currentScale, targetScale, response);
        _airBubbleTransform.localScale = new Vector3(scale, scale, 0);
    }

    public void RemoveHealth(float damage)
    {
        var targetHealth = CurrentHealth - damage;
        targetHealth = Mathf.Max(targetHealth, 0);

        OnHealthUpdated(targetHealth);
    }

    public void AddHealth(float health)
    {
        var targetHealth = CurrentHealth + health;
        targetHealth = Mathf.Min(targetHealth, _maximumHealth);

        OnHealthUpdated(targetHealth);
    }

    public void OnHealthUpdated(float health)
    {
        CurrentHealth = health;
        HealthUpdated?.Invoke(health);
    }

    private void Attributes_ResetGameState() => OnHealthUpdated(_maximumHealth);
}