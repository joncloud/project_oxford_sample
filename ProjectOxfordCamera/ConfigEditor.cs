using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOxfordCamera
{
    public partial class ConfigEditor : Form
    {
        private BindingSource _bindingSource;

        public ConfigEditor()
        {
            InitializeComponent();
        }

        public AppConfig Config { get; private set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            _bindingSource.EndEdit();
            AppConfigStore store = new AppConfigStore();
            store.Save(Config);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            _bindingSource.CancelEdit();
            Close();
        }

        private void ConfigEditor_Load(object sender, EventArgs e)
        {
            if (DesignMode) { return; }

            AppConfigStore store = new AppConfigStore();
            Config = store.Load();

            _bindingSource = new BindingSource(Config, null);

            textBoxApiKey.DataBindings.Add(new Binding("Text", _bindingSource, "OxfordSubscriptionKey", false, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}
