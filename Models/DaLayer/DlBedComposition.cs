﻿using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;
using System.Net;
using Newtonsoft.Json;
using System.Data;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlBedComposition
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlBedComposition bl)
        {
            MySqlParameter[] pm;
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.hospitalRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Hospital Registration No !";
                return rb;
            }
            string query = "";
            bool isValidated = true;
            if (isValidated)
            {
                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("bedCompositionId", MySqlDbType.Int32) { Value = item.bedCompositionId },
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("noOfBeds", MySqlDbType.Int16) { Value = item.noOfBeds },
                        new MySqlParameter("rentPerDay", MySqlDbType.Int16) { Value = item.rentPerDay },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                    };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO bedcomposition (hospitalRegNo,noOfBeds,rentPerDay,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@noOfBeds,@rentPerDay,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "bedcomposition");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"UPDATE bedcomposition 
                                        SET noOfBeds = @noOfBeds,rentPerDay = @rentPerDay
                                WHERE bedCompositionId = @bedCompositionId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "bedcomposition");
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetBedCompositionDetail(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @"SELECT bc.bedCompositionId,bc.hospitalRegNo,bc.noOfBeds,bc.rentPerDay,cat.nameEnglish 
		                        FROM bedcomposition AS bc
			                        INNER JOIN ddlcatlist AS cat ON bc.bedCompositionId = cat.id
			                        AND cat.category='bedComposition' 
		                        WHERE bc.hospitalRegNo=@hospitalRegNo   
			                        ORDER BY cat.sortOrder";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
    }
}
