﻿/*
 * Authors: Isha Afzaal, Sammy Elrafih, Ainslie Veltheon
 * Info: CellBehavior.cs specifies agent behaviour in the quorum-sensing system of l. pneumophila.
 *       Cells use quorum-sensing to understand their population's cell density. Once the cell density passes a
 *       certain threshold, agents start exhibiting emergent behaviour.
 * References:
 *  Using timers in Unity: https://answers.unity.com/questions/1453479/how-to-slow-down-random-enemy-spawn.html
 *  Making objects spawn other objects in Unity: https://answers.unity.com/questions/420177/how-do-i-make-an-object-spawn-another-one.html
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{

    // Cell Reproduction Configuration
    private int reproduction_limit = 5;
    private int cells_reproduced = 0;

    // Quorum Sensing (QS) Configuration
    private int radius = 1;
    private int threshold_value = 5;

    // Cell Movement/Life and Death Control
    private Rigidbody physicsBody;
    Vector3 force;
    private bool quorum_sensing_switch = false;
    private float target_time = 5.0f;
    private int energy = 4000;
   
    // Can this constructor be removed?
    public void constructor()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        physicsBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Make the cell move
        force = new Vector3 (Random.Range(-10,10),-0.1f,Random.Range(-10,10));
        physicsBody.AddForce(force/500);

        quorum_sensing();
        consume_energy();
    }

    /*
     * Specify the emergent behavior the cells should have here. For now, will simply change cell colours
     */
    private void emergent_behavior ()
    {
        // Change the current agent cell's colour
        GetComponent<Renderer>().material.color = Color.red;
        quorum_sensing_switch = true;
    }

    /*
     * Adapted from Sammy's cellSpawner.cs
     */
    private void createCell(Vector3 spawn_location)
    {
        var go = GameObject.Find("Cell(Clone)");
        if (go != null)
        {
            Instantiate(go, spawn_location, Quaternion.identity);
        }
    }

    /*
     * Quorum-sensing: Agents detect cell density and exhibit emergent behavior if threshold reached or
     * reproduce if threshold not reached
     */
    private void quorum_sensing ()
    {
        // QS Step 1: Sense the number of surrounding bacteria
        Collider[] nearby_objects = Physics.OverlapSphere(transform.position, radius);

        // QS Step 2: Check surrounding population cell density
        if (!quorum_sensing_switch)
        {
            // Case 1: Cell density has reached the threshold value
            if (nearby_objects.Length >= threshold_value)
            {
                Debug.Log("Activating emergent behavior");
                emergent_behavior();
            }

            // Case 2: Cell density lower than threshold value - reproduce a new cell
            else
            {
                // Wait for some time before spawning
                target_time -= Time.deltaTime;
                if (target_time <= 0.0f)
                {
                    target_time = 5.0f;
                    Debug.Log("Creating New Bacteria.");

                    if (cells_reproduced < reproduction_limit)
                    {
                        // Create new game object
                        createCell(transform.position);
                        cells_reproduced++;
                        energy = energy - 10;
                    }
                }
            }
        }
    }

    /*
     * As the cells move, they consume energy and will die upon its depletion
     */
    private void consume_energy ()
    {
        Debug.Log("Consuming Energy.");
        energy = energy - 1;

        if (globals.total_agar > 2)
        {
            energy = energy + 2; Debug.Log("2");
            globals.total_agar = globals.total_agar - 2;
        }

        // QS Step 3: Cells die upon having no energy
        if (energy <= 0)
        {
            Destroy(gameObject);
        }
    }
}
