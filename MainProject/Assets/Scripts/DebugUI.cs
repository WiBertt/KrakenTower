using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WibertStudio;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI xInputText;
    [SerializeField] private TextMeshProUGUI yInputText;
    [SerializeField] private TextMeshProUGUI xVelocityText;
    [SerializeField] private TextMeshProUGUI yVelocityText;
    [SerializeField] private TextMeshProUGUI groundedText;
    [SerializeField] private TextMeshProUGUI leftWallText;
    [SerializeField] private TextMeshProUGUI rightWallText;
    [SerializeField] private TextMeshProUGUI ceilingText;

    private Player player;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }
    private void Update()
    {
        xInputText.text = player.GetAxis("Move Horizontal").ToString("F1");
        yInputText.text = player.GetAxis("Y Axis").ToString("F1");
        xVelocityText.text = PlayerManager.instance.Rb.velocity.x.ToString("F1");
        yVelocityText.text = PlayerManager.instance.Rb.velocity.y.ToString("F1");
        groundedText.text = PlayerManager.instance.IsGrounded.ToString();
        leftWallText.text = PlayerManager.instance.IsOnLeftWall.ToString();
        rightWallText.text = PlayerManager.instance.IsOnRightWall.ToString();
        ceilingText.text = PlayerManager.instance.IsOnCeiling.ToString();

        if(Input.GetKeyDown(KeyCode.F1))
            if(uiPanel.activeSelf)
                uiPanel.SetActive(false);
        else
                uiPanel.SetActive(true);

    }
}
