using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOneShop
{
   public class AOneShopDBHelper
    {
        // SQL connection with connection string
        SqlConnection conn = new SqlConnection("Data Source=LAPTOP-88F3P2E9;Initial Catalog=Rent;Integrated Security=True");
        // reader to read data from cmd
        SqlDataReader reader;
        // prepareing query using cmd object
        SqlCommand cmd = new SqlCommand();
        string query = ""; // to store query

        // method to fetch rented record
        public DataTable fetchRentedRecord()
        {
            DataTable dtRentedRecord = new DataTable();
            try
            {
                cmd.Connection = conn;
                query = "SELECT * FROM RentedMovies Order by RMID DESC";
                cmd.CommandText = query;
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    dtRentedRecord.Load(reader);
                }
                return dtRentedRecord;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
                return null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
        // method to insert rented movie
        public void insertRentedRecord(int MovieIDFK, int CustIDFK, DateTime dateRented, int copies, int rented)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                query = "INSERT INTO RentedMovies(MovieIDFK, CustIDFK, DateRented,Rented) VALUES (@MovieIDFK,@CustIDFK,@DateRented,@Rented)";
                cmd.Parameters.AddWithValue("@MovieIDFK", MovieIDFK);
                cmd.Parameters.AddWithValue("@CustIDFK", CustIDFK);
                cmd.Parameters.AddWithValue("@DateRented", dateRented);
                cmd.Parameters.AddWithValue("@Rented", rented);
                cmd.CommandText = query;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        // method to update rented record
        public void updateRentedRecord(int RMID, int MovieID, DateTime dateRented, DateTime dateReturned)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                int rentTotal = 0, cost = 0;
                // calculate total movie rented days
                double days = (dateReturned - dateRented).TotalDays;
                query = "SELECT Rental_Cost FROM Movies WHERE MovieID = @MovieIDFK";
                cmd.Parameters.AddWithValue("@MovieIDFK", MovieID);
                cmd.CommandText = query;
                conn.Open();
                cost = Convert.ToInt32(cmd.ExecuteScalar());

                MessageBox.Show(cost.ToString());
                if (Convert.ToInt32(days) == 0)
                {
                   // set one day movie rent
                    rentTotal = cost;
                }
                else
                // calculate more than 1 days movie rent
                {
                    rentTotal = Convert.ToInt32(days) * cost;
                }
                // query to set date returned
                query = "UPDATE RentedMovies SET DateReturned = @DateReturned WHERE RMID = @RMID";
                cmd.Parameters.AddWithValue("@DateReturned", dateReturned);
                cmd.Parameters.AddWithValue("@RMID", RMID);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                // query to minus no of copies
                query = "UPDATE Movies SET copies = copies+1 WHERE MovieID = @MovieIDFK";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                // qery to reset rented status
                query = "UPDATE RentedMovies SET Rented = 0 WHERE RMID = @RMID";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                // display total rent
                MessageBox.Show("Customer Total Rent for Rented Movie is  " + rentTotal,"total rent",MessageBoxButtons.OK,MessageBoxIcon.Information);



            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Exception: " + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
      

    
        public DataTable fetchMovies()
        { //show all the movies in list view
            DataTable dt = new DataTable();
            try
            {
                cmd.Connection = conn;
                query = "Select * from Movies";

                cmd.CommandText = query;
                //open connection
                conn.Open();

                // reader execute from command
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }
                return dt;
            }
            catch (Exception ex)
            {
                // display message
                MessageBox.Show("Database Exception" + ex.Message);
                return null;
            }
            finally
            {
                // close reader object
                if (reader != null)
                {
                    reader.Close();
                }

                // connection close finally
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }



            // method to insert data in movies table
        public void insertMovieRecord(string Rating, string Title, string Year, string Rental_Cost, string Plot, string Genre, int copies)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                query = "Insert into Movies(Rating, Title, Year, Rental_Cost, Plot, Genre, copies) Values( @Rating, @Title, @Year, @Rental_Cost, @Plot, @Genre, @copies)";
                cmd.Parameters.AddWithValue("@Rating", Rating);
                cmd.Parameters.AddWithValue("@Title", Title);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Rental_Cost", Rental_Cost);
                cmd.Parameters.AddWithValue("@Plot", Plot);
                cmd.Parameters.AddWithValue("@Genre", Genre);
                cmd.Parameters.AddWithValue("@copies", copies);

                cmd.CommandText = query;

                // open connection
                conn.Open();

                // query execute
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                // display error
                MessageBox.Show("Database Exception" + ex.Message);
            }
            finally
            {
                // close connection finally
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        // method to delete movie
        public void deleteMovieRecord(int MovieID)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;


                //blow code is to check if the Movie is rented
                String check = "";
                check = "select Count(*) from RentedMovies where MovieIDFK = @MovieID and Rented ='1' ";
                SqlParameter[] parameterArray = new SqlParameter[] { new SqlParameter("@MovieID", MovieID) };
                cmd.Parameters.Add(parameterArray[0]);

                cmd.CommandText = check;
                conn.Open();
                //this code will delete the movie if its not rented otherwise the else statement would work
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    String k = "Delete from Movies where MovieID like @MovieID";
                    cmd.CommandText = k;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted", "total rent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //display the message if he has a movie on rent 
                    MessageBox.Show("Movie is rented by this customer", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("Database Exception" + exception.Message);
            }
            finally
            {
                if (this.conn != null)
                {
                    this.conn.Close();
                }

            }
        }



            //method to update movie record 
        public void updateMovieRecord(int MovieID, string Rating, string Title, int Year, string Plot, string Genre, int copies)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                // query
                query = "Update Movies Set Rating = @Rating, Title = @Title, Year = @Year,  Plot = @Plot, Genre = @Genre, copies = @copies where MovieID like @MovieID";
               // set parameters
                cmd.Parameters.AddWithValue("@MovieID", MovieID);
                cmd.Parameters.AddWithValue("@Rating", Rating);
                cmd.Parameters.AddWithValue("@Title", Title);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Plot", Plot);
                cmd.Parameters.AddWithValue("@Genre", Genre);
                cmd.Parameters.AddWithValue("@copies", copies);

                cmd.CommandText = query;

                //open db connection
                conn.Open();

                // Execute non query
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // display error msg
                MessageBox.Show("Database Exception" + ex.Message);
            }
            finally
            {
                // close connection finally
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
       
        public DataTable fetchCustomerRecords()
        {
            DataTable dtCustomerRecords = new DataTable();
            try
            {
                cmd.Connection = conn;
                query = "SELECT * from Customer";
                cmd.CommandText = query;
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    dtCustomerRecords.Load(reader);
                }
                return dtCustomerRecords;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        // method to insert new customer
        public void insertCustomerRecord(string firstname, string lastname, string address, string phone)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                query = "INSERT INTO Customer(FirstName,LastName,Address,Phone) VALUES (@fname,@lname,@addr,@phone)";
                cmd.Parameters.AddWithValue("@fname", firstname);
                cmd.Parameters.AddWithValue("@lname", lastname);
                cmd.Parameters.AddWithValue("@addr", address);
                cmd.Parameters.AddWithValue("@phone", phone);

                cmd.CommandText = query;

                conn.Open();
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            finally
            {
                // close objects finally
                if (conn != null)
                {
                    conn.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
        // method to delete customer record
        public void deleteCustomerRecord(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                query = "SELECT Count(*) FROM RentedMovies WHERE CustIDFK=@custid";
                cmd.Parameters.AddWithValue("@custid", id);
                cmd.CommandText = query;
                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    query = "DELETE FROM Customer WHERE CustID = @custid";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("This customer has rented a movie", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }

            }
        }
            //method to  update customer record
        public void updateCustomerRecord(int CustID, string FirstName, string LastName, string Address, string Phone)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                query = "Update Customer Set FirstName = @FirstName, LastName = @LastName, Address = @Address, Phone = @Phone where CustID = @CustID";


                cmd.Parameters.AddWithValue("@CustID", CustID);
                cmd.Parameters.AddWithValue("@FirstName", FirstName);
                cmd.Parameters.AddWithValue("@LastName", LastName);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@Phone", Phone);

                cmd.CommandText = query;

                //connection opened
                conn.Open();

                // Executed query
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // show error Message
                MessageBox.Show("Database Exception" + ex.Message);
            }
            finally
            {
                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
       

        public bool databaseConnectionTest()
        {
            if(conn.State!=ConnectionState.Open)
            {
                conn.Open();
            }
            return true;
        }
       
    }
}
