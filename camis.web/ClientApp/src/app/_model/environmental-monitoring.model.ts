export interface ChangeDetectionRequest {
  parcelUpid?: string;
  startDate: Date;
  endDate: Date;
  changeThreshold?: number;
  changeTypes?: string[];
  region?: string;
}

export interface EnvironmentalAnalysisResult {
  changes: EnvironmentalChangeEventDto[];
  changeSummary: { [key: string]: number };
  totalAffectedArea: number;
  averageChanges: { [key: string]: number };
  changeMapUrl?: string;
}

export interface EnvironmentalChangeEventDto {
  parcelUpid: string;
  eventDate: Date;
  eventType: string;
  eventSubtype: string;
  beforeValue: number;
  afterValue: number;
  changeAmount: number;
  changePercentage: number;
  affectedArea: number;
  severity: string;
  confidence: number;
  geometry: string;
  centroid: string;
  description: string;
}

export interface SpectralIndicesRequest {
  date: Date;
  parcelUpid: string;
}

export interface ParcelEnvironmentalMonitoring {
  parcelUpid: string;
  monitoringDate: Date;
  ndvi?: number;
  ndwi?: number;
  ndbi?: number;
  evi?: number;
  mndwi?: number;
  vegetationHealth?: number;
  waterPresence?: number;
  soilMoisture?: number;
  areaSqkm: number;
  cloudCover: number;
}
