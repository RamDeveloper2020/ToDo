using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDo
{
    public partial class New_Task : Form
    {
        public New_Task()
        {
            InitializeComponent();
        }
        public New_Task(string id)
        {
            InitializeComponent();
            label_id.Text = id;
        }

        private void textBox_Title_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Title.Text))
            {
                e.Cancel = true;
                textBox_Title.Focus();
                errorProvider.SetError(textBox_Title, "Title should not be left blank!");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(textBox_Title, "");
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            //string query = "INSERT INTO dbo.ToDoTable (Title, Description, DueDate,Completed) " +
            //    "VALUES (@Title, @Description, @DueDate,@Completed) ";

            //// create connection and command
            //using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ToString()))
            //using (SqlCommand cmd = new SqlCommand(query, cn))
            //{
            //    // define parameters and their values
            //    cmd.Parameters.Add("@Title", SqlDbType.VarChar, 300).Value = textBox_Title.Text;
            //    cmd.Parameters.Add("@Description", SqlDbType.VarChar, 1000).Value = textBox_description.Text;
            //    cmd.Parameters.Add("@DueDate", SqlDbType.DateTime, 50).Value = Convert.ToDateTime(dateTimePicker_date.Value.ToShortDateString() + " " + dateTimePicker_time.Value.ToShortTimeString());
            //    cmd.Parameters.Add("@Completed", SqlDbType.Bit, 50).Value = checkBox_status.Checked; 

            //    // open connection, execute INSERT, close connection
            //    cn.Open();
            //    cmd.ExecuteNonQuery();
            //    cn.Close();
            //}

            ///add in database
            ToDoTable ob = new ToDoTable();
            ob.Title = textBox_Title.Text;
            ob.Description = textBox_description.Text;
            if (!string.IsNullOrEmpty(dateTimePicker_date.Text.Trim()))
            {
                ob.DueDate = Convert.ToDateTime(dateTimePicker_date.Value.ToShortDateString() + " " + dateTimePicker_time.Value.ToShortTimeString());
            }
            ob.Completed = checkBox_status.Checked;
            bool result = SaveToDoTables(ob);
            ShowStatus(result, "Save");
            this.Close();
        }
        public bool SaveToDoTables(ToDoTable ob)
        {
            bool result = false;
            using (ToDoEntities _entity = new ToDoEntities())
            {
                _entity.ToDoTables.Add(ob);
                _entity.SaveChanges();
                result = true;
            }
            return result;
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(label_id.Text))
            {
                bool result = UpdateToDoTables(Convert.ToInt32(label_id.Text));
                ShowStatus(result, "Update");
                this.Close();
            }
            else { ShowStatus(false, "Update"); }
        }
        public bool UpdateToDoTables(int id)
        {
            bool result = false;
            using (ToDoEntities _entity = new ToDoEntities())
            {
                ToDoTable _task = _entity.ToDoTables.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
                _task.Title = textBox_Title.Text;
                _task.Description = textBox_description.Text;
                _task.Completed = checkBox_status.Checked;
                if (!string.IsNullOrEmpty(dateTimePicker_date.Text))
                {
                    _task.DueDate = Convert.ToDateTime(dateTimePicker_date.Value.ToShortDateString() + " " + dateTimePicker_time.Value.ToShortTimeString());
                }
                if (checkBox_status.Checked)
                {
                    _task.CompletedDate = DateTime.Now;
                }
                _entity.SaveChanges();
                result = true;
            }
            return result;
        }

        public void ShowStatus(bool result, string Action)
        {
            if (result)
            {
                if (Action.ToUpper() == "SAVE")
                {
                    MessageBox.Show("Saved Successfully!..", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (Action.ToUpper() == "UPDATE")
                {
                    MessageBox.Show("Updated Successfully!..", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Deleted Successfully!..", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Something went wrong!. Please try again!..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public async void Display()
        {
            ToDoTable _task = null;
            await Task.Run(() =>
            {
                using (ToDoEntities _entity = new ToDoEntities())
                {
                    int id = Convert.ToInt32(label_id.Text);
                    _task = _entity.ToDoTables.Where(x => x.Id == id).FirstOrDefault();
                }
            });

            textBox_Title.Text = _task.Title;
            textBox_description.Text = _task.Description;
            checkBox_status.Checked = (bool)_task.Completed;
            if (_task.DueDate.HasValue)
            {
                dateTimePicker_date.Format = DateTimePickerFormat.Long;
                dateTimePicker_time.Format = DateTimePickerFormat.Time;
                dateTimePicker_date.Value = _task.DueDate.Value;
                dateTimePicker_time.Value = _task.DueDate.Value;
            }
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(label_id.Text))
            {
                bool result = DeleteToDoTables(Convert.ToInt32(label_id.Text));
                ShowStatus(result, "Delete");
                this.Close();
            }
            else { ShowStatus(false, "Delete"); }
        }
        public bool DeleteToDoTables(int id) 
        {
            bool result = false;
            using (ToDoEntities _entity = new ToDoEntities())
            {
                ToDoTable _task = _entity.ToDoTables.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
                _entity.ToDoTables.Remove(_task);
                _entity.SaveChanges();
                result = true;
            }
            return result;
        }

        private void dateTimePicker_date_Validating(object sender, CancelEventArgs e)
        {
            //if (string.IsNullOrEmpty(dateTimePicker_date.Text.ToString().Trim()))
            //{
            //    e.Cancel = true;
            //    dateTimePicker_date.Focus();
            //    errorProvider.SetError(dateTimePicker_date, "The due date and time should not be left blank!");
            //}
            //else
            //{
            //    e.Cancel = false;
            //    errorProvider.SetError(dateTimePicker_date, "");
            //}
        }

        private void dateTimePicker_time_Validating(object sender, CancelEventArgs e)
        {
            //if (string.IsNullOrEmpty(dateTimePicker_time.Text.ToString().Trim()))
            //{
            //    e.Cancel = true;
            //    dateTimePicker_time.Focus();
            //    errorProvider.SetError(dateTimePicker_time, "The due date and time should not be left blank!");
            //}
            //else
            //{
            //    e.Cancel = false;
            //    errorProvider.SetError(dateTimePicker_time, "");
            //}
        }

        private async void New_Task_Load(object sender, EventArgs e)
        {
            if (label_id.Text.Trim() != "")
            {
                Display();
            }
        }
    }
}
