using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using camis.types.Utils;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.LandBankGood.ViewModel;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;

namespace intapscamis.camis.domain.LandBank
{
    public class LandBankService : CamisService, ILandBankService
    {
        UserSession _session;
        NrlaisInterface _nrlais = null;

        public LandBankService()
        {
            _nrlais = new RestNrlaisInterface();
        }

        public override void SetContext(CamisContext context)
        {
            base.SetContext(context);
        }

        public void SetSession(UserSession session)
        {
            _session = session;
        }

        public void TransferToCertification(Guid wiid, Guid landID)
        {
        }

        public NrlaisInterfaceModel.Parcel getParcel(String upid)
        {
            var r = _nrlais.getLandData(upid);
            if (!String.IsNullOrEmpty(r.error))
            {
                throw new InvalidOperationException($"Error trying to retrieve parcel data from nrlais.\n{r.error}");
            }

            return r.res;
        }

        public Guid RegisterLand(LandBankFacadeModel.LandData data)
        {
            intapscamis.camis.data.Entities.Land l = new Land();
            //spatial        
            l.Id = Guid.NewGuid();
            l.Wid = Guid.Parse(data.WID);

            l.Description = data.Description;
            l.LandType = data.LandType;
            Context.Land.Add(l);

            saveLandDetail(data, l);

            //additional data
            SaveAdditionalData(data, l);
            return l.Id;
        }

        private void SaveAdditionalData(LandBankFacadeModel.LandData data, Land l) //Reverted Push
        {
            #region Uploading Files

            Documents.DocumentService documentService = new Documents.DocumentService();
            documentService.SetSession(_session);
            documentService.SetContext(Context);

            var documentLIst = data.UploadDocument;
            documentLIst?.ForEach(doc =>
            {
                var tempDoc = documentService.CreateDocument(doc);
                var tempLandDoc = new LandDoc
                {
                    Id = Guid.NewGuid(),
                    LandId = l.Id,
                    DocId = tempDoc.Id
                };
                Context.LandDoc.Add(tempLandDoc);
                Context.SaveChanges();
            });

            #endregion

            #region Saving Current Land Usage

            var usageList = data.ExistLandUse;
            usageList?.ForEach(usage =>
            {
                var landUsage = new LandUsage
                {
                    LandId = l.Id,
                    Use = usage
                };
                Context.LandUsage.Add(landUsage);
                Context.SaveChanges();
            });

            #endregion

            #region Saving Irrigation Values

            var irrigationJson = new LandBankFacadeModel.IrrigationValues();
            irrigationJson = data.IrrigationValues;


            var irrigation = new Irrigation
            {
                LandId = l.Id
            };

            Context.Irrigation.Add(irrigation);
            Context.SaveChanges();

            irrigation.Id = Context.Irrigation
                .FirstOrDefault(x => (x.LandId == l.Id)).Id;
            Context.SaveChanges();

            var groundWater = irrigationJson.GroundWater;
            groundWater.ForEach(wsp =>
            {
                var temp = new GroundData
                {
                    GrndType = wsp,
                    Irrigation = irrigation.Id
                };
                Context.GroundData.Add(temp);
            });
            Context.SaveChanges();

            var waterSourceParameters = irrigationJson.WaterSourceParameter;
            waterSourceParameters.ForEach(wsp =>
            {
                var temp = new WaterSrcParam
                {
                    SrcType = wsp.WaterSourceType,
                    Irrigation = irrigation.Id,
                    Result = wsp.Result
                };
                Context.WaterSrcParam.Add(temp);
            });
            Context.SaveChanges();

            var surfaceWater = irrigationJson.SurfaceWater;
            surfaceWater?.ForEach(sw =>
            {
                var temp = new SurfaceWater
                {
                    Type = sw.SurfaceWaterType,
                    Irrigation = irrigation.Id,
                    Result = sw.Result
                };
                Context.SurfaceWater.Add(temp);
            });

            Context.SaveChanges();

            #endregion

            #region AgriculturalZone

            var n = new[] { "" };

            var agriculturalZone = new AgriculturalZone
            {
                LandId = l.Id,
                IsAgriZone = data.IsAgriculturalZone
            };
            Context.AgriculturalZone.Add(agriculturalZone);
            Context.SaveChanges();

            #endregion

            #region Land Moisture

            data.MoistureSource?.ForEach(moi =>
            {
                var moisture = new LandMoisture
                {
                    LandId = l.Id,
                    Moisture = moi
                };
                Context.LandMoisture.Add(moisture);
                Context.SaveChanges();
            });

            #endregion

            #region Topography

            var topographyList = data.Topography;

            topographyList?.ForEach(i =>
            {
                var topography = new Topography
                {
                    Id = Guid.NewGuid(),
                    LandId = l.Id,
                    Type = i.TopographyType,
                    Result = i.Result
                };
                Context.Topography.Add(topography);
                Context.SaveChanges();
            });

            #endregion

            #region Land Investment

            var investment = data.InvestmentType;
            investment?.ForEach(i =>
            {
                Context.LandInvestment.Add(new LandInvestment
                {
                    LandId = l.Id,
                    Investment = i
                });
                Context.SaveChanges();
            });

            #endregion

            #region AgroEchology

            var agroEchology = new AgroEchology();

            foreach (var i in data.AgroEchologyZone)
            {
                agroEchology = new AgroEchology
                {
                    LandId = l.Id,
                    Type = i.AgroType,
                    Result = i.Result
                };
                Context.AgroEchology.Add(agroEchology);
                Context.SaveChanges();
            }

            #endregion

            Context.SaveChanges(_session.Username, (int)Admin.UserActionType.CreateLand);
        }

