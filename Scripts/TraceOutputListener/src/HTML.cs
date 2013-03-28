namespace System.HTML
{
	/// <summary>
	/// Represents HTML formatted text colors.
	/// </summary>
	public class HTMLColors
	{
		/// <summary>
		/// White color.
		/// </summary>
		public static string White = "#ffffff";

		/// <summary>
		/// Black color.
		/// </summary>
		public static string Black = "#000000";

		/// <summary>
		/// Red color.
		/// </summary>
		public static string Red = "#ff0000";

		/// <summary>
		/// Green color.
		/// </summary>
		public static string Green = "#00ff00";

		/// <summary>
		/// Blue color.
		/// </summary>
		public static string Blue = "#0000ff";

		/// <summary>
		/// Light gray color.
		/// </summary>
		public static string LightGray = "#c0c0c0";

		/// <summary>
		/// Dark gray color.
		/// </summary>
		public static string DarkGray = "#404040";

		/// <summary>
		/// Gray color.
		/// </summary>
		public static string Gray = "#808080";
	}

	/// <summary>
	/// Represents HTML text elements.
	/// </summary>
	public class HTMLElements
	{
		private static string NL = Environment.NewLine;

		/// <summary>
		/// Line break.
		/// </summary>
		public static string LineBreak = "<br>"+NL;

		/// <summary>
		/// Horizontal rule.
		/// </summary>
		public static string HorizontalRule = NL+"<hr>"+NL;
	}

	/// <summary>
	/// Represents HTML font.
	/// </summary>
	public class HTMLFont
	{
		private string _FormatString = String.Empty;

		/// <summary>
		/// Creates new HTML font of specified family.
		/// </summary>
		/// <param name="Family">Font family.</param>
		public HTMLFont(string Family)
		{
			this._FormatString = "<font face=\""+Family+"\">{0}</font>";
		}

		/// <summary>
		/// Creates new HTML font with specified size.
		/// </summary>
		/// <param name="Size">Size of the font (from 1 to 6).</param>
		public HTMLFont(int Size)
		{
			this._FormatString = "<font size=\""+Size.ToString()+"\">{0}</font>";
		}

		/// <summary>
		/// Creates new HTML font of specified family and size.
		/// </summary>
		/// <param name="Family">Font family.</param>
		/// <param name="Size">Size of the font.</param>
		public HTMLFont(string Family, int Size)
		{
			this._FormatString = "<font face=\""+Family+"\" size=\""+Size.ToString()+"\">{0}</font>";
		}

		/// <summary>
		/// Gets format string for methods like <see cref="System.String.Format"/>.
		/// </summary>
		/// <returns>The format string for methods like <see cref="System.String.Format"/>.</returns>
		public string GetFormat()
		{
			return this._FormatString;
		}
	}

	/// <summary>
	/// Represents HTML formatted string bulder.
	/// </summary>
	public class HTMLStringBuilder
	{
		private System.Text.StringBuilder _Text;

		/// <summary>
		/// Creates new instance of the class.
		/// </summary>
		public HTMLStringBuilder()
		{
			this._Text = new System.Text.StringBuilder();
		}

		/// <summary>
		/// Appends specified string to the end of the text.
		/// </summary>
		/// <param name="Text">String to append.</param>
		public void Append(string Text)
		{
			this._Text.Append(Text);
		}

		/// <summary>
		/// Appends specified string to the end of the text using specified color.
		/// </summary>
		/// <param name="Text">String to append.</param>
		/// <param name="Color">Color of the appended text.</param>
		public void Append(string Text, string Color)
		{
			this._Text.Append("<span style=\"color:");
			this._Text.Append(Color);
			this._Text.Append("\">");
			this._Text.Append(Text);
			this._Text.Append("</span>");
		}

		/// <summary>
		/// Appends specified string to the end of the text using specified font.
		/// </summary>
		/// <param name="Text">String to append.</param>
		/// <param name="Font">Font of the text.</param>
		public void Append(string Text, HTMLFont Font)
		{
			this._Text.AppendFormat(Font.GetFormat(), Text);
		}

		/// <summary>
		/// Appends specified string to the end of the text using specified font and color.
		/// </summary>
		/// <param name="Text">String to append.</param>
		/// <param name="Font">Font of the text.</param>
		/// <param name="Color">Color of the appended text.</param>
		public void Append(string Text, HTMLFont Font, string Color)
		{
			this._Text.Append("<span style=\"color:");
			this._Text.Append(Color);
			this._Text.Append("\">");
			this._Text.AppendFormat(Font.GetFormat(), Text);
			this._Text.Append("</span>");
		}

		/// <summary>
		/// Appends specified string to the end of the text using specified font and color.
		/// </summary>
		/// <param name="Text">String to append.</param>
		/// <param name="Font">Font of the text.</param>
		/// <param name="Color">Color of the appended text.</param>
		/// <param name="Bold">Indicates should the text be bold or not.</param>
		public void Append(string Text, HTMLFont Font, string Color, bool Bold)
		{
			this._Text.Append("<span style=\"color:");
			this._Text.Append(Color);
			this._Text.Append("\">");
			if(Bold) this._Text.Append("<b>");
			this._Text.AppendFormat(Font.GetFormat(), Text);
			if(Bold) this._Text.Append("</b>");
			this._Text.Append("</span>");
		}
	}

	/// <summary>
	/// Represents the structure of HTML document.
	/// </summary>
	public class HTMLDocument
	{
		/// <summary>
		/// Storage for 'new line' string - for shortest writing.
		/// </summary>
		private static string NL = Environment.NewLine;

		/// <summary>
		/// The title of the document.
		/// </summary>
		public string Title;

		/// <summary>
		/// Document body background color.
		/// </summary>
		public string BodyBgColor;

		/// <summary>
		/// The part of text to appear before the main text of the document.
		/// </summary>
		public string BodyHeader;

		/// <summary>
		/// The part of text to appear after the main text of the document.
		/// </summary>
		public string BodyFooter;

		/// <summary>
		/// The main text part of the document.
		/// </summary>
		public System.Collections.Specialized.StringCollection Paragraphs;

		/// <summary>
		/// Creates new unnamed blank HTML-formatted document with white background color.
		/// </summary>
		public HTMLDocument()
		{
			this.Title = "Unnamed";
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = String.Empty;
			this.BodyFooter = String.Empty;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
		}

		/// <summary>
		/// Creates new blank HTML-formatted document with white background color and spacified title.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		public HTMLDocument(string DocumentTitle)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = String.Empty;
			this.BodyFooter = String.Empty;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on white background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Content">Document content.</param>
		public HTMLDocument(string DocumentTitle, string Content)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = String.Empty;
			this.BodyFooter = String.Empty;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
			this.Paragraphs.Add(Content);
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on white background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Header">The top part of the document contents.</param>
		/// <param name="Footer">The bottom part of the document content.</param>
		public HTMLDocument(string DocumentTitle, string Header, string Footer)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = Header;
			this.BodyFooter = Footer;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on white background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Header">The top part of the document contents.</param>
		/// <param name="Footer">The bottom part of the document contents.</param>
		/// <param name="Content">Document content.</param>
		public HTMLDocument(string DocumentTitle, string Header, string Footer, string Content)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = Header;
			this.BodyFooter = Footer;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
			this.Paragraphs.Add(Content);
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on specified background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Header">The top part of the document contents.</param>
		/// <param name="Footer">The bottom part of the document contents.</param>
		/// <param name="Content">Document content.</param>
		/// <param name="BgColor">Text background color.</param>
		public HTMLDocument(string DocumentTitle, string Header, string Footer, string Content, string BgColor)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = BgColor;
			this.BodyHeader = Header;
			this.BodyFooter = Footer;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
			this.Paragraphs.Add(Content);
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on white background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Content">Document content.</param>
		public HTMLDocument(string DocumentTitle, string [] Content)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = String.Empty;
			this.BodyFooter = String.Empty;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
			this.Paragraphs.AddRange(Content);
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on white background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Header">The top part of the document contents.</param>
		/// <param name="Footer">The bottom part of the document contents.</param>
		/// <param name="Content">Document content.</param>
		public HTMLDocument(string DocumentTitle, string Header, string Footer, string [] Content)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = "#ffffff";
			this.BodyHeader = Header;
			this.BodyFooter = Footer;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
			this.Paragraphs.AddRange(Content);
		}

		/// <summary>
		/// Creates new HTML-formatted document with specified title and content on specified background.
		/// </summary>
		/// <param name="DocumentTitle">Title for the document.</param>
		/// <param name="Header">The top part of the document contents.</param>
		/// <param name="Footer">The bottom part of the document contents.</param>
		/// <param name="Content">Document content.</param>
		/// <param name="BgColor">Text background color.</param>
		public HTMLDocument(string DocumentTitle, string Header, string Footer, string [] Content, string BgColor)
		{
			this.Title = DocumentTitle;
			this.BodyBgColor = BgColor;
			this.BodyHeader = Header;
			this.BodyFooter = Footer;
			this.Paragraphs = new System.Collections.Specialized.StringCollection();
			this.Paragraphs.AddRange(Content);
		}

		/// <summary>
		/// Represents the whole document as a single string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			System.Text.StringBuilder _Document = new System.Text.StringBuilder();
			_Document.AppendFormat("<html>{0}<head>{0}<meta http-equiv=\"Content-Type\" content=\"text/html; charset=unicode\">{0}<title>{1}</title>{0}</head>{0}", NL, this.Title);
			_Document.AppendFormat("<body bgcolor=\"{1}\">{0}", NL, this.BodyBgColor);
			_Document.AppendFormat("{0}<!-- body header -->{0}{1}{0}<!-- end of body header -->{0}", NL, this.BodyHeader);
			for(int i = 0; i < this.Paragraphs.Count; i++) _Document.AppendFormat("{0}<!-- Paragraph # {2} -->{0}<p>{0}{1}{0}</p>{0}<!-- end of paragraph # {2} -->{0}", NL, this.Paragraphs[i], i);
			_Document.AppendFormat("</body>{0}</html>{0}", NL);
			return _Document.ToString();
		}

		/// <summary>
		/// Saves document to the file with specified path.
		/// </summary>
		/// <param name="FilePath">Path to file to save document to.</param>
		/// <returns>True if document was successfully saved.</returns>
		public bool ToFile(string FilePath)
		{
			try
			{
				System.IO.StreamWriter sw = new System.IO.StreamWriter(FilePath, false, System.Text.Encoding.Unicode);
				sw.Write(this.ToString());
				sw.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Puts document to a specified stream.
		/// </summary>
		/// <param name="TargetStream">The <see cref="System.IO.StreamWriter"/> to write document to.</param>
		/// <returns>True if document was successfully written.</returns>
		public bool ToStream(System.IO.StreamWriter TargetStream)
		{
			try
			{
				TargetStream.Write(this.ToString());
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
