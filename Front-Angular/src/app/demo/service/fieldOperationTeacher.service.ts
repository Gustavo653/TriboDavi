import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { StorageService } from './storage.service';

@Injectable({
    providedIn: 'root',
})
export class FieldOperationTeacherService {
    constructor(private http: HttpClient, private storageService: StorageService) {}

    private getAPIURL(): Observable<string> {
        return this.storageService.getAPIURL();
    }

    getFieldOperationTeachers(): Observable<any> {
        return this.getAPIURL().pipe(
            switchMap((url) => {
                const apiUrl = `${url}/fieldOperationTeacher`;
                return this.http.get(apiUrl);
            })
        );
    }

    createFieldOperationTeacher(device: any): Observable<any> {
        return this.getAPIURL().pipe(
            switchMap((url) => {
                const apiUrl = `${url}/fieldOperationTeacher`;
                return this.http.post(apiUrl, device);
            })
        );
    }

    updateFieldOperationTeacher(id: string, device: any): Observable<any> {
        return this.getAPIURL().pipe(
            switchMap((url) => {
                const apiUrl = `${url}/fieldOperationTeacher/${id}`;
                return this.http.put(apiUrl, device);
            })
        );
    }

    deleteFieldOperationTeacher(id: string): Observable<any> {
        return this.getAPIURL().pipe(
            switchMap((url) => {
                const apiUrl = `${url}/fieldOperationTeacher/${id}`;
                return this.http.delete(apiUrl);
            })
        );
    }
}
