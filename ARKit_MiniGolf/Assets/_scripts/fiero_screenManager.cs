using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class fiero_screenManager : MonoBehaviour {

	public static fiero_screenManager instance;

	//Transition Types
	// 0 - Slide left
	// 1 - Slide right
	// 2 - Slide up
	// 3 - Slide down
	// 4 - Fade
	// 5 - Instant

	//Screen to open automatically at the start of the Scene
	public Animator initiallyOpen;

	//Currently Open Screen
	public Animator m_Open;
	public Animator last_Open;

	//Hash of the parameter we use to control the transitions.
	private int m_OpenParameterId;

	//The GameObject Selected before we opened the current Screen.
	//Used when closing a Screen, so we can go back to the button that opened it.
	private GameObject m_PreviouslySelected;

	//Animator State and Transition names we need to check against.
	const string k_OpenTransitionName = "active";
	const string k_ClosedStateName = "screenOff";

	public Animator screen_mainmenu;
    public Animator screen_courseSelect;
    public Animator screen_levelSelect;

	public Animator screen_play_placeCourse;
	public Animator screen_play_placeBall;
	public Animator screen_play;
	public Animator screen_play_pause;
	public Animator screen_play_outOfBounds;
	public Animator screen_play_maxHitCount;

	public Animator screen_completeCourse;
	public Animator screen_completeRound;

    public Animator screen_about;
    public Animator screen_thankYou;
    public Animator screen_supportTheDevs;
    public Animator screen_share;

    void Awake(){
		instance = this;

		screen_mainmenu.gameObject.SetActive (false);
        screen_courseSelect.gameObject.SetActive (false);
        screen_levelSelect.gameObject.SetActive (false);

		screen_play_placeCourse.gameObject.SetActive (false);
		screen_play_placeBall.gameObject.SetActive (false);
		screen_play_pause.gameObject.SetActive (false);
		screen_play.gameObject.SetActive (false);
		screen_play_outOfBounds.gameObject.SetActive (false);
		screen_play_maxHitCount.gameObject.SetActive (false);

		screen_completeCourse.gameObject.SetActive (false);
		screen_completeRound.gameObject.SetActive (false);

        screen_about.gameObject.SetActive(false);
        screen_thankYou.gameObject.SetActive(false);
        screen_supportTheDevs.gameObject.SetActive(false);
        screen_share.gameObject.SetActive(false);
    }

	void Start(){
		//OpenPanel (screen_mainMenu, 5);
		//OpenPanelOverTop (screen_splash, 5);
		//OpenPanel (screen_setup, 5);
	}

	public void OnEnable()
	{
		//We cache the Hash to the "Open" Parameter, so we can feed to Animator.SetBool.
		m_OpenParameterId = Animator.StringToHash (k_OpenTransitionName);

		//If set, open the initial Screen now.
		if (initiallyOpen == null)
			return;
		OpenPanel(initiallyOpen,5);
	}

	//Closes the currently open panel and opens the provided one.
	//It also takes care of handling the navigation, setting the new Selected element.
	public void OpenPanel (Animator anim)
	{
		if (m_Open == anim)
			return;

		//Activate the new Screen hierarchy so we can animate it.
		anim.gameObject.SetActive(true);
		//Save the currently selected button that was used to open this Screen. (CloseCurrent will modify it)
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
		//Move the Screen to front.
		anim.transform.SetAsLastSibling();

		print ("OpenPanel:" + m_Open);
		last_Open = m_Open;

		CloseCurrent();

		m_PreviouslySelected = newPreviouslySelected;


		//Set the new Screen as then open one.
		m_Open = anim;
		//Start the open animation
		m_Open.SetBool(m_OpenParameterId, true);

		//Set an element in the new screen as the new Selected one.
		GameObject go = FindFirstEnabledSelectable(anim.gameObject);
		SetSelected(go);
	}

	public void OpenPanel (Animator anim, int transitionType)
	{
		if (m_Open == anim)
			return;

		//Activate the new Screen hierarchy so we can animate it.
		anim.gameObject.SetActive(true);
		//Save the currently selected button that was used to open this Screen. (CloseCurrent will modify it)
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
		//Move the Screen to front.
		anim.transform.SetAsLastSibling();

//		print ("OpenPanel:" + m_Open);
		last_Open = m_Open;

		CloseCurrent(transitionType);

		m_PreviouslySelected = newPreviouslySelected;


		//Set the new Screen as then open one.
		m_Open = anim;
		//Start the open animation
		m_Open.SetInteger("transitionType",transitionType);
		m_Open.SetBool(m_OpenParameterId, true);

		//Set an element in the new screen as the new Selected one.
		GameObject go = FindFirstEnabledSelectable(anim.gameObject);
		SetSelected(go);
	}

	public void OpenPanelOverTop (Animator anim)
	{
		if (m_Open == anim)
			return;

		//Activate the new Screen hierarchy so we can animate it.
		anim.gameObject.SetActive(true);
		//Save the currently selected button that was used to open this Screen. (CloseCurrent will modify it)
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
		//Move the Screen to front.
		anim.transform.SetAsLastSibling();

		m_PreviouslySelected = newPreviouslySelected;

		last_Open = m_Open;

		//Set the new Screen as then open one.
		m_Open = anim;
		//Start the open animation
		m_Open.SetBool(m_OpenParameterId, true);

		//Set an element in the new screen as the new Selected one.
		GameObject go = FindFirstEnabledSelectable(anim.gameObject);
		SetSelected(go);
	}

	public void OpenPanelOverTop (Animator anim, int transitionType)
	{
		if (m_Open == anim)
			return;

		//Activate the new Screen hierarchy so we can animate it.
		anim.gameObject.SetActive(true);
		//Save the currently selected button that was used to open this Screen. (CloseCurrent will modify it)
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
		//Move the Screen to front.
		anim.transform.SetAsLastSibling();

		m_PreviouslySelected = newPreviouslySelected;

		last_Open = m_Open;

		//Set the new Screen as then open one.
		m_Open = anim;
		//Start the open animation
		m_Open.SetInteger("transitionType",transitionType);
		m_Open.SetBool(m_OpenParameterId, true);

		//Set an element in the new screen as the new Selected one.
		GameObject go = FindFirstEnabledSelectable(anim.gameObject);
		SetSelected(go);
	}

	//Finds the first Selectable element in the providade hierarchy.
	static GameObject FindFirstEnabledSelectable (GameObject gameObject)
	{
		GameObject go = null;
		var selectables = gameObject.GetComponentsInChildren<Selectable> (true);
		foreach (var selectable in selectables) {
			if (selectable.IsActive () && selectable.IsInteractable ()) {
				go = selectable.gameObject;
				break;
			}
		}
		return go;
	}

	//Closes the currently open Screen
	//It also takes care of navigation.
	//Reverting selection to the Selectable used before opening the current screen.
	public void CloseCurrent()
	{
		if (m_Open == null)
			return;

		//Start the close animation.
		m_Open.SetBool(m_OpenParameterId, false);

		//Reverting selection to the Selectable used before opening the current screen.
		SetSelected(m_PreviouslySelected);
		//Start Coroutine to disable the hierarchy when closing animation finishes.
		StartCoroutine(DisablePanelDeleyed(m_Open));

		Animator currentOpen = m_Open;

		if (last_Open != null) {
			m_Open = last_Open;
		} else {
			//No screen open.
			m_Open = null;
		}

		last_Open = currentOpen;

	}

	public void CloseCurrent(int transitionType)
	{
		if (m_Open == null)
			return;

		//Start the close animation.
		m_Open.SetInteger("transitionType",transitionType);
		m_Open.SetBool(m_OpenParameterId, false);

		//Reverting selection to the Selectable used before opening the current screen.
		SetSelected(m_PreviouslySelected);
		//Start Coroutine to disable the hierarchy when closing animation finishes.
		StartCoroutine(DisablePanelDeleyed(m_Open));

		Animator currentOpen = m_Open;

		if (last_Open != null) {
			m_Open = last_Open;
		} else {
			//No screen open.
			m_Open = null;
		}

		last_Open = currentOpen;

	}

	public void CloseSpecific(Animator anim){
		anim.SetBool(m_OpenParameterId, false);
		StartCoroutine(DisablePanelDeleyed(anim));

		if (last_Open != null) {
			m_Open = last_Open;
		} else {
			//No screen open.
			m_Open = null;
		}

		last_Open = anim;
	}

	public void CloseSpecific(Animator anim, int transitionType){
		m_Open.SetInteger("transitionType",transitionType);
		anim.SetBool(m_OpenParameterId, false);
		StartCoroutine(DisablePanelDeleyed(anim));

		if (last_Open != null) {
			m_Open = last_Open;
		} else {
			//No screen open.
			m_Open = null;
		}

		last_Open = anim;
	}

	//Coroutine that will detect when the Closing animation is finished and it will deactivate the
	//hierarchy.
	IEnumerator DisablePanelDeleyed(Animator anim)
	{
		bool closedStateReached = false;
		bool wantToClose = true;
		while (!closedStateReached && wantToClose)
		{
			if (!anim.IsInTransition(0))
				closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

			wantToClose = !anim.GetBool(m_OpenParameterId);

			yield return new WaitForEndOfFrame();
		}

		if (wantToClose)
			anim.gameObject.SetActive(false);
	}

	//Closes the currently open Screen
	//It also takes care of navigation.
	//Reverting selection to the Selectable used before opening the current screen.
	public void CloseCurrentThenOpen(Animator anim)
	{
		if (m_Open == null)
			return;

		//Start the close animation.
		m_Open.SetBool(m_OpenParameterId, false);

		//Reverting selection to the Selectable used before opening the current screen.
		SetSelected(m_PreviouslySelected);
		//Start Coroutine to disable the hierarchy when closing animation finishes.
		StartCoroutine(DisablePanelDeleyedThenOpenOverTop(m_Open,anim));


		if (last_Open != null) {
			m_Open = last_Open;
		} else {
			//No screen open.
			m_Open = null;
		}
	}

	IEnumerator DisablePanelDeleyedThenOpenOverTop(Animator anim, Animator toOpenAnim)
	{
		bool closedStateReached = false;
		bool wantToClose = true;
		while (!closedStateReached && wantToClose)
		{
			if (!anim.IsInTransition(0))
				closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

			wantToClose = !anim.GetBool(m_OpenParameterId);

			yield return new WaitForEndOfFrame();
		}

		if (wantToClose) {
			anim.gameObject.SetActive (false);

			OpenPanelOverTop (toOpenAnim);
		}
	}

	//Make the provided GameObject selected
	//When using the mouse/touch we actually want to set it as the previously selected and 
	//set nothing as selected for now.
	private void SetSelected(GameObject go)
	{
		//Select the GameObject.
		EventSystem.current.SetSelectedGameObject(go);

		//If we are using the keyboard right now, that's all we need to do.
		var standaloneInputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
		if (standaloneInputModule != null && standaloneInputModule.inputMode == StandaloneInputModule.InputMode.Buttons)
			return;

		//Since we are using a pointer device, we don't want anything selected. 
		//But if the user switches to the keyboard, we want to start the navigation from the provided game object.
		//So here we set the current Selected to null, so the provided gameObject becomes the Last Selected in the EventSystem.
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void SetPanelToFront(Animator anim){
		anim.transform.SetAsLastSibling();
	}

	public void OutsideOfScreenFlow_OpenPanelOverTop (Animator anim, int transitionType)
	{
		if (m_Open == anim)
			return;

		//Activate the new Screen hierarchy so we can animate it.
		anim.gameObject.SetActive(true);
		//Save the currently selected button that was used to open this Screen. (CloseCurrent will modify it)
		var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
		//Move the Screen to front.
		anim.transform.SetAsLastSibling();

		m_PreviouslySelected = newPreviouslySelected;

		//Start the open animation
		anim.SetInteger("transitionType",transitionType);
		anim.SetBool(m_OpenParameterId, true);

		//Set an element in the new screen as the new Selected one.
		GameObject go = FindFirstEnabledSelectable(anim.gameObject);
		SetSelected(go);
	}

	//No record of opening, doesn't mess up last open flow
	public void OutsideOfScreenFlow_CloseSpecific(Animator anim, int transitionType){
		m_Open.SetInteger("transitionType",transitionType);
		anim.SetBool(m_OpenParameterId, false);
		StartCoroutine(DisablePanelDeleyed(anim));
	}


}