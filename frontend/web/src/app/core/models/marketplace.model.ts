export interface MarketplaceListing {
  id: string;
  deviceId: string;
  sellerId: string;
  title: string;
  description: string;
  price: number;
  currency: string;
  condition?: string;
  status: string;
  category?: string;
  location?: string;
  isShippingAvailable: boolean;
  isFeatured: boolean;
  viewCount: number;
  listedAt: Date;
  soldAt?: Date;
  expiresAt?: Date;
  buyerId?: string;
  device?: any;
  seller?: any;
}

export interface CreateListingRequest {
  deviceId: string;
  title: string;
  description: string;
  price: number;
  currency: string;
  condition?: string;
  category?: string;
  location?: string;
  isShippingAvailable: boolean;
  expiresAt?: Date;
}
