using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Coursemo
{
    public partial class Form1 : Form
    {
        public readonly string dbfilename = "Coursemo.mdf";

        public Form1()
        {
            InitializeComponent();
        }

        private CoursemoEntities db;

        private void Form1_Load(object sender, EventArgs e)
        {
            db = new CoursemoEntities();
        }

        // Load students
        private void button1_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();

            var query = from s
                        in db.Students
                        orderby s.LastName
                        select s;
            foreach (var q in query)
            {
                this.listBox1.Items.Add(q);
            }
        }// End button1 (load students)

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBox4.Items.Clear();

            Student s = (Student)this.listBox1.SelectedItem;

            listBox4.Items.Add("Courses Enrolled ");

            listBox4.Items.Add("Courses Waitlisted ");


        }




        // Load Courses
        private void button2_Click(object sender, EventArgs e)
        {
            this.listBox2.Items.Clear();

            var query = from c
                        in db.Courses
                        orderby c.CourseNum
                        select c;
            foreach (var q in query)
            {
                this.listBox2.Items.Add(q);
            }
        }//  End load courses




        // Displays Courses
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBox3.Items.Clear();
            Cours c = (Cours)this.listBox2.SelectedItem;

            listBox3.Items.Add(c.CRN + " " + c.Dept + " " + c.CourseNum + " " + c.Semester + " " + c.Year);
            listBox3.Items.Add(c.Type + " " + c.Day + " " + c.MeetingTime);

            // Get class size
            var q1 = from cid in db.Courses
                     where cid.CRN == c.CRN
                     select cid.ClassSize;

            int classSize = q1.First();
            listBox3.Items.Add("Class Size " + classSize);

            // get count of enrolled
            var q2 = from eid in db.Enrolleds
                     where eid.CRN == c.CRN
                     select eid;
            int enrolled = q2.Count();
            listBox3.Items.Add("Enrolled " + enrolled);

            // get and display netid under enrolled
            var q4 = from id in db.Enrolleds
                     where id.CRN  == c.CRN
                     select id;
            foreach(var i in q4)
            {
                this.listBox3.Items.Add("   " + i.NetId);
            }
            
            //Waitlisted
            // get count of waitlisted
            var q3 = from wid in db.Waitlists
                     where wid.CRN == c.CRN
                     select wid;
            int waitlisted = q3.Count();
            listBox3.Items.Add("Waitlist " + waitlisted); 
            //
            // get and display netid under waitlisted
            var q5 = from id in db.Waitlists
                     where id.CRN == c.CRN
                     select id;
            foreach (var i in q5)
            {
                this.listBox3.Items.Add("   " + i.NetId);
            }
        }// end display courses




        // Enroll Button
        private void button3_Click(object sender, EventArgs e)
        {

            //Error checking: 
            //set a delay for multi users and make sure a student and course are selected
            //
            // delay 
            int timeInMS;
            if (System.Int32.TryParse(this.textBox1.Text, out timeInMS) == true) ;
            else timeInMS = 0;    //  no  delay:
            System.Threading.Thread.Sleep(timeInMS);
            //
            //select a student
            if (this.listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a student");
                return;
            }
            //
            // select a course
            if (this.listBox2.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a course");
                return;
            }
            // End Error Checks on multi user, student and course selection


            // Enrolling:
            //
            // Selected student and course grabbing
            Student sid = (Student)this.listBox1.SelectedItem;
            Cours cid = (Cours)this.listBox2.SelectedItem;

            using (var db = new CoursemoEntities())
            {
                var TxOptions = new System.Transactions.TransactionOptions();
                    TxOptions.IsolationLevel = System.Transactions.IsolationLevel.Serializable;

                using (var Tx = new System.Transactions.TransactionScope(
                    System.Transactions.TransactionScopeOption.Required, TxOptions))
                {
                    try
                    {
                        // Enrolled and Waitlisted checks
                        //
                        // check is student is already enrolled
                        var q1 = from eid in db.Enrolleds
                                 where eid.NetId == sid.NetId && eid.CRN == cid.CRN
                                 select eid;
                        //
                        if (q1.Count() > 0)
                        {
                            MessageBox.Show("Student is already enrolled");
                            return;
                        }
                        //
                        // check is student is already waitlisted
                        var q2 = from wid in db.Waitlists
                                 where wid.NetId == sid.NetId && wid.CRN == cid.CRN
                                 select wid;
                        //
                        if (q2.Count() > 0)
                        {
                            MessageBox.Show("Student is already waitlisted");
                            return;
                        }
                        // End checks enrolled and waitlist checks

                        // get count of enrolled
                        var q3 = from eid in db.Enrolleds
                                 where eid.CRN == cid.CRN
                                 select eid;
                                               
                        int enrolled = q3.Count();
                        
                        // Get class size
                        var q4 = from c in db.Courses
                                 where c.CRN == cid.CRN
                                 select c.ClassSize;

                        int classSize = q4.First();
                        
                        // check if class is full
                       // System.Diagnostics.Debug.Assert(enrolled <= classSize);
                        
                        if (enrolled < classSize)
                        {
                            Enrolled enroll = new Enrolled();
                            enroll.NetId = sid.NetId;
                            enroll.CRN = cid.CRN;
                            
                            // save changes, close db, display confirmation
                            db.Enrolleds.Add(enroll);
                            db.SaveChanges();
                            Tx.Complete();

                            MessageBox.Show("Student Enrolled");
                        }// End Enroll
                         //
                         // Class full student to be waitlisted
                        // 
                        else 
                        {
                            Waitlist waitlist = new Waitlist();
                            waitlist.NetId = sid.NetId;
                            waitlist.CRN = cid.CRN;
                            
                            // save changes, close db, display confimation
                            db.Waitlists.Add(waitlist);
                            db.SaveChanges();
                            Tx.Complete();
                            MessageBox.Show("Student Waitlisted");
                        }// End Waitlist                        
                    }// End try
                    catch (Exception ex)
                    {
                        MessageBox.Show("Enrollment Failed" + ex.Message);
                    }// End Catch
                    finally
                    {

                    } // End fianlly                   
                }// End Using (Tx)
            }// End Using (db)      

            listBox2_SelectedIndexChanged(sender, e);
            listBox1_SelectedIndexChanged(sender, e);

        }// End Button 3  Enrolling and waitlisting students



        // Drop class button
        private void button5_Click(object sender, EventArgs e)
        {

        }
   

        


        // Display Course info
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            

            
            
        }

        // Reset button clears db
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Format(@"
                DELETE FROM Enrolled;
                DELETE FROM Waitlist;
                ");

                DataAccessTier.Data datatier = new DataAccessTier.Data(dbfilename);

                datatier.ExecuteActionQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //
            listBox2_SelectedIndexChanged(sender, e);
            listBox1_SelectedIndexChanged(sender, e);          
            //
        }// End Reset button

       
        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }// End Class    
}// End namespace
