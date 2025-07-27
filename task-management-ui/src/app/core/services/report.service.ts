import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';
import { ProjectPerformanceReport, BatchUpdateTaskStatus } from '../../shared/models/report.models';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = environment.apiUrl + '/Reports';

  constructor(private http: HttpClient) { }

  getProjectPerformanceReport(): Observable<ProjectPerformanceReport[]> {
    return this.http.get<ProjectPerformanceReport[]>(`${this.apiUrl}/project-performance`);
  }

  batchUpdateTaskStatus(model: BatchUpdateTaskStatus): Observable<any> {
    return this.http.post(`${this.apiUrl}/batch-update-task-status`, model);
  }
}