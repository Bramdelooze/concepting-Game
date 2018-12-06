using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public bool playerDied;
    bool isRestarting;
	
	// Update is called once per frame
	void Update () {
        //if (playerDied && !isRestarting)
        //{
        //    StartCoroutine(NextLevel());
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator NextLevel()
    {
        isRestarting = true;
        yield return new WaitForSeconds(3);
        if(SceneManager.GetActiveScene().buildIndex < 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
