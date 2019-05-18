using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;
using LiteNetLib.Utils;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Player))]
public class FPSController : MonoBehaviour {

    public Text tileText;
    private Rigidbody rb;
    private Player player;

    public float speed = 10f;
    public float gravity = 10f;
    public float maxVelocityChange = 10f;
    public bool canJump = true;
    public float jumpHeight = 2f;

    private bool grounded = false;

    private NetDataWriter writer;

    void Start() {
        player = GetComponent<Player>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        StartCoroutine(BreathingLoop());

        // Networking
        writer = new NetDataWriter();

        StartCoroutine(SendMovement_Coroutine());
    }

    void FixedUpdate() {
        if (grounded) {
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            if (canJump) {
                //rb.velocity = new Vector3(velocity.x, Mathf.Sqrt(2 * jumpHeight * gravity), velocity.z);
                Ray ray = new Ray(transform.position, -transform.up);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit, 5f)) {
                    if (hit.transform != null) {
                        Tile tile = hit.transform.GetComponent<Tile>();

                        if (tile != null) {
                            tileText.text = tile.GetTileInformation();
                            if (Input.GetButton("Jump")) {
                                tile.AddPressure(4f, Gas.OXYGEN.GetIndex());
                            }
                        }
                    }
                }
            }
        }

        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

        grounded = false;
    }

    private IEnumerator BreathingLoop() {
        // Take normal breaths every 8 seconds
        while (true) {
            Breathe();
            yield return new WaitForSeconds(8f);
        }
    }

    private void Breathe() {
        // Person inhales and exhales 7 or 8 liters of air
        // 8 liters = 0.33 moles
        // 0.33 moles * atomic mass = grams

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 3f)) {
            if (hit.transform != null) {
                Tile tile = hit.transform.GetComponent<Tile>();

                if (tile != null) {
                    float oxygenIncrease = 0.0178f * Gas.OXYGEN.GetAtomicMass();
                    float carbonDioxide = 0.0178f * Gas.CARBON_DIOXIDE.GetAtomicMass();

                    if (tile.gasMixture.gases[Gas.OXYGEN.GetIndex()] >= oxygenIncrease) {
                        tile.gasMixture.gases[Gas.OXYGEN.GetIndex()] -= oxygenIncrease;

                        tile.gasMixture.gases[Gas.CARBON_DIOXIDE.GetIndex()] += carbonDioxide;
                    }

                    
                }
            }
        }
    }

    private IEnumerator SendMovement_Coroutine() {
        while (true) {
            Packet04PlayerMove playerMove = new Packet04PlayerMove(player.guid, transform.position, transform.eulerAngles);
            playerMove.Serialize(writer);
            Client.Instance.SendDataToServer(writer, DeliveryMethod.Unreliable);

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnCollisionStay(Collision collision) {
        grounded = true;
    }
}
