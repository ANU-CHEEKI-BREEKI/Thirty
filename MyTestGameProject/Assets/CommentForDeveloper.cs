using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentForDeveloper : MonoBehaviour
{
    [SerializeField] [Multiline(7)] private string comment = "это комментарий к объекту в инспекторе для разработчика";
}
