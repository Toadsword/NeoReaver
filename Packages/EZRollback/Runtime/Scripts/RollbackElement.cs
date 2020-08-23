using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {

/*
 * \brief Element to rollback that takes any type of data and handles the storage of the history of its value through frames.
 * THe data are stored in the form of a closed list.
 */
[Serializable]
public class RollbackElement<T> {
    const int DEFAULT_SIZE = 51; // Default array size
    
    [SerializeField] T[] _history; // History through frames of the value

    [SerializeField] public T value; // Current value of the frame
    
    [SerializeField] int _head = 0;
    [SerializeField] int _tail = 0;
    [SerializeField] int _size = 0;
    
    public RollbackElement(T initValue = default, int baseSize = DEFAULT_SIZE) {
        value = initValue;
        _history = new T[baseSize];

        Clear();
    }
    
    public void Clear() {
        _head = 0;
        _tail = 0;
        _size = 0;
    }
    
    /**
     * \brief Get the value stored at the requested frame number.
     * If the frame number is higher than the size or below 0, returns the current value instead
     * \param frameNum Number of the frame requested
     */
    public T GetValue(int frameNum) {
        if (frameNum <= _size && frameNum > 0) {
            if (frameNum == _size) {
                frameNum = _size - 1;
            }
            return _history[GetCorrectFrameNumber(frameNum)];
        }

        return value;
    }

    /**
     * \brief Set and save in the history the value passed in parameters
     * \param newValue 
     */
    public void SetAndSaveValue(T newValue) {
        value = newValue;
        SaveFrame();
    }

    /**
     * \brief Set a new value to a specific frame number. Used to correct the value due to a probable mistake in the values (notably in inputs with networking)
     * \param correctedValue New value
     * \param frameNum Frame number to correct 
     */
    public bool CorrectValue(T correctedValue, int frameNum) {
        if (frameNum <= _size && frameNum > 0) {
            if (!_history[GetCorrectFrameNumber(frameNum)].Equals(correctedValue)) {
                _history[GetCorrectFrameNumber(frameNum)] = correctedValue;
                return true;
            }
        }

        return false;
    }

    /**
     * \brief Save the current value in the history
     */
    public void SaveFrame() {
        _history[_head] = value;

        _head++;
        _size++;
        
        CheckHistorySize();
    }

    /**
     * \brief Restore the current value to an anterior frame.
     */
    public void SetValueFromFrameNumber(int frameNum) {
        if (1 > frameNum) {
            //Debug.LogWarning("Cannot go back from higher number of registered frames. Returning last");
            frameNum = 1;
        }

        if (frameNum == _size) {
            frameNum--;
        }
        
        value = GetValue(frameNum - 1);
    }
    
    /**
     * \brief Delete frames in the history.
     * \param numFramesToDelete number of frames to delete
     * \param deleteMode Deletion mode
     */
    public void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {

        if (deleteMode == RollbackManager.DeleteFrameMode.FIT_TO_BUFFER) {
            if (_size >= RollbackManager.Instance.bufferSize) {
                deleteMode = RollbackManager.DeleteFrameMode.FIRST_FRAMES;
            } else {
                return;
            }
        }
        
        switch (deleteMode) {
            case RollbackManager.DeleteFrameMode.FIRST_FRAMES:
                _tail += numFramesToDelete;
                _tail = _tail % _history.Length;
                break;
            case RollbackManager.DeleteFrameMode.LAST_FRAMES:
                _head -= numFramesToDelete;
                if (_head < 0) {
                    _head += _history.Length;
                }
                break;
        }
        
        _size -= numFramesToDelete;

        //That means we deleted all the frames
        if (_size <= 0) {
            _size = 0;
            _head = 0;
            _tail = 0;
        }
    }

    
    /**
     * \brief Check the size of the history. If the needs are higher than the capacity, we resize the history
     */
    private void CheckHistorySize() {
        _head = _head % _history.Length;

        if (_head == _tail && _size == _history.Length) {
            Resize(_history.Length * 2);
        }
    }

    /**
     * \brief Resize the closed list History to a new size.
     * \param newSize New size wanted of the array
     */
    private void Resize(int newSize) {
        int currentSize = _history.Length;
        
        T[] old_history = _history;
        _history = new T[newSize];

        //For each element of the previous buffer
        for (int i = 0; i < _size; i++) {
            _history[i] = old_history[( _tail + i) % currentSize];
        }

        _tail = 0;
        _head = currentSize;
        _size = currentSize;
    }


    private int GetCorrectFrameNumber(int frameNumber) {
        return (_tail + frameNumber) % _history.Length;
    }
}
}
