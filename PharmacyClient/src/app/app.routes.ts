import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { MedicineListComponent } from './components/medicine/medicine-list/medicine-list.component';
import { MedicineDetailComponent } from './components/medicine/medicine-detail/medicine-detail.component';
import { PrescriptionUploadComponent } from './components/prescription/prescription-upload/prescription-upload.component';
import { PrescriptionStatusComponent } from './components/prescription/prescription-status/prescription-status.component';
import { CartPageComponent } from './components/cart/cart-page/cart-page.component';
import { OrderConfirmationComponent } from './components/order/order-confirmation/order-confirmation.component';
import { OrderHistoryComponent } from './components/order/order-history/order-history.component';
import { OrderDetailComponent } from './components/order/order-detail/order-detail.component';
import { AdminDashboardComponent } from './components/admin/admin-dashboard/admin-dashboard.component';
import { ManageMedicinesComponent } from './components/admin/manage-medicines/manage-medicines.component';
import { ManagePrescriptionsComponent } from './components/admin/manage-prescriptions/manage-prescriptions.component';
import { ManageOrdersComponent } from './components/admin/manage-orders/manage-orders.component';
import { ManageInventoryComponent } from './components/admin/manage-inventory/manage-inventory.component';
import { NotFoundComponent } from './components/shared/not-found/not-found.component';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'medicines', component: MedicineListComponent },
  { path: 'medicines/:id', component: MedicineDetailComponent },
  { path: 'prescription/upload', component: PrescriptionUploadComponent, canActivate: [authGuard] },
  { path: 'prescription/status', component: PrescriptionStatusComponent, canActivate: [authGuard] },
  { path: 'cart', component: CartPageComponent, canActivate: [authGuard] },
  { path: 'order-confirmation/:id', component: OrderConfirmationComponent, canActivate: [authGuard] },
  { path: 'orders', component: OrderHistoryComponent, canActivate: [authGuard] },
  { path: 'orders/:id', component: OrderDetailComponent, canActivate: [authGuard] },
  { path: 'admin/dashboard', component: AdminDashboardComponent, canActivate: [adminGuard] },
  { path: 'admin/medicines', component: ManageMedicinesComponent, canActivate: [adminGuard] },
  { path: 'admin/prescriptions', component: ManagePrescriptionsComponent, canActivate: [adminGuard] },
  { path: 'admin/orders', component: ManageOrdersComponent, canActivate: [adminGuard] },
  { path: 'admin/inventory', component: ManageInventoryComponent, canActivate: [adminGuard] },
  { path: '**', component: NotFoundComponent }
];