        private void saveLandDetail(LandBankFacadeModel.LandData data, Land l)
        {
            foreach (LandBankFacadeModel.Climate c in data.Climate)
            {
                intapscamis.camis.data.Entities.LandClimate climate = new LandClimate
                {
                    LandId = l.Id,
                    Month = c.month,
                    Precipitation = (float)c.precipitation,
                    TempAvg = (float)c.temp_avg,
                    TempLow = (float)c.temp_low,
                    TempHigh = (float)c.temp_high,
                };
                Context.LandClimate.Add(climate);
            }

            foreach (var a in data.Accessablity)
            {
                intapscamis.camis.data.Entities.LandAccessibility accessibility = new LandAccessibility
                {
                    LandId = l.Id,
                    Accessibility = a
                };
                Context.LandAccessibility.Add(accessibility);
            }

            foreach (var t in data.SoilTests)
            {
                intapscamis.camis.data.Entities.SoilTest test = new SoilTest
                {
                    LandId = l.Id,
                    TestType = t.TestType,
                    Result = t.Result,
                };
                Context.SoilTest.Add(test);
            }

            var geoms = new Dictionary<String, String>();
            foreach (var u in data.Upins)
            {
                String prof = null;
                if (data.parcels.ContainsKey(u))
                {
                    var p = data.parcels[u];
                    if (!String.IsNullOrEmpty(p.geometry))
                    {
                        geoms.Add(u, p.geometry);
                        p.geometry = null;
                        prof = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    }
                }

                intapscamis.camis.data.Entities.LandUpin upin = new LandUpin
                {
                    LandId = l.Id,
                    Upin = u,
                    Profile = prof
                };
                Context.LandUpin.Add(upin);
            }

            Context.SaveChanges(_session.Username, (int)Admin.UserActionType.CreateLand);
            foreach (var g in geoms)
            {
                var sql = $@"Update lb.land_upin set 
                        geometry=ST_GeomFromText('{g.Value}'), 
                        area=ST_Area(ST_GeomFromText('{g.Value}')),
                        centroid_x=ST_X(st_centroid(ST_GeomFromText('{g.Value}'))),
                        centroid_y=ST_Y(st_centroid(ST_GeomFromText('{g.Value}')))                
                where land_id='{l.Id}'::uuid and upin='{g.Key}'";
                new Npgsql.NpgsqlCommand(sql,
                    (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection()).ExecuteScalar();
            }
        }

        internal LandBankFacadeModel.Bound GetLandMapBound()
        {
            Context.Database.OpenConnection();
            var obj = new Npgsql.NpgsqlCommand(
                "Select st_geometryfromtext(ST_AsText(ST_Extent(geometry))) from lb.land_upin",
                (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection()).ExecuteScalar();
            if (obj is Polygon poly && poly.Coordinates.Length > 0)
            {
                var p = poly.Coordinates;
                return new LandBankFacadeModel.Bound()
                {
                    X1 = p.Min(c => c.X),
                    Y1 = p.Min(c => c.Y),
                    X2 = p.Max(c => c.X),
                    Y2 = p.Max(c => c.Y)
                };
            }

            return null;
        }

        public LandBankFacadeModel.LandData GetLand(Guid landId, bool geom, bool dd)
        {
            var r = Context.Land.Where(x => x.Id == landId);
            if (r.Any())
            {
                var l = r.First();
                pupulateChildList(l);
                return buildLandData(l, geom, dd);
            }

            return null;
        }

        public void Updateland(LandBankFacadeModel.LandData data)
        {
            intapscamis.camis.data.Entities.Land l = Context.Land.Find(data.LandID);
            //spatial
            l.Description = data.Description;
            l.LandType = data.LandType;
            Context.Land.Update(l);

            Context.LandClimate.RemoveRange(l.LandClimate);
            Context.LandAccessibility.RemoveRange(l.LandAccessibility);
            Context.SoilTest.RemoveRange(l.SoilTest);
            Context.LandUpin.RemoveRange(l.LandUpin);
            saveLandDetail(data, l);

            //updating additional infomation
            Context.AgroEchology.RemoveRange(l.AgroEchology);
            Context.LandInvestment.RemoveRange(l.LandInvestment);
            Context.LandMoisture.RemoveRange(l.LandMoisture);
            Context.LandDoc.RemoveRange(l.LandDoc);
            Context.LandUsage.RemoveRange(l.LandUsage);

            var irrigation = Context.Irrigation.Find(l.Irrigation);
            Context.WaterSrcParam.RemoveRange(irrigation.WaterSrcParam);
            Context.GroundData.RemoveRange(irrigation.GroundData);
            Context.SurfaceWater.RemoveRange(irrigation.SurfaceWater);
            Context.Irrigation.RemoveRange(l.Irrigation);

            Context.Topography.RemoveRange(l.Topography);
            Context.AgriculturalZone.RemoveRange(l.AgriculturalZone);

            SaveAdditionalData(data, l);
            Context.SaveChanges(_session.Username, (int)Admin.UserActionType.UpdateLand);
        }

        public List<LandBankFacadeModel.LandData> GetLandData(Guid[] excludedIds)
        {
            var ids = Context.Land.Where(m => excludedIds.Any(id => id != m.Id)).Select(m => m.Id).ToList();
            var landIds = Context.LandUpin.Select(m => (Guid)m.LandId).ToList();
            var remaining = landIds.Except(excludedIds);
            List<LandBankFacadeModel.LandData> data = new List<LandBankFacadeModel.LandData>();
            foreach (var id in remaining)
            {
                data.Add(GetLand(id, true, true));
            }

            return data;
        }

        internal LandBankFacadeModel.LatLng GetLandCoordinate(Guid landId)
        {
            Context.Database.OpenConnection();
            var l = GetLand(landId, false, false);
            var sql =
                $"select ST_AsText(ST_Transform(ST_GeometryFromText('SRID=20137;Point({l.CentroidX} {l.CentroidY})'),4326));";
            var point = new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection())
                .ExecuteScalar().ToString();

            return LandBankFacadeModel.LatLng.FromWkt(point);
        }

        public void RemoveLand(Guid landID)
        {
            intapscamis.camis.data.Entities.Land l = Context.Land.Find(landID);

            Context.LandClimate.RemoveRange(l.LandClimate);
            Context.LandAccessibility.RemoveRange(l.LandAccessibility);
            Context.SoilTest.RemoveRange(l.SoilTest);


            #region removing additional data

            Context.AgroEchology.RemoveRange(l.AgroEchology);
            Context.LandInvestment.RemoveRange(l.LandInvestment);
            Context.LandMoisture.RemoveRange(l.LandMoisture);
            Context.LandDoc.RemoveRange(l.LandDoc);
            Context.LandUsage.RemoveRange(l.LandUsage);

            var irrigation = Context.Irrigation.Find(l.Irrigation);
            Context.WaterSrcParam.RemoveRange(irrigation.WaterSrcParam);
            Context.SurfaceWater.RemoveRange(irrigation.SurfaceWater);
            Context.GroundData.RemoveRange(irrigation.GroundData);
            Context.Irrigation.RemoveRange(l.Irrigation);

            Context.Topography.RemoveRange(l.Topography);
            Context.AgriculturalZone.RemoveRange(l.AgriculturalZone);

            #endregion

            Context.LandUpin.RemoveRange(l.LandUpin);
            Context.Land.Remove(l);
            Context.SaveChanges(_session.Username, (int)Admin.UserActionType.UpdateLand);
        }

        LandBankFacadeModel.LandData buildLandData(Land l, bool geom, bool dd)
        {
            var ret = new LandBankFacadeModel.LandData();

            ret.LandID = l.Id.ToString();
            ret.WID = l.Wid.ToString();
            ret.LandType = l.Locked?7:l.LandType;
            ret.Accessablity = new List<int>();
            ret.Description = l.Description;


            foreach (var a in l.LandAccessibility)
                ret.Accessablity.Add(a.Accessibility.Value);
            ret.SoilTests = new List<LandBankFacadeModel.SoilTest>();


            foreach (var s in l.SoilTest)
                ret.SoilTests.Add(new LandBankFacadeModel.SoilTest()
                    { Result = s.Result, TestType = s.TestType.Value });
            ret.Upins = new List<string>();
            ret.parcels = new Dictionary<string, NrlaisInterfaceModel.Parcel>();
            ret.Area = 0;
            ret.CentroidX = 0;
            ret.CentroidY = 0;
            ret.Locked = l.Locked;
            ret.LandSplit = new List<LandBankFacadeModel.LandSplitResponse>();
            using (var tempContext = new CamisContext())
            {
                foreach (var u in l.LandUpin)
                {
                    ret.Upins.Add(u.Upin);
                    if (!string.IsNullOrEmpty(u.Profile))
                    {
                        var geometrySql =
                            $"SELECT {(dd ? "ST_AsText(ST_Transform(geometry,4326))" : "ST_AsText(geometry)")} FROM lb.land_upin WHERE land_id=@lid AND upin=@upin";

                        // Open connection if needed
                        if (tempContext.Database.GetDbConnection().State != ConnectionState.Open)
                            tempContext.Database.OpenConnection();

                        using (var cmd = tempContext.Database.GetDbConnection().CreateCommand())
                        {
                            cmd.CommandText = geometrySql;
                            cmd.Parameters.Add(new NpgsqlParameter("lid", l.Id));
                            cmd.Parameters.Add(new NpgsqlParameter("upin", u.Upin));

                            var geometry = cmd.ExecuteScalar()?.ToString();

                            var p =
                                Newtonsoft.Json.JsonConvert.DeserializeObject<NrlaisInterfaceModel.Parcel>(u.Profile);
                            if (geom) p.geometry = geometry;
                            ret.parcels.Add(u.Upin, p);
                            ret.landHolderType = p.GetHolder().partyType;
                        }
                    }

                    ret.Area += u.Area.Value;
                    ret.CentroidX += u.CentroidX.Value;
                    ret.CentroidY += u.CentroidY.Value;
                    foreach (var lr in l.LandRights)
                    {
                        if (u.LandId == lr.LandId && lr.SplitIndex == 0)
                        {
                            ret.LandRight = MapLandRight(lr);
                        }
                    }
                }

                foreach (var lsp in l.LandSplits)
                {
                    var landRights = new LandBankFacadeModel.LandRightResponse();
                    landRights = null;
                    foreach (var lr in l.LandRights)
                    {
                        if (lr.LandId == lsp.LandId && lr.SplitIndex == lsp.Id)
                        {
                            landRights = MapLandRight(lr);
                        }
                    }

                    var geometrySql =
                        $"SELECT {(dd ? "ST_AsText(ST_Transform(geometry,4326))" : "ST_AsText(geom)")} FROM lb.land_split WHERE land_id=@lid AND id=@id";

                    // Open connection if needed
                    if (tempContext.Database.GetDbConnection().State != ConnectionState.Open)
                        tempContext.Database.OpenConnection();

                    using (var cmd = tempContext.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = geometrySql;
                        cmd.Parameters.Add(new NpgsqlParameter("lid", lsp.LandId));
                        cmd.Parameters.Add(new NpgsqlParameter("id", lsp.Id));

                        var geometry = cmd.ExecuteScalar()?.ToString();

                        var p = new LandBankFacadeModel.LandSplitResponse()
                        {
                            Id = lsp.Id,
                            Area = lsp.Area,
                            Geom = geometry,
                            LandId = lsp.LandId.ToString(),
                            Status = lsp.Locked?7:lsp.Status,
                            Indexes = lsp.Indexes,
                            LandRight = landRights,
                            Locked = lsp.Locked
                        };

                        ret.LandSplit.Add(p);
                    }
                }
            }

            if (ret.parcels.Count > 0)
            {
                var p = ret.parcels[l.LandUpin.First().Upin];
                if (p.IsStateLand())
                {
                    ret.Holdership = "State Land";
                }
                else
                {
                    ret.Holdership = "Private Land";
                }
            }

            if (l.LandUpin.Count > 0)
            {
                ret.CentroidX /= l.LandUpin.Count;
                ret.CentroidY /= l.LandUpin.Count;
            }

            ret.Climate = new List<LandBankFacadeModel.Climate>();
            foreach (var c in l.LandClimate)
            {
                ret.Climate.Add(new LandBankFacadeModel.Climate()
                {
                    month = c.Month,
                    precipitation = c.Precipitation == null ? -1 : c.Precipitation.Value,
                    temp_avg = c.TempAvg == null ? -1 : c.TempAvg.Value,
                    temp_low = c.TempLow == null ? -1 : c.TempLow.Value,
                    temp_high = c.TempHigh == null ? -1 : c.TempHigh.Value,
                });
            }

            ;

            #region Adding Additional data

            ret.AgroEchologyZone = new List<LandBankFacadeModel.AgroEchologyZone>();
            foreach (var agro in l.AgroEchology)
            {
                ret.AgroEchologyZone.Add(new LandBankFacadeModel.AgroEchologyZone
                {
                    AgroType = agro.Type,
                    Result = agro.Result
                });
            }

            ret.InvestmentType = new List<int>();
            foreach (var inv in l.LandInvestment)
            {
                ret.InvestmentType.Add(inv.Investment);
            }

            ret.MoistureSource = new List<int>();
            foreach (var moi in l.LandMoisture)
            {
                ret.MoistureSource.Add(moi.Moisture);
            }
            //ret.MoistureSource = l.LandMoisture.FirstOrDefault(m => m.LandId == l.Id).Moisture;

            var irrigation = l.Irrigation.ToList().FirstOrDefault();
            ret.IrrigationValues = new LandBankFacadeModel.IrrigationValues
            {
                GroundWater = new List<int>(),
                WaterSourceParameter = new List<LandBankFacadeModel.WaterSourceParameter>(),
                SurfaceWater = new List<LandBankFacadeModel.SurfaceWater>()
            };
            if (irrigation != null)
            {
                var srcParam = Context.WaterSrcParam.Where(wsp => wsp.Irrigation == irrigation.Id).ToList();
                List<LandBankFacadeModel.WaterSourceParameter> wtrSrcParam =
                    new List<LandBankFacadeModel.WaterSourceParameter>();
                foreach (var item in srcParam)
                {
                    wtrSrcParam.Add(new LandBankFacadeModel.WaterSourceParameter
                    {
                        WaterSourceType = item.SrcType,
                        Result = item.Result
                    });
                }

                List<LandBankFacadeModel.SurfaceWater> srfWtr = new List<LandBankFacadeModel.SurfaceWater>();
                var srfCol = Context.SurfaceWater.Where(s => s.Irrigation == irrigation.Id).ToList();
                foreach (var item in srfCol)
                {
                    srfWtr.Add(new LandBankFacadeModel.SurfaceWater
                    {
                        SurfaceWaterType = item.Type,
                        Result = item.Result
                    });
                }

                List<int> grndWtr = new List<int>();
                var grndCol = Context.GroundData.Where(s => s.Irrigation == irrigation.Id).ToList();
                foreach (var item in grndCol)
                {
                    grndWtr.Add(item.GrndType);
                }

                ret.IrrigationValues = new LandBankFacadeModel.IrrigationValues
                {
                    WaterSourceParameter = wtrSrcParam,
                    GroundWater = grndWtr,
                    SurfaceWater = srfWtr
                };
            }

            ret.Topography = new List<LandBankFacadeModel.Topography>();
            foreach (var topography in l.Topography)
            {
                ret.Topography.Add(new LandBankFacadeModel.Topography
                {
                    TopographyType = topography.Type,
                    Result = topography.Result
                });
            }


            ret.ExistLandUse = new List<int>();
            if (l.LandUsage.Count != 0)
                foreach (var elu in l.LandUsage)
                {
                    ret.ExistLandUse.Add(elu.Use);
                }


            var agrZone = l.AgriculturalZone;
            ret.IsAgriculturalZone = agrZone != null ? agrZone.IsAgriZone : "Yes";

            ret.UploadDocument = new List<Documents.Models.DocumentRequest>();

            var uplDoc = l.LandDoc.ToList();
            foreach (var doc in uplDoc)
            {
                var document = Context.Document.FirstOrDefault(d => d.Id == doc.DocId);
                ret.UploadDocument.Add(new Documents.Models.DocumentRequest
                {
                    Date = document.Date,
                    File = document.File.ToString(),
                    Filename = document.Filename,
                    Id = document.Id,
                    Mimetype = document.Mimetype,
                    Note = document.Note,
                    Ref = document.Ref,
                    Type = document.Type,
                    OverrideFilePath = document.OverrideFilePath
                });
            }

            #endregion


            return ret;
        }

        public LandBankFacadeModel.LandSearchResult SearchLand(LandBankFacadeModel.LandSearchPar par)
        {
            String cr = null;
            if (!String.IsNullOrEmpty(par.Upin))
                cr = StringExtensions.addDelimitedListItem(cr, " and ",
                    $"l.id in (Select land_id from lb.land_upin where upin='{par.Upin}')");
            if (par.AreaMin > 0)
                cr = StringExtensions.addDelimitedListItem(cr, " and ", $"l.area>={par.AreaMin}");
            if (par.AreaMax > 0)
                cr = StringExtensions.addDelimitedListItem(cr, " and ", $"l.area<={par.AreaMax}");
            if (par.LandType != -1)
            {
                if (par.LandType == (int)LandBankFacadeModel.LandTypeEnum.TransactionLocked)
                {
                    cr = StringExtensions.addDelimitedListItem(cr, " and ", $"l.locked=true");
                }
                else
                {
                    cr = StringExtensions.addDelimitedListItem(cr, " and ", $"l.land_type={par.LandType} and l.locked=false");
                }
            }
                
            var sql = $"Select l.* from lb.Land l {(cr == null ? "" : " where " + cr)}";
            var ret = new LandBankFacadeModel.LandSearchResult();
            ret.Result = new List<LandBankFacadeModel.LandData>();
            var lands = Context.Land.FromSqlRaw(sql).ToList();
            foreach (var l in lands)
            {
                pupulateChildList(l);
                ret.Result.Add(buildLandData(l, false, false));
            }

            return ret;
        }

        private void pupulateChildList(Land l)
        {
            l.LandAccessibility = Context.LandAccessibility.Where(x => x.LandId == l.Id).ToList();
            l.LandClimate = Context.LandClimate.Where(x => x.LandId == l.Id).ToList();
            l.LandUpin = Context.LandUpin.Where(x => x.LandId == l.Id).ToList();
            l.SoilTest = Context.SoilTest.Where(x => x.LandId == l.Id).ToList();


            l.AgroEchology = Context.AgroEchology.Where(x => x.LandId == l.Id).ToList();
            l.LandInvestment = Context.LandInvestment.Where(x => x.LandId == l.Id).ToList();
            l.LandMoisture = Context.LandMoisture.Where(x => x.LandId == l.Id).ToList();

            l.LandDoc = Context.LandDoc.Where(x => x.LandId == l.Id).ToList();
            l.LandUsage = Context.LandUsage.Where(x => x.LandId == l.Id).ToList();
            l.Irrigation = Context.Irrigation.Where(x => x.LandId == l.Id).ToList();
            l.LandSplits = Context.LandSplit.Where(x => x.LandId == l.Id).ToList();
            l.Topography = Context.Topography.Where(x => x.LandId == l.Id).ToList();
            l.AgriculturalZone = Context.AgriculturalZone.FirstOrDefault(x => x.LandId == l.Id);
            l.LandRights = Context.LandRight.Where(x => x.LandId == l.Id).ToList();
        }

        public LandBankFacadeModel.LandData GetLandByUpin(string upin)
        {
            var r = Context.LandUpin.Where(x => x.Upin.Equals(upin));
            if (r.Any())
            {
                return GetLand(r.First().LandId.Value, false, false);
            }

            return null;
        }

        public void SetLandState(Guid landId, LandBankFacadeModel.LandTypeEnum transfered)
        {
            var l = Context.Land.Where(x => x.Id == landId).First();
            l.LandType = (int)transfered;
            Context.Land.Update(l);
            Context.SaveChanges();
        }
        public void SetLandLock(Guid landId, bool val)
        {
            var l = Context.Land.Where(x => x.Id == landId).First();
            l.Locked = val;
            Context.Land.Update(l);
            Context.SaveChanges();
        }

        public void SetSubLandState(Guid landID, int subLandId, LandBankFacadeModel.LandTypeEnum transfered)
        {
            var l = Context.LandSplit.Where(x => x.Id == subLandId && x.LandId == landID).First();
            l.Status = (int)transfered;
            Context.LandSplit.Update(l);
            Context.SaveChanges();
        }
        public void SetSubLandLock(Guid landId,int subLandId, bool val)
        {
            var l = Context.LandSplit.Where(x => x.LandId == landId && x.Id==subLandId).First();
            l.Locked = val;
            Context.LandSplit.Update(l);
            Context.SaveChanges();
        }
        public void TransferLand(LandBankFacadeModel.TransferRequest request)
        {
            var l = Context.Land.Where(x => x.Id == request.landID);
            var landParts=Context.LandSplit.Where(x=>x.LandId == request.landID && x.Id==request.landPart).FirstOrDefault();
            CamisUtils.Assert(l.Any(), "No land record found with id " + request.landID);
            var land = l.First();
            var landUpin=Context.LandUpin.First(x=>x.LandId==request.landID);
            var geom = landUpin.Geometry;
            if (request.right == LandBankFacadeModel.LandRightType.SubLease)
            {
                CamisUtils.Assert(land.LandType is (int)LandBankFacadeModel.LandTypeEnum.Transferred or (int)LandBankFacadeModel.LandTypeEnum.HalfTransferred,
                    "Land is not transferred (for sub-leasing). LandID:" + request.landID);
            }
            else
            {
                CamisUtils.Assert(land.LandType is (int)LandBankFacadeModel.LandTypeEnum.Prepared or (int)LandBankFacadeModel.LandTypeEnum.PreparedWithSplit or (int)LandBankFacadeModel.LandTypeEnum.HalfTransferred,
                    "Land is not prepared id:" + request.landID);
                if (landParts != null)
                {
                    geom = landParts.Geom;
                    landParts.Status=(int)LandBankFacadeModel.LandTypeEnum.Transferred;
                    landParts.Locked=false;
                    Context.LandSplit.Update(landParts);
                }
                var partStatus=Context.LandSplit.Where(x=>x.LandId==request.landID && (x.Status==(int)LandBankFacadeModel.LandTypeEnum.Prepared || x.Status==(int)LandBankFacadeModel.LandTypeEnum.PreparedWithSplit) && x.Id!=request.landPart);
                if (partStatus.Any())
                {
                    land.LandType=(int)LandBankFacadeModel.LandTypeEnum.HalfTransferred;
                }
                else
                {
                    land.LandType = (int)LandBankFacadeModel.LandTypeEnum.Transferred;
                }

                land.Locked = false;
            }

            Context.Land.Update(land);
            Context.LandRight.Add(new LandRight()
            {
                LandId = land.Id,
                RightFrom = request.leaseFrom.Ticks,
                RightTo = request.leaseTo.Ticks,
                RightType = (int)request.right,
                YearlyRent = request.right != LandBankFacadeModel.LandRightType.ContractFarming
                    ? request.yearlyLease
                    : null,
                LandSectionArea = request.right == LandBankFacadeModel.LandRightType.SubLease
                    ? request.landSectionArea
                    : null,
                SplitIndex = request.landPart ?? 0,
                CommonTxtUid = Guid.NewGuid(),
                Status = 4,
                FarmId = Guid.Parse(request.farmId),
                Geom = geom
            });
            Context.SaveChanges();
        }

        public void CalculateCentroid(LandBankFacadeModel.LandData land)
        {
            Context.Database.OpenConnection();
            var sql = $"select ST_AsText(ST_Centroid(ST_GeometryFromText('{land.parcels[land.Upins[0]].geometry}')));";
            var point = new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection())
                .ExecuteScalar().ToString();

            sql = $"select ST_AsText(ST_Transform(ST_GeometryFromText('SRID=20137;{point}'),4326));";
            point = new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection())
                .ExecuteScalar().ToString();

