import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const requiredRoles = route.data['roles'] as string[];

    if (!requiredRoles || requiredRoles.length === 0) {
      return true; // No specific roles required, allow access
    }

    if (this.authService.isLoggedIn() && this.authService.hasRole(requiredRoles)) {
      return true; // User is logged in and has at least one required role
    } else {
      // User does not have the required role, redirect to dashboard or unauthorized page
      this.router.navigate(['/dashboard']); // Or a dedicated unauthorized page
      return false;
    }
  }
}