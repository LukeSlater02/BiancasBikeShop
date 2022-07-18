using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BiancasBikeShop.Models;

namespace BiancasBikeShop.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection("server=localhost\\SQLExpress;database=BiancasBikeShop;integrated security=true;TrustServerCertificate=true");
            }
        }

        public List<Bike> GetAllBikes()
        {
            var conn = Connection;
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                {
                    cmd.CommandText = @"select Bike.Id BikeId, Brand, Owner.Name, Color
from bike join owner on bike.ownerid = owner.id
";
                    var reader = cmd.ExecuteReader();

                    var bikes = new List<Bike>();
                    while (reader.Read())
                    {
                        bikes.Add(new Bike()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("BikeId")),
                            Brand = reader.GetString(reader.GetOrdinal("Brand")),
                            Color = reader.GetString(reader.GetOrdinal("Color")),
                            Owner = new()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            }
                        }); ;
                    }
                    return bikes;
                }
            }
        }

        public Bike GetBikeById(int id)
        {
            var conn = Connection;
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                {
                    cmd.CommandText = @"select bike.Id BikeId, Brand, Owner.Name OwnerName, Address, BikeType.Name BikeTypeName, Color, WorkOrder.Id WOId, DateCompleted, Description, DateInitiated from bike join owner on bike.OwnerId = owner.id join WorkOrder on WorkOrder.BikeId = bike.Id join BikeType on bike.BikeTypeId = BikeType.id where bike.id = @id";
                    cmd.Parameters.AddWithValue("id", id);
                    var reader = cmd.ExecuteReader();
                    {
                        Bike bike = null;
                        while (reader.Read())
                        {
                            if (bike is null)
                            {
                                bike = new Bike()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BikeId")),
                                    Color = reader.GetString(reader.GetOrdinal("Color")),
                                    Brand = reader.GetString(reader.GetOrdinal("Brand")),
                                    BikeType = new()
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("BikeTypeName")),
                                    },
                                    Owner = new()
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                        Address = reader.GetString(reader.GetOrdinal("Address"))
                                    },
                                    WorkOrders = new()
                                };

                            }
                            bike.WorkOrders.Add(new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WOId")),
                                DateInitiated = reader.GetDateTime(reader.GetOrdinal("DateInitiated")),
                                DateCompleted = !reader.IsDBNull(reader.GetOrdinal("DateCompleted")) ? reader.GetDateTime(reader.GetOrdinal("DateCompleted")) : null,
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                            });
                        }
                        return bike;
                    }
                }
            }
        }

        public int GetBikesInShopCount()
        {
            var conn = Connection;
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                {
                    cmd.CommandText = @"select count(bike.id) from bike left join WorkOrder on bike.id = WorkOrder.BikeId where WorkOrder.DateInitiated is not null and WorkOrder.DateCompleted is null";

                    int count = (Int32)cmd.ExecuteScalar();
                    return count;

                }
            }
        }
    }
}
