using System;

namespace CodeBase {

	///<summary>A helper "item" for combo boxes and list boxes so that we do not have to override .ToString() on all of our objects.</summary>
	public class ODBoxItem<T> {
		private string _text;
		private T _tag;

		///<summary>Set text to the phrase that should display to the user.  Set tag to the object that corresponds to the text.
		///ODComboItem has overridden .ToString() to simply return the passed in text.
		///This is so that we can easily get objects from combo boxes without having to have a synchronized local list of objects.</summary>
		public ODBoxItem(string text, T tag=default(T)) {
			_text=text;
			_tag=tag;
		}

		public string Text {
			get { return _text; }
			set { _text=value; }
		}

		public T Tag {
			get { return _tag; }
			set { _tag=value; }
		}

		public override string ToString() {
			return _text;
		}

	}



}