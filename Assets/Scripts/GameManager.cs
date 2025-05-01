using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int thisStageNum = 0;

    public GameObject StartBtn; 
    public GameObject Move;
    public GameObject PickUp;
    public GameObject Crawl;
    public GameObject Jump;
    public GameObject Outro;

    public void Start()
    {
        // // if Full Body
        // tracker 켜야되나?
        // // else
        // tracker 꺼야되나?
    }

    public void NextStage()
    {
        StageOn(++thisStageNum);
    }

    public void StageOn(int thisStageNum)
    {
        Debug.Log("문제 " + thisStageNum);

        switch (thisStageNum)
        {
            case 0:
                StartBtn.SetActive(false);
                Move.SetActive(true);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                Jump.SetActive(false);
                break;

            case 1:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(true);
                Crawl.SetActive(false);
                Jump.SetActive(false);
                break;
            
            case 2:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(true);
                Jump.SetActive(false);
                break;

            case 3:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                Jump.SetActive(true);
                break;

            default:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                Jump.SetActive(false);
                Debug.Log("실험 끝");
                break;
        }
    }
}
