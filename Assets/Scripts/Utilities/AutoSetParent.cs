using UnityEngine;

public class AutoSetParent : MonoBehaviour
{
    [Tooltip("The parent GameObject to which this object will be moved.")]
    public string parentName = "PoolContainer";

    private void Awake()
    {
        SetParent();
    }

    private void OnEnable()
    {
        Debug.Log("enabled");
        SetParent();
    }

    /// <summary>
    /// Finds or creates the parent GameObject and assigns this object to it.
    /// </summary>
    private void SetParent()
    {
        // Find or create the specified parent GameObject
        GameObject container = GameObject.Find(parentName);
        if (container == null)
        {
            container = new GameObject(parentName);
        }

        // Set this object's parent to the specified container
        transform.SetParent(container.transform);
    }
}
