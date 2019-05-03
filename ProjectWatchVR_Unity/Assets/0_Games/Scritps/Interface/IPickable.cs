using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    void OnPickStart(Transform pickPointer);
    void OnPickFinish();
    void OnPointEnter();
    void OnPointOut();
    Transform GetTransform();
}