            var latlng = LandBankFacadeModel.LatLng.FromWkt(point);
            land.CentroidX = latlng.lng;
            land.CentroidY = latlng.lat;
        }

        public LandAttributeName GetLandAttributeName()
        {
            LandAttributeName attribName = new LandAttributeName();

            var agroType = Context.AgroType.ToList();
            agroType.ForEach(at => attribName.AgroType.Add(
                new LandBankGood.ViewModel.Type
                {
                    Id = at.Id,
                    Name = at.Name
                }
            ));


            var invType = Context.InverstmentType.ToList();
            invType.ForEach(it => attribName.InvestmentType.Add(new LandBankGood.ViewModel.Type
                {
                    Id = it.Id,
                    Name = it.Name
                }
            ));


            var wtrSrcType = Context.WaterSourceType.ToList();
            wtrSrcType.ForEach(wst => attribName.WaterSourceType.Add(new LandBankGood.ViewModel.Type
                {
                    Id = wst.Id,
                    Name = wst.Name
                }
            ));

            var srfWtrType = Context.SurfaceWaterType.ToList();
            srfWtrType.ForEach(swt => attribName.SurfaceWaterType.Add(
                new LandBankGood.ViewModel.Type
                {
                    Id = swt.Id,
                    Name = swt.Name
                }
            ));


            var topoType = Context.TopographyType.ToList();
            topoType.ForEach(tt => attribName.TopographyType.Add(new LandBankGood.ViewModel.Type
            {
                Id = tt.Id,
                Name = tt.Name
            }));

            var lndUsageType = Context.UsageType.ToList();
            lndUsageType.ForEach(lut => attribName.LandUsageType.Add(
                new LandBankGood.ViewModel.Type
                {
                    Id = lut.Id,
                    Name = lut.Name
                }
            ));
            return attribName;
        }

