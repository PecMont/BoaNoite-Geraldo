// Scripts/SanityBarUI.cs
using UnityEngine;
using UnityEngine.UI; // Necessário para Slider

[RequireComponent(typeof(Slider))]
public class SanityBarUI : MonoBehaviour
{
    private Slider sanitySlider;

    void Awake()
    {
        sanitySlider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        // Se inscreve no evento para receber atualizações de sanidade
        PlayerSanity.OnSanityChanged += UpdateSanityBar;
    }
     // Start é chamado depois de Awake e OnEnable, garantindo que tudo está pronto.
    void Start()
    {
        // Ação Proativa: Assim que o jogo começa, vamos verificar o estado atual da sanidade.
        // Isso garante que a barra seja preenchida corretamente, mesmo que tenhamos perdido o evento inicial.
        if (PlayerSanity.Instance != null)
        {
            //Debug.Log("SanityBarUI.Start(): Atualizando a barra com o valor inicial do PlayerSanity.");
            // Atualiza a barra diretamente com os valores atuais do PlayerSanity
            UpdateSanityBar(PlayerSanity.Instance.CurrentSanity, PlayerSanity.Instance.maxSanity);
        }
        else
        {
            // Este log de erro aparecerá se o PlayerSanity ainda não tiver sido inicializado.
            // Se isso acontecer, o problema de ordem de execução é mais severo.
            Debug.LogError("SanityBarUI.Start(): PlayerSanity.Instance ainda não está disponível!");
        }
    }

    void OnDisable()
    {
        // Cancela a inscrição para evitar erros quando o objeto é desativado
        PlayerSanity.OnSanityChanged -= UpdateSanityBar;
    }

    private void UpdateSanityBar(float currentSanity, float maxSanity)
    {
        // Calcula a porcentagem (0 a 1) e atualiza o valor do slider
        sanitySlider.value = currentSanity / maxSanity;
    }
}