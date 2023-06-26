using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProcedulalRoomGenerator : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _spriteRenderers;

    private void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        RandomizeSpritePositionsAndRotations(_spriteRenderers);
    }

    public void RandomizeSpritePositionsAndRotations(List<SpriteRenderer> sprites)
    {
        int count = sprites.Count;

        // Create a copy of the positions so that we don't overwrite the original positions
        List<Vector3> positions = new List<Vector3>();
        foreach (SpriteRenderer sprite in sprites)
        {
            positions.Add(sprite.transform.position);
        }

        // Shuffle the list of positions
        System.Random rng = new System.Random();
        int n = positions.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector3 value = positions[k];
            positions[k] = positions[n];
            positions[n] = value;
        }

        // Set new positions and random Z rotations
        for (int i = 0; i < count; i++)
        {
            sprites[i].transform.position = positions[i];
            float randomZRotation = Random.Range(0f, 360f);
            //sprites[i].transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
        }
    }
}
