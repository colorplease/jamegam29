using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Events/Non Generic Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<IGameEventListener> _gameEventListeners = new List<IGameEventListener>();

        public void Raise()
        {
            for (int i = _gameEventListeners.Count - 1; i >= 0; i--)
            {
                _gameEventListeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(IGameEventListener listener)
        {
            if (!_gameEventListeners.Contains(listener))
                _gameEventListeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener listener)
        {
            if (_gameEventListeners.Contains(listener))
                _gameEventListeners.Remove(listener);
        }
    }
    
    public interface IGameEventListener
    {
        void OnEventRaised();
    }
}

// Generic versions
[CreateAssetMenu(fileName = "GameEvent", menuName = "Events/Generic Game Event")]
public class GameEvent<T> : ScriptableObject
{
    private readonly List<IGameEventListener<T>> _gameEventListeners = new List<IGameEventListener<T>>();

    public void Raise(T item)
    {
        for (int i = _gameEventListeners.Count - 1; i >= 0; i--)
        {
            _gameEventListeners[i].OnEventRaised(item);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener)
    {
        if (!_gameEventListeners.Contains(listener))
            _gameEventListeners.Add(listener);
    }

    public void UnregisterListener(IGameEventListener<T> listener)
    {
        if (_gameEventListeners.Contains(listener))
            _gameEventListeners.Remove(listener);
    }
}

public interface IGameEventListener<T>
{
    void OnEventRaised(T item);
}


