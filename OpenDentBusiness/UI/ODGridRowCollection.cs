using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental.UI{ 
	
	///<summary>A strongly typed collection of ODGridRows</summary>
	public class ODGridRowCollection:CollectionBase,IEnumerable<ODGridRow>{

		///<summary>This ID is used to set a unique ID for every row added to the collection.  Not user editable.
		///Set to 1 when Clear() is called.</summary>
		private long _incrementId = 1;

		///<summary>Returns the GridRow with the given index.</summary>
		public ODGridRow this[int index]{
      get{
				return (ODGridRow)List[index];
      }
      set{
				List[index]=value;
      }
		}

		///<summary></summary>
		public int Add(ODGridRow value) {
			if(value.RowNum==0) {
				value.RowNum=_incrementId++;
			}
			return (List.Add(value));
		}

		///<summary></summary>
		public int IndexOf(ODGridRow value){
      return(List.IndexOf(value));
		}

		///<summary></summary>
		public void Insert(int index,ODGridRow value) {
			if(value.RowNum==0) {
				value.RowNum=_incrementId++;
			}
			List.Insert(index,value);
		}

		///<summary></summary>
		public void Remove(ODGridRow value){
      List.Remove(value);
		}

		///<summary></summary>
		protected override void OnClear() {
			_incrementId=1;
		}

		///<summary></summary>
		public bool Contains(ODGridRow value){
      //If value is not of type ODGridRow, this will return false.
      return(List.Contains(value));
		}

		///<summary></summary>
		protected override void OnInsert(int index,Object value){
      if(value.GetType()!=typeof(ODGridRow))
				throw new ArgumentException("value must be of type ODGridRow.","value");
		}

		///<summary></summary>
		protected override void OnRemove( int index,Object value ){
      if(value.GetType()!=typeof(ODGridRow))
        throw new ArgumentException("value must be of type ODGridRow.","value");
		}

		///<summary></summary>
		protected override void OnSet(int index,Object oldValue,Object newValue){
      if(newValue.GetType()!=typeof(ODGridRow))
        throw new ArgumentException("newValue must be of type ODGridRow.","newValue");
		}

		///<summary></summary>
		protected override void OnValidate(Object value){
      if(value.GetType()!=typeof(ODGridRow))
        throw new ArgumentException("value must be of type ODGridRow.");
		}

		IEnumerator<ODGridRow> IEnumerable<ODGridRow>.GetEnumerator() {
			for(int i=0;i<this.Count;i++) {
				yield return this[i];
			}
		}







		/*
				///<summary></summary>
				public void Remove(int index){
					if((index>Count-1) || (index<0)){
						throw new System.IndexOutOfRangeException();
					}
					else{
						List.RemoveAt(index);
					}
				}*/

		/*
		///<summary>The button is retrieved from List and explicitly cast to the button type.</summary>
		public ODToolBarButton Item(int index){
			return (ODToolBarButton)List[index];
		}*/

		/*
		///<summary></summary>
		public int IndexOf(ODGridColumn value){
			return(List.IndexOf(value));
		}*/

	}
}





















