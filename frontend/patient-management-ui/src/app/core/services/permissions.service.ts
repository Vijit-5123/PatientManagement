import { Injectable, computed } from '@angular/core';
import { AuthService } from './auth.service';

/**
 * Centralizes UI-level permission checks so authorization logic isn't scattered
 * across components. This mirrors (but does not replace) the server-side
 * authorization policies enforced by the API.
 */
@Injectable({ providedIn: 'root' })
export class PermissionsService {
  constructor(private readonly authService: AuthService) {}

  readonly canCreatePatient = computed(() => this.hasAnyRole(['Admin', 'Doctor']));
  readonly canUpdatePatient = computed(() => this.hasAnyRole(['Admin', 'Doctor']));
  readonly canDeletePatient = computed(() => this.hasAnyRole(['Admin']));

  private hasAnyRole(roles: string[]): boolean {
    const user = this.authService.currentUser();
    return !!user && roles.includes(user.role);
  }
}
