using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class shoot : MonoBehaviour
{
    private Transform transform;

    public GameObject[] cards = new GameObject[4];

    public GameObject icon;
    public float iconSelectedSize = 1.1f;
    //The default width of a card
    public float iconWidth = 40f;

    GameObject inventory;

    public Color disabledColor;

    private ArrayList inventoryList = new ArrayList();
    private ArrayList isInInventory = new ArrayList();
    private ArrayList inventoryIcon = new ArrayList();
    private int selectedIndex;

    //Lets us control whether or not clicking has the player throw a card
    private bool enableShooting;

    void Start()
    {
        transform = GetComponent<Transform>();

        inventory = GameObject.FindGameObjectWithTag("Inventory");

        selectedIndex = 0;

        enableShooting = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && enableShooting)
        {
            throwCard();
        }

        //Detect mouse scrolling
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //If we scroll up, go down the inventory list
        if (scroll > 0) setSelectedIndex(selectedIndex + 1);
        //If we scroll down, go back up the inventory list
        if (scroll < 0) setSelectedIndex(selectedIndex - 1);

        //Check to see if any numkeys were pressed. If so, set index to that number
        for (int x = 0; x < inventoryList.Count; x++) {
            //Checks to see if 1 pressed, then 2, and so on (keycode of 1 is 49)
            if (Input.GetKey((KeyCode)49+x)) setSelectedIndex(x);
        }
    }

    public void setEnableShooting (bool newState) {
        enableShooting = newState;
    }

    private void setSelectedIndex(int newIndex) {
        //If we have nothing in our inventory. just don't change selectedIndex
        if (inventoryList.Count == 0) return;

        //Set the last icon back to normal
        ((GameObject)inventoryIcon[selectedIndex]).GetComponent<UnityEngine.UI.Image>().rectTransform.localScale = Vector3.one;

        //Sets selected index to a new value and have it loop around if it goes out of bound
        selectedIndex = newIndex;
        if (selectedIndex < 0) selectedIndex = inventoryList.Count - 1;
        if (selectedIndex >= inventoryList.Count) selectedIndex = 0;

        //Set the new icon back to be slightly bigger
        ((GameObject)inventoryIcon[selectedIndex]).GetComponent<UnityEngine.UI.Image>().rectTransform.localScale = Vector3.one * iconSelectedSize;
    }

    void throwCard ()
    {
        //If we do not have the selected card in our hand, (or we don't have any cards at all) we can't throw it
        if (inventoryList.Count == 0 || !(bool)isInInventory[selectedIndex]) return;

        //Create a new instance of a bullet
        GameObject newCard = Instantiate((GameObject)inventoryList[selectedIndex]);
        //Has the newly spawned card keep track of where it goes in your inventory
        newCard.GetComponent<cardMovement>().setIndex(selectedIndex);

        //Set its position and rotation to that of the players
        newCard.transform.position = transform.position;
        newCard.transform.rotation = transform.rotation;
        
        //The card is now out of our hand
        isInInventory[selectedIndex] = false;
        //So we set it's icon to disabled
        ((GameObject)inventoryIcon[selectedIndex]).GetComponent<UnityEngine.UI.Image>().color = disabledColor;

        //Play the card throw sound
        FindObjectOfType<AudioManager>().playSound("Card Throw");
    }

    public void pickupCard (int inventoryIndex) {
        //When we pick the card back up, it is back in our hand
        isInInventory[inventoryIndex] = true;

        //Set icon's opacity back to full
        ((GameObject)inventoryIcon[inventoryIndex]).GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }

    public void addCard (int cardIndex) {
        GameObject card = cards[cardIndex];

        //Adds a new card to our inventory
        inventoryList.Add(card);
        isInInventory.Add(true);
        //We must also display this in the inventory, so create a new icon to display
        GameObject newIcon = Instantiate(icon, inventory.transform);
        //Set it's image to that of the card we added to the inventory
        newIcon.GetComponent<UnityEngine.UI.Image>().sprite = card.GetComponent<SpriteRenderer>().sprite;
        //And move it to the appropriate place along the bottom depending on what number card it is.
        //We get the width of the icon and for each icon have it go 1/10 of the screen width * the width of the icon for equal spacing.
        newIcon.transform.position += (Vector3.right * iconWidth) * (inventoryList.Count - 1);

        inventoryIcon.Add(newIcon);

        //If this is our first card, select it
        if (inventoryList.Count == 1) {
            //Set the first icon back to be slightly bigger (selected)
            ((GameObject)inventoryIcon[0]).GetComponent<UnityEngine.UI.Image>().rectTransform.localScale = Vector3.one * iconSelectedSize;
        }
    }
}
