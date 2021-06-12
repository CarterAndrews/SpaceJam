using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SpawnManager : MonoBehaviour
{
    public List<Transform> startingSpawns;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        SetPlayerPositionAndColor(playerInput.transform);
    }
    private void SetPlayerPositionAndColor(Transform spawn)
    {
        int index = Random.Range(0, startingSpawns.Count);
        spawn.position = startingSpawns[index].position;
        startingSpawns.RemoveAt(index);
    }
}
