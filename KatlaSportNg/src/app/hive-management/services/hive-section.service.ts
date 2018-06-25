import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';
import { HiveSection } from '../models/hive-section';
import { HiveSectionListItem } from '../models/hive-section-list-item';
import { UpdateSectionRequest } from '../models/update-hive-section-request';

@Injectable({
  providedIn: 'root'
})
export class HiveSectionService {
  private url = environment.apiUrl + 'api/sections/';

  constructor(
    private http: HttpClient
  ) { }

  getHiveSections(): Observable<Array<HiveSectionListItem>> {
    return this.http.get<Array<HiveSectionListItem>>(this.url);
  }

  getHiveSection(hiveSectionId: number): Observable<HiveSection> {
    return this.http.get<HiveSection>(`${this.url}${hiveSectionId}`);
  }

  setHiveSectionStatus(hiveSectionId: number, deletedStatus: boolean): Observable<number> {
    return this.http.put<number>(`${this.url}${hiveSectionId}/status/${deletedStatus}`, {});
  }

  addHiveSection(hiveId: number, updateSectionRequest: UpdateSectionRequest): Observable<Object> {
    return this.http.post(`${this.url}add/${hiveId}`, updateSectionRequest);
  }

  updateHiveSection(sectionId: number, updateSectionRequest: UpdateSectionRequest): Observable<Object> {
    return this.http.put(`${this.url}update/${sectionId}`, updateSectionRequest);
  }

  deleteHiveSection(sectionId: number): Observable<Object> {
    return this.http.delete(`${this.url}${sectionId}`);
  }
}
