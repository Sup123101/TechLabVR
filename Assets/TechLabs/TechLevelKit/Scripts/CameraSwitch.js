public var topCamera : Camera ;
public var fpCamera : Camera ;
var Player : GameObject;
function Update() {


	//this is a hard wired connection to the "F1" Key on the keyboard, switch it any keyboard key if you like
	//this will disable the camera inside of the car, and enable the camera outside of the vehicle
if(Input.GetMouseButtonDown(0))
    {
        fpCamera.GetComponent.<Camera>().enabled = true;
        topCamera.GetComponent.<Camera>().enabled = false;
        //Player.GetComponent(MouseLook).enabled = true;
    }
	//this is a hard wired connection to the "F2" Key on the keyboard, switch it any keyboard key if you like
	//this will disable the camera behind the car, and enable the camera inside of the vehicle
if(Input.GetMouseButtonDown(1))
    {
        topCamera.GetComponent.<Camera>().enabled = true;
        fpCamera.GetComponent.<Camera>().enabled = false;
     	transform.rotation = Quaternion.identity;
        //Player.GetComponent(MouseLook).enabled = false;
    }
}