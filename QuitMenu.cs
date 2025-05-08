using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class QuitMenu : MonoBehaviour
{
    DatabaseManager databaseManager;
    [SerializeField] GameObject ready;
    [SerializeField] GameObject input;
    [SerializeField] GameObject exit;
    Image lifeToLose;
    private Coroutine escapeWatcher;
    private Coroutine flashingCoroutine;
    
    private void Awake()
    {
        Debug.Log("QuitMenu Awake called");
        databaseManager = FindObjectOfType<DatabaseManager>();
        if (databaseManager != null)
            Debug.Log("Database Manager Found!");
        else
            Debug.Log("Database Manager NOT Found!");
            
        // Verify all serialized fields are set
        Debug.Log($"Ready GameObject reference: {(ready != null ? "Set" : "NULL")}");
        Debug.Log($"Input GameObject reference: {(input != null ? "Set" : "NULL")}");
        Debug.Log($"Exit GameObject reference: {(exit != null ? "Set" : "NULL")}");
    }
    
    public void Exit()
    {
        float scr = GridScript.score;
        if (scr > 1000)
        {
            ready.SetActive(false);
            input.SetActive(true);
            Lives.addLife();
        }
        else
        {
            Lives.substractLife();
            GridScript.difficulty = 3;
            SceneManager.LoadScene("mainMenu");
        }
    }
    
    public void XButton()
    {
        Debug.Log("XButton pressed");
        
        if(exit.activeInHierarchy){
            Debug.Log("Exit panel already active, calling Cancel()");
            Cancel();
        } else{
            Debug.Log("Activating exit confirmation panels");
            int liv = PlayerPrefs.GetInt("lives");
            string objName = $"life({liv - 1})";  // Adjust name if needed
            Debug.Log($"Looking for life image: {objName}");
            
            GameObject tempGO = GameObject.Find($"Canvas/LivesManager/{objName}");
            if (tempGO != null)
            {
                Debug.Log($"Found life image: {tempGO.name}");
                lifeToLose = tempGO.GetComponent<Image>();
                
                if (lifeToLose != null)
                    Debug.Log($"Successfully got Image component from {tempGO.name}, current alpha: {lifeToLose.color.a}");
                else
                    Debug.LogError($"Failed to get Image component from {tempGO.name}");
            }
            else
            {
                Debug.LogWarning($"Could not find life object: Canvas/LivesManager/{objName}");
            }
            
            if (flashingCoroutine != null)
            {
                Debug.Log("Stopping existing flashing coroutine");
                StopCoroutine(flashingCoroutine);
                flashingCoroutine = null;
            }

            exit.SetActive(true);
            ready.SetActive(true);
            
            Debug.Log("Starting new flashing coroutine");
            flashingCoroutine = StartCoroutine(FlashingEffect(lifeToLose));
            
            
            Debug.Log($"Exit panel active: {exit.activeInHierarchy}, Ready panel active: {ready.activeInHierarchy}");
        }
        
        if (escapeWatcher == null)
        {
            Debug.Log("Starting escape watcher coroutine");
            escapeWatcher = StartCoroutine(WatchForEscape());
        }
    }
    
    IEnumerator FlashingEffect(Image img)
    {
        Debug.Log($"FlashingEffect started for image: {(img != null ? img.name : "null")}");
        
        if (img == null)
        {
            Debug.LogWarning("FlashingEffect received null image!");
            yield break;
        }
        
        int flashCount = 0;
        while (ready.activeInHierarchy && img != null)
        {
            Debug.Log($"Flash cycle {flashCount}: Fading to 0");
            img.DOFade(0, 1);
            yield return new WaitForSeconds(1f);
            
            Debug.Log($"Flash cycle {flashCount}: Fading to 1");
            img.DOFade(1, 1);
            yield return new WaitForSeconds(1f);
            
            flashCount++;
        }
        
        Debug.Log($"FlashingEffect ended after {flashCount} cycles. ready active: {ready.activeInHierarchy}, img null: {img == null}");
    }
    
    public void Cancel()
    {
        Debug.Log("Cancel called");
        
        ready.SetActive(false);
        exit.SetActive(false);
        Debug.Log($"Deactivated panels - Exit panel active: {exit.activeInHierarchy}, Ready panel active: {ready.activeInHierarchy}");
        
        if(lifeToLose != null)
        {
            Debug.Log($"Handling lifeToLose image: {lifeToLose.name}, current alpha: {lifeToLose.color.a}");
            
            // Stop flashing animation
            if (flashingCoroutine != null)
            {
                Debug.Log("Stopping flashing coroutine");
                StopCoroutine(flashingCoroutine);
                flashingCoroutine = null;
                
                // Reset alpha to 1
                Debug.Log("Resetting alpha to 1");
                lifeToLose.DOFade(1, 0.1f).OnComplete(() => {
                    Debug.Log($"Alpha reset complete, current alpha: {lifeToLose.color.a}");
                });
            }
            else
            {
                Debug.Log("No flashing coroutine to stop");
                // Still reset alpha just in case
                lifeToLose.DOFade(1, 0.1f);
            }
        }
        else
        {
            Debug.Log("lifeToLose is null, nothing to reset");
        }
        
        if (escapeWatcher != null)
        {
            Debug.Log("Stopping escape watcher coroutine");
            StopCoroutine(escapeWatcher);
            escapeWatcher = null;
        }
        else
        {
            Debug.Log("No escape watcher coroutine to stop");
        }
    }
    
    public void HighScore()
    {
        databaseManager.UpdateData();
        Debug.Log("Sending HighScore");
    }
    
    private IEnumerator WatchForEscape()
    {
        while (exit.activeSelf || ready.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel();
                yield break;
            }
            yield return null;
        }
    }
}