using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeBase {
	public static class ODUIExtensions {
		///<summary>Instance of ITranslate used to translate items added to ListBoxess.</summary>
		private static ITranslate _itemTranslator;

		///<summary>Instance of ITranslate used to translate items added to ListBoxess. Will never be null.</summary>
		public static ITranslate ItemTranslator {
			get {
				return _itemTranslator??NoTranslate.Instance;
			}
			set {
				_itemTranslator=value;
			}
		}
		
		///<summary>Attempts to select the index provided for the combo box. If idx is -1 or otherwise invalid, then getOverrideText() will be used to get displayed text instead.</summary>
		/// <param name="combo">This is an extension method. This is the control that will be affected.</param>
		/// <param name="idx">Instead of providing an idx directly, use a function that determines the index to select. i.e. _listObj.FindIndex(x=>x.Num==SelectedNum).</param>
		/// <param name="getOverrideText">This should be a function that takes no parameters and returns a string. Linq statements and anonymous functions work best. 
		/// I.E. comboBox.SelectIndex(myCombo,_listObj.FindIndex(x=>x.Num==SelectedNum),</param>
		public static void IndexSelectOrSetText(this ComboBox combo,int idx,Func<string> getOverrideText) {
			combo.SelectedIndexChanged-=ComboSelectedIndex;//Reset visual style when user selects from dropdown.
			combo.KeyPress-=NullKeyPressHandler;//re-enable editing if neccesary
			combo.KeyDown-=NullKeyEventHandler;//re-enable editing if neccesary
			combo.KeyUp-=NullKeyEventHandler;//re-enable editing if neccesary
			//Normal index selection. No special logic needed.
			if(idx>-1 && idx<combo.Items.Count) {
				combo.DropDownStyle=ComboBoxStyle.DropDownList;
				combo.SelectedIndex=idx;
				return;
			}
			//Selected index is not part of the selected list, use the getOverrideText function provided.
			combo.SelectedIndexChanged+=ComboSelectedIndex;//Reset visual style when user selects from dropdown.
			combo.KeyPress+=NullKeyPressHandler;//disable editing.
			combo.KeyDown+=NullKeyEventHandler;//disable editing.
			combo.KeyUp+=NullKeyEventHandler;//disable editing.
			combo.SelectedIndex=-1;
			combo.DropDownStyle=ComboBoxStyle.DropDown;
			combo.Text=getOverrideText();
			if(string.IsNullOrEmpty(combo.Text)) {
				//In case the getOverrideText function returns an empty string
				combo.DropDownStyle=ComboBoxStyle.DropDownList;
			}
		}

		///<summary>Only use this overload if you already have a display string available. 
		///Use SelectIndex(int idx,Func&lt;string> getOverrideText) instead to delay execution of the delegate if it is not needed.</summary>
		public static void SelectIndex(this ComboBox combo,int idx,string OverrideText) {
			combo.IndexSelectOrSetText(idx,() => { return OverrideText; });
		}

		///<summary>Sets the selected item that matches the func passed in. Will only work if the Items in the combo box are ODBoxItems.</summary>
		///<param name="fSelectItem">A func that takes an object that is the same type as the ODBoxItems Tags and returns a bool, i.e.
		///x => x.ClinicNum==0.</param>
		///<param name="overrideText">The text to display in the combo box if a matching item cannot be found.</param>
		public static void SetSelectedItem<T>(this ComboBox combo,Func<T,bool> fSelectItem,string overrideText) {
			int idx=-1;
			for(int i=0;i<combo.Items.Count;i++) {
				ODBoxItem<T> odBoxItem=combo.Items[i] as ODBoxItem<T>;
				if(odBoxItem==null) {
					continue;
				}
				if(fSelectItem(odBoxItem.Tag)) {
					idx=i;
					break;
				}
			}
			combo.SelectIndex(idx,overrideText);
		}

		///<summary>Sets the selected item(s) that match the func passed in. Will only work if the Items in the ListBox are ODBoxItems.</summary>
		///<param name="fSelectItem">A func that takes an object that is the same type as the ODBoxItems Tags and returns a bool, i.e.
		///x => x.ClinicNum==0.</param>
		public static void SetSelectedItem<T>(this ListBox listBox,Func<T,bool> fSelectItem) {
			for(int i=0;i<listBox.Items.Count;i++) {
				ODBoxItem<T> odBoxItem=listBox.Items[i] as ODBoxItem<T>;
				if(odBoxItem==null) {
					continue;
				}
				if(fSelectItem(odBoxItem.Tag)) {
					listBox.SetSelected(i,true);
				}
			}
		}

		///<summary>Sets the items for the listBox to the given items list.</summary>
		///<param name="fItemToString">Func that takes an object that is the same type as the ODBoxItems Tags and returns a string to be displayed for this item, i.e.
		///x => x.Abbr.</param>
		///<param name="fSelectItem">Optional func that takes an object that is the same type as the ODBoxItems Tags and returns a bool, i.e.
		///x => x.ClinicNum==0. Pass null if you don't want to set initial item(s) to be selected.</param>
		public static void SetItems<T>(this ListBox listBox,IEnumerable<T> items,Func<T,string> fItemToString=null,Func<T,bool> fSelectItem=null) {
			listBox.Items.Clear();
			fItemToString=fItemToString??(x => x.ToString());
			foreach(T item in items) {
				listBox.Items.Add(new ODBoxItem<T>(fItemToString(item),item));
			}
			if(fSelectItem!=null) {
				listBox.SetSelectedItem(fSelectItem);
			}
		}

		///<summary>Sets the items for the listBox with the values of the given enum.</summary>
		///<param name="fItemToString">Optional func that takes an enum item and returns a string to be displayed for this item. 
		///Omitting this argument will translate each item's description.</param>
		///<param name="fSelectItem">Optional func that takes an enum item and returns a bool, i.e. x => x==pat.Position. 
		///Pass null if you don't want to set initial item(s) to be selected.</param>
		public static void SetItemsWithEnum<T>(this ListBox listBox,Func<T,string> fItemToString=null,Func<T,bool> fSelectItem=null) 
			where T : struct,IConvertible
		{
			fItemToString=fItemToString??(x => ItemTranslator.Translate("enum"+typeof(T).Name,(x as Enum).GetDescription()));
			SetItems(listBox,Enum.GetValues(typeof(T)).Cast<T>(),fItemToString,fSelectItem);
		}

		///<summary>Sets the items for the comboBox to the given items list.</summary>
		///<param name="fItemToString">Func that takes an object that is the same type as the ODBoxItems Tags and returns a string to be displayed for this item, i.e.
		///x => x.Abbr.</param>
		///<param name="fSelectItem">Optional func that takes an object that is the same type as the ODBoxItems Tags and returns a bool, i.e.
		///x => x.ClinicNum==0. Pass null if you don't want to set initial item(s) to be selected.</param>
		public static void SetItems<T>(this ComboBox comboBox,IEnumerable<T> items,Func<T,string> fItemToString=null,Func<T,bool> fSelectItem=null,
			ComboBoxSpecialValues specialValue=ComboBoxSpecialValues.NotApplicable)
		{
			comboBox.Items.Clear();
			fItemToString=fItemToString??(x => x.ToString());
			if(specialValue!=ComboBoxSpecialValues.NotApplicable) {
				comboBox.Items.Add(new ODBoxItem<ComboBoxSpecialValues>(ItemTranslator.Translate(nameof(ComboBoxSpecialValues)
					,specialValue.GetDescription(useShortVersionIfAvailable:true)),specialValue));
			}
			foreach(T item in items) {
				comboBox.Items.Add(new ODBoxItem<T>(fItemToString(item),item));
			}
			if(fSelectItem!=null) {
				comboBox.SetSelectedItem(fSelectItem,"");
			}
		}

		///<summary>Sets the items for the comboBox with the values of the given enum.</summary>
		///<param name="fItemToString">Optional func that takes an enum item and returns a string to be displayed for this item. 
		///Omitting this argument will translate each item's description.</param>
		///<param name="fSelectItem">Optional func that takes an enum item and returns a bool, i.e. x => x==pat.Position. 
		///Pass null if you don't want to set initial item(s) to be selected.</param>
		public static void SetItemsWithEnum<T>(this ComboBox comboBox,Func<T,string> fItemToString=null,Func<T,bool> fSelectItem=null,
			ComboBoxSpecialValues specialValue=ComboBoxSpecialValues.NotApplicable) where T : struct, IConvertible 
		{
			fItemToString=fItemToString??(x => ItemTranslator.Translate("enum"+typeof(T).Name,(x as Enum).GetDescription()));
			SetItems(comboBox,Enum.GetValues(typeof(T)).Cast<T>(),fItemToString,fSelectItem,specialValue);
		}

		///<summary>Returns true if the 'All' item is selected. Only useful if you've called SetItems or SetItemsWithEnum with doIncludeAll as true.
		///</summary>
		public static bool IsAllSelected(this ComboBox comboBox) {
			if(comboBox.SelectedItem==null) {
				return false;
			}
			ODBoxItem<ComboBoxSpecialValues> selectedItem=comboBox.SelectedItem as ODBoxItem<ComboBoxSpecialValues>;
			return selectedItem!=null && selectedItem.Tag==ComboBoxSpecialValues.All;
		}

		///<summary>Gets all the Tags for all ODBoxItems added to Items.</summary>
		public static List<T> AllTags<T>(this ListBox listBox) {
			return listBox.Items.AsEnumerable<ODBoxItem<T>>().Select(x => x.Tag).ToList();
		}

		///<summary>Sets e.Handled to true to prevent event from firing.</summary>
		private static void NullKeyPressHandler(object sender,KeyPressEventArgs e) {
			e.Handled=true;
		}

		///<summary>Sets e.Handled to true to prevent event from firing.</summary>
		private static void NullKeyEventHandler(object sender,KeyEventArgs e) {
			e.Handled=true;
		}

		///<summary>Used to call comboBox.SelectIndex after user manually selects an item from the list. Should reset visual style to dropdown.</summary>
		private static void ComboSelectedIndex(object sender,EventArgs e) {
			ComboBox box = (ComboBox)sender;//capturing variable for readability
			box.IndexSelectOrSetText(box.SelectedIndex,() => { return box.Text; });
		}

		///<summary>Returns the tag of the selected item. The items in the ComboBox must be ODBoxItems.</summary>
		public static T SelectedTag<T>(this ComboBox comboBox) {
			if(comboBox.SelectedItem is ODBoxItem<T>) {
				return (comboBox.SelectedItem as ODBoxItem<T>).Tag;
			}
			return default(T);
		}

		///<summary>Returns true if a SelectedItem exists and the SelectedItem's Tag is of type T.</summary>
		public static bool HasSelectedTag<T>(this ListBox listBox) {
			if(listBox.SelectedItem==null) {
				return false;
			}
			return listBox.SelectedItem is ODBoxItem<T>;
		}

		///<summary>Returns the tag of the selected item. The items in the ComboBox must be ODBoxItems.</summary>
		public static T SelectedTag<T>(this ListBox listBox) {
			if(listBox.SelectedItem is ODBoxItem<T>) {
				return (listBox.SelectedItem as ODBoxItem<T>).Tag;
			}
			return default(T);
		}
		
		///<summary>Returns the tags of the selected items. The items in the ComboBox must be ODBoxItems.</summary>
		public static List<T> SelectedTags<T>(this ListBox listBox) {
			List<T> listSelected=new List<T>();
			foreach(object selectedItem in listBox.SelectedItems) {
				if(selectedItem is ODBoxItem<T>) {
					listSelected.Add((selectedItem as ODBoxItem<T>).Tag);
				}
			}
			return listSelected;
		}

		///<summary>Gets all controls and their children controls recursively.</summary>
		public static IEnumerable<Control> GetAllControls(this Control control) {
			IEnumerable<Control> controls=control.Controls.OfType<Control>();
			return controls.SelectMany(GetAllControls).Concat(controls);
		}

		///<summary>Sometimes ODProgress can cause other forms to open up behind other applications. Call this method to force this form to the front.
		///</summary>
		public static void ForceBringToFront(this Form form) {
			form.TopMost=true;
			Application.DoEvents();
			form.TopMost=false;
		}
		
	}

	///<summary>Adds special, non-typed items to combo boxes, such as "None" and "All".</summary>
	public enum ComboBoxSpecialValues {
		///<summary>0 - The default option when you do not want to add an additional option to the combo box.</summary>
		NotApplicable,
		///<summary>1 - Adds a "None" option to the top of a combo box.</summary>
		[System.ComponentModel.Description("None")]
		None,
		///<summary>2 - Adds an "All" option to the top of the combo box.</summary>
		[System.ComponentModel.Description("All")]
		All,
	}

	///<summary>Interface for translating text. Its purpose is to abstract Lans.g().</summary>
	public interface ITranslate {
		///<summary>Returns the translated string. Equivalent to Lans.g().</summary>
		string Translate(object sender,string text);
	}

	///<summary>Doesn't actually do any translating.</summary>
	public class NoTranslate :ITranslate {
		private static NoTranslate _instance;

		public static NoTranslate Instance {
			get {
				if(_instance==null) {
					_instance=new NoTranslate();
				}
				return _instance;
			}
		}
		public string Translate(object sender,string text) {
			return text;
		}
	}
}
