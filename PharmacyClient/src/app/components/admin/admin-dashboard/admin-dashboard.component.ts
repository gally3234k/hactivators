import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { Dashboard } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],})
export class AdminDashboardComponent implements OnInit {
  orderService = inject(OrderService);
  
  dashboard: Dashboard | null = null;
  loading = true;

  ngOnInit() {
    this.orderService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
