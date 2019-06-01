using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class golf_courseButton_clickHandler : MonoBehaviour, IPointerUpHandler, IPointerClickHandler {

	public void OnPointerUp (PointerEventData eventData) {
		// Do action
		print("OnPointerUp");
		golf_levelSelect_ScrollManager.instance._scrollManager.DragEnd();
	}

	public void OnPointerClick(PointerEventData eventData) {
		//this.GetComponent<golf_courseButton>().Select ();
	}
}
