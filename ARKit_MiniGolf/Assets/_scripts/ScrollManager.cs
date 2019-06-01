using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour {

	public ScrollRectSnap _scrollRectSnap;

	public Transform _contentParent;

	public GameObject _scrollBufferPrefab;
	public GameObject _scrollObjectPrefab;
    

	void Awake(){
		_scrollRectSnap = gameObject.GetComponent<ScrollRectSnap>();
		_contentParent = this.transform.Find ("Viewport/Content").transform;

		//_contentParent = GameObject.fin
		//SetupBuffers ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetupBuffers (){
//		print ("SetupBuffers");
		float bufferWidth;

		bufferWidth = (this.GetComponent<RectTransform> ().sizeDelta.x - _scrollObjectPrefab.GetComponent<LayoutElement> ().minWidth - (_contentParent.GetComponent<HorizontalLayoutGroup> ().spacing * 2f)) * 0.5f;
//		print ("bufferWidth" + bufferWidth);
		GameObject buffer;

		if (_contentParent.GetChild (0).name != _scrollBufferPrefab.name) {
			buffer = Instantiate (_scrollBufferPrefab) as GameObject;
			buffer.transform.SetParent (_contentParent);
			buffer.transform.localScale = new Vector3 (1f, 1f, 1f);
			buffer.transform.SetAsFirstSibling ();
		} else {
			buffer = _contentParent.GetChild (0).gameObject;
		}
		buffer.GetComponent<LayoutElement> ().minWidth = bufferWidth;

		if (_contentParent.GetChild (_contentParent.childCount - 1).name != _scrollBufferPrefab.name) {

			buffer = Instantiate (_scrollBufferPrefab) as GameObject;
			buffer.transform.SetParent (_contentParent);
			buffer.transform.localScale = new Vector3 (1f, 1f, 1f);
			buffer.transform.SetAsLastSibling ();
		}else {
			buffer = _contentParent.GetChild (_contentParent.childCount - 1).gameObject;
		}
		buffer.GetComponent<LayoutElement> ().minWidth = bufferWidth;
	}

	public void OnDrag(){
		_scrollRectSnap.OnDrag ();
	}

	public void DragEnd(){
		_scrollRectSnap.DragEnd ();
	}
}
