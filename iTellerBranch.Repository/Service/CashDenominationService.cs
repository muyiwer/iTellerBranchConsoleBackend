using iTellerBranch.Repository.Interface;
using iTellerBranch.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public class CashDenominationService : BaseService, ICashDenominationService
    {
        public object GetCashDenomination(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex,
                data = db.CashDenomination.Select(x => new
                {
                    x.ID,
                    x.Currency,
                    x.SubunitName,
                    x.RelationshipUnit,
                    x.Abbrev,
                    x.CurrencyCode,
                    x.DateCreated,
                    Denomination = x.Denomination.Select(y => new
                    {
                        y.ID,
                        y.Name,
                        y.Value,
                        y.DateCreated
                    })
                }).ToArray()
            };
        }

        public object CreateCashDenomination(CashDenomination cashDenomination)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    if (!IsCashDenominationNameExist(cashDenomination.Currency))
                    {
                        var param = new SqlParameter[]
                        {
                           new SqlParameter("@Currency", cashDenomination.Currency),
                           new SqlParameter("@SubunitName", cashDenomination.SubunitName),
                           new SqlParameter("@Relationshipunit", cashDenomination.RelationshipUnit),
                           new SqlParameter("@Abbrev", cashDenomination.Abbrev)
                        };
                        result = db.Database.SqlQuery<int>("dbo.CashDenomination_CreateCashDenomination " +
                            "@Currency,@Relationshipunit,@SubunitName,@Abbrev", param)
                            .FirstOrDefault();

                        // create denomination
                        foreach(var denomination in cashDenomination.Denomination)
                        {
                            denomination.CashDenominationID = result;
                        }
                        db.Denomination.AddRange(cashDenomination.Denomination);
                        db.SaveChanges();
                        transaction.Commit();
                        return GetCashDenomination(true, "Currency created successfully");

                    }
                    else
                    {
                        return GetCashDenomination(false, "Currency already exist");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public object UpdateCashDenomination(CashDenomination cashDenomination)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int result = 0;
                    if (IsCashDenominationExist(cashDenomination.ID))
                    {
                        var param = new SqlParameter[]
                        {
                            new SqlParameter("@ID", cashDenomination.ID),
                           new SqlParameter("@Currency", cashDenomination.Currency),
                           new SqlParameter("@SubunitName", cashDenomination.SubunitName),
                           new SqlParameter("@Relationshipunit", cashDenomination.RelationshipUnit),
                           new SqlParameter("@Abbrev", cashDenomination.Abbrev)
                        };
                        try
                        {
                            result = db.Database.SqlQuery<int>("dbo.CashDenomination_UpdateCashDenomination" +
                                " @ID,@Currency,@Relationshipunit,@SubunitName,@Abbrev", param)
                          .FirstOrDefault();
                        }
                        catch (SqlException ex)
                        {
                            throw ex;
                        }

                        // update denomination
                        var denominationDB = db.Denomination.Where(x => x.CashDenominationID == cashDenomination.ID).ToArray();
                        if(denominationDB.Count() > 0)
                        {
                            db.Denomination.RemoveRange(denominationDB);
                            db.SaveChanges();
                        }
                        List<Denomination> denominations = new List<Denomination>();
                        foreach(var denomination in cashDenomination.Denomination)
                        {
                            denominations.Add(new Denomination
                            {
                                Name = denomination.Name,
                                CashDenominationID = cashDenomination.ID,
                                Value = denomination.Value,
                                DateCreated = DateTime.Now
                            });
                        }
                        db.Denomination.AddRange(denominations); 
                        db.SaveChanges();
                        transaction.Commit();
                        return GetCashDenomination(true, "Currency updated successfully");

                    }
                    else
                    {
                        return GetCashDenomination(false, "Currency does not exist");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private bool IsCashDenominationNameExist(string cashDenominationName) 
        {
            return db.CashDenomination.Where(x => x.Currency == cashDenominationName)
                 .ToList().Count() > 0 ? true : false; 
        }

        private bool IsCashDenominationExist(int ID)
        {
            return db.CashDenomination.Find(ID)
                 != null ? true : false;
        }
    }
}
