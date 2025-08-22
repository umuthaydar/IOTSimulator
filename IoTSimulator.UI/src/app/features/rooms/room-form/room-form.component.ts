import { Component, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RoomService } from '../../../core/services/room.service';
import { HouseService } from '../../../core/services/house.service';
import { Room, House, CreateRoomDto, UpdateRoomDto } from '../../../core/models';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-room-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './room-form.component.html',
  styleUrl: './room-form.component.scss'
})
export class RoomFormComponent implements OnInit {
  roomForm!: FormGroup;
  isEditing = false;
  isLoading = false;
  room?: Room;
  houses: House[] = [];

  constructor(
    private fb: FormBuilder,
    private roomService: RoomService,
    private houseService: HouseService,
    @Optional() public dialogRef: MatDialogRef<RoomFormComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { room?: Room, houseId?: string }
  ) {
    if (data?.room) {
      this.room = data.room;
      this.isEditing = true;
    }
  }

  ngOnInit(): void {
    this.loadHouses();
    this.initializeForm();
  }

  private loadHouses(): void {
    this.houseService.getHouses().subscribe({
      next: (houses) => {
        this.houses = houses;
      },
      error: (error) => {
        console.error('Error loading houses:', error);
        Swal.fire({
          title: 'Error!',
          text: 'Failed to load houses. Please try again.',
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

  private initializeForm(): void {
    this.roomForm = this.fb.group({
      name: [this.room?.name || '', [Validators.required, Validators.minLength(2)]],
      houseId: [this.room?.houseId || this.data?.houseId || '', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.roomForm.invalid) {
      this.roomForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.roomForm.value;

    if (this.isEditing && this.room) {
      const updateDto: UpdateRoomDto = {
        name: formValue.name,
        houseId: formValue.houseId
      };

      this.roomService.updateRoom(this.room.id, updateDto).subscribe({
        next: (updatedRoom) => {
          this.isLoading = false;
          Swal.fire({
            title: 'Success!',
            text: 'Room updated successfully!',
            icon: 'success',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
          });
          this.dialogRef?.close({ action: 'updated', room: updatedRoom });
        },
        error: (error) => {
          console.error('Error updating room:', error);
          this.isLoading = false;
          Swal.fire({
            title: 'Error!',
            text: 'Failed to update room. Please try again.',
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
      const createDto: CreateRoomDto = {
        name: formValue.name,
        houseId: formValue.houseId
      };

      this.roomService.createRoom(createDto).subscribe({
        next: (newRoom) => {
          this.isLoading = false;
          Swal.fire({
            title: 'Success!',
            text: 'Room created successfully!',
            icon: 'success',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
          });
          this.dialogRef?.close({ action: 'created', room: newRoom });
        },
        error: (error) => {
          console.error('Error creating room:', error);
          this.isLoading = false;
          Swal.fire({
            title: 'Error!',
            text: 'Failed to create room. Please try again.',
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
    const field = this.roomForm.get(fieldName);
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
