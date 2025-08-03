using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Net;
using camis.types.Utils;
using RestSharp;
using RestSharp.Authenticators;

namespace intapscamis.camis.domain.LandBank
{
    public class RestNrlaisInterface : NrlaisInterface
    {
        String sessionID = null;
        String urlBase = "http://localhost:8540/";

        class VoidType
        {
        }

        GetType invokeServer<DataType, GetType>(String cmd, DataType data)
            where DataType : class
            where GetType : class, new()
        {
            var url = urlBase + cmd;
            var options = new RestClientOptions(url)
            {
                ThrowOnAnyError = true,
                Timeout = TimeSpan.FromMilliseconds(30000)
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            
            // Add session cookie if exists
            if (sessionID != null)
                request.AddCookie("JSESSIONID", this.sessionID, path: "/", domain: new Uri(urlBase).Host);

            if (data == null)
            {
                request.Method = Method.Get;
            }
            else
            {
                request.Method = Method.Post;
                request.AddJsonBody(data);
            }

            var resp = client.Execute<GetType>(request);
            var sessionCookie = resp.Cookies.FirstOrDefault(c => c.Name == "JSESSIONID");
            if (sessionCookie != null)
            {
                this.sessionID = sessionCookie.Value;
            }
            if (!resp.IsSuccessful)
            {
                throw new InvalidOperationException($"Request failed: {resp.StatusCode} - {resp.ErrorMessage}");
            }

            return resp.Data;
        }

        public class NrlaisUserNamePasword
        {
            public String userName { get; set; }
            public String password { get; set; }
            public String lang = "en-us";
        }

        public class NrlaisLoginRes
        {
            public String res { get; set; }
            public String error { get; set; }
        }

        public class StringRes
        {
            public String res { get; set; }
            public String error { get; set; }
        }

        void Login(String userName, String password)
        {
            var res = invokeServer<NrlaisUserNamePasword, NrlaisLoginRes>("api/login",
                new NrlaisUserNamePasword() {userName = userName, password = password});
            if (!String.IsNullOrEmpty(res.error))
            {
                throw new InvalidOperationException("Credential for not accepted by NRLAIS.\n" + res.error);
            }
        }

        public NrlaisInterfaceModel.NrlaisParcelResult getLandData(string upid)
        {
            String cmd = String.Format("api/get_parcel?upid={0}", upid);
            Login("Officer", "Officer");
            var parcel = invokeServer<VoidType, NrlaisInterfaceModel.NrlaisParcelResult>(cmd, null);
            return parcel;
        }

        public Guid RequestLandSplit(NrlaisInterfaceModel.Parcel parcel, List<string> geometries)
        {
            Login("Officer", "Officer");

            NrlaisInterfaceModel.NrlaisSplitTransaction splitTransaction =
                new NrlaisInterfaceModel.NrlaisSplitTransaction()
                {
                    holdingUID = parcel.GetHolding(),
                    splitSingleParcel = parcel.parcelUID,
                    geoms = geometries,
                    landHoldingCertifcateDocument = new NrlaisInterfaceModel.NrlaisSourceDocument()
                    {
                        archiveType = NrlaisInterfaceModel.NrlaisSourceDocument.ARCHIVE_TYPE_PAPER,
                        refText = "Unknown",
                        description = "Sent from CAMIS",
                        sourceType = NrlaisInterfaceModel.NrlaisSourceDocument.DOC_TYPE_LAND_HOLDING_CERTIFICATE,
                    },
                };
            splitTransaction.applicants.Add(new NrlaisInterfaceModel.NrlaisApplicant()
            {
                selfPartyUID = parcel.GetHolder().partyUID
            });

            var tran = new NrlaisInterfaceModel.NrlaisTransaction()
            {
                transactionType = NrlaisInterfaceModel.NrlaisTransaction.TRAN_TYPE_SPLIT,
                data = splitTransaction,
                notes = "Split transaction transfered from CAMIS",
                time = NrlaisInterfaceModel.ToJavaTicks(DateTime.Now),
                year = 2011,
                nrlais_kebeleid = parcel.nrlais_kebeleid,
            };
            var res = invokeServer<NrlaisInterfaceModel.NrlaisTransaction, StringRes>("api/register_transaction", tran);
            if (!String.IsNullOrEmpty(res.error))
                throw new InvalidOperationException($"Nrlais rejected split transaciton registration\n{res.error}");
            return Guid.Parse(res.res);
        }

        public class ParcelsListRes
        {
            public List<NrlaisInterfaceModel.Parcel> res { get; set; }
            public String error { get; set; }
        }

        internal List<NrlaisInterfaceModel.Parcel> GetParcelsByTranscation(Guid txuid)
        {
            Login("Officer", "Officer");
            var res = invokeServer<VoidType, ParcelsListRes>("api/get_tran_parcel?txuid=" + txuid, null);
            if (!string.IsNullOrEmpty(res.error))
                throw new InvalidOperationException("Couldn't retrieve transaction data from nrlais.\n" + res.error);
            return res.res;
        }

        public NrlaisInterfaceModel.NrlaisApplicationStatus GetApplicationStatus(Guid txuid)
        {
            Login("Officer", "Officer");
            var res = invokeServer<VoidType, NrlaisInterfaceModel.NrlaisRestRes<NrlaisInterfaceModel.NrlaisTransaction>>
                ("api/get_tran?txuid=" + txuid, null);
            if (!String.IsNullOrEmpty(res.error))
                throw new InvalidOperationException($"Failed to get tansaction status from NRLAIS\n{res.error}");

            if (res.res == null)
                return NrlaisInterfaceModel.NrlaisApplicationStatus.Canceled;
            switch (res.res.status)
            {
                case NrlaisInterfaceModel.NrlaisTransaction.TRAN_STATUS_FINISHED:
                    return NrlaisInterfaceModel.NrlaisApplicationStatus.Completd;
                case NrlaisInterfaceModel.NrlaisTransaction.TRAN_STATUS_CANCELED:
                case NrlaisInterfaceModel.NrlaisTransaction.TRAN_STATUS_ERROR:
                    return NrlaisInterfaceModel.NrlaisApplicationStatus.Canceled;
                default:
                    return NrlaisInterfaceModel.NrlaisApplicationStatus.Processing;
            }
        }

        internal Guid RequestLandTransfer(NrlaisInterfaceModel.Parcel parcel,
            LandBankFacadeModel.TransferRequest request)
        {
            CamisUtils.Assert(request.right == LandBankFacadeModel.LandRightType.LeaseFromState ||
                              request.right == LandBankFacadeModel.LandRightType.RentFromPrivate ||
                              request.right == LandBankFacadeModel.LandRightType.SubLease,
                "Only lease and rent transfers can be prossed by nrlais");
            Login("Officer", "Officer");

            NrlaisInterfaceModel.NrlaisRentTransaction rentTransaction =
                new NrlaisInterfaceModel.NrlaisRentTransaction()
                {
                    holdingUID = parcel.GetHolding(),
                    amount = request.yearlyLease ?? 0,
                    applicants = parcel.GetHolders().Select(x => new NrlaisInterfaceModel.NrlaisApplicant()
                    {
                        selfPartyUID = x.partyUID,
                    }).ToList(),
                    contractTime = NrlaisInterfaceModel.ToJavaTicks(DateTime.Now),
                    isRent = request.right == LandBankFacadeModel.LandRightType.RentFromPrivate,
                    leaseFrom = NrlaisInterfaceModel.ToJavaTicks(request.leaseFrom),
                    leaseTo = NrlaisInterfaceModel.ToJavaTicks(request.leaseTo),
                    tenants = new List<NrlaisInterfaceModel.NrlaisPartyItem>(new NrlaisInterfaceModel.NrlaisPartyItem[]
                    {
                        new NrlaisInterfaceModel.NrlaisPartyItem()
                        {
                            idDoc = new NrlaisInterfaceModel.NrlaisSourceDocument()
                            {
                                archiveType = NrlaisInterfaceModel.NrlaisSourceDocument.ARCHIVE_TYPE_PAPER,
                                refText = "Unknown",
                                description = "Sent from CAMIS",
                                sourceType = NrlaisInterfaceModel.NrlaisSourceDocument.DOC_TYPE_KEBELE_ID,
                            },
                            share = new NrlaisInterfaceModel.NrlaisRationalNum() {num = 1, denum = 1},
                            party = new NrlaisInterfaceModel.Party()
                            {
                                partyType = request.farmer.TypeId == 1
                                    ? NrlaisInterfaceModel.Party.PARTY_TYPE_PERSON
                                    : NrlaisInterfaceModel.Party.PARTY_TYPE_NON_NATURAL_PERSON,
                                idText = "Unkown",
                                idType = 0,
                                dateOfBirth = NrlaisInterfaceModel.ToJavaTicks(new DateTime(1980, 1, 1)),
                                name1 = request.farmer.Name,
                                name2 = "",
                                name3 = "",
                                notes = "investor from CAMIS",
                                orgatype = NrlaisInterfaceModel.Party.ORG_TYPE_OTHER,
                                sex = 1,
                            },
                        }
                    }),
                    transfers = new List<NrlaisInterfaceModel.NrlaisParcelTransfer>(
                        new NrlaisInterfaceModel.NrlaisParcelTransfer[]
                        {
                            new NrlaisInterfaceModel.NrlaisParcelTransfer()
                            {
                                parcelUID = parcel.parcelUID,
                                area = parcel.areaGeom,
                                share = new NrlaisInterfaceModel.NrlaisRationalNum() {num = 1, denum = 1},
                            }
                        }
                    ),
                    rentContractDoc = new NrlaisInterfaceModel.NrlaisSourceDocument()
                    {
                        archiveType = NrlaisInterfaceModel.NrlaisSourceDocument.ARCHIVE_TYPE_PAPER,
                        refText = "Unknown",
                        description = "Sent from CAMIS",
                        sourceType = NrlaisInterfaceModel.NrlaisSourceDocument.DOC_TYPE_RENT_AGREEMENT,
                    },
                    landHoldingCertificateDoc = new NrlaisInterfaceModel.NrlaisSourceDocument()
                    {
                        archiveType = NrlaisInterfaceModel.NrlaisSourceDocument.ARCHIVE_TYPE_PAPER,
                        refText = "Unknown",
                        description = "Sent from CAMIS",
                        sourceType = NrlaisInterfaceModel.NrlaisSourceDocument.DOC_TYPE_LAND_HOLDING_CERTIFICATE,
                    },
                };

            var tran = new NrlaisInterfaceModel.NrlaisTransaction()
            {
                transactionType = NrlaisInterfaceModel.NrlaisTransaction.TRAN_TYPE_RENT,
                data = rentTransaction,
                notes = "Rent/Lease transaction transfered from CAMIS",
                time = NrlaisInterfaceModel.ToJavaTicks(DateTime.Now),
                year = 2011,
                nrlais_kebeleid = parcel.nrlais_kebeleid,
            };
            var res = invokeServer<NrlaisInterfaceModel.NrlaisTransaction, StringRes>("api/register_transaction", tran);
            if (!String.IsNullOrEmpty(res.error))
                throw new InvalidOperationException($"Nrlais rejected rent transaciton registration\n{res.error}");
            return Guid.Parse(res.res);
        }
    }
}