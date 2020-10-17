using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public float waitAfterDying = 2f;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + -1);
        // }
    }

    public void PlayerDied()
    {
        StartCoroutine(PlayerDiedCo());
    }

    private IEnumerator PlayerDiedCo()
    {
        yield return new WaitForSeconds(waitAfterDying);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}
