using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using User.Models;

namespace User.Models
{
    public class BALUser
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbuser"].ToString());


        public void RegisterUser(string firstname,string lastname,string email,string password,bool emailverification,string activationcode)
        {
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "RegisterUser");
            cmd.Parameters.AddWithValue("@firstname", firstname);
            cmd.Parameters.AddWithValue("@lastname", lastname);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@emailverification", emailverification);
            cmd.Parameters.AddWithValue("@activetioncode", activationcode);
            cmd.ExecuteNonQuery();
            con.Close();

        }

        public SqlDataReader EmailVerification(string email)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "EmailVerification");
            cmd.Parameters.AddWithValue("@email",email);
            SqlDataReader drEmail;
            drEmail = cmd.ExecuteReader();
            return drEmail;
            //con.Close();
        }

        public SqlDataReader UserVerification(string activationcode)
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "UserVerification");
            cmd.Parameters.AddWithValue("@activetioncode", activationcode);
            SqlDataReader drActivationCode;
            drActivationCode = cmd.ExecuteReader();
            return drActivationCode;
           //con.Close();
        }

        public void UpdateEmailVerificationStatus(bool emailverification,string activetioncode)
        {
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "EmailVerificationStatus");
            cmd.Parameters.AddWithValue("@emailverification", emailverification);
            cmd.Parameters.AddWithValue("@activetioncode", activetioncode);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        public SqlDataReader SignIn(string email,string password)
        {
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "SignIn");
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            SqlDataReader drSignIn;
            drSignIn = cmd.ExecuteReader();
            return drSignIn;
            //con.Close();
        }

        public void ForgetPassword(string email,string activetioncode,string otp)
        {
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "ForgetPassword");
            cmd.Parameters.AddWithValue("@email",email);
            cmd.Parameters.AddWithValue("@activetioncode", activetioncode);
            cmd.Parameters.AddWithValue("@otp", otp);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public SqlDataReader OTPVerification(string activetioncode)
        {
            
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "OTPVerification");
            cmd.Parameters.AddWithValue("@activetioncode", activetioncode);
            SqlDataReader drOTP;
            drOTP = cmd.ExecuteReader();
            return drOTP;
            //con.Close();
        }

        public SqlDataReader GetEmailForchangePassword(string activetioncode)
        {
           
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "GetEmailForChangeP");
            cmd.Parameters.AddWithValue("@activetioncode", activetioncode);
            SqlDataReader drEmailForChangeP;
            drEmailForChangeP = cmd.ExecuteReader();
            return drEmailForChangeP;
            //con.Close();
        }

        public void ChangePassword(string email,string password)
        {
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "ChangePassword");
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password",password);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public DataSet GetUserList()
        {
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "ShowDataToList");
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            DataSet dsUser = new DataSet();
            adpt.Fill(dsUser);
            return dsUser;

        }

        /*************************************************Bind Country,state,city************************************/
       
        public DataSet Get_Country()
        {
            
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Flag", "BindCountry");
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adpt.Fill(ds);
            return ds;


        }
        public DataSet Get_Sate(int countryId)
        {

            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Flag", "BindState");
            cmd.Parameters.AddWithValue("@countryid", countryId);
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adpt.Fill(ds);
            return ds;

        }
        public DataSet Get_City(int StateId)
        {


            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Flag", "BindCity");
            cmd.Parameters.AddWithValue("@stateid", StateId);
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adpt.Fill(ds);
            return ds;

        }
        public void SaveUserDetails(string fullname,string gender,string mobileno,DateTime dob,string address,int cityid)
        {
            con.Close();
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "SaveUserDetails");
            cmd.Parameters.AddWithValue("@fullname", fullname);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@mobileno", mobileno);
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@cityid", cityid);
            cmd.ExecuteNonQuery();
            con.Close();

        }

        public SqlDataReader ShowUserDetailsForEdit(int UserdetailsId)
        {
           
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "ShowForEdit");
            cmd.Parameters.AddWithValue("@userdetailsid", UserdetailsId);
            SqlDataReader dr;
            dr = cmd.ExecuteReader();
            return dr;
           

        }

        public void EditUserDetails(int Userdetailsid, string fullname, string gender, string mobileno, DateTime dob, string address, int cityid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "ShowForEdit");
            cmd.Parameters.AddWithValue("@userdetailsid", Userdetailsid);
            cmd.Parameters.AddWithValue("@fullname", fullname);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@mobileno", mobileno);
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@cityid", cityid);
            cmd.ExecuteNonQuery();
            con.Close();

        }

        public void UpdateUserDetails(int Userdetailsid, string fullname, string gender, string mobileno, DateTime dob, string address, int cityid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "UpdateUserDetails");
            cmd.Parameters.AddWithValue("@userdetailsid", Userdetailsid);
            cmd.Parameters.AddWithValue("@fullname", fullname);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@mobileno", mobileno);
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@cityid", cityid);
            cmd.ExecuteNonQuery();
            con.Close();

        }

        public void DeleteUser(int Userdetailsid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UserInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@flag", "DeleteUser");
            cmd.Parameters.AddWithValue("@userdetailsid", Userdetailsid);
            cmd.ExecuteNonQuery();
            con.Close();

        }

    }
}