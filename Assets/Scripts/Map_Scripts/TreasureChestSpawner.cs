using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreasureChestSpawner : MonoBehaviour
{
    public List<GameObject> chests;
    GameObject[] pooledChests;

    public float spawnTimer;

    Dictionary<Vector3, bool> availablePositions = new Dictionary<Vector3, bool>();

    float spawnTimerDefault;

    int index = 0;

    public void Initialize()
    {
        spawnTimerDefault = spawnTimer;

        GameObject spawnersParentObject = GameObject.Find( "TreasureChests_Spawners" );
        for ( int spawnerIndex = 0; spawnerIndex < spawnersParentObject.transform.childCount - 1; spawnerIndex++ )
            availablePositions.Add( spawnersParentObject.transform.GetChild( spawnerIndex ).position, true );

        pooledChests = new GameObject[ 10 ];
        for ( int i = 0; i < 10; i++ )
            pooledChests[ i ] = Instantiate( chests[ Random.Range( 0, chests.Count ) ]
                , new Vector3(500, 200, 800)
                , Quaternion.identity
                , GameObject.Find( "TreasureChests_Spawners" ).transform );
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if ( spawnTimer < 0.0f )
        {
            if ( GameObject.Find( "Physic_Objects" ).transform.childCount < 20 )
            {
                Vector3 spawnPos = RandomizeSpawner();
                if ( spawnPos.x > 400 ) 
                { 
                    spawnTimer = spawnTimerDefault + Random.Range( -0.5f, 1.3f );
                    return;
                }

                index++;
                if ( index >= pooledChests.Length )
                    index = 0;

                pooledChests[ index ].GetComponent<TreasureChest>().Initialize( spawnPos );
                availablePositions[ spawnPos ] = false;
            }

            spawnTimer = spawnTimerDefault + Random.Range( -0.5f, 1.3f );
        }
    }
    
    Vector3 RandomizeSpawner()
    {
        List<Vector3> spawners = availablePositions.Keys.ToList();
        List<Vector3> available = new List<Vector3>();
        foreach ( Vector3 pos in spawners )
            if ( availablePositions[ pos ] == true )
                available.Add( pos );

        if( available.Count >= 1)
            return available[ Random.Range( 0, available.Count ) ];
        else
            return new Vector3( 404, 404, 404 );
    }

    public void MakeSpawnerAvailable( Vector3 pos )
    {
        availablePositions[ pos ] = true;
    }
}
