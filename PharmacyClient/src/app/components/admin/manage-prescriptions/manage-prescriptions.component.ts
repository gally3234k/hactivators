import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PrescriptionService } from '../../../services/prescription.service';
import { Prescription } from '../../../models/prescription.model';
import { LoaderComponent } from '../../shared/loader/loader.component';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-manage-prescriptions',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './manage-prescriptions.component.html',
  styleUrls: ['./manage-prescriptions.component.css'],})
export class ManagePrescriptionsComponent implements OnInit {
  prescriptionService = inject(PrescriptionService);
  
  prescriptions: Prescription[] = [];
  filteredPrescriptions: Prescription[] = [];
  loading = true;
  currentFilter = 'Pending';
  processingId: number | null = null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.prescriptionService.getAll().subscribe({
      next: (data) => {
        this.prescriptions = data.sort((a,b) => new Date(b.uploadedAt).getTime() - new Date(a.uploadedAt).getTime());
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  setFilter(status: string) {
    this.currentFilter = status;
    this.applyFilter();
  }

  applyFilter() {
    this.filteredPrescriptions = this.prescriptions.filter(p => p.status === this.currentFilter);
  }

  review(id: number, status: string, reason?: string) {
    this.processingId = id;
    this.prescriptionService.review(id, status, reason).subscribe({
      next: (updated) => {
        const idx = this.prescriptions.findIndex(p => p.id === id);
        if (idx !== -1) {
          this.prescriptions[idx] = updated;
        }
        this.applyFilter();
        this.processingId = null;
      },
      error: () => {
        this.processingId = null;
        alert('Failed to update prescription status');
      }
    });
  }

  reject(id: number) {
    const reason = prompt('Please enter a reason for rejection (e.g. Blurry image, Invalid details):');
    if (reason !== null) {
      if (reason.trim() === '') {
        alert('Rejection reason is required.');
        return;
      }
      this.review(id, 'Rejected', reason);
    }
  }

  getImageUrl(url: string): string {
    if (url && !url.startsWith('http')) {
      const baseUrl = environment.apiUrl.replace('/api', '');
      return `${baseUrl}${url}`;
    }
    return url;
  }

  handleImageError(event: any) {
    event.target.src = 'https://placehold.co/400x300?text=Prescription';
  }
}
