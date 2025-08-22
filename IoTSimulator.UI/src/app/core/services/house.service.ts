import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { House, CreateHouseDto, UpdateHouseDto } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HouseService {
  private readonly apiUrl = `${environment.apiUrl}/houses`;

  constructor(private http: HttpClient) { }

  getHouses(): Observable<House[]> {
    return this.http.get<House[]>(this.apiUrl);
  }

  getHouse(id: string): Observable<House> {
    return this.http.get<House>(`${this.apiUrl}/${id}`);
  }

  getHouseWithRooms(id: string): Observable<House> {
    return this.http.get<House>(`${this.apiUrl}/${id}/with-rooms`);
  }

  createHouse(house: CreateHouseDto): Observable<House> {
    return this.http.post<House>(this.apiUrl, house);
  }

  updateHouse(id: string, house: UpdateHouseDto): Observable<House> {
    return this.http.put<House>(`${this.apiUrl}/${id}`, house);
  }

  deleteHouse(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  houseExists(id: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/${id}/exists`);
  }
}