        private string GetWktFromGeom(Geometry geom)
        {
            var writer = new WKTWriter();
            return writer.Write(geom);
        }

        private LandBankFacadeModel.LandRightResponse MapLandRight(LandRight lr)
        {
            var farmLand=Context.FarmLand.Where(x=>x.LandId==lr.LandId && x.FarmId==lr.FarmId && x.SplitIndex==lr.SplitIndex).FirstOrDefault();
            if (farmLand != null)
            {
                lr.CertificateDocument = farmLand.CertificateDoc;
                lr.ContractDocument = farmLand.LeaseContractDoc;
            }
            var cd = Context.Document.FirstOrDefault(c => c.Id == lr.CertificateDocument);
            var cdDoc = new DocumentResponse();
            cdDoc = null;
            if (cd != null)
            {
                cdDoc = new DocumentResponse()
                {
                    Id = cd.Id,
                    Date = cd.Date,
                    Filename = cd.Filename,
                    Mimetype = cd.Mimetype,
                    OverrideFilePath = cd.OverrideFilePath,
                    Ref = cd.Ref,
                    Type = cd.Type,
                    Note = cd.Note
                };
            }

            var cod = Context.Document.FirstOrDefault(c => c.Id == lr.ContractDocument);
            var codDoc = new DocumentResponse();
            codDoc = null;
            if (cod != null)
            {
                codDoc = new DocumentResponse()
                {
                    Id = cod.Id,
                    Date = cod.Date,
                    Filename = cod.Filename,
                    Mimetype = cod.Mimetype,
                    OverrideFilePath = cod.OverrideFilePath,
                    Ref = cod.Ref,
                    Type = cod.Type,
                    Note = cod.Note
                };
            }

            return new LandBankFacadeModel.LandRightResponse()
            {
                LandId = lr.LandId.ToString(),
                FarmId = lr.FarmId.ToString(),
                SplitIndex = lr.SplitIndex,
                RightFrom = new DateTime(lr.RightFrom ?? 0),
                RightTo = new DateTime(lr.RightTo ?? 0),
                RightType = lr.RightType,
                LandSectionArea = lr.LandSectionArea,
                Status = lr.Status,
                CommonTxtUid = lr.CommonTxtUid.ToString(),
                YearlyRent = lr.YearlyRent,
                Geom = GetWktFromGeom(lr.Geom),
                CertificateDocument = cdDoc,
                ContractDocument = codDoc,
            };
        }

