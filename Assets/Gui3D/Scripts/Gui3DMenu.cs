﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gui3D
{
	public class Gui3DMenu : Gui3DObject 
	{
		public bool UseMenuIndex = false;
		public List<Gui3DObject> MenuObjects;
		
		public string UpButton = "MenuUp";
		public string DownButton = "MenuDown";
		
		public int GridColumns = 1;
		
		public string LeftButton = "MenuLeft";
		public string RightButton = "MenuRight";
		
		public bool Wrap = true;
		
		public bool UseMouse = true;
		
		protected int SelectedIndex = 0;
		
		public bool Locked = false;
		public bool LockParentMenu = true;
		
		protected Gui3DMenu parent = null;
		
		// Use this for initialization
		void Start () 
		{
			RefillMenu();
			parent = transform.parent.GetComponent<Gui3DMenu>();
		}
		
		void RefillMenu()
		{
			if(UseMenuIndex)
			{
				FillMenuByIndex();
			}
			else if(MenuObjects.Count <= 0)
			{
				FillMenuAutomatically();
			}
		}
		
		// Update is called once per frame
		void Update () 
		{
			if(Selected && Locked)
			{
				Focus();
			}
			if((MenuObjects.Count <= 0))
			{
				return;
			}
			
			MenuObjects[SelectedIndex].GetComponent<Gui3DObject>().Deselect();
			
			if(!Locked)
			{
				if(Input.GetButtonDown(DownButton))
				{
					UpdateIndex(GridColumns);
				}
				else if(Input.GetButtonDown(UpButton))
				{
					UpdateIndex(-GridColumns);
				}
				
				if(Input.GetButtonDown(RightButton))
				{
					UpdateIndex(1);
				}
				else if(Input.GetButtonDown(LeftButton))
				{
					UpdateIndex(-1);
				}
			}
			
			if(UseMouse)
			{
				if (GetGui3D().HoverObject != null)
				{
					Gui3DObject hoverobj = MenuObjects.Find(obj => obj.gameObject == GetGui3D().HoverObject.gameObject);
					if(hoverobj != null)
					{						
						if(Locked && LockParentMenu)
						{
							Focus();
						}
						SelectedIndex = MenuObjects.IndexOf(hoverobj);
					}
					else
					{
						UnFocus();
					}
				}
			}
			
			if(!Locked)
			{
				MenuObjects[SelectedIndex].Select();
			}
		}
		
		void UpdateIndex(int increment)
		{
			SelectedIndex += increment;
			
			if(SelectedIndex < 0)
			{
				if(Wrap)
				{
					SelectedIndex += MenuObjects.Count;
				}
				else if(LockParentMenu)
				{
					UnFocus();
				}
				else SelectedIndex -= increment;
					
			}
			else if(SelectedIndex > MenuObjects.Count - 1)
			{
				if(Wrap)
				{
					SelectedIndex -= MenuObjects.Count;
				}
				else if(LockParentMenu)
				{
					UnFocus();
				}
				else SelectedIndex -= increment;
			}
		}
		
		void FillMenuByIndex()
		{
			FillMenuAutomatically();
			MenuObjects.Sort(Gui3DMenuUtils.IndexCompare);
		}
		
		virtual protected void FillMenuAutomatically()
		{
			MenuObjects = new List<Gui3DObject>();
			foreach(Transform child in transform)
			{
				Gui3DObject obj = child.gameObject.GetComponent<Gui3DObject>();
				if(obj != null)
				{
					if(obj.Selectable == true)
					{
						obj.Deselect();
						MenuObjects.Add(obj);
					}
				}
			}
			MenuObjects.Sort(Gui3DMenuUtils.LocationCompare);
		}
		
		
		void DeselectAll()
		{
			foreach(Gui3DObject obj in MenuObjects)
			{
				obj.Deselect();
			}
		}
		
		
		void Focus()
		{
			if(LockParentMenu)
			{
				if(parent == null)
				{
					return;
				}
				parent.Locked = true;
				Locked = false;
				SelectedIndex = 0;
			}
		}
		void UnFocus()
		{
			if(LockParentMenu)
			{
				if(parent == null)
				{
					return;
				}
				parent.Locked = false;
				Locked = true;
				SelectedIndex = 0;
				DeselectAll();
			}
		}
	}
}
