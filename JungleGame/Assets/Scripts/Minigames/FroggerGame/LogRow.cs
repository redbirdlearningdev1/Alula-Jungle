using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogRow : MonoBehaviour
{
    public float moveTime;
    public List<Vector2> movePositions;
    public List<SingleLog> logs;
    public List<SingleLog> coinLogs;

    void Start()
    {

    }

    void Update()
    {
        // for dev testing in-game
        if (GameManager.instance.devModeActivated)
        {
            // log animations
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                GameManager.instance.SendLog(this, "Rising logs");
                RiseAllLogs();
            }
            else if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                GameManager.instance.SendLog(this, "Sinking logs");
                SinkAllLogs();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                GameManager.instance.SendLog(this, "Sinking logs except for log 0");
                SinkAllExcept(0);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager.instance.SendLog(this, "Sinking logs except for log 1");
                SinkAllExcept(1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.instance.SendLog(this, "Sinking logs except for log 2");
                SinkAllExcept(2);
            }

            // log movements
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                GameManager.instance.SendLog(this, "Centering log 0");
                MoveToCenterLog(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GameManager.instance.SendLog(this, "Centering log 1");
                MoveToCenterLog(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GameManager.instance.SendLog(this, "Centering log 2");
                MoveToCenterLog(2);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.instance.SendLog(this, "Reseting logs");
                ResetLogRow();
            }
        }
    }

    /* 
    ################################################
    #   LOG ANIMATIONS
    ################################################
    */

    public void SinkAllLogs()
    {
        StartCoroutine(SinkAllLogsRoutine());
    }

    private IEnumerator SinkAllLogsRoutine()
    {
        List<SingleLog> logs_cpy = new List<SingleLog>(logs);

        int size = logs_cpy.Count;
        for (int i = 0; i < size; i++)
        {
            int index = Random.Range(0, logs_cpy.Count);
            SingleLog log = logs_cpy[index];
            logs_cpy.RemoveAt(index);
            log.LogSink();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void RiseAllLogs()
    {
        StartCoroutine(RiseAllLogsRoutine());
    }

    private IEnumerator RiseAllLogsRoutine()
    {
        List<SingleLog> logs_cpy = new List<SingleLog>(logs);

        int size = logs_cpy.Count;
        for (int i = 0; i < size; i++)
        {
            int index = Random.Range(0, logs_cpy.Count);
            SingleLog log = logs_cpy[index];
            logs_cpy.RemoveAt(index);
            log.LogRise();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SinkAllExcept(int logIndex)
    {
        // assert that position exists
        if (logIndex >= 0 && logIndex < movePositions.Count)
        {
            StartCoroutine(SinkAllExcpetRoutine(logIndex));
        }
    }

    private IEnumerator SinkAllExcpetRoutine(int logIndex)
    {
        List<SingleLog> logs_cpy = new List<SingleLog>(logs);
        SingleLog excludedLog = coinLogs[logIndex];
        logs_cpy.Remove(excludedLog);

        int size = logs_cpy.Count;
        for (int i = 0; i < size; i++)
        {
            int index = Random.Range(0, logs_cpy.Count);
            SingleLog log = logs_cpy[index];
            logs_cpy.RemoveAt(index);
            log.LogSink();
            yield return new WaitForSeconds(0.2f);
        }
    }

    /* 
    ################################################
    #   LOG MOVEMENT
    ################################################
    */

    public void MoveToCenterLog(int logIndex)
    {
        // assert that position exists
        if (logIndex >= 0 && logIndex < movePositions.Count)
        {
            StartCoroutine(MoveRowTo(movePositions[logIndex]));
        }
    }

    public void ResetLogRow()
    {
        StartCoroutine(MoveRowTo(new Vector2(0f, 0f)));
    }

    private IEnumerator MoveRowTo(Vector2 _newPos)
    {
        Vector3 oldPos = transform.localPosition;
        Vector3 newPos = new Vector3(_newPos.x, _newPos.y, 0f);

        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                break;
            }
            
            transform.localPosition = Vector3.Lerp(oldPos, newPos, timer/moveTime);
            yield return null;
        }
    }
}
