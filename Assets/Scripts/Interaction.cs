using System.Collections;
using TMPro;
using UnityEngine;

public enum InteractionType
{
    WaitExit, Exit, Sit, StandUp
}

[RequireComponent(typeof(Collider))]
public class Interaction : MonoBehaviour
{
    public InteractionType interaction;
    [Range(1, 10)]
    public int cameraSpeed = 5;
    [Range(1, 60)]
    public float openDoorAfterSec = 60;

    private TMP_Text text;
    private TMP_Text exitText;
    private bool activated;
    private bool isPlaying;
    private GameObject player;
    private Deformation deform;
    private Door door;
    private float sitTime;

    private Vector3 startPos;
    private Vector3 endPos;

    private Color textColor;
    private Color dimColor = new Color(.4f, .4f, .4f);

    public delegate void OnPlayerInteract();
    public static OnPlayerInteract PlayerInteractEvent;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        text = GameObject.FindGameObjectWithTag("InteractionText").GetComponent<TMP_Text>();
        exitText = GameObject.FindGameObjectWithTag("ExitText").GetComponent<TMP_Text>();

        deform = GameObject.FindGameObjectWithTag("Deform").GetComponent<Deformation>();
        door = GameObject.FindGameObjectWithTag("Door").GetComponent<Door>();

        textColor = text.color;
        text.SetText("");
    }

    private void Update()
    {
        if (activated && !isPlaying)
        {
            // don't display when sit.
            // flash
            if (player.GetComponent<CharacterController>().enabled)
            {
                text.SetText("*");
                text.color = Color.Lerp(dimColor, textColor, Mathf.Sin(Time.time * 2f));
            }
        }
    }

    private void Interact()
    {
        if (!activated) return;
        if (isPlaying) return;

        // activate with space control
        switch (interaction)
        {
            case InteractionType.WaitExit:
                WaitExit();
                break;
            case InteractionType.Exit:
                Exit();
                break;
            case InteractionType.Sit:
                Sit();
                break;
            case InteractionType.StandUp:
                StandUp();
                break;
        }
    }

    private void WaitExit()
    {
        // display text: are you ready to leave the world?
        // It will erase the game from your disk.
        exitText.SetText("Are you ready to exit the world? This will erase the game from your disk.");
        // you can exit now;
        interaction = InteractionType.Exit;
    }

    private void Exit()
    {
        Debug.Log("exit game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Cmd.DestroyGame();
        Application.Quit();
#endif
    }

    private void Sit()
    {
        // disable character controller
        TogglePlayer();
        // lerp camera to position
        startPos = player.transform.position;
        Vector3 curr = transform.position;
        curr.y += .5f;
        endPos = curr;

        StartCoroutine(LerpPlayerPos());
    }

    private void StandUp()
    {
        // enable player movement & look
        TogglePlayer();

        interaction = InteractionType.Sit;
        deform.playing = false;
    }

    private void TogglePlayer()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        //PlayerLook look = player.GetComponentInChildren<PlayerLook>();
        bool plEnabled = controller.enabled;

        controller.enabled = !plEnabled;
        //look.enabled = !plEnabled;
    }

    private IEnumerator LerpPlayerPos()
    {
        isPlaying = true;
        text.SetText("");

        float t = 0;
        while (Vector3.Distance(player.transform.position, endPos) > .01f)
        {
            t += Time.deltaTime * cameraSpeed;
            player.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return new WaitForEndOfFrame();
        }
        player.transform.LookAt(endPos + Vector3.forward);
        isPlaying = false;

        if (interaction == InteractionType.Sit)
        {
            // listen to space key to stand up.
            // change to stand up after anim finished
            interaction = InteractionType.StandUp;
        }
        // start playing deformation
        deform.playing = true;

        yield return null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && interaction == InteractionType.StandUp)
        {
            sitTime += Time.deltaTime;

            if (sitTime >= openDoorAfterSec)
            {
                door.Spin();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
            Debug.Log("Player activate the installation " + name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activated = false;
            deform.playing = false;
            Debug.Log("Player leaving the installation " + name);

            text.SetText("");
            exitText.SetText("");

            // reset exit
            if (interaction == InteractionType.Exit)
                interaction = InteractionType.WaitExit;
            // reset sit time
            sitTime = 0;
        }
    }

    private void OnEnable()
    {
        PlayerInteractEvent += Interact;
    }

    private void OnDisable()
    {
        PlayerInteractEvent -= Interact;
    }
}
