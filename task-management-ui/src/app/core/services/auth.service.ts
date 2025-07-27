import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, map } from 'rxjs';
import { environment } from '../../../environment/environment';
import { JwtHelperService } from '@auth0/angular-jwt'; // npm install @auth0/angular-jwt
import { LoginRequest, LoginResponse, RegisterRequest } from '../../shared/models/auth.models';
import { User } from '../../shared/models/user.models'

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/Auth';
  private jwtHelper: JwtHelperService = new JwtHelperService();

  // BehaviorSubject to hold the current user information
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient) {
    // Initialize currentUserSubject from localStorage on app startup
    const storedToken = localStorage.getItem('jwt_token');
    const user = storedToken ? this.decodeToken(storedToken) : null;
    this.currentUserSubject = new BehaviorSubject<User | null>(user);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  register(model: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, model);
  }

  login(model: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, model).pipe(
      map(response => {
        if (response && response.token) {
          localStorage.setItem('jwt_token', response.token);
          const user = this.decodeToken(response.token);
          this.currentUserSubject.next(user);
        }
        return response;
      })
    );
  }

  logout(): void {
    localStorage.removeItem('jwt_token');
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('jwt_token');
    return token != null && !this.jwtHelper.isTokenExpired(token);
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  private decodeToken(token: string): User | null {
    try {
      const decodedToken = this.jwtHelper.decodeToken(token);
      // Extract user info and roles from the decoded token
      // Claims typically include 'sub' (userId), 'email', 'name', and 'role'
      const userId = decodedToken.sub || decodedToken.nameid;
      const email = decodedToken.email;
      const firstName = decodedToken.name; 
      const lastName = decodedToken.family_name; 
      const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
      const roles = decodedToken[roleClaim] ? (Array.isArray(decodedToken[roleClaim]) ? decodedToken[roleClaim] : [decodedToken[roleClaim]]) : [];

      return {
        id: userId,
        email: email,
        firstName: firstName,
        lastName: lastName,
        roles: roles
      };
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  hasRole(requiredRoles: string[]): boolean {
    const user = this.currentUserSubject.value;
    if (!user || !user.roles) {
      return false;
    }
    return requiredRoles.some(role => user.roles.includes(role));
  }
}