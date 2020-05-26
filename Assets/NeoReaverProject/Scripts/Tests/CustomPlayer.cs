using System.Collections.Generic;
using ExitGames.Client.Photon;
using global::Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Extends Player with some Particle Demo specific properties and methods.
/// </summary>
/// <remarks>
/// Instances of this class are created by GameLogic.CreatePlayer.
/// There's a GameLogic.LocalPlayer field, that represents this user's player (in the room).
///
/// This class does not make use of networking directly. It's updated by incoming events but
/// the actual sending and receiving is handled in GameLogic.
///
/// The WriteEv* and ReadEv* methods are simple ways to create event-content per player.
/// Only the LocalPlayer per client will actually send data. This is used to update the remote
/// clients of position (and color, etc).
/// Receiving clients identify the corresponding Player and call ReadEv* to update that
/// (remote) player.
/// Read the remarks in WriteEvMove.
/// </remarks>
public class CustomPlayer : Player
{
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Color { get; set; }
    
    // Implement current player input and send it through network
    public List<FrameInput> inputHistory;

    private int LastUpdateFrame { get; set; }

    /// <summary>
    /// Stores this client's "group interest currently set on server" of this player (not necessarily the current one).
    /// </summary>
    public byte VisibleGroup { get; set; }

    public CustomPlayer(string nickName, int actorID, bool isLocal, Hashtable actorProperties) : base(nickName, actorID, isLocal, actorProperties)
    {
        if (isLocal)
        {
            // we pick a random color when we create a local player
            this.RandomizeColor();
            this.RandomizePosition();
        }
    }

    /// <summary>Creates a random color by generating a new integer and setting the highest byte to 255.</summary>
    /// <remarks>RGB colors can be represented as integer (3 bytes, ignoring the first which represents alpha).</remarks>
    internal void RandomizeColor()
    {
        this.Color = (int)((uint)SupportClass.ThreadSafeRandom.Next() | 0xFF000000);
    }

    /// <summary>Randomizes position within the gridSize.</summary>
    internal void RandomizePosition()
    {
        int gridSize = 16;

        this.PosX = SupportClass.ThreadSafeRandom.Next() % gridSize;
        this.PosY = SupportClass.ThreadSafeRandom.Next() % gridSize;
    }

/*
    public Hashtable WriteEvInputChange() {
        Hashtable evContent = new Hashtable();
        evContent[(int)1] = 1;    
        evContent[(int)2] = new FrameInput(, 0.5f, 0.4f);
        return evContent;
    }
    */
    public void ReadEvInputChange(Hashtable evContent) {
        int bufferSize = 1;
        // Know the buffer size
        if (evContent.ContainsKey((byte) 1)) {
            bufferSize = (int)evContent[(byte)1];
        }
        if (evContent.ContainsKey((byte) 2)) {
            FrameInput[] inputs = (FrameInput[])evContent[(byte)2];

        }
        
        //Debug.Log("Update from  : " + this.LastUpdateTimestamp + " to : " + GameLogic.Timestamp);
        this.LastUpdateFrame = GameLogic.Timestamp;
    }
    
    /// <summary>Creates the "custom content" Hashtable that is sent as position update.</summary>
    /// <remarks>
    /// As with event codes, the content of this event is arbitrary and "made up" for this demo.
    /// Your game (e.g.) could use floats as positions or you send a height and actions or state info.
    /// It makes sense to use numbers (best: bytes) as Hashtable key type, cause they are use less space.
    /// But this is not a requirement as you see in WriteEvColor.
    ///
    /// The position can only go up to 128 in this demo, so a byte[] technically is the best (leanest)
    /// choice here.
    /// </remarks>
    /// <returns>Hashtable for event "move" to update others</returns>
    public Hashtable WriteEvMove()
    {
        Hashtable evContent = new Hashtable();
        evContent[(int)1] = new int[] { this.PosX, this.PosY };
        return evContent;
    }

    /// <summary>Reads the "custom content" Hashtable that is sent as position update.</summary>
    /// <returns>Hashtable for event "move" to update others</returns>
    public void ReadEvMove(Hashtable evContent)
    {
        if (evContent.ContainsKey((int)1))
        {
            int[] posArray = (int[])evContent[(int)1];
            this.PosX = posArray[0];
            this.PosY = posArray[1];
        }
        else if (evContent.ContainsKey("1"))
        {
            // js client event support (those can't send with byte-keys)
            var posArray = (object[])evContent["1"];   // NOTE: this is subject to change while we update the serialization in JS/Server
            this.PosX = System.Convert.ToInt32(posArray[0]);
            this.PosY = System.Convert.ToInt32(posArray[1]);
        }
        
        //Debug.Log("Update from  : " + this.LastUpdateTimestamp + " to : " + GameLogic.Timestamp);
        this.LastUpdateFrame = GameLogic.Timestamp;
    }

    /// <summary>Creates the "custom content" Hashtable that is sent as color update.</summary>
    /// <returns>Hashtable for event "color" to update others</returns>
    public Hashtable WriteEvColor()
    {
        Hashtable evContent = new Hashtable();
        evContent[(byte)1] = this.Color;
        return evContent;
    }

    /// <summary>Reads the "custom content" Hashtable that is sent as color update.</summary>
    /// <returns>Hashtable for event "color" to update others</returns>
    public void ReadEvColor(Hashtable evContent)
    {
        if (evContent.ContainsKey((byte)1))
        {
            this.Color = (int)evContent[(byte)1];
        }
        else if (evContent.ContainsKey("1"))
		{
            // js client event support (those can't send with byte-keys)
    		this.Color = System.Convert.ToInt32(evContent["1"]);
        }
        this.LastUpdateFrame = GameLogic.Timestamp;
    }
    
    /// <summary>
    /// Converts the player info into a string.
    /// </summary>
    /// <returns>String showing basic info about this player.</returns>
    public override string ToString()
    {
        return this.ActorNumber + "'" + this.NickName + "':" + " " + this.PosX + ":" + this.PosY + " PlayerProps: " + SupportClass.DictionaryToString(this.CustomProperties);
    }
}
