using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObjectsSpawner : MonoBehaviour
{
    public List<GameObject> objects;

    public float spawnTimer;

    List<Vector3> spawnPositions = new List<Vector3>();

    float spawnTimerDefault;

    private void Start()
    {
        spawnTimerDefault = spawnTimer;

        GameObject spawnersParentObject = GameObject.Find( "Physics_Objects_Spawners" );
        for ( int spawnerIndex = 0; spawnerIndex < spawnersParentObject.transform.childCount - 1; spawnerIndex++ )
            spawnPositions.Add( spawnersParentObject.transform.GetChild( spawnerIndex ).position );
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if ( spawnTimer < 0.0f )
        {
            if ( GameObject.Find( "Physic_Objects" ).transform.childCount < 20 )
            {
                int spawnerIndex = RandomizeSpawner();
                GameObject newObject = Instantiate( objects[ Random.Range( 0, objects.Count ) ], spawnPositions[ spawnerIndex ], Quaternion.identity, GameObject.Find( "Physic_Objects" ).transform );
                newObject.name += "_&&";
            }
            
            spawnTimer = spawnTimerDefault + Random.Range( -0.5f, 0.3f );
        }
    }

    int RandomizeSpawner()
    {
        return Random.Range( 0, spawnPositions.Count - 1 );
    }
}
