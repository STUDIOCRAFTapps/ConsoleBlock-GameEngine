using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System;

//Note:
//
//There's a lot of bug due to the Chunk system. Some of the hit.collider.transform need to be replaced by Block.FindBlock(hit.collider.transform) and some of the
//hit.transform need to be replace with Block.FindChunk to get accurate result.
//

public class Player : MonoBehaviour {

	public RectTransform ConsoleUI;
	public RectTransform EDGestionnairyUI;
	public RectTransform TextField;
	public RectTransform Popup;
	public Console SelectedConsole;
	public Transform PlayerMove;
	public Transform CanvasPos;
	public Transform Canvas;

	public Material ADVPlaceOlder;
	public Material DefaultPlaceOlder;

	public Text StringMessage;
	public RectTransform TextFieldConnector;

	GameObject InputN;
	GameObject OutputN;
	GameObject PlaceOlder;
	
	public Text ColliderUI;

	public Rigidbody PlayerMain;
	ObjectSystem InventoryAccess;
	PlaceOlder GetPO;
	float Size = 1;

	public bool PositionLock;
	Vector3 SP = new Vector3(0, 0, 0);
	bool IsSitting;

	bool StrikeMovement;

	public string WTMode;
	UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController controller;

	Collider savecon;

	byte StatusPos = 0;
	byte StatusDir = 0;

	const string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	string Name;
	GameObject connectorMemory;
	GameObject EconnectorMemory;

	//public UIBuilder solarPanelProductionBoard;
	
	// Use this for initialization
	void Start () {
		//solarPanelProductionBoard = new UIBuilder();
		controller = transform.parent.GetComponent<RigidbodyFirstPersonController>();
		InventoryAccess = gameObject.GetComponentInParent<ObjectSystem>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		ConsoleUI.gameObject.SetActive(false);
		TextField.gameObject.SetActive(false);
		PlaceOlder = (GameObject)Instantiate(Resources.Load("user.placeOlder"), Vector3.zero, Quaternion.identity);
		GetPO = PlaceOlder.GetComponent<PlaceOlder>();
	}

