using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MyClient : MonoBehaviour {
    Logic _logic;

    string _serverAdress = "";
    string _appId = "6bf6b4e7-b37c-40e2-8548-11f41fbf3ae0";
    string _gameVersion = "0.1.0";

    [SerializeField] GameObject _connectionPanel;
    [SerializeField] GameObject _lobbyPanel;

    [SerializeField] Text _serverAddressInput;
    [SerializeField] Text _appIdInput;
    [SerializeField] Text _gameVersionInput;

    private Timer inputRepeatTimer;
    
    // Start is called before the first frame update
    void Start() {
        _lobbyPanel.SetActive(false);
        inputRepeatTimer = new Timer(10);
    }

    public void ConnectGame() {
        _logic = new Logic();
        _logic.StartGame(_serverAddressInput.text.ToString(), _appIdInput.text.ToString(), _gameVersionInput.text.ToString());
        _connectionPanel.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_logic != null) {
            _logic.UpdateLocal();

            if (_logic.LocalPlayerJoined())
            {
                RenderPlayers();
                InputForControlledCube();
            }
        }
    }

    void OnGUI() {
        if (_logic != null){
            GUI.Label(new Rect(10, 10, 300, 50), this._logic.localPlayer.State.ToString());
        }
    }
    
    /// <summary>
    /// Convert integer value to Color
    /// </summary>
    public static Color IntToColor(int colorValue)
    {
        float r = (byte)(colorValue >> 16) / 255.0f;
        float g = (byte)(colorValue >> 8) / 255.0f;
        float b = (byte)colorValue / 255.0f;

        return new Color(r, g, b);
    }

    private void InputForControlledCube()
    {
        if (!inputRepeatTimer.ShouldExecute)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        if (x < -0.2f)
        {
            _logic.localPlayer.LocalPlayer.PosX -= 1;
            inputRepeatTimer.Reset();
        }
        else if (x > 0.2f)
        {
            _logic.localPlayer.LocalPlayer.PosX += 1;
            inputRepeatTimer.Reset();
        }

        float y = Input.GetAxis("Vertical");
        if (y < -0.2f)
        {
            _logic.localPlayer.LocalPlayer.PosY -= 1;
            inputRepeatTimer.Reset();
        }
        else if (y > 0.2f)
        {
            _logic.localPlayer.LocalPlayer.PosY += 1;
            inputRepeatTimer.Reset();
        }

        this._logic.localPlayer.LocalPlayer.ClampPosition();
    }
    
    /// <summary>
    /// Render cubes onto the scene
    /// </summary>
    void RenderPlayers()
    {
        lock (_logic.localPlayer)
        {
            foreach (CustomPlayer p in _logic.localPlayer.LocalRoom.Players.Values)
            {
                foreach (GameObject cube in _logic.cubes)
                {
                    if (cube.name == p.NickName)
                    {
                        float alpha = 1.0f;
                        if (!p.IsLocal && p.UpdateAge > 500)
                        {
                            cube.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
                            alpha = (p.UpdateAge > 1000) ? 0.3f : 0.8f;
                        }
                        
                        Color cubeColor = IntToColor(p.Color);
                        cube.GetComponent<Renderer>().material.color = new Color(cubeColor.r, cubeColor.g, cubeColor.b, alpha);
                        cube.transform.position = new Vector3(p.PosX / 10f, p.PosY/ 10f, 0);
                        break;
                    }
                }
            }
        }
    }
}
