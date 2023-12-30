using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Linq.Expressions;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{

    public class DlMasters
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        //public async Task<ReturnClass.ReturnBool> CUDOperation(BlCity bl)
        //{
        //    ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
        //    MySqlParameter[] pm;
        //    if (bl.CRUD == (Int16)CRUD.Update)
        //    {
        //        if (bl.cityId == 0)
        //        {
        //            rb.status = false;
        //            rb.message = "Invalid CityId !";
        //            return rb;
        //        }
        //    }
        //    else if (bl.CRUD == (Int16)CRUD.Create)
        //    {
        //        string qr = @"SELECT c.cityNameEnglish 
        //                  FROM city AS c
        //            WHERE c.cityNameEnglish=@cityNameEnglish ";
        //        MySqlParameter[] pmc = new MySqlParameter[]
        //          {
        //                 new MySqlParameter("cityNameEnglish", MySqlDbType.VarChar,99) { Value = bl.cityNameEnglish },
        //          };
        //        dt = await db.ExecuteSelectQueryAsync(qr, pmc);
        //        if (dt.table.Rows.Count > 0)
        //        {
        //            rb.status = false;
        //            rb.message = "City Name already exists !";
        //            return rb;
        //        }
        //        bl.cityId = await GetCityId(bl.stateId);
        //    }
        //    string query = "";
        //    bool isValidated = true;
        //    if (string.IsNullOrEmpty(bl.districtId.ToString()))
        //    {
        //        rb.status = false;
        //        rb.message = "Select District Name !";
        //        return rb;
        //    }
        //    else if (bl.cityNameEnglish == string.Empty)
        //    {
        //        rb.status = false;
        //        rb.message = "Enter City Name in English !";
        //        return rb;
        //    }
        //    else if (bl.cityNameLocal == string.Empty)
        //    {
        //        rb.status = false;
        //        rb.message = "Enter City Name in Hindi !";
        //        return rb;
        //    }
        //    if (isValidated)
        //    {
        //        pm = new MySqlParameter[]
        //        {
        //            new MySqlParameter("districtId", MySqlDbType.Int16) { Value = bl.districtId },
        //            new MySqlParameter("cityId", MySqlDbType.Int32) { Value = bl.cityId },
        //            new MySqlParameter("cityNameEnglish", MySqlDbType.VarChar, 99) { Value = bl.cityNameEnglish },
        //            new MySqlParameter("cityNameLocal", MySqlDbType.VarChar, 99) { Value = bl.cityNameLocal },
        //            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
        //            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
        //            new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp }
        //        };
        //        if (bl.CRUD == (Int16)CRUD.Create)
        //        {
        //            query = @"INSERT INTO city (districtId,cityId,cityNameEnglish,cityNameLocal,clientIp,entryDateTime,userId)
        //                                VALUES (@districtId,@cityId,@cityNameEnglish,@cityNameLocal,@clientIp,@entryDateTime,@userId)";
        //            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "city");
        //        }
        //        else if (bl.CRUD == (Int16)CRUD.Update)
        //        {
        //            query = @"UPDATE city 
        //                                SET districtId=@districtId,cityNameEnglish = @cityNameEnglish,cityNameLocal = @cityNameLocal
        //                        WHERE cityId = @cityId";
        //            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "city");
        //        }
        //    }
        //    return rb;
        //}

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnString> GetCityId(Int16 stateId)
        {
            ReturnClass.ReturnString rs = new();
            try
            {
                string query = @"SELECT IFNULL(MAX(cityCount), 0) + 1 AS cityCount 
                             FROM city AS c
                             WHERE c.stateId=@stateId";
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = stateId });
                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    rs.any_id = (stateId).ToString() + dt.table.Rows[0]["cityCount"].ToString();
                    rs.value = dt.table.Rows[0]["cityCount"].ToString()!;
                    rs.status = true;
                }
            }
            catch (Exception ex) { }
            return rs;
        }

    }


}