	// Update is called once per frame
	void Update () {
		if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] != "item.IOSystem" && connectorMemory != null) {
			if(connectorMemory.GetComponent<OutputConnecter>()) {
				connectorMemory.GetComponent<OutputConnecter>().IsSelected = false;
			} else if(connectorMemory.GetComponent<InputConnector>()) {
				connectorMemory.GetComponent<InputConnector>().IsSelected = false;
			}
		}
		if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] != "item.EnergyJoiner" && EconnectorMemory != null) {
			if(EconnectorMemory.GetComponent<EnergyOutputSaver>()) {
				EconnectorMemory.GetComponent<EnergyOutputSaver>().IsSelected = false;
			} else if(EconnectorMemory.GetComponent<EnergyInputSaver>()) {
				EconnectorMemory.GetComponent<EnergyInputSaver>().IsSelected = false;
			}
		}

		if(Input.GetKeyDown(KeyCode.R)) {
			StatusPos++; 
			if(StatusPos >= 2) {
				StatusPos = 0;
			}
		}
		if(Input.GetKeyDown(KeyCode.T)) {
			StatusDir++; 
			if(StatusDir >= 4) {
				StatusDir = 0;
			}
		}
		if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == null) {
			if(PlaceOlder.activeInHierarchy == true) {
				PlaceOlder.SetActive(false);
			}
		} else {
			if(PlaceOlder.activeInHierarchy == false) {
				PlaceOlder.SetActive(true);
			}
		}
		//Debug.Log(WTMode);
		if(SelectedConsole == null && string.IsNullOrEmpty(WTMode)) {
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.lockState = CursorLockMode.None;
		}
		if(Input.GetKeyDown(KeyCode.LeftShift) && PositionLock && IsSitting) {          //>,<
			transform.parent.parent = null;
			IsSitting = false;
			//Debug.Log(transform.parent.position + " / " + SP);
			transform.parent.position = SP;
			PositionLock = false;
			transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		}
		if(Input.GetKeyDown(KeyCode.Escape) && !IsSitting) {
			if(SelectedConsole != null) {
				if(Canvas.gameObject.GetComponent<CanvasObjects>().childs[5].transform.GetChild(1).gameObject.activeInHierarchy) {
					TextField.GetComponent<InputField>().text = SelectedConsole.Text;
					ConsoleUI.gameObject.SetActive(false);
					TextField.gameObject.SetActive(false);
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					SelectedConsole.isOp = false;
					SelectedConsole = null;

					controller.cam = transform.GetComponent<Camera>();
					WTMode = "";
				}
			}
			if(WTMode == "EDG") {
				//Debug.Log("Nuh");
				controller.cam = transform.GetComponent<Camera>();
				WTMode = null;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				EDGestionnairyUI.gameObject.SetActive(false);
				PositionLock = false;
				PlayerMain.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		if(SelectedConsole != null) {
			PlayerMain.GetComponent<Rigidbody>().isKinematic = true;
			//Time.timeScale = 1.0f;
			SelectedConsole.Text = TextField.GetComponent<InputField>().text;
		} else {
			if(!PositionLock) {
				PlayerMain.GetComponent<Rigidbody>().isKinematic = false;
				//Time.timeScale = 0.0f;
			}
		}
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) {
			if(Input.GetMouseButtonDown(0)) {
				if(WTMode == "" && hit.collider.tag != "Unbreakable" && SelectedConsole == null) {
					Destroy(Build.FindBlock(hit.collider.transform).gameObject);
				}
			}
			if(WTMode == "EDG") {
				if(string.IsNullOrEmpty(EDGestionnairyUI.GetComponent<InputField>().text)) {
					EDGestionnairyUI.GetComponent<InputField>().text = "0";
				}
				EDGestionnairyUI.GetComponent<InputField>().text = Mathf.Clamp(int.Parse(EDGestionnairyUI.GetComponent<InputField>().text),0f,100f).ToString();
				EDGestionnairyUI.GetChild(4).GetComponent<Image>().fillAmount = (int.Parse(EDGestionnairyUI.GetComponent<InputField>().text)/100f);
				EDGestionnairyUI.GetChild(5).GetComponent<Text>().text = int.Parse(EDGestionnairyUI.GetComponent<InputField>().text).ToString() + " %";
				EDGestionnairyUI.GetChild(6).GetComponent<Text>().text = (100-int.Parse(EDGestionnairyUI.GetComponent<InputField>().text)).ToString() + " %";

				hit.collider.GetComponent<EnergyDiviser>().Percent1 = int.Parse(EDGestionnairyUI.GetComponent<InputField>().text);
			}
			//Debug.Log(hit.collider.transform.name);
			if(hit.transform.tag == "Stabilize") {
				//Debug.Log(hit.transform.parent.parent.parent.name);
				if(hit.transform.name == "VPositionner") {
					hit.transform.parent.parent.parent.GetComponent<VehiculeControllerScript>().Static = true;
				} else if(hit.transform.name == "Stabilizer") {
					hit.transform.parent.GetChild(0).GetComponent<JoystickController>().Static = true;
				}
				StrikeMovement = true;
				savecon = hit.collider;
			} else {
				if(savecon != null) {
					if(savecon.name == "VPositionner") {
						savecon.transform.parent.parent.parent.GetComponent<VehiculeControllerScript>().Static = false;
					} else if(savecon.name == "Stabilizer") {
						savecon.transform.parent.GetChild(0).GetComponent<JoystickController>().Static = false;
					}
					savecon = null;
				}
				StrikeMovement = false;
			}
			if(Input.GetKeyDown(KeyCode.Escape) && WTMode == "Battry") {
				controller.cam = transform.GetComponent<Camera>();
				WTMode = null;
				CanvasPos.GetComponent<CanvasObjects>().childs[9].gameObject.SetActive(false);
				hit.collider.GetComponent<BattryOperator>().IsOpen = false;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				PositionLock = false;
				PlayerMain.GetComponent<Rigidbody>().isKinematic = false;
				//Time.timeScale = 0.0f;
			}
			if(Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift)) {
				if(hit.collider.tag == "Button") {
					hit.collider.transform.GetComponent<ButtonObject>().GetButton = true;
				}
			} else {
				if(hit.collider.tag == "Button") {
					hit.collider.transform.GetComponent<ButtonObject>().GetButton = false;
				}
			}
			if(Input.GetMouseButtonUp(1) && !Input.GetKey(KeyCode.LeftShift)) {
				if(hit.collider.tag == "Button") {
					hit.collider.transform.GetComponent<ButtonObject>().GetButtonUp = true;
				}
			} else {
				if(hit.collider.tag == "Button") {
					hit.collider.transform.GetComponent<ButtonObject>().GetButtonUp = false;
				}
			}
			if(Input.GetMouseButtonDown(1)) {
				if(hit.collider.tag == "EnergyDiviser" && !Input.GetKey(KeyCode.LeftShift)) {
					controller.cam = null;
					EDGestionnairyUI.gameObject.SetActive(true);
					PositionLock = true;
					transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					WTMode = "EDG";
				}
				if(hit.collider.tag == "Switch" && !Input.GetKey(KeyCode.LeftShift)) {
					hit.collider.transform.GetComponent<SwitchObject>().IsPress = !hit.collider.transform.GetComponent<SwitchObject>().IsPress;
					hit.collider.transform.GetComponent<SwitchObject>().Updated = true;
				}
				if(hit.collider.tag == "Button" && !Input.GetKey(KeyCode.LeftShift)) {
					hit.collider.transform.GetComponent<ButtonObject>().GetButtonDown = true;
				}
			} else if(hit.collider.tag == "Button" && !Input.GetKey(KeyCode.LeftShift)) {
				hit.collider.transform.GetComponent<ButtonObject>().GetButtonDown = false;
			}
			if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] != null) {
				if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount].StartsWith("block.")) {
					if(PlaceOlder.GetComponent<MeshRenderer>().enabled == false) {
						PlaceOlder.GetComponent<MeshRenderer>().enabled = true;
					}
					PlaceOlder.GetComponent<Renderer>().material = DefaultPlaceOlder;
					Bounds bounds = (Resources.Load(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount]) as GameObject).transform.GetChild(0).GetComponent<Renderer>().bounds;

					if(hit.transform.tag == "Chunk") {
						PlaceOlder.transform.position = BlockBuildingMath.AlingV3(hit.point, 0.1f, hit.transform.position);
					} else {
						PlaceOlder.transform.position = BlockBuildingMath.AlingV3(hit.point, 0.1f, Vector3.zero);
					}
					PlaceOlder.transform.localScale = new Vector3(bounds.size.x,bounds.size.y,bounds.size.z);
					if(StatusPos == 0) {
						PlaceOlder.transform.rotation = Quaternion.Euler(new Vector3(0, RoundToBound(PlayerMove.eulerAngles.y - 270, 90), 0));
					} else {
						PlaceOlder.transform.rotation = Quaternion.Euler(new Vector3(StatusDir*-90, RoundToBound(PlayerMove.eulerAngles.y - 270, 90), -90));
					}
				}
				if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount].StartsWith("material.")) {
					if(PlaceOlder.GetComponent<MeshRenderer>().enabled == false) {
						PlaceOlder.GetComponent<MeshRenderer>().enabled = true;
					}
					RaycastHit Down;
					RaycastHit Up;
					RaycastHit Left;
					RaycastHit Right;
					RaycastHit Forward;
					RaycastHit Backward;
					Vector3 Direction = new Vector3(0,0,0);

					//Down +
					if(Physics.Raycast(hit.point + (Vector3.up * 0.01f), Vector3.down, out Down)) {
						if(Down.distance < 0.5f) {
							if(!Input.GetKey(KeyCode.Tab)) {
								Direction.y += (Size / 2);
							} else if(Input.GetKey(KeyCode.Tab)) {
								Direction.y = (hit.transform.localScale.y / 2f + Size / 2f);
							}
						} else if(Down.distance > 0.5f && Input.GetKey(KeyCode.Tab)) {
							Direction.y += 0;
						}
					} else if(Input.GetKey(KeyCode.Tab)) {
						Direction.y += 0;
					}

					//Up - (Working)
					if(Physics.Raycast(hit.point + (Vector3.down * 0.01f), Vector3.up, out Up)) {
						if(Up.distance < 0.5f) {
							if(!Input.GetKey(KeyCode.Tab)) {
								Direction.y -= (Size / 2);
							} else if(Input.GetKey(KeyCode.Tab)) {
								Direction.y = (hit.transform.localScale.y / 2f + Size / 2f) * -1;
							}
						} else if(Down.distance > 0.5f && Input.GetKey(KeyCode.Tab)) {
							Direction.y += 0;
						}
					} else if(Input.GetKey(KeyCode.Tab)) {
						Direction.y += 0;
					}

					//Left +
					if(Physics.Raycast(hit.point + (Vector3.right * 0.01f), Vector3.left, out Left)) {
						if(Left.distance < 0.5f) {
							if(!Input.GetKey(KeyCode.Tab)) {
								Direction.x += (Size / 2);
							} else if(Input.GetKey(KeyCode.Tab)) {
								Direction.x = (hit.transform.localScale.x / 2f + Size / 2f);
							}
						} else if(Down.distance > 0.5f && Input.GetKey(KeyCode.Tab)) {
							Direction.x += 0;
						}
					} else if(Input.GetKey(KeyCode.Tab)) {
						Direction.x += 0;
					}

					//Right - (Working)
					if(Physics.Raycast(hit.point + (Vector3.left * 0.01f), Vector3.right, out Right)) {
						if(Left.distance < 0.5f) {
							if(!Input.GetKey(KeyCode.Tab)) {
								Direction.x -= (Size / 2);
							} else if(Input.GetKey(KeyCode.Tab)) {
								Direction.x = (hit.transform.localScale.x / 2f + Size / 2f) * -1;
							}
						} else if(Down.distance > 0.5f && Input.GetKey(KeyCode.Tab)) {
							Direction.x += 0;
						}
					} else if(Input.GetKey(KeyCode.Tab)) {
						Direction.x += 0;
					}

					//Forward - (Working)
					if(Physics.Raycast(hit.point + (Vector3.back * 0.01f), Vector3.forward, out Forward)) {
						if(Forward.distance < 0.5f) {
							if(!Input.GetKey(KeyCode.Tab)) {
								Direction.z -= (Size / 2);
							} else if(Input.GetKey(KeyCode.Tab)) {
								Direction.z = (hit.transform.localScale.z / 2f + Size / 2f) * -1;
							}
						} else if(Down.distance > 0.5f && Input.GetKey(KeyCode.Tab)) {
							Direction.z += 0;
						}
					} else if(Input.GetKey(KeyCode.Tab)) {
						Direction.z += 0;
					}

					//Backward +
					if(Physics.Raycast(hit.point + (Vector3.forward * 0.01f), Vector3.back, out Backward)) {
						if(Backward.distance < 0.5f) {
							if(!Input.GetKey(KeyCode.Tab)) {
								Direction.z += (Size / 2);
							} else if(Input.GetKey(KeyCode.Tab)) {
								Direction.z = (hit.transform.localScale.z / 2f + Size / 2f);
							}
						} else if(Down.distance > 0.5f && Input.GetKey(KeyCode.Tab)) {
							Direction.z += 0;
						}
					} else if(Input.GetKey(KeyCode.Tab)) {
						Direction.z += 0;
					}

					Vector3 DirectionModif = BlockBuildingMath.FindPosition(hit.collider.transform.position, hit.point);
					DirectionModif = new Vector3(DirectionModif.x * ((hit.collider.transform.lossyScale.x/2) + 0.5f), DirectionModif.y * ((hit.collider.transform.lossyScale.y/2) + 0.5f), DirectionModif.z * ((hit.collider.transform.lossyScale.z/2) + 0.5f)); //0.5f = half a block

					//Tab Controller
					if(Input.GetKey(KeyCode.Tab)) {
						PlaceOlder.transform.position = hit.collider.transform.position + DirectionModif; //Wierd thing no longer happen here
					} else {
						if(hit.transform.tag == "Chunk") {
							PlaceOlder.transform.position = BlockBuildingMath.AlingV3(hit.point + Direction, 0.1f, hit.transform.position);
						} else {
							PlaceOlder.transform.position = BlockBuildingMath.AlingV3(hit.point + Direction, 0.1f, Vector3.zero);
						}
					}
					PlaceOlder.GetComponent<Renderer>().material = DefaultPlaceOlder;
					PlaceOlder.transform.localScale = new Vector3(1,1,1);
					PlaceOlder.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
				}
				if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount].StartsWith ("item.")) {
					if(hit.collider.tag != "Unbreakable") {
						if(PlaceOlder.GetComponent<MeshRenderer>().enabled == false) {
							PlaceOlder.GetComponent<MeshRenderer>().enabled = true;
						}
						PlaceOlder.transform.position = hit.collider.transform.position;
						PlaceOlder.GetComponent<Renderer>().material = ADVPlaceOlder;
						PlaceOlder.transform.localScale = hit.collider.transform.GetComponent<Collider>().bounds.size + new Vector3(0.1f, 0.1f, 0.1f);
					} else {
						if(PlaceOlder.GetComponent<MeshRenderer>().enabled == true) {
							PlaceOlder.GetComponent<MeshRenderer>().enabled = false;
						}
					}
				}
			}
			if(Input.GetKeyDown(KeyCode.Escape)) {
				if(hit.collider.tag == "DataFusio") {
					//WTMode = "DF";
					hit.transform.gameObject.GetComponent<DataFusioScript>().UpdateErrorFix();
					hit.transform.gameObject.GetComponent<DataFusioScript>().Canvas.Find("Gestionnairy_UI").Find("<Arrow").GetComponent<Button>().onClick.RemoveAllListeners();
					hit.transform.gameObject.GetComponent<DataFusioScript>().Canvas.Find("Gestionnairy_UI").Find(">Arrow").GetComponent<Button>().onClick.RemoveAllListeners();
					hit.transform.gameObject.GetComponent<DataFusioScript>().currentPlayer = null;
					hit.transform.gameObject.GetComponent<DataFusioScript>().Canvas.Find("Gestionnairy_UI").gameObject.SetActive(false);
					PositionLock = false;
					transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					controller.cam = GetComponent<Camera>();
				}
			}
			if(Input.GetMouseButtonDown(1)) {
				//Place or Operate
				if(hit.collider.tag == "VehiculeHolder") {
					hit.collider.GetComponent<VehiculeHolderScript>().SwitchMode();
				} else if(hit.collider.tag == "Console" && !Input.GetKey(KeyCode.LeftShift) && hit.collider.GetComponent<Console>().Consomation <= hit.collider.GetComponent<Console>().EnergieInput.GetComponent<EnergyInputSaver>().EnergySource) {
					ConsoleUI.gameObject.SetActive(true);
					TextField.gameObject.SetActive(true);
					SelectedConsole = hit.collider.GetComponent<Console>();
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					TextField.GetComponent<InputField>().text = SelectedConsole.Text;
					SelectedConsole.isOp = true;
					SelectedConsole.UpdateFunctionsList();

					controller.cam = null;
					WTMode = "Console";
				} else if(hit.collider.tag == "BattryBox" && !Input.GetKey(KeyCode.LeftShift)) {
					controller.cam = null;
					Canvas.GetComponent<CanvasObjects>().childs[9].gameObject.SetActive(true);
					hit.collider.GetComponent<BattryOperator>().IsOpen = true;
					PositionLock = true;
					transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					WTMode = "Battry";
				} else if(hit.collider.tag == "DriverSeat" && !Input.GetKey(KeyCode.LeftShift)) {           //>,< 
					SP = transform.parent.position;
					transform.parent.SetParent(Build.FindBlock(hit.collider.transform));
					//transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
					transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
					IsSitting = true;
					PositionLock = true;
					transform.parent.localPosition = new Vector3(0, 1.6f, 0);
					//transform.parent.localEulerAngles = Vector3.zero;
				} else if(hit.collider.tag == "CO" && InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == "item.IOSystem") {
					if(connectorMemory != null) {
						if(connectorMemory == hit.collider.gameObject) {
							connectorMemory.GetComponent<OutputConnecter>().IsSelected = false;
							connectorMemory = null;
						} else if(connectorMemory.GetComponent<OutputConnecter>() != null) {
							connectorMemory.GetComponent<OutputConnecter>().IsSelected = false;
							connectorMemory = null;
						} else if(connectorMemory.GetComponent<InputConnector>() != null) {
							Name = "";
							for(int i = 0; i < 8; i++) {
								Name+=glyphs[UnityEngine.Random.Range(0,glyphs.Length)];
							}
							hit.collider.name = Name+"_Output";
							connectorMemory.name = Name+"_Input";
							connectorMemory.GetComponent<InputConnector>().IsSelected = false;
							connectorMemory = null;
						}
					} else {
						if(hit.collider.GetComponent<OutputConnecter>().Friend != null) {
							hit.collider.GetComponent<OutputConnecter>().Friend.name = "CI";
							hit.collider.name = "CO";
						} else {
							connectorMemory = hit.collider.gameObject;
							connectorMemory.GetComponent<OutputConnecter>().IsSelected = true;
						}
					}
				} else if(hit.collider.tag == "CI" && InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == "item.IOSystem") {
					if(connectorMemory != null) {
						if(connectorMemory == hit.collider.gameObject) {
							connectorMemory.GetComponent<InputConnector>().IsSelected = false;
							connectorMemory = null;
						} else if(connectorMemory.GetComponent<InputConnector>() != null) {
							connectorMemory.GetComponent<InputConnector>().IsSelected = false;
							connectorMemory = null;
						} else if(connectorMemory.GetComponent<OutputConnecter>() != null) {
							Name = "";
							for(int i = 0; i < 8; i++) {
								Name+=glyphs[UnityEngine.Random.Range(0,glyphs.Length)];
							}
							hit.collider.name = Name+"_Input";
							connectorMemory.name = Name+"_Output";
							connectorMemory.GetComponent<OutputConnecter>().IsSelected = false;
							connectorMemory = null;
						}
					} else {
						if(hit.collider.GetComponent<InputConnector>().Friend != null) {
							hit.collider.GetComponent<InputConnector>().Friend.name = "CO";
							hit.collider.name = "CI";
						} else {
							connectorMemory = hit.collider.gameObject;
							connectorMemory.GetComponent<InputConnector>().IsSelected = true;
						}
					}
				} else if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == "item.EnergyJoiner") {
					if(hit.collider.tag == "Energie") {
						if(EconnectorMemory != null) {
							if(EconnectorMemory == hit.collider.gameObject) {
								EconnectorMemory.GetComponent<EnergyInputSaver>().IsSelected = false;
								EconnectorMemory = null;
							} else if(EconnectorMemory.GetComponent<EnergyInputSaver>() != null) {
								EconnectorMemory.GetComponent<EnergyInputSaver>().IsSelected = false;
								EconnectorMemory = null;
							} else if(EconnectorMemory.GetComponent<EnergyOutputSaver>() != null) {
								Name = "";
								for(int i = 0; i < 8; i++) {
									Name+=glyphs[UnityEngine.Random.Range(0,glyphs.Length)];
								}
								hit.collider.name = Name+"_EI";
								EconnectorMemory.name = Name+"_EO";
								EconnectorMemory.GetComponent<EnergyOutputSaver>().IsSelected = false;
								EconnectorMemory = null;
							}
						} else {
							if(hit.collider.GetComponent<EnergyInputSaver>().Friend != null) {
								hit.collider.GetComponent<EnergyInputSaver>().Friend.name = "EO";
								hit.collider.name = "EI";
							} else {
								EconnectorMemory = hit.collider.gameObject;
								EconnectorMemory.GetComponent<EnergyInputSaver>().IsSelected = true;
							}
						}
					} else if(hit.collider.tag == "EnergieOutput") {
						if(EconnectorMemory != null) {
							if(EconnectorMemory == hit.collider.gameObject) {
								EconnectorMemory.GetComponent<EnergyOutputSaver>().IsSelected = false;
								EconnectorMemory = null;
							} else if(EconnectorMemory.GetComponent<EnergyOutputSaver>() != null) {
								EconnectorMemory.GetComponent<EnergyOutputSaver>().IsSelected = false;
								EconnectorMemory = null;
							} else if(EconnectorMemory.GetComponent<EnergyInputSaver>() != null) {
								Name = "";
								for(int i = 0; i < 8; i++) {
									Name+=glyphs[UnityEngine.Random.Range(0,glyphs.Length)];
								}
								hit.collider.name = Name+"_EO";
								EconnectorMemory.name = Name+"_EI";
								EconnectorMemory.GetComponent<EnergyInputSaver>().IsSelected = false;
								EconnectorMemory = null;
							}
						} else {
							if(hit.collider.GetComponent<EnergyOutputSaver>().Friend != null) {
								hit.collider.GetComponent<EnergyOutputSaver>().Friend.name = "EI";
								hit.collider.name = "EO";
							} else {
								EconnectorMemory = hit.collider.gameObject;
								EconnectorMemory.GetComponent<EnergyOutputSaver>().IsSelected = true;
							}
						}
						/*
						controller.cam = null;
						StringMessage.gameObject.SetActive(true);
						TextFieldConnector.gameObject.SetActive(true);
						PositionLock = true;
						transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
						WTMode = "EO";
						
						StringMessage.text = "Energy Output Connection";
						TextFieldConnector.GetComponent<InputField>().text = hit.collider.name.Replace("_EO", "");
						*/
					}
				} else if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == "item.flowAnalizer") {
					if(hit.collider.tag == "Energie") {
						SendPopup("E"+hit.collider.GetComponent<EnergyInputSaver>().EnergySource.ToString());
					} else if(hit.collider.tag == "EnergieOutput") {
						SendPopup("E"+hit.collider.GetComponent<EnergyOutputSaver>().EnergySource.ToString());
					}
				} else if(hit.collider.tag == "DataFusio" && !Input.GetKey(KeyCode.LeftShift)) {
					WTMode = "DF";
					hit.collider.transform.gameObject.GetComponent<DataFusioScript>().currentPlayer = transform.GetComponent<Player>();
					hit.collider.transform.gameObject.GetComponent<DataFusioScript>().Canvas.Find("Gestionnairy_UI").gameObject.SetActive(true);
					PositionLock = true;
					transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
					Cursor.visible = true;
					controller.cam = null;
					Cursor.lockState = CursorLockMode.None;
					hit.collider.transform.gameObject.GetComponent<DataFusioScript>().CheckError();
				} else {
					if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] != null) {
						if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount].StartsWith("block.")) {
							GameObject Save;
							if(StatusPos == 0) {
								Save = (GameObject)Instantiate(Resources.Load(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount]), PlaceOlder.transform.position/*hit.point*/, Quaternion.Euler(0, RoundToBound(PlayerMove.eulerAngles.y - 270, 90), 0));
							} else {
								Save = (GameObject)Instantiate(Resources.Load(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount]), PlaceOlder.transform.position/*hit.point*/, Quaternion.Euler(StatusDir*-90, RoundToBound(PlayerMove.eulerAngles.y - 270, 90), -90));
							}
							if(hit.transform.root.tag == "Chunk") {
								Save.transform.parent = hit.transform.root;
							}
							if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == "block.console") {
								Save.transform.Find("block.console").GetComponent<Console>().CanvasPos = CanvasPos;
							}
							if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount] == "block.battry") {
								Save.transform.Find("block.battry").GetComponent<BattryOperator>().Canvas = Canvas;
							}
						} else if(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount].StartsWith("material.")) {
							if(true/*!GetPO.isPlaceOlderOverlap*/) {
								GameObject Material;
								Material = (GameObject)Instantiate(Resources.Load(InventoryAccess.HotbarItems[InventoryAccess.SelectorCount]), PlaceOlder.transform.position, Quaternion.identity);
								if(hit.transform.root.tag == "Chunk") {
									Material.transform.parent = hit.transform.root;
								}
							}
						} else {
							
						}
					}
				}
			}
		}
		Collider[] colls = PlayerMove.GetComponents<Collider>();
		foreach(Collider coll in colls) {
			coll.enabled = !IsSitting;
		}
		/*if(IsSitting) {
			transform.parent.localEulerAngles = new Vector3(0, transform.parent.localEulerAngles.y, 0);
		}*/
		//controller.advancedSettings.StrikeMouvement = StrikeMovement;
	}

	/*void OnDrawGizmos () {
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) {
			Gizmos.color = new Color(1, 0, 0, 0.5F);
			Gizmos.DrawCube(hit.point, new Vector3(0.05f, 0.05f, 0.05f));
		}
	}*/

	float RoundToBound (float Value, float Bound) {
		return Mathf.RoundToInt(Value / Bound) * Bound;
	}

	public void RunFunctionFromConsole (string FunctionName) {
		if(SelectedConsole != null) {
			SelectedConsole.ReadFunction(FunctionName);
		}
	}

	public void SendPopup (string Message) {
		StartCoroutine(sPopup(Message));
	}

	IEnumerator sPopup (string Message) {
		Popup.GetChild(0).GetComponent<Text>().text = Message;
		Popup.gameObject.SetActive(true);
		yield return new WaitForSeconds(2f);
		Popup.gameObject.SetActive(false);
	}
}

