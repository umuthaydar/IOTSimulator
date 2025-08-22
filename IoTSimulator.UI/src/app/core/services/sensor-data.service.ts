import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SensorData, SensorDataAggregate } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SensorDataService {
  private readonly apiUrl = `${environment.apiUrl}/sensordata`;

  constructor(private http: HttpClient) { }

  getSensorData(page: number = 1, pageSize: number = 20): Observable<SensorData[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<SensorData[]>(this.apiUrl, { params });
  }

  getSensorDataById(id: string): Observable<SensorData> {
    return this.http.get<SensorData>(`${this.apiUrl}/${id}`);
  }

  getSensorDataByDeviceId(deviceId: string, page: number = 1, pageSize: number = 20): Observable<SensorData[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<SensorData[]>(`${this.apiUrl}/device/${deviceId}`, { params });
  }

  getSensorDataByDateRange(startDate: Date, endDate: Date, page: number = 1, pageSize: number = 20): Observable<SensorData[]> {
    const params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString())
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<SensorData[]>(`${this.apiUrl}/date-range`, { params });
  }

  getLatestSensorData(deviceId: string): Observable<SensorData> {
    return this.http.get<SensorData>(`${this.apiUrl}/device/${deviceId}/latest`);
  }

  getAggregatedData(deviceId: string, hours: number = 24): Observable<SensorDataAggregate> {
    const params = new HttpParams()
      .set('deviceId', deviceId)
      .set('hours', hours.toString());
    return this.http.get<SensorDataAggregate>(`${this.apiUrl}/aggregated`, { params });
  }

  deleteSensorData(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  deleteOldSensorData(days: number): Observable<number> {
    const params = new HttpParams().set('days', days.toString());
    return this.http.delete<number>(`${this.apiUrl}/cleanup`, { params });
  }
}
