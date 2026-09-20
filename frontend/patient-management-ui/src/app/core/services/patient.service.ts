import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreatePatientRequest, PagedResult, Patient, UpdatePatientRequest } from '../models/patient.models';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly baseUrl = `${environment.apiUrl}/patients`;

  constructor(private readonly http: HttpClient) {}

  search(query: string | null, page: number, pageSize: number): Observable<PagedResult<Patient>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (query) {
      params = params.set('query', query);
    }
    return this.http.get<PagedResult<Patient>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.baseUrl}/${id}`);
  }

  create(request: CreatePatientRequest): Observable<Patient> {
    return this.http.post<Patient>(this.baseUrl, request);
  }

  update(id: number, request: UpdatePatientRequest): Observable<Patient> {
    return this.http.put<Patient>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
