using System;
using System.Windows.Forms;

using System.IO;

namespace Archivator
{
    public partial class MainForm : Form
    {
        private Archiever MyArchiver;
        
        public MainForm()
        {
            InitializeComponent();
            
            MyArchiver = new Archiever();
        }

        // Pomocná soukromá metoda, která zpřístupní spodní 3 tlačítka jen v případě že je otevřen nějaký archív
        private void EnableButtons()
        {
            buttonAdd.Enabled = buttonExtract.Enabled = buttonRemove.Enabled = MyArchiver.IsOpened;
        }

        // Událost kliknutí na tlačítko Nový archív
        private void newArchieve_Click(object sender, EventArgs e)
        {
            // Použijeme komponentu pro windowsovský ukládací dialog, jeho metoda ShowDialog() se vrátí až výběru umístění uživatelem pro uložení souboru
            // (tomu se říká tzv. modální dialog). Vybraná cesta k uložení je pak v jeho vlastnosti FileName
            saveDialog.ShowDialog();
            MyArchiver.NewArchieve(saveDialog.FileName);

            // Smaž seznam souborů v archívu
            listFiles.Items.Clear();
            
            EnableButtons();
        }

        // Událost kliknutí na tlačítko Otevřít archív
        private void openArchieve_Click(object sender, EventArgs e)
        {
            // Otevři dialog s výběrem souboru a počkej až si uživatel vybere
            openDialog.ShowDialog();
            try
            {
                // Pokus se otevřít vybraný archív
                MyArchiver.OpenArchieve(openDialog.FileName);
            }
            catch (ArchieverException exp)
            {
                // Takto se vyvolá klasický windowsovská chybová hláška
                MessageBox.Show("Nastala chyba při otevírání archívu: "+exp.Message);
            }
            
            // Všechno se povedlo, tak naplň seznam souborů
            listFiles.Items.Clear();
            foreach (string Name in MyArchiver.Names)
            {
                listFiles.Items.Add(Name);
            }

            // Povol přidávání, mazání a extrakci souborů
            EnableButtons();
        }

        private void saveArchieve_Click(object sender, EventArgs e)
        {
            // Ulož pouze když je alespoň něco otevřeno
            if (MyArchiver.IsOpened)
            {
                MyArchiver.SaveArchieve();
                MessageBox.Show("Uloženo!");
            }
        }

        // Událost po kliknutí na tlačítko Ukončit
        private void closeApp_Click(object sender, EventArgs e)
        {
            // Zavři tento hlavní formulář = ukončení aplikace
            Close();
        }

        // Tato událost nastane těsně před ukončením aplikace
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyArchiver.Close(); // Zavři archív
        }

        // Událost nastávající při kliknutí na tlačítko Přidat soubor
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Otevři dialog a čekej až uživatel vybere soubor, pak jej přihoď do archívu
            openDialog.ShowDialog();

            try
            {
                MyArchiver.AddFile(openDialog.FileName);
            }
            catch (ArchieverException E)
            {
                MessageBox.Show(String.Format("Nastala chyba: {0}",E.Message));
            }
            listFiles.Items.Add(Path.GetFileName(openDialog.FileName));

            // Pozn.: Není potřeba kontrolovat zda je otevřen nějaký archív, protože toto tlačítko se zpřístupní až když nějaký archív otevřen je
        }

        // Událost nastávající při kliknutí na tlačítko Odstranit soubor
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedIndex > -1) // Je-li vybrán nějaký prvek v seznamu
            {
                // Třída MessageBox obsahuje statickou metodu Show - ta zobrazuje klasickou modální hlášku, v tomto případě s tlačítky Ano/Ne
                if (MessageBox.Show(String.Format("Opravdu chcete odstranit soubor {0}", listFiles.SelectedItem.ToString()), "Odstranit", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    // Když uživatel klikne na Ano, odstraň soubor
                    MyArchiver.RemoveFile(listFiles.SelectedItem.ToString());
                    
                    listFiles.Items.RemoveAt(listFiles.SelectedIndex);
                }
            
            }

        }

        // Událost která nastane při kliknutí na tlačítko Extrahovat soubor
        private void buttonExtract_Click(object sender, EventArgs e)
        {
            // Jestliže v seznamu není nic vybráno, nedělej nic
            if (listFiles.SelectedIndex < 0) return;

            // Pokud je soubor fyzicky v archívu, extrahuj ho tam kam uživatel vybere
            if (MyArchiver.HasFile(listFiles.SelectedItem.ToString()))
            {
                saveDialog.ShowDialog(); // Počkej až uživatel vybere nějakou cestu k uložení extrahovaného souboru
                
                MyArchiver.ExtractFile(listFiles.SelectedItem.ToString(), saveDialog.FileName); // Extrahuj soubor
                
                MessageBox.Show("Extrakce dokončena");
            }
        }

        // Po kliknutí na tlačítko O programu, zobraz formulář About
        private void aboutBox_Click(object sender, EventArgs e)
        {
            // Protože třída Form je odvozená od IDisposable, a všechny formuláře vytvořené ve Visual Studiu jsou automaticky
            // odvozené od třídy Form (tedy i můj About), je možné je použít v using()
            using (About AboutBox = new About())
            {
                AboutBox.ShowDialog(); // Zobraz formulář O programu jako modální (tj. čekej až uživatel odklikne tlačítko OK)
            }
        }

    }
}
