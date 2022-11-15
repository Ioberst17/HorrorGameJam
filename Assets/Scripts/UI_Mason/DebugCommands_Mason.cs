using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


//create a class for debug commands and what parameters are important to have for each command
public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;

    public string commandId { get { return _commandId; } }
    public string commandDescription { get { return _commandDescription; } }
    public string commandFormat { get { return _commandFormat; } }

    public DebugCommandBase(string id, string description, string format)
    {
        _commandId = id;
        _commandDescription = description;
        _commandFormat = format;
    }

}


public class DebugCommands_Mason : DebugCommandBase
{
    //the Action is defined in DebugController_Mason it is what each command actually does. follows the () => when defining each command.
    //we create a local Action called command to use later in Invoke.
    private Action command;

    public DebugCommands_Mason(string id, string description, string format, Action command) : base (id, description, format)
    {
        this.command = command; //set local command Action to whatever action is passed in by way of the commandlist
    }

    //run/invoke the action of the command.
    public void Invoke()
    {
        command.Invoke();
    }

}

public class DebugCommands_Mason<T1> : DebugCommandBase
{
    //the Action is defined in DebugController_Mason it is what each command actually does. follows the () => when defining each command.
    //we create a local Action called command to use later in Invoke.
    private Action<T1> command;

    public DebugCommands_Mason(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
        this.command = command; //set local command Action to whatever action is passed in by way of the commandlist
    }

    //run/invoke the action of the command.
    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }

}
