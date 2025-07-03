using UnityEngine;

// Esse script esta na cama e quando o jogador interage com ele,
// o jogador dorme e o tempo passa, fazendo com que o dia mude para noite
public class Dormir : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Verifica se o jogador está perto da cama
            if (Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) < 2f)
            {
                DormirJogador();
            }
        }
    }
    public void DormirJogador()
    {
        Debug.Log("Jogador está dormindo...");

        GameProgression.instance.Progresso++; // Define o progresso para 6, indicando que é noite
    }
}
