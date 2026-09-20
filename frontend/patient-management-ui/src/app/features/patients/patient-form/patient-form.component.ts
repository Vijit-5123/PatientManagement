import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './patient-form.component.html',
  styleUrl: './patient-form.component.scss'
})
export class PatientFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly form = this.fb.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    dateOfBirth: ['', Validators.required],
    gender: ['', Validators.required],
    phone: [''],
    email: ['', Validators.email],
    address: [''],
    city: [''],
    state: [''],
    postalCode: ['']
  });

  readonly isEditMode = signal(false);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly errorMessage = signal<string | null>(null);

  private patientId: number | null = null;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.patientId = Number(idParam);
      this.isEditMode.set(true);
      this.loadPatient(this.patientId);
    }
  }

  private loadPatient(id: number): void {
    this.isLoading.set(true);
    this.patientService.getById(id).subscribe({
      next: (patient) => {
        this.form.patchValue({
          firstName: patient.firstName,
          lastName: patient.lastName,
          dateOfBirth: patient.dateOfBirth,
          gender: patient.gender,
          phone: patient.phone ?? '',
          email: patient.email ?? '',
          address: patient.address ?? '',
          city: patient.city ?? '',
          state: patient.state ?? '',
          postalCode: patient.postalCode ?? ''
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Unable to load patient details.');
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const request = {
      firstName: raw.firstName!,
      lastName: raw.lastName!,
      dateOfBirth: raw.dateOfBirth!,
      gender: raw.gender!,
      phone: raw.phone || null,
      email: raw.email || null,
      address: raw.address || null,
      city: raw.city || null,
      state: raw.state || null,
      postalCode: raw.postalCode || null
    };

    const request$ = this.isEditMode() && this.patientId
      ? this.patientService.update(this.patientId, request)
      : this.patientService.create(request);

    request$.subscribe({
      next: (patient) => {
        this.isSaving.set(false);
        this.router.navigate(['/patients', patient.id]);
      },
      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);
        if (error.status === 400 && error.error?.errors) {
          const firstError = Object.values(error.error.errors)[0] as string[];
          this.errorMessage.set(firstError?.[0] ?? 'Please review the form for errors.');
        } else if (error.status === 403) {
          this.errorMessage.set('You do not have permission to perform this action.');
        } else if (error.status === 404) {
          this.errorMessage.set('Patient not found.');
        } else {
          this.errorMessage.set('Unable to save patient. Please try again.');
        }
      }
    });
  }

  cancel(): void {
    this.router.navigate(this.isEditMode() && this.patientId ? ['/patients', this.patientId] : ['/patients']);
  }
}
