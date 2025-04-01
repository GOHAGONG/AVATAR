using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int thisStageNum = 0;

    public GameObject StartBtn; 
    public GameObject Move;
    public GameObject PickUp;
    public GameObject Crawl;
    public GameObject LieDown;
    public GameObject Jump;
    public GameObject Throw;
    public GameObject Stomp;
    public GameObject Outro;

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
                LieDown.SetActive(false);
                Jump.SetActive(false);
                Throw.SetActive(false);
                Stomp.SetActive(false);
                break;

            case 1:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(true);
                Crawl.SetActive(false);
                LieDown.SetActive(false);
                Jump.SetActive(false);
                Throw.SetActive(false);
                Stomp.SetActive(false);
                break;
            
            case 2:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(true);
                LieDown.SetActive(false);
                Jump.SetActive(false);
                Throw.SetActive(false);
                Stomp.SetActive(false);
                break;

            case 3:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                LieDown.SetActive(true);
                Jump.SetActive(false);
                Throw.SetActive(false);
                Stomp.SetActive(false);
                break;

            case 4:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                LieDown.SetActive(false);
                Jump.SetActive(true);
                Throw.SetActive(false);
                Stomp.SetActive(false);
                break;

            case 5:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                LieDown.SetActive(false);
                Jump.SetActive(false);
                Throw.SetActive(true);
                Stomp.SetActive(false);
                break;

            case 6:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                LieDown.SetActive(false);
                Jump.SetActive(false);
                Throw.SetActive(false);
                Stomp.SetActive(true);
                break;

            default:
                StartBtn.SetActive(false);
                Move.SetActive(false);
                PickUp.SetActive(false);
                Crawl.SetActive(false);
                LieDown.SetActive(false);
                Jump.SetActive(false);
                Throw.SetActive(false);
                Stomp.SetActive(false);
                Debug.Log("실험 끝");
                break;
        }
    }
}
