import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Prescription } from '../models/prescription.model';

@Injectable({ providedIn: 'root' })
export class PrescriptionService {
  private apiUrl = `${environment.apiUrl}/prescriptions`;

  constructor(private http: HttpClient) {}

  upload(file: File): Observable<Prescription> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Prescription>(`${this.apiUrl}/upload`, formData);
  }

  getMyPrescriptions(): Observable<Prescription[]> {
    return this.http.get<Prescription[]>(`${this.apiUrl}/my`);
  }

  getById(id: number): Observable<Prescription> {
    return this.http.get<Prescription>(`${this.apiUrl}/${id}`);
  }

  getAll(status?: string): Observable<Prescription[]> {
    const url = status ? `${this.apiUrl}?status=${status}` : this.apiUrl;
    return this.http.get<Prescription[]>(url);
  }

  review(id: number, status: string, reason?: string): Observable<Prescription> {
    return this.http.patch<Prescription>(`${this.apiUrl}/${id}/review`, { status, reason });
  }
}
