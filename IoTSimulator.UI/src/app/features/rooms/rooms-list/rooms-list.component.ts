import { Component, OnInit, OnDestroy, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatBadgeModule } from '@angular/material/badge';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RoomService } from '../../../core/services/room.service';
import { SignalRService, SensorDataUpdate } from '../../../services/signalr.service';
import { Room } from '../../../core/models';
import { RoomFormComponent } from '../room-form/room-form.component';
import { Subscription } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-rooms-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatBadgeModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './rooms-list.component.html',
  styleUrls: ['./rooms-list.component.scss']
})
export class RoomsListComponent implements OnInit, OnDestroy {
  rooms: Room[] = [];
  displayedColumns: string[] = ['name', 'houseName', 'sensorData', 'createdAt', 'actions'];
  isLoading = false;
  private signalRSubscription?: Subscription;
  private connectionSubscription?: Subscription;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private roomService: RoomService,
    private signalRService: SignalRService,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.initializeSignalR().then(() => {
        this.loadRooms();
      });
    }
  }

  ngOnDestroy(): void {
    this.cleanup();
  }

  private async initializeSignalR(): Promise<void> {
    try {
      // Start SignalR connection
      await this.signalRService.startConnection();
      console.log('SignalR connection initialized successfully');

      // Subscribe to sensor data updates
      this.signalRSubscription = this.signalRService.sensorDataUpdates.subscribe(
        (sensorData: SensorDataUpdate | null) => {
          if (sensorData) {
            console.log('Received sensor data in rooms-list:', sensorData);
            this.updateRoomSensorData(sensorData);
          }
        }
      );

      // Subscribe to device status updates
      this.signalRService.deviceStatusUpdates.subscribe(
        (deviceStatus: any) => {
          if (deviceStatus) {
            console.log('Received device status update in rooms-list:', deviceStatus);
            this.updateRoomDeviceStatus(deviceStatus);
          }
        }
      );

    } catch (error) {
      console.error('Error initializing SignalR:', error);
      Swal.fire({
        title: 'Connection Warning',
        text: 'Real-time sensor data may not be available.',
        icon: 'warning',
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 4000,
        timerProgressBar: true
      });
    }
  }

  private updateRoomSensorData(sensorData: SensorDataUpdate): void {
    console.log('Updating room sensor data for roomId:', sensorData.roomId);
    console.log('Available rooms:', this.rooms.map(r => ({id: r.id, name: r.name})));
    
    const roomIndex = this.rooms.findIndex(room => room.id === sensorData.roomId);
    console.log('Found room index:', roomIndex);
    
    if (roomIndex !== -1) {
      // Create new room object to trigger change detection
      this.rooms = [...this.rooms];
      this.rooms[roomIndex] = {
        ...this.rooms[roomIndex],
        currentTemperature: sensorData.temperature,
        currentHumidity: sensorData.humidity,
        lastSensorUpdate: new Date(sensorData.timestamp),
        sensorConnectionStatus: 'connected'
      };
      console.log('Updated room data:', this.rooms[roomIndex]);
      
      // Manually trigger change detection
      this.cdr.detectChanges();
    } else {
      console.warn('Room not found for sensor data update:', sensorData.roomId);
    }
  }

  private updateRoomDeviceStatus(deviceStatus: any): void {
    console.log('Updating room device status for roomId:', deviceStatus.roomId);
    
    const roomIndex = this.rooms.findIndex(room => room.id === deviceStatus.roomId);
    
    if (roomIndex !== -1) {
      // Create new room object to trigger change detection
      this.rooms = [...this.rooms];
      
      if (!deviceStatus.isActive || deviceStatus.status === 'Disconnected') {
        // Cihaz inaktif - sensor verilerini temizle ve disconnected olarak işaretle
        this.rooms[roomIndex] = {
          ...this.rooms[roomIndex],
          currentTemperature: undefined,
          currentHumidity: undefined,
          lastSensorUpdate: undefined,
          sensorConnectionStatus: 'disconnected'
        };
        console.log('Device disconnected, cleared sensor data for room:', this.rooms[roomIndex].name);
      }
      
      // Manually trigger change detection
      this.cdr.detectChanges();
    } else {
      console.warn('Room not found for device status update:', deviceStatus.roomId);
    }
  }

  private cleanup(): void {
    if (this.signalRSubscription) {
      this.signalRSubscription.unsubscribe();
    }
    if (this.connectionSubscription) {
      this.connectionSubscription.unsubscribe();
    }
    
    // Leave all room groups
    this.rooms.forEach(room => {
      this.signalRService.leaveRoomGroup(room.id);
    });
  }

  loadRooms(): void {
    this.isLoading = true;
    this.roomService.getRooms().subscribe({
      next: (rooms) => {
        this.rooms = rooms.map(room => ({
          ...room,
          sensorConnectionStatus: 'unknown' as const
        }));
        this.isLoading = false;
        
        console.log('Loaded rooms:', this.rooms.map(r => ({id: r.id, name: r.name})));
        
        // Join room groups for SignalR after rooms are loaded
        if (this.signalRService.isConnected()) {
          console.log('Joining room groups...');
          this.rooms.forEach(room => {
            this.signalRService.joinRoomGroup(room.id);
            console.log(`Joining room group: ${room.id} (${room.name})`);
          });
        } else {
          console.warn('SignalR not connected when trying to join room groups');
        }
      },
      error: (error) => {
        console.error('Error loading rooms:', error);
        this.isLoading = false;
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

  addRoom(): void {
    const dialogRef = this.dialog.open(RoomFormComponent, {
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'created') {
        this.loadRooms();
      }
    });
  }

  editRoom(room: Room): void {
    const dialogRef = this.dialog.open(RoomFormComponent, {
      width: '500px',
      disableClose: true,
      data: { room }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'updated') {
        this.loadRooms();
      }
    });
  }

  deleteRoom(room: Room): void {
    Swal.fire({
      title: 'Are you sure?',
      text: `Do you want to delete "${room.name}"? This action cannot be undone!`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.roomService.deleteRoom(room.id).subscribe({
          next: () => {
            this.loadRooms();
            Swal.fire({
              title: 'Deleted!',
              text: 'Room has been deleted successfully.',
              icon: 'success',
              toast: true,
              position: 'top-end',
              showConfirmButton: false,
              timer: 3000,
              timerProgressBar: true
            });
          },
          error: (error) => {
            console.error('Error deleting room:', error);
            Swal.fire({
              title: 'Error!',
              text: 'Failed to delete room. Please try again.',
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

  getSensorStatusIcon(room: Room): string {
    switch (room.sensorConnectionStatus) {
      case 'connected':
        return 'wifi';
      case 'disconnected':
        return 'wifi_off';
      default:
        return 'help_outline';
    }
  }

  getSensorStatusColor(room: Room): string {
    switch (room.sensorConnectionStatus) {
      case 'connected':
        return 'green';
      case 'disconnected':
        return 'red';
      default:
        return 'gray';
    }
  }

  getTemperatureColor(temp?: number): string {
    if (temp === undefined) return 'gray';
    if (temp < 18) return 'blue';
    if (temp > 26) return 'red';
    return 'green';
  }

  getHumidityColor(humidity?: number): string {
    if (humidity === undefined) return 'gray';
    if (humidity < 30) return 'orange';
    if (humidity > 70) return 'red';
    return 'green';
  }

  formatSensorValue(value?: number, unit: string = ''): string {
    if (value === undefined || value === null) return '--';
    return `${value.toFixed(1)}${unit}`;
  }

  getLastUpdateText(lastUpdate?: Date): string {
    if (!lastUpdate) return 'Never';
    const now = new Date();
    const diff = now.getTime() - lastUpdate.getTime();
    const minutes = Math.floor(diff / (1000 * 60));
    
    if (minutes < 1) return 'Just now';
    if (minutes === 1) return '1 minute ago';
    if (minutes < 60) return `${minutes} minutes ago`;
    
    const hours = Math.floor(minutes / 60);
    if (hours === 1) return '1 hour ago';
    if (hours < 24) return `${hours} hours ago`;
    
    return lastUpdate.toLocaleDateString();
  }

  // Debug method to test SignalR updates
  testSignalR(): void {
    console.log('=== SignalR Debug Info ===');
    console.log('Connection state:', this.signalRService.getConnectionState());
    console.log('Is connected:', this.signalRService.isConnected());
    console.log('Rooms loaded:', this.rooms.length);
    console.log('Rooms data:', this.rooms.map(r => ({
      id: r.id, 
      name: r.name, 
      temp: r.currentTemperature, 
      humidity: r.currentHumidity,
      lastUpdate: r.lastSensorUpdate,
      status: r.sensorConnectionStatus
    })));
    
    // Test with fake sensor data
    if (this.rooms.length > 0) {
      const testData: SensorDataUpdate = {
        deviceId: 'test-device',
        roomId: this.rooms[0].id,
        sensorId: 'test-sensor',
        temperature: 25.5,
        humidity: 60.2,
        timestamp: new Date().toISOString(),
        deviceName: 'Test Device'
      };
      console.log('Testing with fake data:', testData);
      this.updateRoomSensorData(testData);
    }
  }
}