public class BlockBuildingMath {
	static public Vector3 FindPosition (Vector3 MainPoint, Vector3 CursorPosition) {
		Vector3 CursorPos = (MainPoint - CursorPosition);
		if(PositiveValue(CursorPos.x) > PositiveValue(CursorPos.y) && PositiveValue(CursorPos.x) > PositiveValue(CursorPos.z)) {
			if(CursorPos.x < 0) {
				return Vector3.right;
			} else {
				return Vector3.left;
			}
		} else if(PositiveValue(CursorPos.y) > PositiveValue(CursorPos.x) && PositiveValue(CursorPos.y) > PositiveValue(CursorPos.z)) {
			if(CursorPos.y < 0) {
				return Vector3.up;
			} else {
				return Vector3.down;
			}
		} else if(PositiveValue(CursorPos.z) > PositiveValue(CursorPos.x) && PositiveValue(CursorPos.z) > PositiveValue(CursorPos.y)) {
			if(CursorPos.z < 0) {
				return Vector3.forward;
			} else {
				return Vector3.back;
			}
		} else {
			return Vector3.one;
		}
	}

	static public float PositiveValue(float Value) {
		if(Value < 0) {
			return Value *=-1;
		} else {
			return Value *=1;
		}
	}

	static public float Aling (float Value, float Grid) {
		return(Mathf.Round(Value/Grid))*Grid;
	}

