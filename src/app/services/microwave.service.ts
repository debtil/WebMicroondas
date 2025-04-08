import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface HeatingRequest {
  timeSeconds: number;
  power?: number;
}

@Injectable({
  providedIn: 'root'
})
export class MicrowaveService {

  private apiUrl = 'https://localhost:5001/api/MicrowaveApi'; // Ajuste a URL conforme necessário

  constructor(private http: HttpClient) {}

  startHeating(request: HeatingRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/start`, request);
  }

  pauseOrCancel(): Observable<any> {
    return this.http.post(`${this.apiUrl}/pause`, {});
  }

  resumeHeating(): Observable<any> {
    return this.http.post(`${this.apiUrl}/resume`, {});
  }

}
