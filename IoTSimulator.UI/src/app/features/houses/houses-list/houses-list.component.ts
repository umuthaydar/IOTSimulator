import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { HouseService } from '../../../core/services/house.service';
import { House } from '../../../core/models';
import { HouseFormComponent } from '../house-form/house-form.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-houses-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatDialogModule
  ],
  templateUrl: './houses-list.component.html',
  styleUrls: ['./houses-list.component.scss']
})
export class HousesListComponent implements OnInit {
  houses: House[] = [];
  displayedColumns: string[] = ['name', 'address', 'createdAt', 'actions'];
  isLoading = false;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private houseService: HouseService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadHouses();
    }
  }

  loadHouses(): void {
    this.isLoading = true;
    this.houseService.getHouses().subscribe({
      next: (houses) => {
        this.houses = houses;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading houses:', error);
        this.isLoading = false;
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

  addHouse(): void {
    const dialogRef = this.dialog.open(HouseFormComponent, {
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'created') {
        this.loadHouses();
      }
    });
  }

  editHouse(house: House): void {
    const dialogRef = this.dialog.open(HouseFormComponent, {
      width: '500px',
      disableClose: true,
      data: { house }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.action === 'updated') {
        this.loadHouses();
      }
    });
  }

  deleteHouse(house: House): void {
    Swal.fire({
      title: 'Are you sure?',
      text: `Do you want to delete "${house.name}"? This action cannot be undone!`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.houseService.deleteHouse(house.id).subscribe({
          next: () => {
            this.loadHouses();
            Swal.fire({
              title: 'Deleted!',
              text: 'House has been deleted successfully.',
              icon: 'success',
              toast: true,
              position: 'top-end',
              showConfirmButton: false,
              timer: 3000,
              timerProgressBar: true
            });
          },
          error: (error) => {
            console.error('Error deleting house:', error);
            Swal.fire({
              title: 'Error!',
              text: 'Failed to delete house. Please try again.',
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
}
