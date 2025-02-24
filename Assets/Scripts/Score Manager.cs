using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    
    private int score=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void AddScore(int scoreToAdd)
    {
        score+=scoreToAdd;

    }

    // Update is called once per frame
    public int GetScore()
    {
        return score;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Debug.Log(GetScore());
    }
}
