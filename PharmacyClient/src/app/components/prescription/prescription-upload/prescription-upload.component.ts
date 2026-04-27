import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PrescriptionService } from '../../../services/prescription.service';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-prescription-upload',
  standalone: true,
  imports: [CommonModule, LoaderComponent],
  templateUrl: './prescription-upload.component.html',
  styleUrls: ['./prescription-upload.component.css'],})
export class PrescriptionUploadComponent {
  prescriptionService = inject(PrescriptionService);
  router = inject(Router);

  isDragging = false;
  selectedFile: File | null = null;
  previewUrl: string | ArrayBuffer | null = null;
  uploading = false;
  error = '';

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
    
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFile(files[0]);
    }
  }

  onFileSelected(event: any) {
    const files = event.target.files;
    if (files && files.length > 0) {
      this.handleFile(files[0]);
    }
  }

  handleFile(file: File) {
    this.error = '';
    
    if (!file.type.match(/image\/(jpeg|jpg|png|webp)/)) {
      this.error = 'Only JPG, PNG and WEBP image formats are supported.';
      return;
    }
    
    if (file.size > 5 * 1024 * 1024) {
      this.error = 'File size exceeds 5MB limit.';
      return;
    }

    this.selectedFile = file;
    
    // Create preview
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = (_event) => {
      this.previewUrl = reader.result;
    };
  }

  clearSelection(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.selectedFile = null;
    this.previewUrl = null;
    this.error = '';
  }

  uploadPrescription() {
    if (!this.selectedFile) return;

    this.uploading = true;
    this.error = '';

    this.prescriptionService.upload(this.selectedFile).subscribe({
      next: (_res) => {
        this.uploading = false;
        this.router.navigate(['/prescription/status']);
      },
      error: (err) => {
        this.uploading = false;
        this.error = err.error?.message || 'Failed to upload prescription. Please try again.';
      }
    });
  }
}
