import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RouterLink } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';
import { PermissionsService } from '../../../core/services/permissions.service';
import { Patient } from '../../../core/models/patient.models';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatDialogModule
  ],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss'
})
export class PatientListComponent implements OnInit {
  private readonly patientService = inject(PatientService);
  private readonly permissionsService = inject(PermissionsService);
  private readonly dialog = inject(MatDialog);

  readonly displayedColumns = ['medicalRecordNumber', 'name', 'dateOfBirth', 'gender', 'phone', 'email', 'actions'];

  readonly patients = signal<Patient[]>([]);
  readonly totalCount = signal(0);
  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly searchQuery = signal('');

  readonly canCreate = this.permissionsService.canCreatePatient;
  readonly canDelete = this.permissionsService.canDeletePatient;

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.patientService.search(this.searchQuery() || null, this.page(), this.pageSize()).subscribe({
      next: (result) => {
        this.patients.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Unable to load patients. Please try again.');
      }
    });
  }

  onSearchSubmit(): void {
    this.page.set(1);
    this.loadPatients();
  }

  onPageChange(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadPatients();
  }

  confirmDelete(patient: Patient): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete patient',
        message: `Are you sure you want to delete ${patient.firstName} ${patient.lastName} (${patient.medicalRecordNumber})? This cannot be undone.`
      }
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.deletePatient(patient);
      }
    });
  }

  private deletePatient(patient: Patient): void {
    this.patientService.delete(patient.id).subscribe({
      next: () => this.loadPatients(),
      error: () => this.errorMessage.set('Unable to delete patient. Please try again.')
    });
  }
}
