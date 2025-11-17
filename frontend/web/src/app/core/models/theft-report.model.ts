export enum ReportType {
  Stolen = 'Stolen',
  Lost = 'Lost',
  Found = 'Found'
}

export interface TheftReport {
  id: string;
  deviceId: string;
  userId: string;
  reportType: ReportType;
  incidentDate: Date;
  location?: string;
  policeReportNumber?: string;
  description: string;
  status: string;
  reportedAt: Date;
  resolvedAt?: Date;
}

export interface CreateTheftReportRequest {
  deviceId: string;
  reportType: ReportType;
  incidentDate: Date;
  location?: string;
  policeReportNumber?: string;
  description: string;
}
