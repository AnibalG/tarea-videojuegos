using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoIntro : MonoBehaviour
{
    public Camera camara;
    public VideoPlayer video;
    public float tiempo;
    public float tiempoAnimacion;
    public GameObject pantalla;
    public GameObject v;
    private void Awake()
    {
        camara = GetComponent<Camera>();
        video = GetComponent<VideoPlayer>();


    }
    // Start is called before the first frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;
        if (tiempo > tiempoAnimacion) {
            pantalla.SetActive(true);
            v.SetActive(false);

        }
        
        
    }
}
