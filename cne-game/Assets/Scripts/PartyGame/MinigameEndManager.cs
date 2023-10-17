using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameEndManager : MonoBehaviour
{

    public float MinigameEndTime;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = MinigameEndTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if( timer <= 0 ){
            SceneManager.LoadScene("Board");
        }
    }
}
