namespace AlgoritmoGenetico
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro; // Importa el espacio de nombres para TextMeshPro
    using UnityEngine.UI; // Importa el espacio de nombres para UI

    public class Genetic_Algorithm : MonoBehaviour
    {
        public int n_population = 30;
        public int cromosome_size = 150;
        public float mutation_prob = 0.15f;
        public Transform Origen;
        public GameObject prefab;

        public static List<Individuo> population;

        public TextMeshProUGUI infoText; // Cambiado a TextMeshProUGUI
        public Scrollbar mutationScrollbar; // Scrollbar para ajustar mutation_prob
        public TextMeshProUGUI mutationValueText; // TextMeshPro para mostrar el valor de mutation_prob

        private void Start()
        {
            // Configura el valor inicial del scrollbar y el texto
            mutationScrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
            mutationScrollbar.value = mutation_prob; // Ajusta el valor inicial del scrollbar
            UpdateMutationValueText(mutation_prob);

            population = generate_population();
            StartCoroutine(StartGeneticAlgorithm());
        }

        private List<Individuo> generate_population()
        {
            List<Individuo> initialPopulation = new List<Individuo>();

            for (int i = 0; i < n_population; i++)
            {
                Individuo individual = new Individuo(cromosome_size);
                initialPopulation.Add(individual);
            }

            return initialPopulation;
        }

        private Individuo GetBestFitness()
        {
            Individuo fittestIndividual = population[0];

            foreach (Individuo individual in population)
            {
                if (individual.fitness > fittestIndividual.fitness)
                {
                    fittestIndividual = individual;
                }
            }

            return fittestIndividual;
        }

        private Individuo SelectParent()
        {
            // Selección de ruleta
            float totalFitness = 0;
            foreach (Individuo individual in population)
            {
                totalFitness += individual.fitness;
            }

            population.Sort((x, y) => x.fitness.CompareTo(y.fitness));

            foreach (Individuo individual in population)
            {
                individual.PorcentajeFitness = (individual.fitness * 100) / totalFitness;
            }

            float randomFitness = Random.Range(0, 100);
            float TopeA = 0;
            float TopeB = 0;
            for (int i = 0; i < population.Count; i++)
            {
                TopeB = TopeA + population[i].PorcentajeFitness;
                if (randomFitness >= TopeA && randomFitness <= TopeB)
                    return population[i];

                TopeA = TopeB;
            }

            return null;
        }

        private Individuo Crossover(Individuo parent1, Individuo parent2)
        {
            Individuo child = new Individuo(cromosome_size);

            int crossoverPoint = Random.Range(0, cromosome_size);

            for (int i = 0; i < cromosome_size; i++)
            {
                if (i < crossoverPoint)
                {
                    child.chromosome[i] = parent1.chromosome[i];
                }
                else
                {
                    child.chromosome[i] = parent2.chromosome[i];
                }
            }

            return child;
        }

        private void Mutate(Individuo individual)
        {
            for (int i = 0; i < individual.chromosome.Length; i++)
            {
                if (Random.value < mutation_prob)
                {
                    individual.chromosome[i] = (MovementType)Random.Range(0, (int)MovementType.Count);
                }
            }
        }

        private List<Individuo> GenerateNewPopulation()
        {
            List<Individuo> newPopulation = new List<Individuo>();

            for (int i = 0; i < n_population / 2; i++)
            {
                // Selección
                Individuo parent1 = SelectParent();
                Individuo parent2 = SelectParent();

                // Cruce
                parent1.ShowCromosomas();
                parent2.ShowCromosomas();
                Individuo child = Crossover(parent1, parent2);

                // Mutación
                Mutate(child);

                newPopulation.Add(child);
            }

            return newPopulation;
        }

        private void EvaluateFitness()
        {
            foreach (Individuo individuo in population)
            {
                // Reinicia la posición del cubo al punto de inicio
                GameObject obj = Instantiate(prefab, Origen.position, Origen.rotation);
                obj.GetComponent<Renderer>().material.color =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                obj.GetComponent<AgentesAG>().individuo = individuo;
            }
            population.Clear();
        }

        private IEnumerator StartGeneticAlgorithm()
        {
            int generation = 0;
            List<Individuo> BestParents = new List<Individuo>();

            while (true)
            {
                EvaluateFitness();

                yield return new WaitUntil(() => population.Count == n_population);

                Individuo fittestIndividual = GetBestFitness();

                // Reproduce y genera una nueva población
                List<Individuo> newPopulation = GenerateNewPopulation();
                for (int i = 0; i < n_population; i++)
                {
                    if (i < n_population / 2)
                        BestParents.Add(newPopulation[i]);
                    else
                        BestParents.Add(population[i]);
                }

                population.Clear();
                foreach (var item in BestParents)
                {
                    population.Add(item);
                }
                BestParents.Clear();

                // Muestra la información en el TextMeshPro
                if (infoText != null)
                {
                    infoText.text = "Generation: " + generation + ", Fitness: " + fittestIndividual.fitness + ", Probabilidad de Mutacion: " + mutation_prob;
                }

                Debug.Log("Generation: " + generation + ", Fitness: " + fittestIndividual.fitness + ", Probabilidad de Mutacion: " + mutation_prob);

                generation++;

                yield return null;
            }
        }

        private void OnScrollbarValueChanged(float value)
        {
            mutation_prob = value;
            UpdateMutationValueText(value);
        }

        private void UpdateMutationValueText(float value)
        {
            if (mutationValueText != null)
            {
                mutationValueText.text = "Probabilidad de Mutación: " + value.ToString("F2");
            }
        }
    }
}
