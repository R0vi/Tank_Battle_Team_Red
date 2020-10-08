using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is adapted and modified from the FSM implementation class available on UnifyCommunity website
/// The license for the code is Creative Commons Attribution Share Alike.
/// It's originally the port of C++ FSM implementation mentioned in Chapter01 of Game Programming Gems 1
/// You're free to use, modify and distribute the code in any projects including commercial ones.
/// Please read the link to know more about CCA license http://creativecommons.org/licenses/by-sa/3.0/
/// </summary>

public enum Transition
{
    None = 0,
    SawPlayer,
    ReachPlayer,
    LostPlayer,
    NoHealth,
}

public enum FSMStateIDTeamRed
{
    None = 0,
    Patrolling,
    Chasing,
    Attacking,
    Dead,
}

public class AdvancedFsmTeamRed : FSMTeamRed 
{
    private List<FSMStateTeamRed> fsmStates;

    //The fsmStates are not changing directly but updated by using transitions
    private FSMStateIDTeamRed currentStateIdTeamRed;
    public FSMStateIDTeamRed CurrentStateIdTeamRed { get { return currentStateIdTeamRed; } }

    private FSMStateTeamRed currentStateTeamRed;
    public FSMStateTeamRed CurrentStateTeamRed { get { return currentStateTeamRed; } }

    public AdvancedFsmTeamRed()
    {
        fsmStates = new List<FSMStateTeamRed>();
    }

    /// <summary>
    /// Add New State into the list
    /// </summary>
    public void AddFSMStateTeamRed(FSMStateTeamRed fsmStateTeamRed)
    {
        // Check for Null reference before deleting
        if (fsmStateTeamRed == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        // First State inserted is also the Initial state
        //   the state the machine is in when the simulation begins
        if (fsmStates.Count == 0)
        {
            fsmStates.Add(fsmStateTeamRed);
            currentStateTeamRed = fsmStateTeamRed;
            currentStateIdTeamRed = fsmStateTeamRed.IdTeamRed;
            return;
        }

        // Add the state to the List if it´s not inside it
        foreach (FSMStateTeamRed state in fsmStates)
        {
            if (state.IdTeamRed == fsmStateTeamRed.IdTeamRed)
            {
                Debug.LogError("FSM ERROR: Trying to add a state that was already inside the list");
                return;
            }
        }

        //If no state in the current then add the state to the list
        fsmStates.Add(fsmStateTeamRed);
    }

    /// <summary>
    /// This method delete a state from the FSM List if it exists, 
    ///   or prints an ERROR message if the state was not on the List.
    /// </summary>
    public void DeleteStateTeamRed(FSMStateIDTeamRed fsmState)
    {
        // Check for NullState before deleting
        if (fsmState == FSMStateIDTeamRed.None)
        {
            Debug.LogError("FSM ERROR: bull id is not allowed");
            return;
        }

        // Search the List and delete the state if it´s inside it
        foreach (FSMStateTeamRed state in fsmStates)
        {
            if (state.IdTeamRed == fsmState)
            {
                fsmStates.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: The state passed was not on the list. Impossible to delete it");
    }

    /// <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn´t have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
    public void PerformTransitionTeamRed(Transition trans)
    {
        // Check for NullTransition before changing the current state
        if (trans == Transition.None)
        {
            Debug.LogError("FSM ERROR: Null transition is not allowed");
            return;
        }

        // Check if the currentState has the transition passed as argument
        FSMStateIDTeamRed idTeamRed = currentStateTeamRed.GetOutputStateTeamRed(trans);
        if (idTeamRed == FSMStateIDTeamRed.None)
        {
            Debug.LogError("FSM ERROR: Current State does not have a target state for this transition");
            return;
        }

        // Update the currentStateID and currentState		
        currentStateIdTeamRed = idTeamRed;
        foreach (FSMStateTeamRed state in fsmStates)
        {
            if (state.IdTeamRed == currentStateIdTeamRed)
            {
                currentStateTeamRed = state;
                break;
            }
        }
    }
}
