using UnityEngine;

public class RealisticFlickeringLight : MonoBehaviour{
    [Header("Luz Defeituosa")]
    public Light targetLight;
    public float baseInterval = 2f;
    public float intervalVariation = 0.5f; // Variação aleatória
    
    [Header("Padrão de Piscar")]
    public int flickerCount = 3; // Quantas vezes pisca seguido
    public float flickerSpeed = 0.15f;
    
    private float timer = 0f;
    private float currentInterval;
    
    void Start(){
        if (targetLight == null)
            targetLight = GetComponent<Light>();
            
        SetNextInterval();
    }
    
    void Update(){
        timer += Time.deltaTime;
        
        if (timer >= currentInterval){
            StartCoroutine(FlickerSequence());
            SetNextInterval();
            timer = 0f;
        }
    }
    
    void SetNextInterval(){
        currentInterval = baseInterval + Random.Range(-intervalVariation, intervalVariation);
    }
    
    System.Collections.IEnumerator FlickerSequence(){
        float originalIntensity = targetLight.intensity;
        
        for (int i = 0; i < flickerCount; i++){
            // Apagar
            targetLight.intensity = Random.Range(0f, 0.2f);
            yield return new WaitForSeconds(flickerSpeed);
            
            // Acender
            targetLight.intensity = originalIntensity;
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
