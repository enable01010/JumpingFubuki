using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface move1
{
    Character character { get;}

    public virtual void move()
    {
        
    }
}

public interface move2
{
    public Character character { get; }

    public virtual void move()
    {

    }
}

public class move : Character, move1, move2
{
    Character move1.character { get => this; }
    Character move2.character { get => this; }
}
