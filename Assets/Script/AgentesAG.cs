using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Asegúrate de incluir el espacio de nombres para UI
using AlgoritmoGenetico;

public class AgentesAG : MonoBehaviour
{
    public Individuo individuo;


    private float distanceFitnnes = 0;
    private int position = 0;

    private void Start()
    {
        // Asegúrate de que el panel de pausa y el mensaje estén ocultos al inicio
        StartCoroutine(MoveTransform(individuo.chromosome));
    }

    IEnumerator MoveTransform(MovementType[] chromosome)
    {
        while (position < chromosome.Length)
        {
            switch (chromosome[position])
            {
                case MovementType.Forward:
                    transform.Translate(Vector3.forward);
                    break;
                case MovementType.Left:
                    transform.Translate(Vector3.left);
                    break;
                case MovementType.Right:
                    transform.Translate(Vector3.right);
                    break;
            }

            yield return new WaitForSeconds(Random.Range(0.015f, 0.025f));
            distanceFitnnes += 1;

            // Verifica si se ha alcanzado un objetivo o colisionado con un obstáculo
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Meta"))
                {
                    Genetic_Algorithm.population.Add(individuo);
                    Destroy(gameObject); // Elimina el agente

                    // Pausar el juego y mostrar mensaje
                    Time.timeScale = 0; // Pausa el juego

                    yield break; // Sale de la corrutina
                }
                else if (collider.CompareTag("Obstacle"))
                {
                    individuo.fitness = distanceFitnnes - 10;
                    Genetic_Algorithm.population.Add(individuo);
                    Destroy(gameObject); // Elimina el agente
                    yield break; // Sale de la corrutina
                }
            }

            position++;
        }

        individuo.fitness = distanceFitnnes;
        Genetic_Algorithm.population.Add(individuo);

        Destroy(gameObject); // Elimina el agente
    }
}
