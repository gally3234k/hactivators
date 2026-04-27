import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PrescriptionService } from '../../../services/prescription.service';
import { Prescription } from '../../../models/prescription.model';
import { LoaderComponent } from '../../shared/loader/loader.component';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-prescription-status',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './prescription-status.component.html',
  styleUrls: ['./prescription-status.component.css'],})
export class PrescriptionStatusComponent implements OnInit {
  prescriptionService = inject(PrescriptionService);

  prescriptions: Prescription[] = [];
  loading = true;

  ngOnInit() {
    this.loadPrescriptions();
  }

  loadPrescriptions() {
    this.prescriptionService.getMyPrescriptions().subscribe({
      next: (data) => {
        // Sort newest first
        this.prescriptions = data.sort((a, b) => 
          new Date(b.uploadedAt).getTime() - new Date(a.uploadedAt).getTime()
        );
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getImageUrl(url: string): string {
    // If it's a relative path, prepend API URL without /api since images are in wwwroot
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
