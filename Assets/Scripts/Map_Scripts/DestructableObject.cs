using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    //////////////////////////////
    // VARIABLES
    //////////////////////////////

    public int healthPerStage;
    int currentHealth;
    int timesHit = 0;

    bool hasStages = false;
    [HideInInspector]
    public bool hasExploded = false;

    public List<DestructableObject> destructableChildren = new List<DestructableObject>();
    public List<DestructableObject> destructableParents = new List<DestructableObject>();
    public int parentsKilledNeededToCollapse = 1;

    public bool overrideColorOn;
    public Color overrideColor;

    //////////////////////////////
    // OBJECTS AND COMPONENTS
    //////////////////////////////

    List<GameObject> stages = new List<GameObject>();

    TrashCollector trashCollector;

    List<Transform> children = new List<Transform>();

    private void Start()
    {
        currentHealth = healthPerStage;
        trashCollector = GameObject.Find( "Coding_Trash" ).GetComponent<TrashCollector>();

        FindMovableChildren( transform );

        if ( transform.Find( "Stages" ) )
        {
            hasStages = true;
            foreach(Transform trans in transform.Find( "Stages" ) )
            {
                stages.Add( trans.gameObject );
                trans.gameObject.SetActive( false );
            }

            stages[ 0 ].SetActive( true );
        }
    }

    public void Explode()
    {
        if ( hasExploded == true )
            return;

        if ( hasStages == true )
        {
            currentHealth--;
            if ( currentHealth <= 0 )
            {
                timesHit++;
                currentHealth = healthPerStage;
            }
            else
                return;

            
            if ( timesHit < stages.Count )
            {
                stages[ timesHit - 1].SetActive( false );
                stages[ timesHit ].SetActive( true );
            }
            else
            {
                hasExploded = true;
                KillParent( stages[ timesHit - 1 ].transform );

                foreach ( DestructableObject obj in destructableChildren )
                    if( obj.hasExploded == false )
                        obj.InstaExplode();

                foreach ( Transform trans in children )
                    if( trans != null )
                        if( trans.parent.gameObject.activeSelf == true )
                            trashCollector.AddToTrash( trans.gameObject );
            }
        }
        else
        {
            currentHealth--;
            if ( currentHealth > 0 )
                return;

            hasExploded = true;
            foreach ( Transform trans in children )
                if( trans != null)
                    trashCollector.AddToTrash( trans.gameObject );

            KillParent( transform );
        }
    }

    // Instantly kill object
    void InstaExplode()
    {
        hasExploded = true;

        if ( hasStages == true )
        {
            if( timesHit < stages.Count )
                stages[ timesHit ].SetActive( false );
            timesHit = stages.Count - 1;
            stages[ timesHit ].SetActive( true );
            KillParent( stages[ timesHit ].transform );
        }


        foreach ( DestructableObject obj in destructableChildren )
            if ( obj.hasExploded == false )
                obj.InstaExplode();

        foreach ( Transform trans in children )
            if ( trans != null )
                if ( trans.parent.gameObject.activeSelf == true )
                    trashCollector.AddToTrash( trans.gameObject );
    }

    //Add rigidbody to children and make them explode
    void KillParent( Transform parent )
    {
        foreach ( Transform trans in parent )
        {

            if ( trans.GetComponent<MeshRenderer>() )
            {
                if ( trans.GetComponent<Rigidbody>() == false )
                    trans.gameObject.AddComponent<Rigidbody>();

                Rigidbody transRB = trans.GetComponent<Rigidbody>();

                transRB.useGravity = true;
                transRB.isKinematic = false;
                trans.GetComponent<Rigidbody>().AddForce( trans.up * 2.0f, ForceMode.Impulse );
            }

            if ( trans.childCount >= 1 )
                KillParent( trans );
        }
    }

    void FindMovableChildren(Transform parent)
    {
        foreach ( Transform trans in parent )
        {
            if( overrideColorOn == true && trans.GetComponent<MeshRenderer>() )
                trans.GetComponent<MeshRenderer>().material.color = overrideColor;

            if ( trans.GetComponent<Rigidbody>() && trans.name.Contains("&&") )
            {
                children.Add( trans );
            }

            if ( trans.childCount >= 1 )
                FindMovableChildren( trans );
        }
    }
}
