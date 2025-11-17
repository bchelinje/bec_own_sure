export enum DeviceStatus {
  Active = 'Active',
  Stolen = 'Stolen',
  Recovered = 'Recovered',
  ForSale = 'ForSale',
  Inactive = 'Inactive'
}

export interface Device {
  id: string;
  serialNumber: string;
  category: string;
  brand?: string;
  model?: string;
  description?: string;
  status: DeviceStatus;
  userId: string;
  verificationCode: string;
  isVerified: boolean;
  registeredAt: Date;
  lastUpdatedAt: Date;
  photos?: DevicePhoto[];
  documents?: DeviceDocument[];
}

export interface DevicePhoto {
  id: string;
  deviceId: string;
  photoUrl: string;
  caption?: string;
  isPrimary: boolean;
  uploadedAt: Date;
}

export interface DeviceDocument {
  id: string;
  deviceId: string;
  documentType: string;
  documentUrl: string;
  fileName: string;
  uploadedAt: Date;
}

export interface RegisterDeviceRequest {
  serialNumber: string;
  category: string;
  brand?: string;
  model?: string;
  description?: string;
  purchaseDate?: Date;
  purchasePrice?: number;
  retailer?: string;
}

export interface DeviceResponse {
  id: string;
  serialNumber: string;
  category: string;
  brand?: string;
  model?: string;
  description?: string;
  status: string;
  verificationCode: string;
  isVerified: boolean;
  registeredAt: Date;
}
