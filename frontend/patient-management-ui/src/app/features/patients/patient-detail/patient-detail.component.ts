import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Patient } from '../../../core/models/patient.models';
import { PatientService } from '../../../core/services/patient.service';
import { PermissionsService } from '../../../core/services/permissions.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, MatButtonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule, MatDialogModule],
  templateUrl: './patient-detail.component.html',
  styleUrl: './patient-detail.component.scss'
})
export class PatientDetailComponent implements OnInit {
  private readonly patientService = inject(PatientService);
  private readonly permissionsService = inject(PermissionsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly patient = signal<Patient | null>(null);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly notFound = signal(false);

  readonly canUpdate = this.permissionsService.canUpdatePatient;
  readonly canDelete = this.permissionsService.canDeletePatient;

  private patientId!: number;

  ngOnInit(): void {
    this.patientId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadPatient();
  }

  loadPatient(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.notFound.set(false);

    this.patientService.getById(this.patientId).subscribe({
      next: (patient) => {
        this.patient.set(patient);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.isLoading.set(false);
        if (error.status === 404) {
          this.notFound.set(true);
        } else {
          this.errorMessage.set('Unable to load patient details.');
        }
      }
    });
  }

  confirmDelete(): void {
    const patient = this.patient();
    if (!patient) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete patient',
        message: `Are you sure you want to delete ${patient.firstName} ${patient.lastName} (${patient.medicalRecordNumber})? This cannot be undone.`
      }
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.patientService.delete(patient.id).subscribe({
          next: () => this.router.navigate(['/patients']),
          error: () => this.errorMessage.set('Unable to delete patient. Please try again.')
        });
      }
    });
  }
}
