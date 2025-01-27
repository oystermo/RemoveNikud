using System.Globalization;
using Nikse.SubtitleEdit.PluginLogic;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace SubtitleEdit
{
    public partial class MainForm : Form
    {
        private readonly Subtitle _subtitle;
        private const string from = "ְֱֲֳִֵֶַָֹֺֻּֽ֑֖֛֢֣֤֥֦֧֪֚֭֮֒֓֔֕֗֘֙֜֝֞֟֠֡֨֩֫֬֯־ֿ׀ׁׂ׃ׅׄ׆ׇ";

        public string FixedSubtitle { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    DialogResult = DialogResult.Cancel;
                else if (e.KeyCode == Keys.Enter)
                    OnClick(EventArgs.Empty);
            };
            listView1.Columns[2].Width = -2;
            linkLabel1.Click += delegate { Process.Start("https://github.com/SubtitleEdit/plugins/issues/new"); };
        }

        public MainForm(Subtitle sub, string title, string description, Form parentForm)
            : this()
        {
            Text = title;
            _subtitle = sub;
            GeneratePreview(false);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void GeneratePreview(bool setText)
        {
            if (_subtitle == null)
                return;
            listView1.BeginUpdate();
            foreach (Paragraph p in _subtitle.Paragraphs)
            {
                var before = Utilities.RemoveHtmlTags(p.Text, true);
                var after = RemoveNikud(before);
                AddToListView(p, before, after);
                if (setText)
                    p.Text = after;
            }
            listView1.EndUpdate();
        }

        private string RemoveNikud(string text)
        {
            string s = "";
            for (int i=0; i<text.Length; i++)
            {
                if (!from.Contains(text[i] + "")){
                    s = s + text[i];
                }
            }
            return s;
        }

        private void AddToListView(Paragraph p, string before, string after)
        {
            var item = new ListViewItem(p.Number.ToString(CultureInfo.InvariantCulture)) { Tag = p };
            item.SubItems.Add(before);
            item.SubItems.Add(after);
            listView1.Items.Add(item);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            GeneratePreview(true);
            FixedSubtitle = _subtitle.ToText(new SubRip());
            DialogResult = DialogResult.OK;
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            var size = (listView1.Width - (listView1.Columns[0].Width)) >> 2;
            listView1.Columns[1].Width = size;
            listView1.Columns[2].Width = -2;
        }

    }
}