using UnityEngine.SceneManagement;
using Array = Il2CppSystem.Array;
using AttributeTargets = System.AttributeTargets;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

public class EnablePracticeFeatures : Extension
{
    private System.Collections.Generic.List<MethodInfo>? _startMethods;
    private System.Collections.Generic.List<MethodInfo>? _updateMethods;
    private System.Collections.Generic.List<MethodInfo>? _destroyMethods;

    public override void Awake()
    {
        _startMethods?.Clear();
        _updateMethods?.Clear();
        _destroyMethods?.Clear();

        if (!IsPractice()) return;
        
        _startMethods = GetAllMethodsFor(RunFor.Start);
        _updateMethods = GetAllMethodsFor(RunFor.Update);
        
        if (_destroyMethods != null)
            foreach (MethodInfo method in _destroyMethods)
                method.Invoke(this, Array.Empty<object>());

        _destroyMethods = GetAllMethodsFor(RunFor.Disable);
    }
    
    private System.Collections.Generic.List<MethodInfo> GetAllMethodsFor(RunFor rType)
        => GetType()
            .GetMethods()
            .Where(m => m
                .GetCustomAttributesData()
                .FirstOrDefault(d => d.AttributeType.Name == "RunForMapAttribute")?
                .ConstructorArguments is { } a &&
                        (RunFor)a[1].Value == rType && 
                        (string)a[0].Value == SceneManager.GetActiveScene().name)
            .ToList();
    
    [System.AttributeUsage(AttributeTargets.Method)]
    private class RunForMapAttribute : System.Attribute
    {
        // ReSharper disable UnusedParameter.Local
        public RunForMapAttribute(string sceneName, RunFor rType) {}
    }

    private enum RunFor
    {
        // ReSharper disable UnusedMember.Local
        Update,
        Start,
        Disable
    }
    
    public override void Start()
    {
        if (_startMethods == null) return;
        foreach (MethodInfo method in _startMethods!)
            method.Invoke(this, null);
    }

    public override void Update()
    {
        if (_startMethods == null) return;
        foreach (MethodInfo method in _updateMethods!)
            method.Invoke(this, null);
    }

    private float _objectiveTime;
    [RunForMap("Lava climb", RunFor.Update)]
    public void UpdateLavaClimb()
    {
        _objectiveTime += Time.deltaTime;
        if (_objectiveTime > 3)
        {
            _objectiveTime = 0;
            Object.FindObjectOfType<MonoBehaviourPublicTrspGaboSiObInUnique>().SpawnBoulder(UnityEngine.Random.Range(0, 3));
            foreach (MonoBehaviourPublicSitiUnique boulder in Object.FindObjectsOfType<MonoBehaviourPublicSitiUnique>())
            {
                try
                {
                    boulder.GetComponent<SphereCollider>().isTrigger = false;
                    Transform transform = boulder.transform;
                    Collider[] a = Physics.OverlapSphere(transform.position, transform.localScale.x);
                    foreach (Collider collider in a)
                    {
                        try
                        {
                            if (collider.gameObject.name == "Player (clone)")
                            {
                                Vector3 diff = collider.transform.position - boulder.transform.position;
                                Rigidbody rb = collider.GetComponentInChildren<Rigidbody>();
                                if (rb) rb.AddForce(diff.normalized * 20, ForceMode.Impulse);
                            } 
                        } catch {/**/}
                    }
                } catch {/**/}
            }
        }
    }
}