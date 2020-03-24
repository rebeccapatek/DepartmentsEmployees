using DepartmentsEmployees.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DepartmentsEmployees.Data
{
    // This class is for retrieving data from our database
    public class DepartmentRepository
    {
        public SqlConnection Connection
        {
            get
            {
                string _connectionString = "Data Source=localhost\\SQLEXPRESS; Initial Catalog=DepartmentsEmployees; Integrated Security=True";
                return new SqlConnection(_connectionString);
            }
        }

        public List<Department> GetAllDepartmentss()
        {
            // 1. Open a connection to the database
            // 2. Create a SQL SELECT  statement as a C# string
            // 3. Execute that SQL statement against the database
            // 4. From the database, we get "raw data" back. We need to parse this as a C# object
            // 5. Close the connection to the database
            // 6. Return the Department object


            // This opens the connection. SQLConnection is the TUNNEL
            using (SqlConnection conn = Connection)
            {
                // This opens the GATES on either side of the TUNNEL
                conn.Open();

                // SQLCommand is the list of instructions to give to a truck driver when they get to the other side of the TUNNEL
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here is the sql command that we want to be run when the driver gets to the database
                    cmd.CommandText = @"
                        SELECT Id, DeptName
                        FROM Department" ;

                    // ExecuteReader actually has the driver go to the database and executes that command. The driver then comes back with a bunch of data from the database. This is held in the this variable called "reader"
                    SqlDataReader reader = cmd.ExecuteReader();

                    // This is just us initializing the list that we'll eventually return
                    List<Department> allDepartments = new List<Department>();


                    // The reader will read the returned data from the database one row at a time. This is why we put it in a while loop
                    while (reader.Read())
                    {
                        // Get ordinal returns us what "position" the Id column is in
                        int idColumn = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumn);

                        // The reader isn't smart enough to know automatically what TYPE of data it's reading.
                        // For that reason we have to tell it, by saying `GetInt32`, `GetString`, GetDate`, etc
                        int departmentNameColumn = reader.GetOrdinal("DeptName");
                        string departmentNameValue = reader.GetString(departmentNameColumn);

                        // Now that all the data is parsed, we create a new C# object
                        var department = new Department()
                        {
                            Id = idValue,
                            DeptName = departmentNameValue
                            
                        };

                        // Now that we have a parsed C# object, we can add it to the list and continue with the while loop
                        allDepartments.Add(department);
                    }

                    // Now we can close the connection
                    reader.Close();

                    // and return all departments
                    return allDepartments;
                }
            }
        }

        public Department GetDepartmentById(int departmentId)
        {
            // This opens the connection. SQLConnection is the TUNNEL
            using (SqlConnection conn = Connection)
            {
                // This opens the GATES on either side of the TUNNEL
                conn.Open();

                // SQLCommand is the list of instructions to give to a truck driver when they get to the other side of the TUNNEL
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here is the sql command that we want to be run when the driver gets to the database
                    cmd.CommandText = @"
                        SELECT id, DeptName FROM Department
                        WHERE Id = @id";

                    // This is us telling the truck driver that there is a VARIABLE in the sql statement. When you get to the database, replace the string "@id" with departmentId
                    cmd.Parameters.Add(new SqlParameter("@id", departmentId));

                    // ExecuteReader actually has the driver go to the database and executes that command. The driver then comes back with a bunch of data from the database. This is held in the this variable called "reader"
                    SqlDataReader reader = cmd.ExecuteReader();


                    // The reader will read the returned data from the database if it finds the single row we're looking for. If it doesn't find the employee with the given Id, reader.Read() will return false
                    if (reader.Read())
                    {
                        // Get ordinal returns us what "position" the Id column is in
                        int idColumn = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumn);

                        int departmentNameColumn = reader.GetOrdinal("DeptName");
                        string departmentNameValue = reader.GetString(departmentNameColumn);

                        // Now that all the data is parsed, we create a new C# object
                        var department = new Department()
                        {
                            Id = idValue,
                            DeptName = departmentNameValue
                         };

                        // Now we can close the reader
                        reader.Close();

                        return department;
                    }
                    else
                    {
                        // We didn't find the department with that ID in the database. return null
                        return null;
                    }
                }
            }
        }


        // Create a new department
        public Department CreateNewDepartment(Department departmentToAdd)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Department (DeptName)
                        OUTPUT INSERTED.Id
                        VALUES (@deptname)";
 
                    cmd.Parameters.Add(new SqlParameter("@departmentId", departmentToAdd.DeptName));

                    int id = (int)cmd.ExecuteScalar();

                    departmentToAdd.Id = id;

                    return departmentToAdd;
                }
            }
        }

        // Update Department
        public void UpdateDepartment(int departmentId, Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Department
                        SET DeptName = @deptname
                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@deptname", department.DeptName));
                    cmd.Parameters.Add(new SqlParameter("@id", departmentId));

                    // We don't expect anything back from the database (it's not really a "query", so we can say Execute NonQuery
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete a Department
        public void DeleteDepartment(int departmentId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Department WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", departmentId));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

