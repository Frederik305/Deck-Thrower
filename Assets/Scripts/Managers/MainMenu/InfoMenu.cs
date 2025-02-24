using UnityEngine;

public class InfoMenu : MonoBehaviour
{

    [SerializeField] GameObject PanelInfo;
    [SerializeField] GameObject MenuUI;



    [SerializeField] GameObject InfoA;
    [SerializeField] GameObject InfoB;
    [SerializeField] GameObject InfoC;

    public void Pause()
    {
        PanelInfo.SetActive(true);
        Time.timeScale = 0;
    }
    public void RemoveUI()
    {
        MenuUI.SetActive(false);
    }

    public void DisplayUI()
    {
        MenuUI.SetActive(true);
    }

    public void Resume()
    {
        PanelInfo.SetActive(false);
        ResetPanelInfo();
        Time.timeScale = 1;
    }

    //Gestion Panel A
    public void PanelA_B()
    {
        InfoA.SetActive(false);
        InfoB.SetActive(true);
    }



    //Gestion Panel B
    public void PanelB_A()
    {
        InfoA.SetActive(true);
        InfoB.SetActive(false);
    }

    public void PanelB_C()
    {
        InfoC.SetActive(true);
        InfoB.SetActive(false);
    }

    //Gestion Panel C

    public void PanelC_B()
    {
        InfoC.SetActive(false);
        InfoB.SetActive(true);
    }


        public void ResetPanelInfo()
    {
        InfoA.SetActive(true);
        InfoB.SetActive(false);
        InfoC.SetActive(false);
    }


}