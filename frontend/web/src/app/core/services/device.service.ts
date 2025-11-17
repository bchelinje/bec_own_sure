import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { Device, RegisterDeviceRequest, DeviceResponse } from '../models/device.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private apiUrl = `${environment.apiUrl}/devices`;

  constructor(private http: HttpClient) {}

  registerDevice(request: RegisterDeviceRequest): Observable<DeviceResponse> {
    return this.http.post<DeviceResponse>(this.apiUrl, request);
  }

  getUserDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(this.apiUrl);
  }

  getDevice(id: string): Observable<Device> {
    return this.http.get<Device>(`${this.apiUrl}/${id}`);
  }

  checkSerialNumber(serialNumber: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/check/${serialNumber}`);
  }

  deleteDevice(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  uploadPhoto(deviceId: string, file: File, caption?: string, isPrimary: boolean = false): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    if (caption) {
      formData.append('caption', caption);
    }
    formData.append('isPrimary', String(isPrimary));

    return this.http.post(`${this.apiUrl}/${deviceId}/photos`, formData);
  }

  uploadDocument(deviceId: string, file: File, documentType: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('documentType', documentType);

    return this.http.post(`${this.apiUrl}/${deviceId}/documents`, formData);
  }
}
