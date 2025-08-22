import { Component, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HouseService } from '../../../core/services/house.service';
import { House, CreateHouseDto, UpdateHouseDto } from '../../../core/models';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-house-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './house-form.component.html',
  styleUrl: './house-form.component.scss'
})
export class HouseFormComponent implements OnInit {
  houseForm!: FormGroup;
  isEditing = false;
  isLoading = false;
  house?: House;

  constructor(
    private fb: FormBuilder,
    private houseService: HouseService,
    @Optional() public dialogRef: MatDialogRef<HouseFormComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { house?: House }
  ) {
    if (data?.house) {
      this.house = data.house;
      this.isEditing = true;
    }
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.houseForm = this.fb.group({
      name: [this.house?.name || '', [Validators.required, Validators.minLength(2)]],
      address: [this.house?.address || '', [Validators.required, Validators.minLength(5)]]
    });
  }

  onSubmit(): void {
    if (this.houseForm.invalid) {
      this.houseForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.houseForm.value;

    if (this.isEditing && this.house) {
      const updateDto: UpdateHouseDto = {
        name: formValue.name,
        address: formValue.address
      };

      this.houseService.updateHouse(this.house.id, updateDto).subscribe({
        next: (updatedHouse) => {
          this.isLoading = false;
          Swal.fire({
            title: 'Success!',
            text: 'House updated successfully!',
            icon: 'success',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
          });
          this.dialogRef?.close({ action: 'updated', house: updatedHouse });
        },
        error: (error) => {
          console.error('Error updating house:', error);
          this.isLoading = false;
          Swal.fire({
            title: 'Error!',
            text: 'Failed to update house. Please try again.',
            icon: 'error',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true
          });
        }
      });
    } else {
      const createDto: CreateHouseDto = {
        name: formValue.name,
        address: formValue.address
      };

      this.houseService.createHouse(createDto).subscribe({
        next: (newHouse) => {
          this.isLoading = false;
          Swal.fire({
            title: 'Success!',
            text: 'House created successfully!',
            icon: 'success',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
          });
          this.dialogRef?.close({ action: 'created', house: newHouse });
        },
        error: (error) => {
          console.error('Error creating house:', error);
          this.isLoading = false;
          Swal.fire({
            title: 'Error!',
            text: 'Failed to create house. Please try again.',
            icon: 'error',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true
          });
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef?.close();
  }

  getErrorMessage(fieldName: string): string {
    const field = this.houseForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName} is required`;
    }
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `${fieldName} must be at least ${minLength} characters`;
    }
    return '';
  }
}
