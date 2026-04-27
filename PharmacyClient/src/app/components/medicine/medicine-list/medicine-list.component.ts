import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicineService } from '../../../services/medicine.service';
import { Medicine } from '../../../models/medicine.model';
import { MedicineCardComponent } from '../medicine-card/medicine-card.component';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-medicine-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MedicineCardComponent, LoaderComponent],
  templateUrl: './medicine-list.component.html',
  styleUrls: ['./medicine-list.component.css'],})
export class MedicineListComponent implements OnInit {
  medicineService = inject(MedicineService);

  medicines: Medicine[] = [];
  categories: string[] = [];
  loading = true;

  searchTerm = '';
  selectedCategory = '';

  ngOnInit() {
    this.categories = this.medicineService.getCategories();
    this.loadMedicines();
  }

  loadMedicines() {
    this.loading = true;
    this.medicineService.getAll(this.searchTerm, this.selectedCategory).subscribe({
      next: (data) => {
        this.medicines = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  setCategory(category: string) {
    this.selectedCategory = category;
    this.loadMedicines();
  }

  clearFilters() {
    this.searchTerm = '';
    this.selectedCategory = '';
    this.loadMedicines();
  }
}
