﻿using System.Collections.Generic;
using ExitGames.Client.Photon;
using global::Photon.Realtime;
using Packages.EZRollback.Runtime.Scripts;
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
    public int Color { get; set; }
    
    
    // Implement current player input and send it through network
    //public List<FrameInput> inputHistory;

    private int LastUpdateFrame { get; set; }

    /// <summary>
    /// Stores this client's "group interest currently set on server" of this player (not necessarily the current one).
    /// </summary>
    public byte VisibleGroup { get; set; }

    public CustomPlayer(string nickName, int actorID, bool isLocal, Hashtable actorProperties) : base(nickName, actorID, isLocal, actorProperties)
    {
        Debug.Log(nickName + " - " + actorID + " - " + isLocal);
        if (isLocal)
        {
            // we pick a random color when we create a local player
            this.RandomizeColor();
        }
    }

    /// <summary>Creates a random color by generating a new integer and setting the highest byte to 255.</summary>
    /// <remarks>RGB colors can be represented as integer (3 bytes, ignoring the first which represents alpha).</remarks>
    internal void RandomizeColor()
    {
        this.Color = (int)((uint)SupportClass.ThreadSafeRandom.Next() | 0xFF000000);
    }

    public void ReadEvInputChange(Hashtable evContent) {
        int bufferSize = 1;
        // Know the buffer size
        if (evContent.ContainsKey((byte) 1)) {
            bufferSize = (int)evContent[(byte)1];
        }
        if (evContent.ContainsKey((byte) 2)) {
            //FrameInput[] inputs = (FrameInput[])evContent[(byte)2];

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
    public Hashtable WriteEvInput()
    {
        Hashtable evContent = new Hashtable();
        int currentFrame = RollbackManager.Instance.GetDisplayedFrameNum();
        int numFramesToSend = CustomConstants.NetworkBufferSize;
        if (numFramesToSend > currentFrame) {
            numFramesToSend = currentFrame - 1;
        }
        
        
        evContent[0] = numFramesToSend; // Last x frames to pass through the
        evContent[1] = currentFrame; // Current frame
        for (int i = 2; i < numFramesToSend + 2; i++) {
            
        }
        return evContent;
    }

    /// <summary>Reads the "custom content" Hashtable that is sent as position update.</summary>
    /// <returns>Hashtable for event "move" to update others</returns>
    public void ReadEvInput(Hashtable evContent)
    {
        if (evContent.ContainsKey((int)0))
        {
            
        }

        if (evContent.ContainsKey((int) 1)) {
            //Debug.Log("Recieved inputs for frame : #" + evContent[1]);
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

    public Hashtable WriteEvPosition() {
        Hashtable evContent = new Hashtable();
        return evContent;
    }
    
    public void ReadEvPosition(Hashtable evContent)
    {
        this.LastUpdateFrame = GameLogic.Timestamp;
    }
    
    /// <summary>
    /// Converts the player info into a string.
    /// </summary>
    /// <returns>String showing basic info about this player.</returns>
    public override string ToString()
    {
        return this.ActorNumber + "'" + this.NickName + "':" + " " + " PlayerProps: " + SupportClass.DictionaryToString(this.CustomProperties);
    }
}
