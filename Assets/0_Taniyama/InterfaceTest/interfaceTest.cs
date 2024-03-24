using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface move1
{
    Player character { get;}

    public virtual void move()
    {
        
    }
}

public interface move2
{
    public Player character { get; }

    public virtual void move()
    {

    }
}

public class move : Player, move1, move2
{
    Player move1.character { get => this; }
    Player move2.character { get => this; }
}
