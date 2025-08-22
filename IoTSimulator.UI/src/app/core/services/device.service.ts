import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IoTDevice, CreateDeviceDto, UpdateDeviceDto } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private readonly apiUrl = `${environment.apiUrl}/devices`;

  constructor(private http: HttpClient) { }

  getDevices(): Observable<IoTDevice[]> {
    return this.http.get<IoTDevice[]>(this.apiUrl);
  }

  getDevice(id: string): Observable<IoTDevice> {
    return this.http.get<IoTDevice>(`${this.apiUrl}/${id}`);
  }

  getDevicesByRoomId(roomId: string): Observable<IoTDevice[]> {
    return this.http.get<IoTDevice[]>(`${this.apiUrl}/room/${roomId}`);
  }

  getDeviceWithSensorData(id: string): Observable<IoTDevice> {
    return this.http.get<IoTDevice>(`${this.apiUrl}/${id}/with-sensor-data`);
  }

  getActiveDevices(): Observable<IoTDevice[]> {
    return this.http.get<IoTDevice[]>(`${this.apiUrl}/active`);
  }

  createDevice(device: CreateDeviceDto): Observable<IoTDevice> {
    return this.http.post<IoTDevice>(this.apiUrl, device);
  }

  updateDevice(id: string, device: UpdateDeviceDto): Observable<IoTDevice> {
    return this.http.put<IoTDevice>(`${this.apiUrl}/${id}`, device);
  }

  deleteDevice(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  deviceExists(id: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/${id}/exists`);
  }
}
