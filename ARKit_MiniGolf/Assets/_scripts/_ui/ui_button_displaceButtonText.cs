using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ui_button_displaceButtonText : MonoBehaviour {

	public int offsetX = 0, offsetY = 2;
	public RectTransform textRect;
	Vector3 pos;

	void Start()
	{
		pos = textRect.localPosition;
		/*
		EventTrigger trigger = GetComponentInParent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener( (eventData) => { Down(); } );
		trigger.triggers.Add(entry);

		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener( (eventData) => { Up(); } );
		trigger.triggers.Add(entry);
		*/
	}

	public void Down()
	{
		textRect.localPosition = new Vector3(pos.x + (float)offsetX, pos.y - (float)offsetY, pos.z);
	}

	public void Up()
	{
		textRect.localPosition = pos;
	}
}
