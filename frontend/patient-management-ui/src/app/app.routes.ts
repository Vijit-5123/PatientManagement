import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/login/login.component';
import { PatientDetailComponent } from './features/patients/patient-detail/patient-detail.component';
import { PatientFormComponent } from './features/patients/patient-form/patient-form.component';
import { PatientListComponent } from './features/patients/patient-list/patient-list.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'patients' },
  { path: 'login', component: LoginComponent },
  { path: 'patients', component: PatientListComponent, canActivate: [authGuard] },
  { path: 'patients/new', component: PatientFormComponent, canActivate: [authGuard] },
  { path: 'patients/:id', component: PatientDetailComponent, canActivate: [authGuard] },
  { path: 'patients/:id/edit', component: PatientFormComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'patients' }
];
