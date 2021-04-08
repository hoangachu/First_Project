
using Hoang_Project_one.Models.HistoryChat;
using Hoang_Project_one.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hoang_Project_one.Controllers
{
    public class UserController : Controller
    {
        ChatBotController chatBotController = new ChatBotController();
        public IActionResult Index()
        {
            return View();
        }

        public User GetUserByUserID(int UserID)
        {
            User User = new User();
            using (SqlConnection con = new SqlConnection(Startup.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserByUserID", con))
                {
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            con.Open();
                            cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = UserID;

                            SqlDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                User.UserName = (string)dr["UserName"];
                                User.Password = (string)dr["Password"];
                                User.UserID = (int)dr["UserID"];
                                User.imgURL = (string)dr["ImagesUrl"];
                            }




                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                        con.Close();
                    }


                }
            }
            return User;
        }
        public User GetUserByUserName(string UserName)
        {
            User User = new User();
            using (SqlConnection con = new SqlConnection(Startup.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserByUserName", con))
                {
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            con.Open();
                            cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;

                            SqlDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                            {
                                User.UserName = (string)dr["UserName"];
                                User.Password = (string)dr["Password"];
                                User.UserID = (int)dr["UserID"];
                                User.imgURL = (string)dr["FileURL"];
                            }



                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                        con.Close();
                    }


                }
            }
            return User;
        }
        [HttpPost]
        public IActionResult RegistUser([FromForm(Name = "file")] IFormFile file, [FromForm(Name = "UserName")] string UserName, [FromForm(Name = "Password")] string Password)
        {
            int UserID = 0;
            int i = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(Startup.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Add_User", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
                        cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = Multis.Multis.Encrypt(Password);
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        con.Open();
                        i = cmd.ExecuteNonQuery();
                        UserID = Convert.ToInt32(cmd.Parameters["@UserID"].Value);
                        FileController.SaveFile(file, UserID);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok(new { data = i, userID = UserID, url = "Home" });
        }


        [HttpPost]
        public void Update(List<Friend> lstFriend, int typeUpdate, int userID)
        {
            using (SqlConnection con = new SqlConnection(Startup.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Update_FriendStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var table = new DataTable();
                    table.Columns.Add("Item", typeof(int));
                    lstFriend.ForEach(x => table.Rows.Add(x.FriendId));
                    var pList = new SqlParameter("@list", SqlDbType.Structured);
                    pList.TypeName = "dbo.listID";
                    pList.Value = table;
                    cmd.Parameters.Add(pList);
                    cmd.Parameters.Add("@typeUpdate", SqlDbType.Int).Value = typeUpdate;
                    cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }



        }
        [HttpPost]
        public void UpdateUserStatus(int userID)
        {
            using (SqlConnection con = new SqlConnection(Startup.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Update_UserStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }
    }
}
