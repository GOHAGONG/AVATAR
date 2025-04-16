using UnityEngine;

public class TargetCtrl : MonoBehaviour
{
    public GameObject target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if (target != null)
        {
            this.transform.localPosition = this.transform.parent.InverseTransformPoint(target.transform.position);
            this.transform.localRotation = Quaternion.Inverse(this.transform.parent.rotation) * target.transform.rotation;

            // Debug.Log($"Controller Position: {transform.position}, Target Position: {target.transform.position}");
        } 
    }
}
