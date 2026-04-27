import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { InventoryAlert } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-manage-inventory',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './manage-inventory.component.html',
  styleUrls: ['./manage-inventory.component.css'],})
export class ManageInventoryComponent implements OnInit {
  orderService = inject(OrderService);
  
  alerts: InventoryAlert[] = [];
  loading = true;

  ngOnInit() {
    this.orderService.getInventoryAlerts().subscribe({
      next: (data) => {
        this.alerts = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
