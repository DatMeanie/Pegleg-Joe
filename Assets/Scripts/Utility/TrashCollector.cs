using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    List<GameObject> trashObjects = new List<GameObject>();
    List<GameObject> fallenObjects = new List<GameObject>();

    float colliderTimer = 2.0f;
    float colliderTimerDefault;

    float deleteTimer = 0.5f;
    float deleteTimerDefault;

    bool deletingEverything = false;

    private void Start()
    {
        deleteTimerDefault = deleteTimer;
        colliderTimerDefault = colliderTimer;
    }

    void Update()
    {
        if( deletingEverything == false )
        {
            colliderTimer -= Time.deltaTime;
            deleteTimer -= Time.deltaTime;

            //if ( colliderTimer < 0.0f )
            //{
            //    if ( transform.childCount > 300 && transform.childCount < 700 )
            //        RemoveColliders();
                
            //    if ( transform.childCount > 50 )
            //        colliderTimer = colliderTimerDefault / ( transform.childCount / 50.0f );
            //    else
            //        colliderTimer = colliderTimerDefault;
            //}

            if ( deleteTimer < 0.0f )
            {
                if( transform.childCount > 1500 )
                    DeleteObjects();
                if ( transform.childCount > 50 )
                    deleteTimer = deleteTimerDefault / ( transform.childCount / 10.0f );
                else
                    deleteTimer = deleteTimerDefault;
            }

            //if ( transform.childCount > 800 )
            //    DeleteAllObjects();
        }
    }

    void RemoveColliders()
    {
        int childCount = trashObjects.Count - 1;
        for ( int childIndex = 0; childIndex < 10; childIndex++ )
        {
            if ( trashObjects.Count > 0 )
            {
                if( trashObjects[ childCount ] != null )
                {
                    if ( trashObjects[ childCount ].GetComponent<Collider>() )
                    {
                        int colliderCount = trashObjects[ childCount ].GetComponents<Collider>().Length;
                        for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
                            trashObjects[ childCount ].GetComponent<Collider>().enabled = false;
                    }


                    if ( trashObjects[ childCount ].GetComponent<Rigidbody>() )
                    {
                        trashObjects[ childCount ].GetComponent<Rigidbody>().isKinematic = false;
                        trashObjects[ childCount ].GetComponent<Rigidbody>().useGravity = true;
                    }

                    if ( trashObjects[ childCount ].transform.childCount > 0 )
                        foreach ( Transform child in trashObjects[ childCount ].transform )
                            RemoveChildrenColliders( child.gameObject );

                    fallenObjects.Add( trashObjects[ childCount ].gameObject );
                }
                
                trashObjects.RemoveAt( childCount );
                childCount--;
            }
            else
                break;
        }
    }

    void RemoveChildrenColliders( GameObject childObject )
    {
        if ( childObject.GetComponent<Collider>() )
        {
            int colliderCount = childObject.GetComponents<Collider>().Length;
            for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
                childObject.GetComponent<Collider>().enabled = false;
        }


        if ( childObject.GetComponent<Rigidbody>() )
        {
            childObject.GetComponent<Rigidbody>().isKinematic = false;
            childObject.GetComponent<Rigidbody>().useGravity = true;
        }

        if ( childObject.transform.childCount > 0 )
            foreach( Transform child in childObject.transform )
                RemoveChildrenColliders( child.gameObject );
    }
    
    void DeleteObjects()
    {
        for ( int trashIndex = 0; trashIndex < 50; trashIndex++ )
        {
            if ( transform.childCount > 1 )
            {
                if ( transform.GetChild( 1 ) != null )
                    Destroy( transform.GetChild( 1 ).gameObject );
            }
            else
                break;
        }

    }

    void DeleteAllObjects()
    {
        deletingEverything = true;

        trashObjects.Clear();
        fallenObjects.Clear();
        int childCount = transform.childCount;
        for ( int trashIndex = 0; trashIndex < childCount; trashIndex++ )
        {
            if( transform.GetChild( 0 ).gameObject != null )
                Destroy( transform.GetChild( 0 ).gameObject );
        }

        deletingEverything = false;
    }

    public void AddToTrash( GameObject trashObject )
    {
        trashObjects.Add( trashObject );
        fallenObjects.Add( trashObject );
        trashObject.transform.parent = transform;

        if ( trashObject.GetComponent<Rigidbody>() )
        {
            trashObject.GetComponent<Rigidbody>().isKinematic = false;
            trashObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
