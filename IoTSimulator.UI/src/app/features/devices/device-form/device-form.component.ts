import { Component, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DeviceService } from '../../../core/services/device.service';
import { RoomService } from '../../../core/services/room.service';
import { IoTDevice, Room, DeviceType, CreateDeviceDto, UpdateDeviceDto } from '../../../core/models';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-device-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './device-form.component.html',
  styleUrl: './device-form.component.scss'
})
export class DeviceFormComponent implements OnInit {
  deviceForm!: FormGroup;
  isEditing = false;
  isLoading = false;
  device?: IoTDevice;
  rooms: Room[] = [];
  deviceTypes = Object.values(DeviceType).filter(value => typeof value === 'number') as DeviceType[];

  // Helper method to get device type display name
  getDeviceTypeName(deviceType: DeviceType): string {
    const name = DeviceType[Number(deviceType)];
    // Convert from PascalCase to readable format
    return name ? name.replace(/([A-Z])/g, ' $1').trim() : 'Unknown';
  }

  constructor(
    private fb: FormBuilder,
    private deviceService: DeviceService,
    private roomService: RoomService,
    @Optional() public dialogRef: MatDialogRef<DeviceFormComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { device?: IoTDevice, roomId?: string }
  ) {
    if (data?.device) {
      this.device = data.device;
      this.isEditing = true;
    }
  }

  ngOnInit(): void {
    this.loadRooms();
    this.initializeForm();
  }

  private loadRooms(): void {
    this.roomService.getRooms().subscribe({
      next: (rooms) => {
        this.rooms = rooms;
      },
      error: (error) => {
        console.error('Error loading rooms:', error);
        Swal.fire({
          title: 'Error!',
          text: 'Failed to load rooms. Please try again.',
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
    this.deviceForm = this.fb.group({
      name: [this.device?.name || '', [Validators.required, Validators.minLength(2)]],
      deviceType: [this.device?.deviceType || DeviceType.TemperatureSensor, [Validators.required]],
      roomId: [this.device?.roomId || this.data?.roomId || '', [Validators.required]],
      isActive: [this.device?.isActive ?? true]
    });
  }

  onSubmit(): void {
    if (this.deviceForm.invalid) {
      this.deviceForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.deviceForm.value;

    if (this.isEditing && this.device) {
      const updateDto: UpdateDeviceDto = {
        name: formValue.name,
        deviceType: Number(formValue.deviceType), // Ensure it's a number
        roomId: formValue.roomId,
        isActive: formValue.isActive
      };

      this.deviceService.updateDevice(this.device.id, updateDto).subscribe({
        next: (updatedDevice) => {
          this.isLoading = false;
          Swal.fire({
            title: 'Success!',
            text: 'Device updated successfully!',
            icon: 'success',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
          });
          this.dialogRef?.close({ action: 'updated', device: updatedDevice });
        },
        error: (error) => {
          console.error('Error updating device:', error);
          this.isLoading = false;
          Swal.fire({
            title: 'Error!',
            text: 'Failed to update device. Please try again.',
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
      const createDto: CreateDeviceDto = {
        name: formValue.name,
        deviceType: Number(formValue.deviceType), // Ensure it's a number
        roomId: formValue.roomId,
        isActive: formValue.isActive
      };

      this.deviceService.createDevice(createDto).subscribe({
        next: (newDevice) => {
          this.isLoading = false;
          Swal.fire({
            title: 'Success!',
            text: 'Device created successfully!',
            icon: 'success',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
          });
          this.dialogRef?.close({ action: 'created', device: newDevice });
        },
        error: (error) => {
          console.error('Error creating device:', error);
          this.isLoading = false;
          Swal.fire({
            title: 'Error!',
            text: 'Failed to create device. Please try again.',
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
    const field = this.deviceForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName} is required`;
    }
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `${fieldName} must be at least ${minLength} characters`;
    }
    return '';
  }

  getDeviceTypeIcon(deviceType: DeviceType): string {
    switch (Number(deviceType)) {
      case DeviceType.TemperatureSensor:
        return 'thermostat';
      case DeviceType.HumiditySensor:
        return 'water_drop';
      case DeviceType.CombinedSensor:
        return 'sensors';
      case DeviceType.AirQualitySensor:
        return 'air';
      case DeviceType.MotionSensor:
        return 'directions_walk';
      case DeviceType.DoorSensor:
        return 'door_front';
      case DeviceType.WindowSensor:
        return 'window';
      case DeviceType.SmartLight:
        return 'lightbulb';
      case DeviceType.SmartThermostat:
        return 'thermostat';
      case DeviceType.SmartPlug:
        return 'power';
      default:
        return 'device_unknown';
    }
  }
}
