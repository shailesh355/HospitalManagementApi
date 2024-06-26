﻿using BaseClass;
using DmfPortalApi.Models.AppClass;
using HospitalManagementApi.Models.Balayer;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.Json;
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
            string whereSerachHosp = string.Empty;
            string whereSerachDoc = string.Empty;
            string whereSerachSpec = string.Empty;
            string whereSerachClin = string.Empty;
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
                    new MySqlParameter("WebsiteBanner", MySqlDbType.Int16) { Value = DocumentType.WebsiteBanner},
                    new MySqlParameter("Hospital", MySqlDbType.Int16) { Value = DocumentImageGroup.Hospital },
                    new MySqlParameter("HospitalImage", MySqlDbType.Int16) { Value = DocumentType.HospitalImages},
                    new MySqlParameter("Doctor", MySqlDbType.Int16) { Value = DocumentImageGroup.Doctor },
                    new MySqlParameter("DoctorProfile", MySqlDbType.Int16) { Value = DocumentType.DoctorProfilePic},
                    new MySqlParameter("DoctorWorkArea", MySqlDbType.Int16) { Value = DocumentType.DoctorWorkArea},
                    new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = YesNo.Yes },
                    new MySqlParameter("hospitalSpecialization", MySqlDbType.VarChar) { Value = "hospitalSpecialization" },
                    new MySqlParameter("homeSearch", MySqlDbType.VarChar) { Value = "HomeSearch" },
                    
                   };
                if (appParam.districtId != 0)
                    where += " AND ins.districtId = @districtId ";
                if (appParam.lat != 0 && appParam.longi != 0)
                {
                    where += " AND dist.latitude BETWEEN @lat - 0.30 AND @lat + 0.30 " +
                             " AND dist.longitude BETWEEN @long - 0.30 AND @long + 0.30 ";
                }
                if (appParam.searchedText != null)
                {
                    whereSerachHosp += " AND ins.hospitalNameEnglish LIKE @hospitalSpec% ";
                    whereSerachDoc += " AND ins.doctorNameEnglish LIKE @hospitalSpec% ";
                    whereSerachSpec += " AND ins.nameenglish LIKE @hospitalSpec% ";
                    whereSerachClin += " AND ins.doctorNameEnglish LIKE @hospitalSpec% ";
                }

                query = @" SELECT ins.id,ins.nameEnglish AS searchCategory
	                            FROM ddlcatlist AS ins WHERE ins.category=@homeSearch"
                            + " ORDER BY ins.nameEnglish ";
                ReturnClass.ReturnDataTable dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "SearchTabs";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ins.hospitalRegNo,ins.hospitalNameEnglish,ins.districtId,ins.address,ins.mobileNo,ins.emailId,ins.registrationYear,
		                                ins.cityId,ins.pinCode,ins.phoneNumber,ins.landMark,ins.fax,ins.isCovid,ins.latitude,ins.longitude,ins.typeOfProviderId,ins.website,ins.natureOfEntityId,
                                        hosImg.documentId,hosImg.documentName,hosImg.documentExtension		                                
	                                FROM hospitalregistration AS ins 
                                        INNER JOIN district AS dist ON dist.districtId = ins.districtId
                                        LEFT JOIN ( 
                                       	SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId
													     FROM documentstore AS ds
													     INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
													     WHERE dpt.documentImageGroup=@Hospital AND dpt.documentType = @HospitalImage	
													     LIMIT 1
													 ) AS hosImg ON hosImg.documentId = ins.hospitalRegNo
		                            WHERE ins.active=@active AND ins.isVerified =@isVerified " + where + whereSerachHosp
                                    + " ORDER BY ins.hospitalNameEnglish ";
                 dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Hospitals";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ins.doctorRegNo,ins.doctorNameEnglish,ins.doctorNameLocal,ins.stateId,ins.districtId,ins.address AS DoctorAddress,ins.mobileNo,
                                ins.emailId,ins.active,dist.districtNameEnglish AS DoctorDistrictName,ins.cityId,ins.cityName AS DoctorCityname,
                                docImg.documentId,docImg.documentName,docImg.documentExtension
                                FROM doctorregistration AS ins 
				                    INNER JOIN district AS dist ON dist.districtId=ins.districtId 
				                    LEFT JOIN ( 
                                       	SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId
													     FROM documentstore AS ds
													     INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
													     WHERE dpt.documentImageGroup=@Doctor AND dpt.documentType = @DoctorProfile	
													     LIMIT 1
													 ) AS docImg ON docImg.documentId = ins.doctorRegNo
                            WHERE ins.registrationStatus=@active " + where + whereSerachDoc
                            + " ORDER BY ins.doctorNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Doctors";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ins.id,ins.nameEnglish AS specializationName
	                            FROM ddlcatlist AS ins WHERE ins.category=@hospitalSpecialization" + whereSerachSpec
                            + " ORDER BY ins.nameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Specialization";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ins.doctorRegNo,ins.doctorNameEnglish,ins.doctorNameLocal,ins.address AS doctorAddress,ins.mobileNo,
                                    ins.emailId,ins.cityId,ins.cityName AS DoctorCityname,dwa.price,dwa.consultancyTypeName,
		                            dwa.address1 AS workAreaAddress,dwa.phoneNumber AS workAreaPhoneNumber,
		                            dwa.hospitalRegNo,dwa.hospitalNameEnglish,dwa.hospitalAddress,
		                            dwaImg.documentId,dwaImg.documentName,dwaImg.documentExtension
                                    FROM doctorregistration AS ins 
                                INNER JOIN district AS dist ON dist.districtId=ins.districtId 
                                INNER JOIN doctorworkarea AS dwa ON dwa.doctorRegNo = ins.doctorRegNo 
                                LEFT JOIN ( 
                                       	SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId
													     FROM documentstore AS ds
													     INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
													     WHERE dpt.documentImageGroup=@Doctor AND dpt.documentType = @DoctorWorkArea	
													     LIMIT 1
													 ) AS dwaImg ON dwaImg.documentId = dwa.doctorRegNo
                            WHERE ins.registrationStatus = @active AND ins.isVerified=@isVerified AND ins.active=@active " + where + whereSerachSpec
                            + " ORDER BY ins.doctorNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Clinics";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT ds.documentId,ds.documentNumber,ds.documentName,ds.documentExtension,ds.districtId,dist.districtNameEnglish, dpt.documentType, dpt.documentImageGroup
                             FROM documentstore AS ds
                              INNER JOIN district AS dist ON dist.districtId = ds.districtId
                             INNER JOIN documentpathtbl AS dpt ON ds.dptTableId = dpt.dptTableId 
                             WHERE dpt.documentImageGroup=@Website AND dpt.documentType = @WebsiteBanner " + where ;
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "Banners";
                dataSet.dataset.Tables.Add(dtt.table);

                query = @" SELECT dist.districtId,dist.districtNameEnglish,dist.latitude, dist.longitude
                                FROM district AS dist
                            ORDER BY dist.districtNameEnglish ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "District";
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
    }


}
