// session.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AdminServices } from './../../_services/admin.Services';

@Injectable()
export class SessionInterceptor implements HttpInterceptor {

  constructor(private adminService: AdminServices) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Skip session check for login, checksession, and public endpoints
    if (req.url.includes('/admin/login') ||
      req.url.includes('/admin/checksession') ||
      req.url.includes('/login')) {
      return next.handle(req);
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 || error.status === 403) {
          // Session expired or unauthorized
          this.adminService.sessionExpired.next(true);
        }
        return throwError(error);
      })
    );
  }
}
