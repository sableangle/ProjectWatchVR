// DropTriggerScript
using UnityEngine;

public class DropTriggerScript : MonoBehaviour
{
	public bool isGrabbed;

	public bool is_kinematic;

	public bool canSpin;

	public bool canFollow;

	private GameObject[] children;

	private int[] orgChildLayers;

	private void Start()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		children = new GameObject[componentsInChildren.Length - 1];
		int num = 0;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform x = componentsInChildren[i];
			if (x != base.transform)
			{
				children[num] = componentsInChildren[i].gameObject;
				num++;
			}
		}
		orgChildLayers = new int[children.Length];
	}

	public void setChildrenColliderLayers(int layer)
	{
		for (int i = 0; i < children.Length; i++)
		{
			if (!(children[i].transform.root != base.transform.root))
			{
				orgChildLayers[i] = children[i].layer;
				children[i].layer = layer;
			}
		}
	}

	public void returnChildrenColliderLayers()
	{
		for (int i = 0; i < children.Length; i++)
		{
			if (!(children[i].transform.root != base.transform.root))
			{
				children[i].layer = orgChildLayers[i];
			}
		}
	}
}
