using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCombiner
{
    private readonly Dictionary<int, Transform> _rootBoneDictionary = new Dictionary<int, Transform>();
    private readonly Transform[] _boneTransforms;
    private readonly Transform _transform;
    private readonly SkinnedMeshRenderer _skinnedMeshRenderer;

    public BoneCombiner(GameObject rootObj, int count)
    {
        _boneTransforms = new Transform[count];
        _transform = rootObj.transform.parent;
        _skinnedMeshRenderer = rootObj.GetComponent<SkinnedMeshRenderer>();
        ConfigBones(_skinnedMeshRenderer);
        //TraverseHierarchy(_transform);
    }

    public Transform AddLimb(GameObject bonedObj, List<string> boneNames)
    {
        var limb = ProcessBonedObject(bonedObj.GetComponent<SkinnedMeshRenderer>(), boneNames);
        limb.SetParent(_transform);
        return limb;
    }


    private Transform ProcessBonedObject(SkinnedMeshRenderer renderer, List<string> boneNames)
    {
        var bonedObject = new GameObject().transform;

        var meshRenderer = bonedObject.gameObject.AddComponent<SkinnedMeshRenderer>();

        for (var i = 0; i < boneNames.Count; i++)
        {
            //Debug.Log("AddedLimb====" + boneNames[i].GetHashCode() + "====" + boneNames[i]);
            _boneTransforms[i] = _rootBoneDictionary[boneNames[i].GetHashCode()];
        }

        meshRenderer.bones = _boneTransforms;
        meshRenderer.sharedMesh = renderer.sharedMesh;
        meshRenderer.materials = renderer.sharedMaterials;

        return bonedObject;
    }


    private void TraverseHierarchy(Transform root)
    {
        //foreach (Transform child in root)
        //{
        //    //if (_rootBoneDictionary.ContainsKey(child.name.GetHashCode()))
        //    //    continue;


        //    Debug.Log("Bones====" + child.name.GetHashCode() + "====" + child.name);

        //    _rootBoneDictionary.Add(child.name.GetHashCode(), child);
        //    TraverseHierarchy(child);
        //}
        

    }

    private void ConfigBones(SkinnedMeshRenderer mesh)
    {
        var transforms = mesh.bones;
        for (int i = 0; i < transforms.Length; i++)
        {
            Debug.Log($"{transforms[i].name}");
            _rootBoneDictionary.Add(transforms[i].name.GetHashCode(), transforms[i]);
        }
    }
}

