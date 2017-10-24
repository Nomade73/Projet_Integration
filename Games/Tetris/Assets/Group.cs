using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {

    float lastFall = 0;
	// Use this for initialization
	void Start () {
        //si la position par défault n'est pas valide, alors game over
        if (!IsValidGridPos())
        {
            Debug.Log("Game Over");
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //Bouge sur la gauche
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Modifie la position
            transform.position += new Vector3(-1, 0, 0);

            //check si valide
            if (IsValidGridPos())
            {
                //si c'est valide, update de la grille
                updateGrid();
            }
            else
            {
                //si pas valide, revenir en arriere
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        //Bouge sur la droite
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Modifie la position
            transform.position += new Vector3(1, 0, 0);

            //check si valide
            if (IsValidGridPos())
            {
                //si valide, update la grille
                updateGrid();
            }
            else
            {
                //si pas valide, retour en arrière
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        //Rotation
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);
            //check si valide
            if (IsValidGridPos())
            {
                //si valide, update de la grille
                updateGrid();
            }
            else
            {
                //si pas valide, retour en arrière
                transform.Rotate(0, 0, 90);
            }
        }
        //chute
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //modifie la position
            transform.position += new Vector3(0, -1, 0);

            //check si valide

            if (IsValidGridPos())
            {
                //si c'est valide, update la grille
                updateGrid();
            }
            else
            {
                //si pas valide, retour en arrière
                transform.position += new Vector3(0, 1, 0);

                //Nettoye les lignes horizontales remplis
                Grille.DeleteFullRows();

                //Spawn le prochain groupe
                FindObjectOfType<Spawner>().spawnNext();
                enabled = false;
            }
        }
        //Déplacement vers le bas et chute
        else if(Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1){
            //Modifie la position
            transform.position += new Vector3(0, -1, 0);

            //check si valide
            if (IsValidGridPos())
            {
                //si valide, update la grille
                updateGrid();
            }
            else
            {
                //si pas valide, retour en arrière
                transform.position += new Vector3(0, 1, 0);

                //supprime les lignes remplies
                Grille.DeleteFullRows();

                //Spawn prochain groupe
                FindObjectOfType<Spawner>().spawnNext();

                //Disable script
                enabled = false;
            }

            lastFall = Time.time;
        }
        
        
	}

    public bool IsValidGridPos()
    {
        foreach(Transform child in transform)
        {
            Vector2 v = Grille.RoundVec2(child.position);

            //pas à l'intérieur des bordures ?
            if (!Grille.InsideBorder(v))
            {
                return false;
            }

            //Vérifie si les blocks font bien parties du même groupe
            if (Grille.grid[(int)v.x, (int)v.y] != null &&
                Grille.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    public void updateGrid() //empeche que les blocs se positionnent sur eux quand une rotation est effectuée
    {
        //supprime les anciens enfants de la grille
        for(int y = 0; y < Grille.h; ++y)
        {
            for(int x = 0; x < Grille.w; ++x)
            {
                if(Grille.grid[x,y] != null)
                {
                    if(Grille.grid[x,y].parent == transform)
                    {
                        Grille.grid[x, y] = null;
                    }
                }
            }
        }

        //ajoute les nouveaux enfants de la grille
        foreach(Transform child in transform)
        {
            Vector2 v = Grille.RoundVec2(child.position);
            Grille.grid[(int)v.x, (int)v.y] = child;
        }

        //On loop la grille pour vérifier si le block fait bien partie de la grille
        // en utilisant une propriété parent. Si le block parent est égal à la transformation
        //actuelle du groupe. Ensuite on loop de nouveau pour ajouter ces enfants à la grille.
    }
    
    
   
}
