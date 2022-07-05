using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingTarget : MonoBehaviour
{
    public GameObject Model;
    public Renderer DisabledObject;
    public bool UseFade = true;

    public void EnableModel(bool Enable)
    {
        if (Enable)
        {
            Model.gameObject.SetActive(true);
            StopAllCoroutines();
        } else
        {
            StartCoroutine(ShowModel(Enable));
        }

        if (DisabledObject)
        {
            //DisabledObject.gameObject.SetActive(!Enable);

            if (UseFade)
            {
                if (!Enable)
                {
                    Color FinalColor = DisabledObject.material.GetColor("_Color");
                    FinalColor.a = Mathf.Lerp(FinalColor.a, 1, 3 * Time.deltaTime);
                    DisabledObject.material.SetColor("_Color", FinalColor);
                    //Invoke("DisableModel", 3);
                }
                else
                {
                    Color FinalColor = DisabledObject.material.GetColor("_Color");
                    FinalColor.a = Mathf.Lerp(FinalColor.a, 0, 2 * Time.deltaTime);
                    DisabledObject.material.SetColor("_Color", FinalColor);
                    //Model.gameObject.SetActive(Enable);
                }
            } else
            {
                DisabledObject.gameObject.SetActive(!Enable);
            }
        }
    }

    IEnumerator ShowModel(bool Enable)
    {
        yield return new WaitForSeconds(1);
        Model.SetActive(Enable);
    }
}
