using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;//IK

public enum CharacterState {allControll,onlyRot,none};
public class HumanoidCharacter : MonoBehaviour
{
    [SerializeField] GameObject mesh;//animator+transform
    [SerializeField] Animator animator;
    [Header("LookAt")]
    [SerializeField] Transform lookAtTarget;
    Transform m_transform;
    bool lookAt;
    public MultiAimConstraint headIK;
    // Start is called before the first frame update
    void Start()
    {
        animator = mesh.GetComponent<Animator>();
        m_transform = mesh.transform;
    }
    public void StartLookAt(Transform target,float weight = 0.6f)
    {
        headIK.data.sourceObjects.SetTransform(0,target);
        headIK.data.sourceObjects.SetWeight(0, weight);
        headIK.weight = weight;
    }
    public void StopLookAt()
    {
        headIK.weight = 0;
    }

    void LookAtControll()
    {
        if ((lookAtTarget.position - m_transform.position).sqrMagnitude < 10 && !lookAt)
        {
            StartLookAt(lookAtTarget);
            lookAt = true;
        }
        if ((lookAtTarget.position - m_transform.position).sqrMagnitude >= 10 && lookAt)
        {
            StopLookAt();
            lookAt = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        LookAtControll();
    }
}
