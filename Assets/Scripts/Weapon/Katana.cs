using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Blade
{
    // Start is called before the first frame update
    void Start()
    {
        SetIsSlashing(false);
        damage = 200f; 
    }
}
