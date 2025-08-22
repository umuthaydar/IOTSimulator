import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HouseService } from '../../../core/services/house.service';
import { RoomService } from '../../../core/services/room.service';
import { DeviceService } from '../../../core/services/device.service';
import { forkJoin } from 'rxjs';
import Swal from 'sweetalert2';
import { DashboardService } from '../../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  totalHouses = 0;
  totalRooms = 0;
  totalDevices = 0;
  activeDevices = 0;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private dashboardService: DashboardService,
  ) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadDashboardData();
    }
  }

  private loadDashboardData(): void {
    this.dashboardService.getDashboardStats().subscribe({
      next: (data) => {
        this.totalRooms = data.totalRooms;
        this.totalHouses = data.totalHouses;
        this.totalDevices = data.totalDevices;
        this.activeDevices = data.activeDevices;
      },
      error: (error) => {
        console.error('Error loading dashboard data:', error);
        Swal.fire({
          title: 'Error!',
          text: 'Failed to load dashboard data. Please check your connection and try again.',
          icon: 'error',
          confirmButtonText: 'OK'
        });
      }
    });
  }
}