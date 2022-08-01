using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDo
{
    public partial class Form_ToDo : Form
    {
        public Form_ToDo()
        {
            InitializeComponent();
        }

        private async void button_add_Click(object sender, EventArgs e)
        {
            New_Task ob = new New_Task();
            ob.ShowDialog();
            loadDatagrid();
        }

        private async void button_Edit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                if (!Convert.ToBoolean(selectedRow.Cells["Completed"].Value))
                {
                    string cellValue = Convert.ToString(selectedRow.Cells["id"].Value);
                    New_Task ob = new New_Task(cellValue);
                    ob.ShowDialog();
                }
                else 
                {
                    MessageBox.Show("completed task can not be changed", "Task", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            loadDatagrid();
        }

        private async void loadDatagrid()
        {
            List<ToDoTable> allentity = new List<ToDoTable>();
            await Task.Run(() =>
            {
                using (ToDoEntities _entity = new ToDoEntities())
                {
                    allentity = _entity.ToDoTables.ToList();
                }
            });
            dataGridView1.DataSource = allentity;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            loadDatagrid();
        }

        private async void Form_ToDo_Load(object sender, EventArgs e)
        {
            loadDatagrid();
        }

        private async void button_delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];

                string cellValue = Convert.ToString(selectedRow.Cells["id"].Value);
                New_Task ob = new New_Task(cellValue);
                ob.ShowDialog();
            }
            loadDatagrid();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows) 
            {
                var duedate = row.Cells["DueDate"].Value;
                if (duedate != null && DateTime.Compare(Convert.ToDateTime(duedate), DateTime.Now) <= 0)
                {
                    dataGridView1.Rows[row.Cells["DueDate"].RowIndex].Cells[row.Cells["DueDate"].ColumnIndex].Style.BackColor = Color.Red;
                }
            }            
        }
    }
}
