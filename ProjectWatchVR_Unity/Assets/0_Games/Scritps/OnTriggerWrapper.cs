using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerWrapper : MonoBehaviour
{
    public MonoBehaviour target;
    ITrigger _trigger;
    ITrigger trigger
    {
        get
        {
            if (_trigger == null)
            {
                _trigger = target.GetComponent<ITrigger>();
            }
            return _trigger;
        }
    }

    GameObject _gameObject;
    GameObject mGameObject
    {
        get
        {
            if (_gameObject == null)
            {
                _gameObject = gameObject;
            }
            return _gameObject;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        trigger.OnWrapperTriggerEnter(mGameObject, other);
    }

    void OnTriggerExit(Collider other)
    {
        trigger.OnWrapperTriggerExit(mGameObject, other);
    }

    void OnTriggerStay(Collider other)
    {
        trigger.OnWrapperTriggerStay(mGameObject, other);
    }
}
