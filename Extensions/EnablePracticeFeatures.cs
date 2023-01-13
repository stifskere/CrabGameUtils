using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

public class EnablePracticeFeatures : Extension
{
    public override void Awake() { }

    public override void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Lava climb":
                MonoBehaviourPublicTrspGaboSiObInUnique controller = Object.FindObjectOfType<MonoBehaviourPublicTrspGaboSiObInUnique>();
                controller.SpawnBoulder(UnityEngine.Random.Range(0, 3));
                break;
        }
    }

    public override void Update()
    {
        
    }
}