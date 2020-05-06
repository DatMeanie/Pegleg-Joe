using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies; 

    public float spawnTimer = 1.0f;
    public float spawnTimerLimit = 0.8f;

    List<Vector3> spawnPositions = new List<Vector3>();

    float spawnTimerDefault;

    private void Start()
    {
        if ( GameObject.Find( "DataManager" ) )
        {
            DataManager dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();
            spawnTimer = spawnTimer * dataManager.GetGameValue( EGame_Values_Keys.ENEMY_SPAWN_MODIFIER );
            spawnTimerLimit = spawnTimerLimit * dataManager.GetGameValue( EGame_Values_Keys.ENEMY_SPAWN_LIMIT );
        }

        spawnTimerDefault = spawnTimer;

        GameObject spawnersParentObject = GameObject.Find( "Enemy_Spawners" );
        for ( int spawnerIndex = 0; spawnerIndex < spawnersParentObject.transform.childCount - 1; spawnerIndex++ )
            spawnPositions.Add( spawnersParentObject.transform.GetChild( spawnerIndex ).position );
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if ( spawnTimer < 0.0f )
        {
            if( GameObject.Find( "Enemies" ).transform.childCount < 30 )
            {
                int spawnerIndex = RandomizeSpawner();
                GameObject enemy = Instantiate( enemies[ Random.Range( 0, enemies.Count ) ], spawnPositions[ spawnerIndex ], Quaternion.identity, GameObject.Find( "Enemies" ).transform );
                enemy.GetComponent<Enemy>().Initialize();
            }

            spawnTimer = spawnTimerDefault + Random.Range( -0.5f, 0.3f);
            spawnTimerDefault -= 0.001f;
            if ( spawnTimerDefault <= spawnTimerLimit )
                spawnTimerDefault = spawnTimerLimit;
        }
    }

    int RandomizeSpawner()
    {
        return Random.Range(0, spawnPositions.Count - 1);
    }
}
