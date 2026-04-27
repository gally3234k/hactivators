import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MedicineService } from '../../../services/medicine.service';
import { Medicine } from '../../../models/medicine.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-manage-medicines',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './manage-medicines.component.html',
  styleUrls: ['./manage-medicines.component.css'],})
export class ManageMedicinesComponent implements OnInit {
  medicineService = inject(MedicineService);
  medicines: Medicine[] = [];
  loading = true;

  ngOnInit() {
    this.medicineService.getAll().subscribe({
      next: (data) => {
        this.medicines = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
