using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    private void Start()
    {
        StartCoroutine(RequestCube());
    }

    private IEnumerator RequestCube()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.R))
            {
                ClientSend.RequestCube(Vector3.forward + GetComponent<Transform>().position);
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield return null;
        }

        yield return null;
    }
}
