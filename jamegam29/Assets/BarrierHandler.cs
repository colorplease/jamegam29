using GameEvents;
using LevelModule.Scripts;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BarrierHandler : MonoBehaviour, IGameEventListener
{
    [SerializeField] private GameEvent levelCompletedEvent;
    [SerializeField] private int collisionDamage;

    private AudioSource _audioSource;

    private BoxCollider2D _boxCollider2D;
    // Start is called before the first frame update

    private void Awake()
    {
        levelCompletedEvent.RegisterListener(this);
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        levelCompletedEvent.UnregisterListener(this);
    }
    

    public void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.transform.GetComponent<PlayerHealthHandler>();

        if (player)
        {
            player.TakeDamage(collisionDamage);
        }
    }

    public void OnEventRaised()
    {
        _boxCollider2D.enabled = false;
        _audioSource.Play();
    }
}