        public string GetGeoServerURL()
        {
            return Context.SysConfigs.Where(e => e.Name.Equals("GoeServer_url")).Select(e => e.Value).First() ??
                   "http://localhost:8080";
        }

        public LandBankFacadeModel.RegionsAttr GetRegionsAttr()
        {
            var ret=new  LandBankFacadeModel.RegionsAttr();
            
            var code=Context.SysConfigs.First(e=>e.Name.Equals("region_code")).Value;
            var region=Context.TRegions.FirstOrDefault(e=>e.Csaregionid==code);
            if (region != null)
            {
                Context.Database.OpenConnection();
                var sql = $"select ST_AsText(ST_Centroid(ST_GeometryFromText('{region.Geometry}')));";
                var point = new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection()).ExecuteScalar().ToString();

                sql = $"select ST_AsText(ST_Transform(ST_GeometryFromText('SRID=20137;{point}'),4326));";
                point = new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection())
                    .ExecuteScalar().ToString();

                var center = LandBankFacadeModel.LatLng.FromWkt(point);
                ret.CenterX = center.lng;
                ret.CenterY = center.lat;
                ret.Geom=region.Geometry;
                ret.RegionCode=code;
                ret.RegionName=region.Csaregionnameeng;
            }
          

            return ret;
        }
    }
}