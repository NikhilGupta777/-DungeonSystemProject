using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    public int Limite = 9; //Define o limite
    public static List<RandomLevelGenerator> TodosOsCorredores = new List<RandomLevelGenerator>(); //Lista todos os Corredores

    public RandomLevelGenerator Corredor; //O modelo do Corredor
    RandomLevelGenerator CorredorSpawnado { get; set; } //O Corredor spawnado aqui


    //Essa void é chamada quando o scrpit é iniciado
    void Start()
    {
        if (TodosOsCorredores.Count >= Limite) //Verifica se existe mais Corredores do que o Limite
            return;

        CorredorSpawnado = Instantiate(Corredor); //Spawnar o Corredor
        TodosOsCorredores.Add(CorredorSpawnado); //Adiciona o Corredor a listagem

        //Isso define o posicionamento do Corredor
        CorredorSpawnado.transform.parent = this.transform; //Coloca o Corredor como um "parente" desse objeto
        Vector3 Size = CorredorSpawnado.GetComponent<BoxCollider>().size; //Obtem o tamanho do colisor do Corredor
        CorredorSpawnado.transform.localPosition = new Vector3(0, 0, Size.z); //Desloca o Corredor de acordo com o tamnho "Z" do colisor
    }
}


public class VerificarColisão: MonoBehaviour
{
    public RandomLevelGenerator Corredor;

    //Essa void é chamada quando algum colisor entra em contato com ele
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VerificarColisão>()) // Verificar se o objeto que está colidindo tem o script "VerificarColisão"
        {
            //Verifica qual corredor está antes na fila
            if (RandomLevelGenerator.TodosOsCorredores.IndexOf(other.GetComponent<VerificarColisão>().Corredor)
                <= RandomLevelGenerator.TodosOsCorredores.IndexOf(Corredor))
            {
                Destroy(Corredor.gameObject); //Deletar o "Corredor" do objeto
            }
        }
    }
}




