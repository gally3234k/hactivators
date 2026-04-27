export interface Prescription {
  id: number;
  userId: number;
  userName: string;
  imageUrl: string;
  status: string;
  reason?: string;
  uploadedAt: string;
  reviewedAt?: string;
}
