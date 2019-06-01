using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour {

	public float[] points;
	[Tooltip("how many screens or pages are there within the content (steps)")]
	public int screens = 1;
	[Tooltip("How quickly the GUI snaps to each panel")]
	public float snapSpeed;
	public float inertiaCutoffMagnitude;
	float stepSize;

	ScrollRect scroll;
	public bool LerpH;
	public float targetH;
	[Tooltip("Snap horizontally")]
	public bool snapInH = true;

    public bool LerpV;
	public float targetV;
	[Tooltip("Snap vertically")]
	public bool snapInV = true;

	bool dragInit = true;
	int dragStartNearest;

	public bool canSnap = true;

	public int currentScreen;

    public delegate void FinishScrollSnap(int cScreen);
    public FinishScrollSnap finishScrollSnap;

    // Use this for initialization
    void Start()
	{
		scroll = gameObject.GetComponent<ScrollRect>();
		scroll.inertia = true;

		if (screens > 0)
		{
			points = new float[screens];
			stepSize = 1 / (float)(screens - 1);

			for (int i = 0; i < screens; i++)
			{
				points[i] = i * stepSize;
			}
		}
		else
		{
			points[0] = 0;
		}

		ForceScroll (1);
	}

	void Update()
	{
      //  print("scroll.horizontalNormalizedPosition:" + scroll.horizontalNormalizedPosition);

        if (canSnap) {
			if (LerpH) {
				scroll.horizontalNormalizedPosition = Mathf.Lerp (scroll.horizontalNormalizedPosition, targetH, snapSpeed * Time.deltaTime);
                /*if (Mathf.Approximately(scroll.horizontalNormalizedPosition, targetH))
                {
                    print("finishSnap H");
                    LerpH = false;
                    finishScrollSnap(currentScreen);
                }*/
                if(equalsApproximately(scroll.horizontalNormalizedPosition, targetH, 0.005f)) {
                    LerpH = false;
                    scroll.horizontalNormalizedPosition = targetH;
                    finishScrollSnap(currentScreen);
                }
            }
			if (LerpV) {
				scroll.verticalNormalizedPosition = Mathf.Lerp (scroll.verticalNormalizedPosition, targetV, snapSpeed * Time.deltaTime);
                /*if (Mathf.Approximately(scroll.verticalNormalizedPosition, targetV))
                {
                    print("finishSnap V");
                    LerpV = false;
                    finishScrollSnap(currentScreen);
                }*/
                if (equalsApproximately(scroll.verticalNormalizedPosition, targetV, 0.005f))
                {
                    LerpV = false;
                    scroll.verticalNormalizedPosition = targetV;
                    finishScrollSnap(currentScreen);
                }
            }
		}
	}

    private bool equalsApproximately(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

    public void SetSnapPoints(int numberOfScreens){
		//carousel.uiwidth
		screens = numberOfScreens;
		points = new float[screens];
		stepSize = 1/(float)(screens-1);

		//float currentMidPoint = carousel.uiwidth / 2.0f;
		for (int i = 0; i < screens; i++) {
			points[i] = i * stepSize;
		}
	}

	public void ForceScroll(int toScreen){
		targetH = points[toScreen-1];
		LerpH = true;

		currentScreen = toScreen;
	}

	public void DragEnd()
	{
//		print ("drag end:" + scroll.horizontalNormalizedPosition);

		int target = FindNearest(scroll.horizontalNormalizedPosition, points);

		if (target == dragStartNearest && scroll.velocity.sqrMagnitude > inertiaCutoffMagnitude * inertiaCutoffMagnitude)
		{
			if (scroll.velocity.x < 0)
			{
				target = dragStartNearest + 1;
			}
			else if (scroll.velocity.x > 1)
			{
				target = dragStartNearest - 1;
			}
			target = Mathf.Clamp(target, 0, points.Length - 1);
		}

		if (scroll.horizontal && snapInH)
		{
			targetH = points[target];
			LerpH = true;

			currentScreen = target+1;
		}

		if (scroll.vertical && snapInV && scroll.verticalNormalizedPosition > 0f && scroll.verticalNormalizedPosition < 1f)
		{
			targetH = points[target];
			LerpH = true;
		}

		dragInit = true;
	}

	public void OnDrag()
	{
		if (dragInit)
		{
			dragStartNearest = FindNearest(scroll.horizontalNormalizedPosition, points);
			dragInit = false;
		}

		LerpH = false;
		LerpV = false;
	}

	int FindNearest(float f, float[] array)
	{
		float distance = Mathf.Infinity;
		int output = 0;
		for (int index = 0; index < array.Length; index++)
		{
			if (Mathf.Abs(array[index] - f) < distance)
			{
				distance = Mathf.Abs(array[index] - f);
				output = index;
			}
		}
		return output;
	}
}