	static public Vector3 AlingV3 (Vector3 Pos, float Grid, Vector3 Modif) {
		if(Modif != Vector3.zero) {
			return new Vector3(Aling(Pos.x,Grid)+(Modif.x-Aling(Modif.x,Grid)), Aling(Pos.y,Grid)+(Modif.y-Aling(Modif.y,Grid)), Aling(Pos.z,Grid)+(Modif.z-Aling(Modif.z,Grid)));
		} else {
			return new Vector3(Aling(Pos.x, Grid), Aling(Pos.y, Grid), Aling(Pos.z, Grid));
		}
	}
}

public class Build {
	static public Transform FindChunk (Transform target) {
		Transform n = target;
		if(n.tag == "Chunk") {
			return n;
		}
		while(true) {
			if(n.parent != null) {
				if(n.parent.tag == "Chunk") {
					return n.parent;
				} else {
					n=n.parent;
				}
			} else {
				return null;
			}
		}
	}
	static public Transform FindBlock (Transform target) {
		Transform n = target;
		if(n.tag == "Chunk") {
			return null;
		}
		while(true) {
			if(n.parent != null) {
				if(n.parent.tag == "Chunk") {
					return n;
				} else {
					n=n.parent;
				}
			} else {
				return n;
			}
		}
	}
	static public bool IsHoldByChunk (Transform target) {
		if(FindChunk(target) != null) {
			return true;
		} else {
			return false;
		}
	}
}

public class UIBuilder {
	public void Clear () {

	}

	public void SetBackgroundSize (Vector2 size) {

	}

	public void Display () {

	}

	public void Hide () {

	}
}
