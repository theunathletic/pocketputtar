using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class golf_scoreFeedback : MonoBehaviour {

    public static golf_scoreFeedback instance;

    [SerializeField]
    private string[] par_holeInOne;
    [SerializeField]
    private string[] par_minus_4;
    [SerializeField]
    private string[] par_minus_3;
    [SerializeField]
    private string[] par_minus_2;
    [SerializeField]
    private string[] par_minus_1;
    [SerializeField]
    private string[] par;
    [SerializeField]
    private string[] par_plus_1;
    [SerializeField]
    private string[] par_plus_2;
    [SerializeField]
    private string[] par_plus_3_4_5;
    [SerializeField]
    private string[] par_plus_6_to_10;
    [SerializeField]
    private string[] par_plus_over_10;

    void Awake()
    {
        instance = this;

    }

    public string GetRandomFeedbackForHoleInOne() {
        string tempFeedback = "";
        tempFeedback = par_holeInOne[Random.Range(0, par_holeInOne.Length)];
        return tempFeedback;
    }

    public string GetRandomFeedbackForPar(int parNumber) {  
        string tempFeedback="";

        switch (parNumber) {
            case -4:
                tempFeedback = par_minus_4[Random.Range(0, par_minus_4.Length)];
            break;

            case -3:
                tempFeedback = par_minus_3[Random.Range(0, par_minus_3.Length)];
                break;

            case -2:
                tempFeedback = par_minus_2[Random.Range(0, par_minus_2.Length)];
                break;

            case -1:
                tempFeedback = par_minus_1[Random.Range(0, par_minus_1.Length)];
                break;

            case 0:
                tempFeedback = par[Random.Range(0, par.Length)];
                break;

            case 1:
                tempFeedback = par_plus_1[Random.Range(0, par_plus_1.Length)];
                break;

            case 2:
                tempFeedback = par_plus_2[Random.Range(0, par_plus_2.Length)];
                break;

            case 3:
                tempFeedback = par_plus_3_4_5[Random.Range(0, par_plus_3_4_5.Length)];
                break;

            case 4:
                tempFeedback = par_plus_3_4_5[Random.Range(0, par_plus_3_4_5.Length)];
                break;

            case 5:
                tempFeedback = par_plus_3_4_5[Random.Range(0, par_plus_3_4_5.Length)];
                break;

            case 6:
                tempFeedback = par_plus_6_to_10[Random.Range(0, par_plus_6_to_10.Length)];
                break;

            case 7:
                tempFeedback = par_plus_6_to_10[Random.Range(0, par_plus_6_to_10.Length)];
                break;

            case 8:
                tempFeedback = par_plus_6_to_10[Random.Range(0, par_plus_6_to_10.Length)];
                break;

            case 9:
                tempFeedback = par_plus_6_to_10[Random.Range(0, par_plus_6_to_10.Length)];
                break;

            case 10:
                tempFeedback = par_plus_6_to_10[Random.Range(0, par_plus_6_to_10.Length)];
                break;

            default:
                tempFeedback = par_plus_over_10[Random.Range(0, par_plus_over_10.Length)];

                break;

        }

        return tempFeedback;
    }
}
