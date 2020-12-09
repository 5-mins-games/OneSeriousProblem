using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform pivot;
    public AudioSource rainSource;
    public AudioSource musicSource;

    private AudioSource hingeSource;

    private bool done;
    private bool isPlaying;

    private float volume;

    private void Start()
    {
        volume = rainSource.volume;
        hingeSource = GetComponent<AudioSource>();
    }

    // open the door around pivot
    public void Spin()
    {
        if (!pivot || done || isPlaying) return;

        hingeSource.Play();

        StartCoroutine(SpinCoro());
    }

    IEnumerator SpinCoro()
    {
        if (done) yield return null;
        isPlaying = true;

        Debug.Log("opening door.");

        float t = 0;
        while (t < 1)
        {
            t += 0.02f;
            // open door
            transform.RotateAround(pivot.position, Vector3.up, 1.8f);
            // increase vol
            rainSource.volume = Mathf.Lerp(volume, volume * 8, t);
            yield return new WaitForEndOfFrame();
        }
        done = true;
        isPlaying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && done)
        {
            if (!musicSource.isPlaying) musicSource.Play();
        }
    }
}
