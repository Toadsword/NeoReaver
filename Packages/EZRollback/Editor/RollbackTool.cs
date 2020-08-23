using System;
using System.IO;
using Packages.EZRollback.Editor.Utils;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Packages.EZRollback.Editor {
        
    public class RollbackTool : EditorWindow {
        
        const int FIRST_FRAME_NUM = 1;
        
        RollbackManager _rollbackManager;
        RollbackInputBaseActions _rbBaseInput;

        int _numOfInputs = 1;
        int _numFramesToSimulate = 0;
        
        int _controllerId = 0;

        Vector2 _scrollPos = Vector2.zero;

        System.TimeSpan _spentTimeToResimulate;

        EditorWindow _window;
        
        [MenuItem("RollbackTool/Rollback tool")]
        public static void ShowWindow() {
            GetWindow(typeof(RollbackTool));
        }
        
        RollbackTool() {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        void OnGUI() {
            if (_window == null) {
                _window = GetWindow(typeof(RollbackTool));
            }
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(_window.position.width), GUILayout.Height(_window.position.height));
        
            DisplayRollbackEditionButtons();

            if (UnityEditor.EditorApplication.isPlaying && _rollbackManager != null) {
                GUIUtils.GuiLine(3);
                DisplayRollbackInformation();

                GUIUtils.GuiLine(3);
                DisplaySimulateOptions();

                if (_rollbackManager.GetRBInputManager() != null) {
                    GUIUtils.GuiLine(3);
                    DisplayInputSimulation();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        /**
         * \brief Called when changing Unity play mode state.
         */
        private void LogPlayModeState(PlayModeStateChange playModeStateChange) {
            switch (playModeStateChange) {
                case PlayModeStateChange.EnteredPlayMode:
                    _rollbackManager = RollbackManager.Instance;
                    _rbBaseInput = new RollbackInputBaseActions(1 + _numOfInputs / 8);
                    break;
            }
        }

        private void DisplayRollbackEditionButtons() {
            
            
            EditorGUILayout.BeginHorizontal();

            //First frame
            if (GUILayout.Button("<=", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(FIRST_FRAME_NUM, false);
                UnityEditor.EditorApplication.isPaused = true;
            }
            
            //Frame before actual one
            if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetDisplayedFrameNum() - 1, false);
                UnityEditor.EditorApplication.isPaused = true;
            }

            //Play/Stop shortcut
            if (UnityEditor.EditorApplication.isPlaying){
                if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            } else {
                if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPlaying = true;
                }
            }

            //Pause/Resume shortcut
            if (UnityEditor.EditorApplication.isPaused) {
                if (GUILayout.Button("Resume", GUILayout.Width(100), GUILayout.Height(20))) {
                    _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetDisplayedFrameNum(), true);
                    UnityEditor.EditorApplication.isPaused = false;
                }
            } else {
                if (GUILayout.Button("Pause", GUILayout.Width(100), GUILayout.Height(20))) {
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }

            //Load frame after actual one
            if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetDisplayedFrameNum() + 1, false);
            }
            
            //Load last registered frame
            if (GUILayout.Button("=>", GUILayout.Width(30), GUILayout.Height(20))) {
                _rollbackManager.SetValueFromFrameNumber(_rollbackManager.GetMaxFramesNum() - 1, false);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DisplayRollbackInformation() {
            GUILayout.Label("Rollback options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Current Frame", GUILayout.Width(100));
            int newFrameNum = (int) GUILayout.HorizontalSlider(_rollbackManager.GetDisplayedFrameNum(), FIRST_FRAME_NUM, _rollbackManager.GetMaxFramesNum());

            if (newFrameNum != _rollbackManager.GetDisplayedFrameNum()) {
                _rollbackManager.SetValueFromFrameNumber(newFrameNum, false);
                UnityEditor.EditorApplication.isPaused = true;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Current frame number : " + (_rollbackManager.GetDisplayedFrameNum()) + " / " + (_rollbackManager.GetMaxFramesNum()));
        }

        private void DisplaySimulateOptions() {
            GUILayout.Label("Simulations options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            _numFramesToSimulate = EditorGUILayout.IntField("Num frames to simulate : ", _numFramesToSimulate);
            if (_numFramesToSimulate < 0)
                _numFramesToSimulate = 0;
            if (GUILayout.Button("Simulate !")) {
                if (_rollbackManager != null) {
                    _rollbackManager.Simulate(_numFramesToSimulate);
                }
            }
            
            if (GUILayout.Button("Go back x frames")) {
                if (_rollbackManager != null) {
                    _rollbackManager.GoBackInFrames(_numFramesToSimulate, false, false);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayInputSimulation() {
            
            GUILayout.Label("Input simulations options", EditorStyles.boldLabel);

            EditorGUILayout.IntField("ControllerId : ", _controllerId);
            
            //Vertical axis input
            float verticalValue = _rbBaseInput.GetVerticalAxis();
            verticalValue = EditorGUILayout.Slider("Vertical axis", verticalValue, -1f, 1f);
            _rbBaseInput.SetVerticalAxis(verticalValue);
            
            //Vertical axis input
            float horizontalValue = _rbBaseInput.GetHorizontalAxis();
            horizontalValue = EditorGUILayout.Slider("Horizontal axis", horizontalValue, -1f, 1f);
            _rbBaseInput.SetHorizontalAxis(horizontalValue);

            //Button inputs
            GUILayout.Label("Buttons press options : ", EditorStyles.boldLabel);
            int oldNumOfInputs = _numOfInputs;
            _numOfInputs = EditorGUILayout.IntField("NumOfInputs : ", _numOfInputs);
            
            if (_numOfInputs != oldNumOfInputs) {
                _rbBaseInput = new RollbackInputBaseActions(1 + _numOfInputs / 8);
            }
            
            for (int i = 0; i < _numOfInputs; i++) {
                EditorGUILayout.BeginHorizontal();
                bool initValue = _rbBaseInput.GetValueBit(i);
                initValue = EditorGUILayout.Toggle(_rollbackManager.GetRBInputManager().GetActionName(i), initValue);
                _rbBaseInput.SetOrClearBit(i, initValue);
                EditorGUILayout.EndHorizontal();
            }
            
            //Correction of inputs
            if (GUILayout.Button("Correct Inputs")) {
                if (_rollbackManager != null) {
                    
                    RollbackInputBaseActions[] rbInputs = new RollbackInputBaseActions[_numFramesToSimulate];
                    for (int i = 0; i < _numFramesToSimulate; i++) {
                        rbInputs[i] = _rbBaseInput;
                    }
                    
                    DateTime currentTime = System.DateTime.Now;
                    _rollbackManager.GetRBInputManager().CorrectInputs(_controllerId, _numFramesToSimulate, rbInputs);
                    _rollbackManager.ReSimulate(_numFramesToSimulate);
                    _spentTimeToResimulate = System.DateTime.Now - currentTime;
                }
            }

            if (_spentTimeToResimulate != null) {
                GUILayout.Label("Time to resimulate " + _numFramesToSimulate + " frames : " + _spentTimeToResimulate.TotalMilliseconds + "ms.");
            }
        }
    }
}
