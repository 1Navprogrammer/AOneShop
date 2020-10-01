using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOneShop
{
    class HelperFunctions
    {
        SqlConnection conn = new SqlConnection("Data Source = LAPTOP-88F3P2E9; Initial Catalog = Rent; Integrated Security = True");
        SqlCommand cmd = new SqlCommand();

        string query;

        public void fetchBestCustomerRecord()
        {// find best customer
            int Top = 0, Max = 0, Total = 0;
            string Value = "";
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                string Val = "Select IDENT_CURRENT('Customer')";

                cmd.CommandText = Val;
                conn.Open();
                Total = Convert.ToInt32(cmd.ExecuteScalar());

                for (int i = 1; i <= Total; i++)
                {

                    Value = "select Count(*) from RentedMovies where CustIDFK= '" + i + "'";


                    cmd.CommandText = Value;
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > Max)
                    {
                        Max = count;
                        Top = i;
                    }
                }
                this.query = "Select FirstName from Customer where CustID ='" + Top + "'";
                this.cmd.CommandText = this.query;
                String FirstName = Convert.ToString(cmd.ExecuteScalar());
                MessageBox.Show("Best customer of aone shop is " + FirstName + "\n he as rented movies: " + Max, "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Database Exception " + exception.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }


        public void fetchBestMovieRecord()
        {// find best movie 
            int Top = 0, Max = 0, Total = 0;
            string Value = "";
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                string Val = "Select IDENT_CURRENT('Movies')";

                cmd.CommandText = Val;
                conn.Open();
                Total = Convert.ToInt32(cmd.ExecuteScalar());

                for (int i = 1; i <= Total; i++)
                {

                    Value = "select Count(*) from RentedMovies where MovieIDFK= '" + i + "'";


                    cmd.CommandText = Value;
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > Max)
                    {
                        Max = count;
                        Top = i;
                    }
                }


                this.query = "Select Title from Movies where MovieID ='" + Top + "'";
                this.cmd.CommandText = this.query;
                String Title = Convert.ToString(cmd.ExecuteScalar());
                MessageBox.Show("best movie in aone shop is  " + Title + "\nit is rented: " + Max+" times", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Database Exception " + exception.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
    }
}
