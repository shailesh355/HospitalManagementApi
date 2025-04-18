﻿using BaseClass;
using DmfPortalApi.Models.AppClass;
using HospitalManagementApi.Models.Balayer;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static BaseClass.ReturnClass;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlCommon
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<List<ListValue>> GetStateAsync(LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            dt = await db.ExecuteSelectQueryAsync(@"SELECT  s.stateId as id, s.stateName" + fieldLanguage + @" as name
                                                    FROM state s
                                                    ORDER BY name");
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        /// <summary>
        /// Get List of District
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetDistrictAsync(int stateId, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT d.districtId AS id, d.districtName" + fieldLanguage + @" AS name
                             FROM district d
                             WHERE d.stateId = @stateId
                             ORDER BY name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("stateId", MySqlDbType.Int16) { Value= stateId }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        public async Task<List<ListValue>> GetBaseDepartmentAsync(int stateId, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT b.deptId AS id, b.deptName" + fieldLanguage + @" as name 
                             FROM basedepartment b
                             WHERE b.stateId = @stateId and b.active = @active
                             ORDER BY name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("active", MySqlDbType.Int16) { Value = (int) Active.Yes },
                new MySqlParameter("stateId", MySqlDbType.Int16) { Value = stateId }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        #region Get Common List from DDL Cat List
        /// <summary>
        ///Get Category List from ddlCat
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetCommonListAsync(string category, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT d.id AS id, d.name" + fieldLanguage + @" AS name, d.fieldTypeId as extraField 
                             FROM ddlcatlist d
                             WHERE d.active = @active AND d.category = @category AND d.hideFromPublicAPI = @hideFromPublicAPI AND d.isStateSpecific=@isStateSpecific
                             ORDER BY d.sortOrder, name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("hideFromPublicAPI", MySqlDbType.Int16){ Value=(int) YesNo.No},
                new MySqlParameter("active", MySqlDbType.Int16){ Value = (int) Active.Yes},
                new MySqlParameter("isStateSpecific", MySqlDbType.Int16){ Value= (int)YesNo.No},
                new MySqlParameter("category", MySqlDbType.String) { Value= category }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        /// <summary>
        ///Get Sub category List from ddlCat
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetSubCommonListAsync(string category, string id, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT d.id AS id, d.name" + fieldLanguage + @" AS name, d.grouping AS extraField
                             FROM ddlcatlist d
                             WHERE d.active = @active AND d.category = @category AND d.referenceId=@referenceId AND d.hideFromPublicAPI = @hideFromPublicAPI AND 
                                   d.isStateSpecific=@isStateSpecific
                             ORDER BY d.sortOrder, name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("hideFromPublicAPI", MySqlDbType.Int16){ Value=(int) YesNo.No},
                new MySqlParameter("active", MySqlDbType.Int16){ Value = (int) Active.Yes},
                new MySqlParameter("isStateSpecific", MySqlDbType.Int16){ Value= (int)YesNo.No},
                new MySqlParameter("category", MySqlDbType.String) { Value= category }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }

        /// <summary>
        ///Get State Specific Category List from ddlCat
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetCommonListStateAsync(string category, LanguageSupported language, int stateId)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT d.id AS id, d.name" + fieldLanguage + @" AS name, d.grouping as extraField
                             FROM ddlcatlist d
                             WHERE d.active = @active AND d.category = @category AND d.hideFromPublicAPI = @hideFromPublicAPI AND d.isStateSpecific=@isStateSpecific AND
                                   d.stateId = @stateId
                             ORDER BY d.sortOrder, name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("hideFromPublicAPI", MySqlDbType.Int16){ Value=(int) YesNo.No},
                new MySqlParameter("active", MySqlDbType.Int16){ Value = (int) Active.Yes},
                new MySqlParameter("isStateSpecific", MySqlDbType.Int16){ Value= (int)YesNo.Yes},
                new MySqlParameter("category", MySqlDbType.String) { Value= category },
                new MySqlParameter("stateId", MySqlDbType.Int16) { Value= stateId }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        #endregion
        public async Task<List<ListValue>> GetDesignationAsync(int stateId, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT d.designationId AS id, designationName" + fieldLanguage + @" AS name
                             FROM designation d
                             WHERE d.stateId = @stateId
                             ORDER BY d.designationId";
            MySqlParameter[] pm = new MySqlParameter[]{
                new MySqlParameter("stateId", MySqlDbType.Int16){ Value = stateId }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        /// <summary>
        /// Verify captcha 
        /// </summary>
        /// <returns></returns>
        public ReturnClass.ReturnBool VerifyCaptcha(CaptchaReturnType ct, string verification_url)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Uri url = new Uri(verification_url);
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string serialisedData = Newtonsoft.Json.JsonConvert.SerializeObject(ct);
            try
            {
                var response = client.UploadString(url, serialisedData);
                dynamic jsonData = Newtonsoft.Json.Linq.JObject.Parse(response);
                rb.status = jsonData.status;
                rb.message = jsonData.message;
            }
            catch (Exception ex)
            {
                WriteLog.Error("VerifyCaptcha", ex);
            }
            return rb;

        }
        /// <summary>
        /// insert an entry for each login attempt regardless of successfull login or failure.
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnBool> InsertLoginTrail(LoginTrail lt)
        {
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("loginId", MySqlDbType.String) { Value = lt.loginId},
                new MySqlParameter("browserId", MySqlDbType.String){ Value=lt.browserId},
                new MySqlParameter("clientIp", MySqlDbType.String) { Value = lt.clientIp},
                new MySqlParameter("clientOs", MySqlDbType.String) { Value = lt.clientOs},
                new MySqlParameter("clientBrowser", MySqlDbType.String) { Value = lt.clientBrowser},
                new MySqlParameter("userAgent", MySqlDbType.String) { Value = lt.userAgent},
                new MySqlParameter("accessMmode", MySqlDbType.Int16) { Value = lt.siteAccessMode},
                new MySqlParameter("isLoginSuccessful", MySqlDbType.Int16) { Value = lt.isLoginSuccessful},
            };

            string query = @" INSERT INTO logintrail(loginId, browserId, clientIp, clientOs, clientBrowser, userAgent, accessMode, isLoginSuccessful)
                                VALUES(@loginId, @browserId, @clientIp, @clientOs, @clientBrowser, @userAgent, @accessMode, @isLoginSuccessful)";

            ReturnClass.ReturnBool succeded = await db.ExecuteQueryAsync(query, pm, "Logintrail");

            if (succeded.status)
                succeded.message = "Login trail created successfully";
            else
                succeded.message = "Could not create login trail";

            return succeded;
        }
        /// <summary>
        ///get User Detail During Login
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetUser(string emailid, string password, bool isSwsUser)
        {
            User user = new User();
            user.message = "Invalid user id or Password";

            YesNo isSingleWindowUser;
            YesNo changePassword;
            YesNo isDisabled;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                {
                    new MySqlParameter("emailid",MySqlDbType.String) { Value = emailid},
                    //new MySqlParameter("password",MySqlDbType.String) { Value = password},
                    new MySqlParameter("active",MySqlDbType.Int16) { Value = (int)Active.Yes}
                };
                //string where = @"  AND l.password = @password ";
                string query = @" SELECT l.userName, l.userId, l.password, l.changePassword, l.isDisabled, l.userRole, l.isSingleWindowUser
                                  FROM userlogin l
                                  WHERE l.emailId=@emailId AND l.active = @active ";
                //query = !isSwsUser ? query + where : query;
                query = !isSwsUser ? query : query;

                dt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dt.table.Rows.Count > 0)
                {
                    if (dt.table.Rows[0]["password"].ToString().Equals(password, StringComparison.InvariantCultureIgnoreCase))
                    {
                        DataRow dr = dt.table.Rows[0];
                        user.userId = Convert.ToInt64(dr["userId"]);
                        user.userName = dr["userName"].ToString();
                        user.role = Convert.ToInt16(dr["userRole"].ToString());

                        Enum.TryParse(dr["isSingleWindowUser"].ToString(), true, out isSingleWindowUser);
                        user.isSingleWindowUser = isSingleWindowUser;

                        Enum.TryParse(dr["changePassword"].ToString(), true, out changePassword);
                        user.forceChangePassword = changePassword;

                        Enum.TryParse(dr["isDisabled"].ToString(), true, out isDisabled);
                        if (isDisabled == YesNo.Yes)
                            user.message = "Account has been disabled";
                        else
                        {
                            user.isAuthenticated = true;
                            user.message = "Login successfull";
                        }
                        user.hodOfficeId = 0;
                        user.hodOfficeName = null;
                        if (user.role == (Int16)UserRole.Hospital)
                        {
                            pm = new MySqlParameter[]
                           {
                            new MySqlParameter("userId",MySqlDbType.String) { Value = user.userId},
                            new MySqlParameter("isVerified",MySqlDbType.Int16) { Value = (int)Active.Yes}
                           };
                            query = @" SELECT h.hodOfficeId,h.hodOfficeName FROM  hodoffice ho
                                     JOIN hodofficeregistration h ON h.hodOfficeId=ho.hodOfficeId AND h.isVerified=@isVerified
                                     WHERE ho.loginId= @userId; ";
                            dt = await db.ExecuteSelectQueryAsync(query, pm);
                            if (dt.table.Rows.Count > 0)
                            {
                                dr = dt.table.Rows[0];
                                user.hodOfficeId = Convert.ToInt64(dr["hodOfficeId"]);
                                user.hodOfficeName = dr["hodOfficeName"].ToString();
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                WriteLog.Error("DlCommon(GetUser) : ", ex);
            }
            return user;
        }



        public async Task<ReturnClass.ReturnBool> callStoreAPI(BlDocumentNew bl, string urlString)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            try
            {
                Utilities util = new();
                //StoreApiURL = "https://localhost:7168/api/";
                ReturnBool rbBuild = util.GetAppSettings("Build", "Version");
                string buildType = rbBuild.message;
                string StoreApiURL = util.GetAppSettings("StoreApi", buildType, "URL").message;
                //if (StoreApiURL.Contains("localhost"))
                //StoreApiURL = "http://97.74.91.115:224/api/";
                Uri url = new Uri(StoreApiURL + urlString);
                HttpClient client = new();
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   //ACCEPT header

                var request = new BlDocumentNew
                {
                    documentInByteS = bl.documentInByteS,
                    loginId = (Int64)bl.userId!,
                    documentId = (Int64)bl.documentId,
                    documentType = bl.documentType,
                    documentNumber = bl.documentNumber,
                    amendmentNo = bl.amendmentNo,
                    documentName = bl.documentName,
                    documentMimeType = bl.documentMimeType,
                    stateId = bl.stateId,
                    documentImageGroup = bl.documentImageGroup,
                };
                HttpResponseMessage response = await client.PostAsJsonAsync(url, request);
                response.EnsureSuccessStatusCode(); // throws if not 200-299
                var contentStream = await response.Content.ReadAsStreamAsync();
                rb = await JsonSerializer.DeserializeAsync<ReturnClass.ReturnBool>(contentStream);
            }
            catch (Exception ex)
            {
                rb!.message = "Could not upload image , " + ex.Message;
                rb.status = false;
            }
            return rb;
        }

        public async Task<Int32> GetMaxDoctorScheduleDateId()
        {
            Int32 scheduleDateId = 0;
            string query = @"SELECT IFNULL(MAX(scheduleDateId),0) AS scheduleDateId
                                FROM doctorscheduledate ";
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query);
            DataRow dr = dt.table.Rows[0];
            dr = dt.table.Rows[0];
            scheduleDateId = Convert.ToInt32(dr["scheduleDateId"]);
            scheduleDateId++;
            return scheduleDateId;
        }

        /// <summary>
        /// Get List of District
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetCity(Int16 districtId, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT c.cityId AS id, c.cityName" + fieldLanguage + @" AS name
                             FROM city c
                             WHERE c.districtId = @districtId
                             ORDER BY name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("districtId", MySqlDbType.Int16) { Value= districtId }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        /// <summary>
        /// Get List of City by District
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetCityByName(Int16 stateId, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            string query = @"SELECT c.cityId AS id, c.cityName" + fieldLanguage + @" AS name
                                    FROM city AS c
                                WHERE c.stateId = @stateId
                                ORDER BY name";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("stateId", MySqlDbType.Int16) { Value= stateId }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        public async Task<Int32> ReturnCity(BlCity bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            ReturnClass.ReturnString rs = new();
            DlMasters dlmaster = new();
            string query = @"SELECT c.cityId,c.cityNameEnglish 
		                                    FROM city AS c
			                             WHERE c.cityNameEnglish=@cityNameEnglish AND stateId=@stateId";
            MySqlParameter[] pmc = new MySqlParameter[]
              {
                             new MySqlParameter("cityNameEnglish", MySqlDbType.VarChar,99) { Value = bl.cityNameEnglish },
                             new MySqlParameter("stateId", MySqlDbType.Int16) { Value = bl.stateId },
              };
            dt = await db.ExecuteSelectQueryAsync(query, pmc);
            if (dt.table.Rows.Count > 0)
            {
                bl.cityId = Convert.ToInt32(dt.table.Rows[0]["cityId"].ToString());
            }
            else
            {
                rs = await dlmaster.GetCityId(bl.stateId);
                bl.cityId = Convert.ToInt32(rs.any_id);
                bl.cityCount = Convert.ToInt32(rs.value);
                List<MySqlParameter> pm1 = new();
                pm1.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = bl.districtId });
                pm1.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = bl.stateId });
                pm1.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = bl.cityId });
                pm1.Add(new MySqlParameter("cityCount", MySqlDbType.Int32) { Value = bl.cityCount });
                pm1.Add(new MySqlParameter("cityName", MySqlDbType.String) { Value = bl.cityNameEnglish });
                pm1.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp });
                pm1.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId });
                pm1.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime });

                query = @"INSERT INTO city (stateId,districtId,cityId,cityNameEnglish,clientIp,entryDateTime,userId,cityCount)
                                        VALUES (@stateId,@districtId,@cityId,@cityName,@clientIp,@entryDateTime,@userId,@cityCount)";
                rb = await db.ExecuteQueryAsync(query, pm1.ToArray(), "city");
            }
            return bl.cityId;
        }

        public async Task<bool> CheckMailExistOnUserAsync(string emailId)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.emailId
                                    FROM userlogin u
                               WHERE u.emailId = @emailId ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarString) { Value = emailId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            return isAccountExists;
        }

        /// <summary>
        ///Get Category List from ddlCat
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataTable> GetCommonListDs(string category)
        {
            string query = @"SELECT d.id AS id, d.nameEnglish,d.nameLocal,d.description
	                            FROM ddlcatlist d
                            WHERE d.active = @active AND d.category = @category
	                            AND d.hideFromPublicAPI = @hideFromPublicAPI AND d.isStateSpecific=@isStateSpecific
                            ORDER BY d.sortOrder,d.nameEnglish";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("hideFromPublicAPI", MySqlDbType.Int16){ Value=(int) YesNo.No},
                new MySqlParameter("active", MySqlDbType.Int16){ Value = (int) Active.Yes},
                new MySqlParameter("isStateSpecific", MySqlDbType.Int16){ Value= (int)YesNo.No},
                new MySqlParameter("category", MySqlDbType.String) { Value= category }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> ResetPassword(ResetPassword resetPassword)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isExists = await checkOldPassword(resetPassword);
            if (isExists)
            {
                string query = @"Update userlogin  SET changePassword=@changePassword,password=@password
                              WHERE userId=@userId";
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = resetPassword.userId });
                pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.Yes });
                pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = resetPassword.Password });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
                if (rb.status)
                {


                    rb.message = "Password Changed Successfully";
                }

                else
                {
                    rb.message = "Somthing Went Wrong Please Try Again!!!";
                }
            }
            else
            {
                rb.message = "Old Password Not Matched!!";
                rb.error = string.Empty;
            }



            return rb;
        }

        public async Task<bool> checkOldPassword(ResetPassword resetPassword)
        {
            bool isExists = false;
            string query = @"SELECT  l.password
                                  FROM userlogin l
                                  WHERE l.userId=@userId AND l.active = @active ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = resetPassword.userId });
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                if (dt.table.Rows[0]["password"].ToString()!.Equals(resetPassword.oldPassword, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExists = true;
                }
            }
            return isExists;
        }

        public async Task<ReturnClass.ReturnDataSet> HomeSearch(HomeSearch appParam)
        {
            string query = string.Empty;
            string where = string.Empty;
            string whereSearchHosp = string.Empty;
            string whereSearchDoc = string.Empty;
            string whereSearchSpec = string.Empty;
            string whereSearchClin = string.Empty;
            ReturnClass.ReturnDataSet dataSet = new();
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                    new MySqlParameter("searchText", MySqlDbType.VarChar,200) { Value = "%" +appParam.searchedText+"%" },
                    new MySqlParameter("searchTypeId", MySqlDbType.Int16) { Value = appParam.searchTypeId },
                    new MySqlParameter("districtId", MySqlDbType.Int16) { Value = appParam.districtId },
                    new MySqlParameter("lat", MySqlDbType.Decimal) { Value = appParam.lat },
                    new MySqlParameter("long", MySqlDbType.Decimal) { Value = appParam.longi },
                    new MySqlParameter("active", MySqlDbType.Int16) { Value = YesNo.Yes },
                    new MySqlParameter("Website", MySqlDbType.Int16) { Value = DocumentImageGroup.Website },
                    new MySqlParameter("Mobile", MySqlDbType.Int16) { Value = DocumentImageGroup.Mobile },
                    new MySqlParameter("WebsiteBanner", MySqlDbType.Int16) { Value = DocumentType.WebsiteBanner},
                    new MySqlParameter("MobileBanner", MySqlDbType.Int16) { Value = DocumentType.MobileBanner},
                    new MySqlParameter("Hospital", MySqlDbType.Int16) { Value = DocumentImageGroup.Hospital },
                    new MySqlParameter("HospitalImage", MySqlDbType.Int16) { Value = DocumentType.HospitalImages},
                    new MySqlParameter("Doctor", MySqlDbType.Int16) { Value = DocumentImageGroup.Doctor },
                    new MySqlParameter("DoctorProfile", MySqlDbType.Int16) { Value = DocumentType.DoctorProfilePic},
                    new MySqlParameter("DoctorWorkArea", MySqlDbType.Int16) { Value = DocumentType.DoctorWorkArea},
                    new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = YesNo.Yes },
                    new MySqlParameter("hospitalSpecialization", MySqlDbType.VarChar) { Value = "hospitalSpecialization" },
                    new MySqlParameter("homeSearch", MySqlDbType.VarChar) { Value = "HomeSearch" },
                    new MySqlParameter("searchSubCategoryTypeId", MySqlDbType.Int16) { Value= appParam.searchSubCategoryTypeId },
                    new MySqlParameter("specializationId", MySqlDbType.Int16) { Value= appParam.specializationId },
                    new MySqlParameter("genderId", MySqlDbType.Int16) { Value= appParam.genderId },
                    new MySqlParameter("gender", MySqlDbType.VarString) { Value= "gender" }
                   };
                if (appParam.districtId != 0)
                    where += " AND ins.districtId = @districtId ";
                if (appParam.lat != 0 && appParam.longi != 0)
                {
                    where += " AND dist.latitude BETWEEN @lat - 0.30 AND @lat + 0.30 " +
                             " AND dist.longitude BETWEEN @long - 0.30 AND @long + 0.30 ";
                }
                if (appParam.searchedText != null && appParam.searchedText != "")
                {
                    whereSearchHosp += " AND ins.hospitalNameEnglish LIKE @searchText ";
                    whereSearchDoc += " AND ins.doctorNameEnglish LIKE @searchText ";
                    whereSearchSpec += " AND ins.nameenglish LIKE @searchText ";
                    whereSearchClin += " AND ins.doctorNameEnglish LIKE @searchText ";
                }
                if (appParam.searchSubCategoryTypeId == 3 && appParam.specializationId != 0) // Doctor
                {
                    whereSearchClin += whereSearchDoc += @" AND ( dp.specializationId = @specializationId || ds.specializationId=@specializationId ) ";
                    whereSearchHosp += @" AND hs.specializationId = @specializationId ";
                }
                if (appParam.genderId != null && appParam.genderId != 0)
                {
                    whereSearchDoc += " AND dp.genderId = @genderId ";
                    whereSearchClin += " AND dp.genderId = @genderId ";
                }
                //1=Hospital,3=Doctor
                query = @" SELECT ins.id,ins.nameEnglish AS searchCategory, CASE WHEN ins.id IN (1,3) THEN 1 ELSE 0 END AS isSpecialisationAvailable ,
                                CASE WHEN ins.id IN (3) THEN 1 ELSE 0 END AS isGenderVisible
	                            FROM ddlcatlist AS ins WHERE ins.category=@homeSearch"
                            + " ORDER BY ins.sortOrder ";
                ReturnClass.ReturnDataTable dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "SearchTabs";
                dataSet.dataset.Tables.Add(dtt.table);


                query = @" SELECT ins.hospitalRegNo,ins.hospitalNameEnglish,ins.districtId,ins.address,ins.mobileNo,ins.emailId,ins.registrationYear,
		                                ins.cityId,ins.pinCode,ins.phoneNumber,ins.landMark,ins.fax,ins.isCovid,ins.latitude,ins.longitude,ins.typeOfProviderId,ins.website,ins.natureOfEntityId,
                                        hosImg.documentId AS hospitalImagedocumentId,hosImg.documentName AS hospitalImageName,hosImg.documentExtension AS hospitalImageExtension,
                                        GROUP_CONCAT(DISTINCT hs.specializationTypeName) AS specializationTypeName,
		                                  GROUP_CONCAT(DISTINCT hs.specializationName) AS specializationName,
		                                  GROUP_CONCAT(DISTINCT hs.levelOfCareName) AS levelOfCareName, @HospitalImage AS hospitalImageDocumentType
	                                FROM hospitalregistration AS ins 
                                        INNER JOIN district AS dist ON dist.districtId = ins.districtId
                                        LEFT JOIN hospitalspecialization AS hs ON hs.hospitalRegNo=ins.hospitalRegNo
                                        LEFT JOIN ( 
                                        		SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId
													     FROM documentstore AS ds
													     WHERE ds.documentName IN 
                                                        ( SELECT ds.documentName
                                                            FROM documentstore AS ds 
					                                            INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
													         WHERE dpt.documentImageGroup=@Hospital AND dpt.documentType = @HospitalImage	
													      GROUP BY ds.documentId 
                                                        )
													 ) AS hosImg ON hosImg.documentId = ins.hospitalRegNo
		                            WHERE ins.active=@active AND ins.isVerified =@isVerified " + where + whereSearchHosp
                                    + " GROUP BY hs.hospitalRegNo ORDER BY ins.hospitalNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dtt.table.Rows.Count == 0)
                {
                    query = @" SELECT 'Hospitals data for this will be added soon.' AS noData";
                    dtt = await db.ExecuteSelectQueryAsync(query, pm);
                    dtt.status = false;
                    dtt.message = " Hospitals data for this will be added soon.";
                }

                dtt.table.TableName = "Hospitals";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ins.doctorRegNo,ins.doctorNameEnglish,ins.doctorNameLocal,ins.stateId,ins.districtId,ins.address AS DoctorAddress,ins.mobileNo,
                                    ins.emailId,ins.active,dist.districtNameEnglish AS DoctorDistrictName,ins.cityId,ins.cityName AS DoctorCityname,
                                    docImg.documentId AS doctorProfileDocumentId,docImg.documentName AS doctorProfileName,docImg.documentExtension AS doctorProfileExtension,
								    GROUP_CONCAT(distinct ds.specializationTypeName) AS specializationTypeName,
		                            GROUP_CONCAT(distinct ds.specializationName) AS specializationName,
                                    GROUP_CONCAT(DISTINCT ds.levelOfCareName) AS levelOfCareName, IFNULL(ddlcatGender.nameEnglish, '') AS doctorGender,
                                    IFNULL(dp.genderId, 0 ) AS genderId, @DoctorProfile AS doctorProfileDocumentType
                                FROM doctorregistration AS ins 
				                    INNER JOIN district AS dist ON dist.districtId=ins.districtId 
                                    LEFT JOIN doctorprofile AS dp ON dp.doctorRegNo = ins.doctorRegNo 
                                    LEFT JOIN ddlcatlist AS ddlcatGender ON dp.genderId = ddlcatGender.id AND ddlcatGender.category=@gender
				                    LEFT JOIN doctorspecialization AS ds ON ds.doctorRegNo = ins.doctorRegNo 
				                    LEFT JOIN ( 
                                       	SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId
													     FROM documentstore AS ds
													     WHERE ds.documentName IN 
                                                        ( SELECT ds.documentName
                                                            FROM documentstore AS ds 
					                                            INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
													     WHERE dpt.documentImageGroup=@Doctor AND dpt.documentType = @DoctorProfile	
													      GROUP BY ds.documentId 
                                                        )
													 )  AS docImg ON docImg.documentId = ins.doctorRegNo
                            WHERE ins.registrationStatus=@active " + where + whereSearchDoc
                            + " GROUP BY ins.doctorRegNo ORDER BY ins.doctorNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dtt.table.Rows.Count == 0)
                {
                    query = @" SELECT 'Doctors data for this Area will be added soon.' AS noData";
                    dtt = await db.ExecuteSelectQueryAsync(query, pm);
                    dtt.status = false;
                    dtt.message = " Doctors data for this Area will be added soon.";
                }
                dtt.table.TableName = "Doctors";
                dataSet.dataset.Tables.Add(dtt.table);

                if (appParam.isMobileView == 1)
                    query = @" SELECT ins.id,ds.dptTableId, ins.id,ins.nameEnglish AS specializationName, 
		                    ds.documentId AS iconDocumentId ,ds.documentNumber AS iconDocumentNumber, 
		                    ds.documentName AS iconDocumentName,ds.documentExtension AS iconDocumentExtension, 14 AS iconDocumentType
	                    FROM ddlcatlist AS ins
	                    INNER JOIN documentstore AS ds ON ins.id = ds.districtId
	                    INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
                        WHERE dpt.documentImageGroup = 3 AND dpt.documentType = 14 AND ins.category=@hospitalSpecialization 
                        ORDER BY ins.nameEnglish ";
                else
                    query = @" SELECT ins.id,ds.dptTableId, ins.id,ins.nameEnglish AS specializationName, 
		                    ds.documentId AS iconDocumentId ,ds.documentNumber AS iconDocumentNumber, 
		                    ds.documentName AS iconDocumentName,ds.documentExtension AS iconDocumentExtension, 14 AS iconDocumentType
	                    FROM ddlcatlist AS ins
	                    INNER JOIN documentstore AS ds ON ins.id = ds.districtId
	                    INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
                        WHERE dpt.documentImageGroup = 4 AND dpt.documentType = 14 AND ins.category=@hospitalSpecialization 
                        ORDER BY ins.nameEnglish ";

                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Specialization";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ins.doctorRegNo,ins.doctorNameEnglish,ins.doctorNameLocal,ins.address AS doctorAddress,ins.mobileNo,
                                    ins.emailId,ins.cityId,ins.cityName AS DoctorCityname,dwa.price,dwa.consultancyTypeName,
		                            dwa.address1 AS workAreaAddress,dwa.phoneNumber AS workAreaPhoneNumber,
		                            dwa.hospitalRegNo,dwa.hospitalNameEnglish,dwa.hospitalAddress,
		                            dwaImg.documentId AS clinicDocumentId,dwaImg.documentName AS clinicDocumentName,dwaImg.documentExtension AS clinicDocumentExtension,
                                    IFNULL(ddlcatGender.nameEnglish, '') AS doctorGender, IFNULL(dp.genderId, 0 ) AS genderId, @DoctorWorkArea AS clinicDocumentType
                                FROM doctorregistration AS ins 
                                INNER JOIN district AS dist ON dist.districtId=ins.districtId 
                                INNER JOIN doctorworkarea AS dwa ON dwa.doctorRegNo = ins.doctorRegNo 
                                LEFT JOIN doctorprofile AS dp ON dp.doctorRegNo = ins.doctorRegNo 
                                LEFT JOIN ddlcatlist AS ddlcatGender ON dp.genderId = ddlcatGender.id AND ddlcatGender.category=@gender
                                LEFT JOIN ( 
                                       SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId
													     FROM documentstore AS ds
													     WHERE ds.documentName IN 
                                                        ( SELECT ds.documentName
                                                            FROM documentstore AS ds 
					                                            INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
													     WHERE dpt.documentImageGroup=@Doctor AND dpt.documentType = @DoctorWorkArea	
													     GROUP BY ds.documentId 
                                                        )
													 )  AS dwaImg ON dwaImg.documentId = dwa.hospitalRegNo
                            WHERE ins.registrationStatus = @active AND ins.isVerified=@isVerified AND ins.active=@active " + where + whereSearchClin
                            + " ORDER BY ins.doctorNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dtt.table.Rows.Count == 0)
                {
                    query = @" SELECT 'Clinics data for this Area will be added soon.' AS noData";
                    dtt = await db.ExecuteSelectQueryAsync(query, pm);
                    dtt.status = false;
                    dtt.message = " Clinics data for this Area will be added soon.";
                }
                dtt.table.TableName = "Clinics";
                dataSet.dataset.Tables.Add(dtt.table);

                if (appParam.isMobileView == 0) //desktop
                    query = @" SELECT ds.documentId AS websiteBannerId,ds.documentName AS websiteBannerName,ds.documentExtension AS websiteBannerExtension,
                                 ds.districtId,dist.districtNameEnglish, dpt.documentType, dpt.documentImageGroup
                             FROM documentstore AS ds
                              INNER JOIN district AS dist ON dist.districtId = ds.districtId
                             INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
                             WHERE dpt.documentImageGroup=@Website AND dpt.documentType = @WebsiteBanner " + where;
                else //mobile
                    query = @" SELECT ds.documentId AS websiteMobileBannerId,ds.documentName AS websiteMobileBannerName,ds.documentExtension AS websiteMobileBannerExtension,
                                 ds.districtId,dist.districtNameEnglish, dpt.documentType, dpt.documentImageGroup
                             FROM documentstore AS ds
                              INNER JOIN district AS dist ON dist.districtId = ds.districtId
                             INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
                             WHERE dpt.documentImageGroup=@Website AND dpt.documentType = @WebsiteBanner " + where;

                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dtt.table.Rows.Count == 0)
                {
                    query = @" SELECT 'Banners data for this Area will be added soon.' AS noData";
                    dtt = await db.ExecuteSelectQueryAsync(query, pm);
                    dtt.status = false;
                    dtt.message = " Banners data for this Area will be added soon.";
                }
                dtt.table.TableName = "Banners";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT dist.districtId,dist.districtNameEnglish,dist.latitude, dist.longitude
                                FROM district AS dist
                            ORDER BY dist.districtNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "District";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @"  SELECT COUNT(ins.hospitalRegNo) AS totalRegisteredHospital
 	                           FROM hospitalregistration AS ins 
                              INNER JOIN district AS dist ON dist.districtId = ins.districtId 
                            WHERE 1 = 1 " + where;
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "RegisteredHospital";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @"  SELECT COUNT(ins.doctorRegNo) AS totalRegisteredDoctor
 	                           FROM doctorregistration AS ins 
                              INNER JOIN district AS dist ON dist.districtId = ins.districtId 
                            WHERE 1 = 1 " + where;
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "RegisteredDoctor";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT te.testimonialId, te.firstName, te.fullName, te.contentEnglish From testimonials AS te ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Testimonials";
                dataSet.dataset.Tables.Add(dtt.table);



            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dataSet;
        }

        public async Task<List<ListValue>> HomeSearchSub(HomeSearch appParam, LanguageSupported language)
        {
            string fieldLanguage = language == LanguageSupported.Hindi ? "Local" : "English";
            List<ListValue> lv = new();
            string query = "";
            if (appParam.searchSubCategoryTypeId == 3) // Doctor
            {
                query = @" SELECT ddlcat.id, ddlcat.nameEnglish AS name
	                            FROM ddlcatlist AS ddlcat 
	                        WHERE ddlcat.category = @hospitalSpecialization";
            }
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("hospitalSpecialization", MySqlDbType.String) { Value= "hospitalSpecialization" }
            };
            if (query != string.Empty)
            {
                dt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dt.table.Rows.Count > 0)
                    lv = Helper.GetGenericDropdownList(dt.table);
            }
            return lv;
        }

        /// <summary>
        ///get User Email-Id
        /// </summary>
        /// <returns></returns>
        public async Task<UserResponse> GetUserByEmail(string emailid)
        {
            UserResponse user = new UserResponse();
            user.emailId = emailid;
            user.message = "Invalid Email Id";
            YesNo changePassword;
            YesNo isDisabled;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                {
                    new MySqlParameter("emailid",MySqlDbType.String) { Value = emailid},
                    new MySqlParameter("active",MySqlDbType.Int16) { Value = (int)Active.Yes}
                };
                string query = @" SELECT l.userName, l.userId, l.password, l.changePassword, l.isDisabled, l.userRole, l.isSingleWindowUser
                                  FROM userlogin l
                                  WHERE l.emailId=@emailId AND l.active = @active ";
                dt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dt.table.Rows.Count > 0)
                {
                    DataRow dr = dt.table.Rows[0];
                    Int64 userId = Convert.ToInt64(dr["userId"]);
                    user.userName = dr["userName"].ToString();
                    user.role = Convert.ToInt16(dr["userRole"].ToString());

                    Enum.TryParse(dr["isDisabled"].ToString(), true, out isDisabled);
                    if (isDisabled == YesNo.Yes)
                        user.message = "Account has been disabled";
                    else
                    {
                        user.isAuthenticated = true;
                        user.message = "Login successfull";
                    }
                    pm = new MySqlParameter[]
                      {
                            new MySqlParameter("userId",MySqlDbType.Int64) { Value = userId},
                            new MySqlParameter("isVerified",MySqlDbType.Int16) { Value = (int)Active.Yes}
                      };
                    if (user.role == (Int16)UserRole.Hospital)
                    {
                        query = @" SELECT hr.mobileNo,hr.emailId
                                        FROM hospitalregistration hr
                                        WHERE hr.hospitalRegNo=@userId AND hr.active=@isVerified 
                                    AND isVerified=@isVerified; ";
                        dt = await db.ExecuteSelectQueryAsync(query, pm);
                        if (dt.table.Rows.Count > 0)
                        {
                            dr = dt.table.Rows[0];
                            user.mobileNo = dr["mobileNo"].ToString();
                        }
                    }
                    else if (user.role == (Int16)UserRole.Doctor)
                    {
                        query = @" SELECT dr.mobileNo,dr.emailId 
                                FROM doctorregistration dr 
                            WHERE dr.doctorRegNo=@userId AND dr.active=@isVerified AND dr.isVerified=@isVerified; ";
                        dt = await db.ExecuteSelectQueryAsync(query, pm);
                        if (dt.table.Rows.Count > 0)
                        {
                            dr = dt.table.Rows[0];
                            user.mobileNo = dr["mobileNo"].ToString();
                        }
                    }
                    else if (user.role == (Int16)UserRole.Patient)
                    {
                        user.mobileNo = "0";
                        //query = @" SELECT dr.mobileNo,dr.emailId 
                        //        FROM doctorregistration dr 
                        //    WHERE dr.doctorRegNo=@userId AND dr.active=@isVerified AND dr.isVerified=@isVerified; ";
                        //dt = await db.ExecuteSelectQueryAsync(query, pm);
                        //if (dt.table.Rows.Count > 0)
                        //{
                        //    dr = dt.table.Rows[0];
                        //    user.mobileNo = dr["mobileNo"].ToString();
                        //}
                    }
                    else
                    {
                        user = null;
                        user.isAuthenticated = false;
                        user.message = "Login Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error("DlCommon(GetUserByEmail) : ", ex);
            }
            return user;
        }

        /// <summary>
        ///get User Mobile Nomber Exists
        /// </summary>
        /// <returns></returns>
        public async Task<UserResponse> GetUserByMobile(string mobileNumber)
        {
            UserResponse user = new UserResponse();
            user.message = "Invalid Mobile Number";
            YesNo changePassword;
            YesNo isDisabled;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                {
                    new MySqlParameter("mobileNo",MySqlDbType.String) { Value = mobileNumber},
                    new MySqlParameter("isVerified",MySqlDbType.Int16) { Value = (int)Active.Yes}
                };
                string query = @" SELECT pr.patientRegNo as userId ,pr.emailId,
                                    " + (Int16)UserRole.Patient + @" AS userRole
                                    FROM patientregistration pr
                                    WHERE pr.mobileNo=@mobileNo AND pr.active=@isVerified
                                     UNION ALL
                                        SELECT dr.doctorRegNo as userId,dr.emailId,
                                    " + (Int16)UserRole.Doctor + @" AS userRole 
                                FROM doctorregistration dr 
                            WHERE dr.mobileNo=@mobileNo 
                                AND dr.active=@isVerified AND dr.isVerified=@isVerified 
                            UNION ALL
                                        SELECT hr.hospitalRegNo as userId,hr.emailId,
                                    " + (Int16)UserRole.Hospital + @" AS userRole 
                                        FROM hospitalregistration hr
                                        WHERE hr.mobileNo=@mobileNo AND hr.active=@isVerified 
                                    AND isVerified=@isVerified; ";
                dt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dt.table.Rows.Count > 0)
                {
                    DataRow dr = dt.table.Rows[0];
                    //Int64 userId = Convert.ToInt64(dr["userId"]);
                    user.emailId = dr["emailId"].ToString()!;
                    user.role = Convert.ToInt16(dr["userRole"].ToString());
                    user.mobileNo = mobileNumber;
                    if (user.role != (Int16)UserRole.Patient)
                        user = await GetUserByEmail(user.emailId!);
                    else
                        user.isAuthenticated = true;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error("DlCommon(GetUserByMobile) : ", ex);
            }
            return user;
        }

        #region SMS Service
        /// <summary>
        /// Save Sent SMS Details
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public async Task<ReturnBool> SendSmsSaveAsync(AlertMessageBody sb)
        {
            sb.messageServerResponse = sb.messageServerResponse == null ? "" : sb.messageServerResponse;
            sb.mobileNo = Convert.ToInt64(sb.mobileNo.ToString().Substring(sb.mobileNo.ToString().Length - 10));
            string query = @"INSERT INTO smssentdetail
                                          (msgId,msgServerResponse,mobileNo,emailId,msgCategory,msgBody,msgOtp,
                                           isOtpVerified,registrationId,actionId,clientIp)
                             VALUES(@msgId,@msgServerResponse,@mobileNo,@emailId,@msgCategory,@msgBody,@msgOtp,
                                           @isOtpVerified,@registrationId,@actionId,@clientIp)";
            //sb.msgId = Guid.NewGuid().ToString();
            sb.msgId = await GenerateSMSMsgId();

            List<MySqlParameter> pm = new()
            {
                new MySqlParameter("msgId", MySqlDbType.String) { Value = sb.msgId },
                new MySqlParameter("msgServerResponse", MySqlDbType.String) { Value = sb.messageServerResponse },
                new MySqlParameter("mobileNo", MySqlDbType.Int64) { Value = sb.mobileNo },
                new MySqlParameter("emailId", MySqlDbType.VarChar) { Value = sb.emailToReceiver },
                new MySqlParameter("msgCategory", MySqlDbType.Int16) { Value = sb.msgCategory },
                new MySqlParameter("msgBody", MySqlDbType.String) { Value = sb.smsBody },
                new MySqlParameter("msgOtp", MySqlDbType.VarChar) { Value = sb.OTP.ToString() },
                new MySqlParameter("isOtpVerified", MySqlDbType.Int32) { Value = (int)YesNo.No },
                new MySqlParameter("registrationId", MySqlDbType.Int64) { Value = sb.applicationId },
                new MySqlParameter("actionId", MySqlDbType.Int16) { Value = sb.actionId },
                new MySqlParameter("clientIp", MySqlDbType.String) { Value = sb.clientIp }
            };
            ReturnBool rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "SaveSentSMS");
            if (rb.status)
                rb.message = sb.msgId;
            return rb;
        }
        /// <summary>
        /// Resend OTP 
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnString> GetOTPMsgForResend(string msgId)
        {
            ReturnString rs = new();
            BlCommon bl = new();
            string query = @" SELECT s.mobileNo,s.msgBody,
                                    TIMESTAMPDIFF(MINUTE, s.sendingDatetime, CURRENT_TIMESTAMP()) AS SMSSentTime,
                                    TIMESTAMPDIFF(SECOND, s.sendingDatetime, CURRENT_TIMESTAMP()) AS SMSSentTimeInSecond
                            FROM smssentdetail s 
                            WHERE s.msgId=@msgId AND s.isOtpVerified=@isOtpVerified";
            List<MySqlParameter> pm = new List<MySqlParameter>
            {
                new MySqlParameter("msgId", MySqlDbType.String) { Value = msgId },
                new MySqlParameter("isOtpVerified", MySqlDbType.Int16) { Value = (int)YesNo.No }
            };
            ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                long validity = Convert.ToInt64(dt.table.Rows[0]["SMSSentTime"].ToString());
                if (validity > bl.smsvalidity)
                {
                    rs.status = false;
                    rs.message = "OTP expired!!!";
                }
                else
                {
                    rs.message = dt.table.Rows[0]["msgBody"].ToString();
                    rs.value = dt.table.Rows[0]["mobileNo"].ToString();
                    rs.status = true;
                }
            }
            return rs;
        }

        public async Task<string> GenerateSMSMsgId()
        {

        ReExecute:
            string msgId = Guid.NewGuid().ToString();
            bool isExist = await VerifySMSMsgId(msgId);
            if (isExist)
                goto ReExecute;
            else
                return msgId;
        }

        /// <summary>
        /// check ssms MsgId
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        private async Task<bool> VerifySMSMsgId(string msgId)
        {
            string query = @"SELECT d.msgId
                             FROM smssentdetail AS d
                             WHERE d.msgId = @msgId;";
            MySqlParameter[] pm = new[]
            {
                new MySqlParameter("msgId", MySqlDbType.VarChar,100){  Value= msgId}
            };
            ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm);
            if (dataTable.table.Rows.Count > 0)
                return true;
            else
                return false;
        }
        #endregion

        /// <summary>
        /// Send OTP 
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnString> SendOTP(SendOtp bl)
        {
            ReturnClass.ReturnString rs = new();
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();

            bl.mobileNo = Convert.ToInt64(bl.mobileNo.ToString().Substring(bl.mobileNo.ToString().Length - 10));
            string mobileno = bl.mobileNo.ToString();

            Match match = Regex.Match(mobileno,
                              @"^[6-9]\d{9}$", RegexOptions.IgnoreCase);
            if (match.Success == false)
            {
                rs.status = false;
                rs.message = "Invalid Mobile Number";
                return rs;
            }
            dt = await CheckSMSSendDuration(mobileno, (Int16)SMSSendType.Send);
            if (dt.status)
            {
                rs.status = false;
                rs.value = dt.value;
                rs.message = dt.message;
                if (rs.value.ToString().Trim() != ((Int16)OTPStatus.Expired).ToString().Trim())
                {
                    rs.status = true;
                    rs.msg_id = dt.type;
                    rs.any_id = mobileno.ToString();
                }
                return rs;
            }
            DlCommon dlCommon = new();

            Utilities util = new Utilities();
            Int64 smsotp = util.GenRendomNumber(4);
            rs.any_id = "Your Mobile OTP is " + smsotp.ToString();
            string smsServiceActive = util.GetAppSettings("sandeshSmsConfig", "isActive").message;
            string normalSMSServiceActive = util.GetAppSettings("SmsConfiguration", "isActive").message;
            string EmailServiceActive = util.GetAppSettings("EmailConfiguration", "isActive").message;
            // Int32 SMSVerificationLimit = Convert.ToInt32(Utilities.GetAppSettings("SmsConfiguration", "SMSVerificationLimit").message) / 60;
            AlertMessageBody smsbody = new();
            SandeshResponse rbs = new();
            ReturnDataTable dtsmstemplate = await GetSMSEmailTemplate((Int32)SmsEmailTemplate.OTPSWS);
            sandeshMessageBody sandeshMessageBody = new();
            string smsTemplate = dtsmstemplate.table.Rows[0]["msgBody"].ToString()!;
            sandeshMessageBody.templateId = Convert.ToInt64(dtsmstemplate.table.Rows[0]["templateId"].ToString()!);
            if (sandeshMessageBody.templateId > 0)
            {
                #region create Parameter To send SMS
                object[] values = new object[] { smsotp.ToString() };
                sandeshMessageBody.message = GetFormattedMsg(smsTemplate, values);

                sandeshMessageBody.contact = mobileno;
                sandeshMessageBody.msgCategory = (Int16)SandeshmsgCategory.Info;
                sandeshMessageBody.msgPriority = (Int16)SandeshmsgPriority.HighVolatile;
                smsbody.smsBody = sandeshMessageBody.message;
                sandeshMessageBody.clientIp = bl.clientIp;
                sandeshMessageBody.isOTP = true;
                //rs.any_id = "0";
                //SandeshSms sms = new SandeshSms();
                #endregion
                try
                {
                    rbs.status = "success";
                    /*
                    #region Send sansesh SMS
                    if (smsServiceActive.ToUpper() == "TRUE")
                        rbs = await sms.callSandeshAPI(sandeshMessageBody);
                    #endregion

                    #region Send Normal SMS
                    if (normalSMSServiceActive.ToUpper() == "TRUE")
                        rbs = await sms.CallSMSAPI(sandeshMessageBody);
                    #endregion

                    #region Email OTP 
                    //New code To Send Email From 31.103
                    if (bl.emailId != string.Empty && EmailServiceActive.ToUpper() == "TRUE")
                    {
                        Email em = new();
                        emailSenderClass emailSenderClass = new();
                        emailSenderClass.emailSubject = "OTP Verification for SWS Chhattisgarh"!;
                        emailSenderClass.emailBody = sandeshMessageBody.message!;
                        emailSenderClass.emailToId = bl.emailId!;
                        emailSenderClass.emailToName = "";
                        await em.SendEmailViaURLAsync(emailSenderClass);
                    }
                    #endregion
                    */

                }
                catch (Exception ex)
                { }
            }

            #region Save OTP Details in DB
            smsbody.OTP = smsotp;
            smsbody.smsTemplateId = 0;
            smsbody.isOtpMsg = true;
            smsbody.applicationId = bl.id == null ? 0 : bl.id;
            smsbody.mobileNo = bl.mobileNo;
            smsbody.msgCategory = (Int16)MessageCategory.OTP;
            smsbody.clientIp = bl.clientIp;
            smsbody.smsLanguage = LanguageSupported.English;
            smsbody.emailToReceiver = bl.emailId;
            smsbody.emailSubject = "OTP Verification";
            smsbody.messageServerResponse = rbs.status;
            smsbody.actionId = 1;
            rb = await dlCommon.SendSmsSaveAsync(smsbody);
            if (rb.status)
            {
                rs.status = true;
                rs.msg_id = rb.message;
            }
            #endregion


            return rs;
        }
        public static string GetFormattedMsg(string smsText, params object[] values)
        {
            return string.Format(smsText, values);
        }
        public async Task<ReturnDataTable> GetSMSEmailTemplate(Int32 smsTemplateId)
        {

            string query = @"SELECT se.templateId,se.categoryName,se.isOTP,se.msgBody,se.emailBody,se.noofSMSParam,se.noofEmailParam,
                                        se.smsParamDescription,se.emailParamDescription,se.emailSubject
                                    FROM smsemailtemplate se
                                    WHERE se.id=@id AND se.isActive=@isActive ";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("id", MySqlDbType.Int32){ Value= smsTemplateId},
                new MySqlParameter("isActive", MySqlDbType.Int16){ Value= YesNo.Yes},
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            if (dt.table.Rows.Count == 0)
                dt.message = "No Templates Available";
            return dt;
        }
        /// <summary>
        /// 
        /// Retrive Last OTP by Mobile num only
        /// </summary>
        /// <returns>Verify OTP</returns>
        private async Task<ReturnDataTable> CheckSMSSendDuration(string Mobile, Int16 smsSendType)
        {

            string query = "";
            Utilities utilities = new Utilities();
            string durationType = (Int16)SMSSendType.Send == smsSendType ? "SMSVerificationLimit" : "SMSResendDurationInSecond";
            Int32 SMSResendDurationInSecond = Convert.ToInt32(utilities.GetAppSettings("SmsConfiguration", durationType).message);
            Int32 repeatCounter = Convert.ToInt32(utilities.GetAppSettings("SmsConfiguration", "ResendLimit").message);
            MySqlParameter[] pm = new MySqlParameter[]
           {
                new MySqlParameter("mobileNo", MySqlDbType.String) { Value = Mobile},
                new MySqlParameter("msgCategory", MySqlDbType.Int16) { Value = (Int16)MessageCategory.OTP},


           };
            Mobile = Mobile.ToString().Substring(Mobile.ToString().Length - 10);
            string mobileno = Mobile.ToString();

            Match match = Regex.Match(mobileno,
                         @"^[6-9]\d{9}$", RegexOptions.IgnoreCase);

            if (match.Success == false)
            {
                dt.status = false;
                dt.message = "Invalid Mobile Number";
                dt.value = "";
                return dt;
            }
            query = @"SELECT e.msgId,e.isOtpVerified,
                        TIMESTAMPDIFF(SECOND, e.sendingDatetime, CURRENT_TIMESTAMP()) AS SMSSentTimeInSecond,
                            e.OTPAttemptLimit,e.msgOtp,e.repeatCounter
                          FROM smssentdetail e
                          WHERE   e.mobileNo = @mobileNo 
            AND e.msgCategory=@msgCategory ORDER BY e.sendingDatetime DESC LIMIT 1 ";
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            if (dt.table.Rows.Count > 0)
            {
                dt.status = false;
                dt.value = dt.table.Rows[0]["isOtpVerified"].ToString();
                if (Convert.ToInt16(dt.table.Rows[0]["isOtpVerified"].ToString()) == (Int16)OTPStatus.Pending
                        && Convert.ToInt32(dt.table.Rows[0]["SMSSentTimeInSecond"].ToString()) < SMSResendDurationInSecond)
                {
                    dt.status = true;
                    durationType = (Int16)SMSSendType.Send == smsSendType ? (((SMSResendDurationInSecond - Convert.ToInt32(dt.table.Rows[0]["SMSSentTimeInSecond"].ToString())) / 60) + 1).ToString() + @" minutes." : SMSResendDurationInSecond.ToString() + @" second.";
                    //dt.message = "SMS will be send after " + durationType;
                    dt.message = "Please try again, After the validity of SMS expires.";
                    dt.type = dt.table.Rows[0]["msgId"].ToString();
                    dt.table.Rows.Clear();

                }
            }
            else
                dt.status = false;


            return dt;
        }
        #region Request Token
        /// <summary>
        /// Method for creating random number for post request
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnString> GenerateRequestToken()
        {
            ReturnString returnString = new();
            Guid guid = Guid.NewGuid();
            string query = @"INSERT INTO requesttoken(guid)
                             VALUES (@guid)";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("guid", MySqlDbType.VarChar) { Value = guid.ToString() },
            };
            ReturnClass.ReturnBool rb = await db.ExecuteQueryAsync(query, pm, "GenerateRequestToken");
            if (rb.status)
            {
                returnString.status = true;
                returnString.msg_id = guid.ToString();
            }
            return returnString;
        }

        public async Task<ReturnClass.ReturnBool> VerifyRequestToken(string requestToken)
        {
            ReturnBool rb = new();
            Utilities utilities = new();
            ReturnBool rbValidityPeriod = utilities.GetAppSettings("AppSettings", "TokenValidityInMinutes");
            int validityPeriod = 10;
            if (rbValidityPeriod.status)
                validityPeriod = Convert.ToInt32(rbValidityPeriod.message);

            string query = @"SELECT p.tableKey, p.isUtilized, TIMESTAMPDIFF(MINUTE, p.entryDateTime, NOW()) as tokenValidity
                             FROM requesttoken p
                             WHERE p.guid = @guid ";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("guid", MySqlDbType.VarChar) { Value = requestToken},
            };
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm);
            if (dt.table.Rows.Count > 0)
            {
                if (dt.table.Rows[0]["isUtilized"].ToString() == "0")
                {
                    int tokenValidity = Convert.ToInt32(dt.table.Rows[0]["tokenValidity"].ToString());
                    if (tokenValidity <= validityPeriod)
                    {
                        rb.status = true;
                        await ExpireRequestToken(requestToken);
                    }
                    else
                        rb.message = "Token Expired";
                }
                else
                    rb.message = "Token Expired";
            }
            else
                rb.message = "Invalid Token";
            return rb;
        }

        private async Task<ReturnClass.ReturnBool> ExpireRequestToken(string requestToken)
        {
            string query = @" UPDATE requesttoken p
                              SET p.isUtilized = @isUtilized
                              WHERE p.guid = @guid ";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("isUtilized", MySqlDbType.Int16) { Value = (int)YesNo.Yes },
                new MySqlParameter("requestToken", MySqlDbType.VarChar) { Value = requestToken },
            };
            return await db.ExecuteQueryAsync(query, pm, "ExpireRequestToken");
        }
        #endregion

        public async Task<User> GetUserByOTP(string emailid, string mobileNo, string OTP, string msgId)
        {
            User user = new User();
            user.message = "Invalid OTP Details";
            ReturnBool rb = await VerifyPublicOTP(msgId, Convert.ToInt32(OTP), mobileNo);
            if (!rb.status)
            {
                user.message = rb.message;
                user.userName = rb.value;
                return user;
            }
            YesNo changePassword;
            YesNo isDisabled;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                {
                    new MySqlParameter("emailid",MySqlDbType.String) { Value = emailid},
                    new MySqlParameter("active",MySqlDbType.Int16) { Value = (int)Active.Yes}
                };
                string query = @" SELECT l.userName, l.userId, l.changePassword, l.isDisabled, l.userRole, l.isSingleWindowUser
                                  FROM userlogin l
                                  WHERE l.emailId=@emailId AND l.active = @active ";

                dt = await db.ExecuteSelectQueryAsync(query, pm);
                if (dt.table.Rows.Count > 0)
                {
                    DataRow dr = dt.table.Rows[0];
                    user.userId = Convert.ToInt64(dr["userId"]);
                    user.userName = dr["userName"].ToString();
                    user.role = Convert.ToInt16(dr["userRole"].ToString());
                    Enum.TryParse(dr["changePassword"].ToString(), true, out changePassword);
                    user.forceChangePassword = changePassword;
                    Enum.TryParse(dr["isDisabled"].ToString(), true, out isDisabled);
                    if (isDisabled == YesNo.Yes)
                        user.message = "Account has been disabled";
                    else
                    {
                        user.isAuthenticated = true;
                        user.message = "Login successfull";
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error("DlCommon(GetUserByOTP) : ", ex);
            }
            return user;
        }

        /// <summary>
        /// 
        /// Verify Public Mobile OTP
        /// </summary>
        /// <returns>Verify OTP</returns>
        public async Task<ReturnClass.ReturnBool> VerifyPublicOTP(string msgId, Int32 OTP, string Mobile)
        {
            ReturnClass.ReturnBool rb = new();
            string query = "";
            Utilities utilities = new();
            Int32 repeatCounter = Convert.ToInt32(utilities.GetAppSettings("SmsConfiguration", "ResendLimit").message);
            MySqlParameter[] pm = new MySqlParameter[]
           {
                new MySqlParameter("msgId", MySqlDbType.String) { Value = msgId},
                new MySqlParameter("mobileNo", MySqlDbType.String) { Value = Mobile},
                new MySqlParameter("msgOtp", MySqlDbType.Int32) { Value = OTP},
                new MySqlParameter("isOtpVerified", MySqlDbType.Int16) { Value = (Int16)OTPStatus.Verified},
                new MySqlParameter("notVerified", MySqlDbType.Int16) { Value = (Int16)OTPStatus.Pending},
           };
            Mobile = Mobile.ToString().Substring(Mobile.ToString().Length - 10);
            string mobileno = Mobile.ToString();

            Match match = Regex.Match(mobileno.ToString(),
                         @"^[6-9]\d{9}$", RegexOptions.IgnoreCase);
            if (match.Success == false)
            {
                rb.status = false;
                rb.message = "Invalid Mobile Number";
                return rb;
            }
            rb = await VerifyOTP(msgId, (Int16)ContactVerifiedType.Mobile, OTP, Mobile);
            if (rb.status)
            {
                query = @"UPDATE smssentdetail
                        SET isOtpVerified=@isOtpVerified,otpVerificationDate=NOW()
                             WHERE msgId = @msgId AND  mobileNo = @mobileNo  
                            AND msgOtp=@msgOtp AND isOtpVerified= @notVerified;";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "VerifyOTP");
            }
            return rb;
        }
        /// <summary>
        /// Verify Email OTP
        /// isEmailOrMobileOTP=1:MobileOTP, IF isEmailOrMobileOTP=2 : Email OTP
        /// </summary>
        /// <returns>Industrial User Registration Id</returns>
        public async Task<ReturnClass.ReturnBool> VerifyOTP(string msgId, Int16 contactVerifiedType, Int32 OTP, string MobileOrEmailId)
        {
            ReturnClass.ReturnBool rb = new();
            string query = "", query1 = "";
            Utilities utilities = new();
            MySqlParameter[] pm = new MySqlParameter[]
           {
                new MySqlParameter("msgId", MySqlDbType.String) { Value = msgId},
                new MySqlParameter("mobileNo", MySqlDbType.String) { Value = MobileOrEmailId},
                new MySqlParameter("msgOtp", MySqlDbType.Int32) { Value = OTP},
                new MySqlParameter("emailId", MySqlDbType.String) { Value = MobileOrEmailId},
                 new MySqlParameter("OtpExpire", MySqlDbType.Int16) { Value = (Int16)OTPStatus.Expired},
           };
            if (contactVerifiedType == (Int16)ContactVerifiedType.Mobile)
            {

                MobileOrEmailId = MobileOrEmailId.ToString().Substring(MobileOrEmailId.ToString().Length - 10);
                string mobileno = MobileOrEmailId.ToString();

                Match match = Regex.Match(mobileno,
                             @"^[6-9]\d{9}$", RegexOptions.IgnoreCase);
                if (match.Success == false)
                {
                    rb.status = false;
                    rb.message = "Invalid Mobile Number";
                    return rb;
                }
                query = @"SELECT e.msgId,e.isOtpVerified,TIMESTAMPDIFF(SECOND, e.sendingDatetime, CURRENT_TIMESTAMP()) AS SMSSentTimeInSecond,
                            e.OTPAttemptLimit,e.msgOtp, e.emailId
                          FROM smssentdetail e
                          WHERE e.msgId = @msgId AND  e.mobileNo = @mobileNo";
                query1 = @"UPDATE smssentdetail SET OTPAttemptLimit= OTPAttemptLimit + 1 ";
            }
            else if (contactVerifiedType == (Int16)ContactVerifiedType.Email)
            {
                Match match = Regex.Match(MobileOrEmailId.ToString(),
                               @"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", RegexOptions.IgnoreCase);
                if (match.Success == false)
                {
                    rb.status = false;
                    rb.message = "Given email id is not valid.";
                    return rb;
                }
                query = @"SELECT e.msgId,e.isOtpVerified,TIMESTAMPDIFF(SECOND, e.sendingDatetime, CURRENT_TIMESTAMP()) AS SMSSentTimeInSecond,
                                 e.OTPAttemptLimit,e.msgOtp
                          FROM emailsentdetail e
                          WHERE e.msgId = @msgId AND  e.emailId = @emailId ";
                query1 = @"UPDATE emailsentdetail SET OTPAttemptLimit= OTPAttemptLimit + 1 ";
            }

            dt = await db.ExecuteSelectQueryAsync(query, pm);
            Int32 OTPAttemptLimit = Convert.ToInt32(utilities.GetAppSettings("SmsConfiguration", "OTPAttemptLimit").message);
            if (dt.table.Rows.Count > 0)
            {
                rb.value = dt.table.Rows[0]["isOtpVerified"].ToString();
                if (Convert.ToInt16(dt.table.Rows[0]["isOtpVerified"].ToString()) == (Int16)OTPStatus.Expired)
                {
                    rb.status = false;
                    rb.message = "OTP Expired.";
                    rb.value = dt.table.Rows[0]["isOtpVerified"].ToString();
                    return rb;
                }
                if (Convert.ToInt32(dt.table.Rows[0]["msgOtp"].ToString()) == OTP)
                {
                    rb.value = dt.table.Rows[0]["msgId"].ToString();
                    Int32 smsVerificationLimit = Convert.ToInt32(utilities.GetAppSettings("SmsConfiguration", "SMSVerificationLimit").message);

                    if (Convert.ToInt32(dt.table.Rows[0]["SMSSentTimeInSecond"].ToString()) > smsVerificationLimit)//&& contactVerifiedType == (Int16)ContactVerifiedType.Mobile
                    {
                        query1 += @" , isOtpVerified=@OtpExpire  WHERE msgId = @msgId AND  mobileNo = @mobileNo ";
                        await db.ExecuteQueryAsync(query1, pm.ToArray(), "UPADATEOTPExpired");
                        rb.status = false;
                        rb.message = "OTP Expired.";
                        rb.value = dt.table.Rows[0]["isOtpVerified"].ToString();
                        return rb;
                    }

                    if (Convert.ToInt16(dt.table.Rows[0]["isOtpVerified"].ToString()) == (Int16)YesNo.Yes)
                    {
                        rb.status = false;
                        rb.message = "Invalid OTP Details.";
                        rb.value = "0";
                        return rb;
                    }
                    if (Convert.ToInt16(dt.table.Rows[0]["isOtpVerified"].ToString()) == (Int16)YesNo.No)
                    {
                        rb.status = true;
                        rb.value = dt.table.Rows[0]["emailId"].ToString();
                    }

                }
                else
                {
                    if ((Convert.ToInt16(dt.table.Rows[0]["OTPAttemptLimit"].ToString()) + 1) >= OTPAttemptLimit)
                        query1 += @" , isOtpVerified=@OtpExpire  WHERE msgId = @msgId AND  mobileNo = @mobileNo ";
                    else
                        query1 += @" WHERE msgId = @msgId AND  mobileNo = @mobileNo ";


                    rb = await db.ExecuteQueryAsync(query1, pm.ToArray(), "UPADATEOTPAttemptLimit");
                    if (rb.status)
                        rb.message = @"Invalid OTP, You have tried " + (Convert.ToInt16(dt.table.Rows[0]["OTPAttemptLimit"].ToString()) + 1).ToString()
                                    + @" attempts out of " + OTPAttemptLimit.ToString() + @".";
                    rb.status = false;
                    rb.value = dt.table.Rows[0]["isOtpVerified"].ToString();
                }
            }
            else
                rb.message = "Invalid Details.";

            return rb;
        }
    }


}
