using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player; //pega infos do jogador
    public Transform bulletSpawn; //posição do spawn da bala
    public Slider healthBar;   //define formato da barra de vida
    public GameObject bulletPrefab; //define o gameObject que representará a bala

    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float health = 100.0f;      //vitalidade total
    float rotSpeed = 5.0f;      //velocidade de rotação

    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>(); //acessa o componente NavMesh
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f); //atualização da barra de vida
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position); //renderiza a barra de vida
        healthBar.value = (int)health; //valor corrente da barra de vida
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0); //posição da barra de vida
    }

    void UpdateHealth() //atualiza a barra de vida
    {
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet") //tira 10 de vida quando colide com a bala
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination() //selecionar destino aleatório no mapa
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100)); //limites do random
        agent.SetDestination(dest);//define a variável de destino
        Task.current.Succeed();//conclui e segue a próxima tarefa
    }
    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected) //exibe no componente do local do agente
        {
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        }

        //se chegou no local com certa distânca do programado completa a ação atual
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}

