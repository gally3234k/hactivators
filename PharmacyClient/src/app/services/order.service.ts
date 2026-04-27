import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Order, CreateOrder, Dashboard, InventoryAlert, Payment } from '../models/order.model';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = `${environment.apiUrl}/orders`;
  private paymentUrl = `${environment.apiUrl}/payments`;
  private adminUrl = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  placeOrder(order: CreateOrder): Observable<Order> {
    return this.http.post<Order>(this.apiUrl, order);
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/my`);
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${id}`);
  }

  getAllOrders(status?: string): Observable<Order[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    return this.http.get<Order[]>(this.apiUrl, { params });
  }

  updateOrderStatus(id: number, status: string): Observable<Order> {
    return this.http.patch<Order>(`${this.apiUrl}/${id}/status`, { status });
  }

  cancelOrder(id: number): Observable<Order> {
    return this.http.patch<Order>(`${this.apiUrl}/${id}/cancel`, {});
  }

  processPayment(orderId: number, method: string): Observable<Payment> {
    return this.http.post<Payment>(`${this.paymentUrl}/process/${orderId}`, { method });
  }

  getPayment(orderId: number): Observable<Payment> {
    return this.http.get<Payment>(`${this.paymentUrl}/${orderId}`);
  }

  getDashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>(`${this.adminUrl}/dashboard`);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.adminUrl}/users`);
  }

  getInventoryAlerts(): Observable<InventoryAlert[]> {
    return this.http.get<InventoryAlert[]>(`${this.adminUrl}/inventory-alerts`);
  }
}
