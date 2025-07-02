using UnityEngine;

public class Sensor : MonoBehaviour
{
    [Header("Configurações do Sensor")]
    [Tooltip("A que distância o sensor detecta o jogador.")]
    public float detectionRadius = 1f;
    [Tooltip("Para qual criatura este sensor está vinculado.")]
    public GameObject linkedCreature;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if(linkedCreature == null)
       {
           Debug.LogError("O sensor precisa estar vinculado a uma criatura. Por favor, arraste o GameObject da criatura no campo 'Linked Creature' no Inspector.", this);
           enabled = false; // Desativa o script se não encontrar a criatura vinculada.
       }
    }

    // Update is called once per frame
    void Update()
    {
        // verifica se o jogador está dentro do raio de detecção
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                // O jogador está dentro do raio de detecção
                linkedCreature.SetActive(true); // Ativa a criatura vinculada
                Invoke("DeactivateLinkedCreature", 1f); // Desativa a criatura após 5 segundos
            }
        }
    }
    // Método para desativar a criatura vinculada e apagar todos os objetos vinculados com o script
    public void DeactivateLinkedCreature(){
        if (linkedCreature != null)
        {   
            // apaga todos os objetos vinculados com o script
            foreach (var obj in FindObjectsOfType<Sensor>())
            {
                if (obj.linkedCreature != null && obj.linkedCreature == linkedCreature)
                {
                    Destroy(obj.gameObject); // Destrói o objeto do sensor
                }
            }
            // Aumenta o progresso do jogador
            if(GameProgression.instance.Progresso < 1) // Verifica se o progresso é menor que 3
                GameProgression.instance.Progresso++;

            linkedCreature.SetActive(false); // Desativa a criatura vinculada
        
        }
        else
        {
            Debug.LogWarning("Nenhuma criatura vinculada para desativar.", this);
        }
    }
}
