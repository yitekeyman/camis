using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace intapscamis.camis.domain.LandBank
{

    public class NrlaisInterfaceModel
    {

        public const int RIGHT_PUR = 1;
        public const int RIGHT_LEASE = 2;
        public const int RIGHT_RENT = 3;
        public const int RIGHT_SUR = 4;
        public const int RIGHT_SHARED_CROPPING = 5;

        public static long ToJavaTicks(DateTime dateTime)
        {
            var ts = dateTime.Subtract(new DateTime(1970, 1, 1));
            return (long)ts.TotalMilliseconds;
        }
        public static DateTime FromJavaTicks(long ticks)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(ticks);
        }
        public class NrlaisTransaction
        {
            public const int TRAN_STATUS_INITIATED = 1;
            public const int TRAN_STATUS_REGISTERD = 2;
            public const int TRAN_STATUS_ACCEPT_REQUESTED = 3;
            public const int TRAN_STATUS_ACCEPTED = 4;
            public const int TRAN_STATUS_FINISHED = 5;
            public const int TRAN_STATUS_CANCEL_REQUESTED = 6;
            public const int TRAN_STATUS_CANCELED = 7;
            public const int TRAN_STATUS_ERROR = 99;

            public const int TRAN_TYPE_RENT = 9;
            public const int TRAN_TYPE_SPLIT = 12;
            public int transactionType { get; set; }
            public int year { get; set; }
            public long time { get; set; }
            public String notes { get; set; } = "";
            public Object data { get; set; }
            public int status { get; set; }
            public String nrlais_kebeleid { get; set; }
        }
        public class NrlaisSourceDocument
        {
            public const int DOC_TYPE_LAND_HOLDING_CERTIFICATE = 16;
            public const int DOC_TYPE_PASSPORT = 31;
            public const int DOC_TYPE_KEBELE_ID = 32;
            public const int DOC_TYPE_RENT_AGREEMENT = 33;

            public const int ARCHIVE_TYPE_PAPER = 2;
            public int sourceType = 16;
            public String refText { get; set; }
            public int archiveType { get; set; } = 2;
            public String description { get; set; }

        }
        public class NrlaisApplicant
        {
            public String selfPartyUID { get; set; } = null;
            public NrlaisSourceDocument idDocument { get; set; } =
                new NrlaisSourceDocument()
                {
                    archiveType=NrlaisSourceDocument.ARCHIVE_TYPE_PAPER,
                    refText="Unknown",
                    sourceType=NrlaisSourceDocument.DOC_TYPE_KEBELE_ID,
                };
        }
        public class NrlaisSplitTransaction
        {
            public List<NrlaisApplicant> applicants { get; set; } = new List<NrlaisApplicant>();
            public String holdingUID { get; set; }
            public String splitSingleParcel { get; set; }
            public List<String> geoms { get; set; } = null;
            public NrlaisSourceDocument landHoldingCertifcateDocument { get; set; } = null;
        }
        public class NrlaisRationalNum
        {
            public int num { get; set; } = 0;
            public int denum { get; set; } = 1;
        }
        public class NrlaisParcelTransfer
        {
            public String parcelUID;
            public double area = 0;
            public NrlaisRationalNum share = null;
            public bool splitParcel = false;
            public String rightUID = null;

        }
        public class NrlaisPartyItem
        {
            public String existingPartyUID { get; set; } = null;
            public Party party { get; set; } = null;
            public NrlaisRationalNum share { get; set; } = null;
            public NrlaisSourceDocument idDoc { get; set; } = null;
        }
        public class NrlaisRentTransaction
        {
            public String holdingUID { get; set; } = null;
            public List<NrlaisApplicant> applicants { get; set; } = new List<NrlaisApplicant>();
            public List<NrlaisParcelTransfer> transfers { get; set; } = new List<NrlaisParcelTransfer>();
            public List<NrlaisPartyItem> tenants = new List<NrlaisPartyItem>();
            public NrlaisSourceDocument rentContractDoc = null;
            public NrlaisSourceDocument landHoldingCertificateDoc = null;
            public long contractTime { get; set; }
            public long leaseFrom { get; set; }
            public long leaseTo { get; set; }
            public bool isRent { get; set; }
            public double amount { get; set; }
        }
        public class Share
        {
            public int num { get; set; }
            public int denum { get; set; }
        }

        public class Party
        {
            public const int PARTY_TYPE_STATE = 3;
            public const int PARTY_TYPE_PERSON = 1;
            public const int PARTY_TYPE_NON_NATURAL_PERSON = 2;

            public const int ORG_TYPE_OTHER = 6;    

            public string partyUID { get; set; }
            public string notes { get; set; }
            public int partyType { get; set; }
            public string name1 { get; set; }
            public string name2 { get; set; }
            public string name3 { get; set; }
            public int sex { get; set; }
            public long dateOfBirth { get; set; }
            public String idText { get; set; }
            public int idType { get; set; }
            public int orgatype { get; set; }
            public string csaregionid { get; set; }
            public string nrlais_zoneid { get; set; }
            public string nrlais_woredaid { get; set; }
            public string nrlais_kebeleid { get; set; }
        }

        public class Right
        {
            public string rightUID { get; set; }
            public object notes { get; set; }
            public int rightType { get; set; }
            public Share share { get; set; }
            public long startLeaseDate { get; set; }
            public long endLeaseDate { get; set; }
            public double leaseAmount { get; set; }
            public object leaseRef { get; set; }
            public long startRentDate { get; set; }
            public long endRentDate { get; set; }
            public double rentSize { get; set; }
            public long startSharedCroppingDate { get; set; }
            public long endCroppingDate { get; set; }
            public string holdingUID { get; set; }
            public string partyUID { get; set; }
            public string parcelUID { get; set; }
            public Party party { get; set; }
            public string editStatus { get; set; }
            public string csaregionid { get; set; }
            public string nrlais_zoneid { get; set; }
            public string nrlais_woredaid { get; set; }
            public string nrlais_kebeleid { get; set; }
        }

        public class Parcel
        {
            public string parcelUID { get; set; }
            public int seqNo { get; set; }
            public string upid { get; set; }
            public double areaGeom { get; set; }
            public double areaLegal { get; set; }
            public double areaSurveyed { get; set; }
            public string adjudicationID { get; set; }

            public String GetHolding()
            {
                foreach (var r in this.rights)
                {
                    return r.holdingUID;
                }
                return null;
            }

            public Party GetHolder()
            {
                foreach (var r in this.rights)
                {
                    if (r.rightType == RIGHT_PUR || r.rightType == RIGHT_SUR)
                        return r.party;
                }
                return null;
            }

            public int adjudicatedBy { get; set; }
            public string geometry { get; set; }
            public string referencePoint { get; set; }
            public int landUse { get; set; }
            public int level { get; set; }
            public string notes { get; set; }
            public bool isPrimary { get; set; }
            public int mreg_teamid { get; set; }
            public object mreg_mapsheetNo { get; set; }

            public List<Right> GetHolders()
            {
                return this.rights.Where(x => x.rightType == RIGHT_PUR || x.rightType == RIGHT_SUR).ToList();
            }

            public int mreg_stage { get; set; }
            public int mreg_actype { get; set; }
            public int mreg_acyear { get; set; }
            public int soilfertilityType { get; set; }
            public long mreg_surveyDate { get; set; }
            public List<Right> rights { get; set; }
            public string csaregionid { get; set; }
            public string nrlais_zoneid { get; set; }
            public string nrlais_woredaid { get; set; }
            public string nrlais_kebeleid { get; set; }

            public bool IsStateLand()
            {
                if (rights.Count == 0)
                    return false;
                foreach (var r in rights)
                {
                    if (r.rightType != RIGHT_SUR)
                        return false;
                }
                return true;
            }
        }

        public class Holding
        {
            public string holdingUID { get; set; }
            public string uhid { get; set; }
            public int holdingSeqNo { get; set; }
            public int holdingType { get; set; }
            public string notes { get; set; }
            public List<Parcel> parcels { get; set; }
            public string csaregionid { get; set; }
            public string nrlais_zoneid { get; set; }
            public string nrlais_woredaid { get; set; }
            public string nrlais_kebeleid { get; set; }
        }

        public class NrlaisLandResult
        {
            public String error { get; set; }
            public Holding res { get; set; }
        }
        public class NrlaisParcelResult
        {
            public String error { get; set; }
            public Parcel res { get; set; }
        }
        public class NrlaisRestRes<T>
        {
            public String error { get; set; }
            public T res { get; set; }
            public NrlaisRestRes()
            {

            }
            public NrlaisRestRes(T res)
            {
                this.res = res;
            }
        }
        public enum NrlaisApplicationStatus
        {
            Processing,
            Canceled,
            Completd,
        }
    }
}
