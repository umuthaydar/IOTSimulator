import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DeviceService } from '../../../core/services/device.service';
import { IoTDevice, DeviceType } from '../../../core/models';
import { DeviceFormComponent } from '../device-form/device-form.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-devices-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDialogModule
  ],
  templateUrl: './devices-list.component.html',
  styleUrls: ['./devices-list.component.scss']
})
export class DevicesListComponent implements OnInit {
  devices: IoTDevice[] = [];
  displayedColumns: string[] = ['name', 'deviceType', 'room', 'status', 'createdAt', 'actions'];
  isLoading = false;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private deviceService: DeviceService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadDevices();
    }
  }

  loadDevices(): void {
    this.isLoading = true;
    this.deviceService.getDevices().subscribe({
      next: (devices) => {
        this.devices = devices;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading devices:', error);
        this.isLoading = false;
        Swal.fire({
          title: 'Error!',
          text: 'Failed to load devices. Please try again.',
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

  addDevice(): void {
    const dialogRef = this.dialog.open(DeviceFormComponent, {
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'created') {
        this.loadDevices();
      }
    });
  }

  editDevice(device: IoTDevice): void {
    const dialogRef = this.dialog.open(DeviceFormComponent, {
      width: '500px',
      disableClose: true,
      data: { device }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'updated') {
        this.loadDevices();
      }
    });
  }

  deleteDevice(device: IoTDevice): void {
    Swal.fire({
      title: 'Are you sure?',
      text: `Do you want to delete "${device.name}"? This action cannot be undone!`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.deviceService.deleteDevice(device.id).subscribe({
          next: () => {
            this.loadDevices();
            Swal.fire({
              title: 'Deleted!',
              text: 'Device has been deleted successfully.',
              icon: 'success',
              toast: true,
              position: 'top-end',
              showConfirmButton: false,
              timer: 3000,
              timerProgressBar: true
            });
          },
          error: (error) => {
            console.error('Error deleting device:', error);
            Swal.fire({
              title: 'Error!',
              text: 'Failed to delete device. Please try again.',
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
    });
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

  // Helper method to get device type display name
  getDeviceTypeName(deviceType: DeviceType): string {
    const name = DeviceType[Number(deviceType)];
    // Convert from PascalCase to readable format
    return name ? name.replace(/([A-Z])/g, ' $1').trim() : 'Unknown';
  }
}
