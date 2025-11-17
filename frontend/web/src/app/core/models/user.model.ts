export enum UserRole {
  User = 'User',
  Police = 'Police',
  Business = 'Business',
  Admin = 'Admin'
}

export enum SubscriptionTier {
  Free = 'Free',
  Basic = 'Basic',
  Premium = 'Premium',
  Enterprise = 'Enterprise'
}

export interface User {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  role: UserRole;
  subscriptionTier: SubscriptionTier;
  isEmailVerified: boolean;
  isPhoneVerified: boolean;
  profilePhotoUrl?: string;
  createdAt: Date;
  updatedAt: Date;
}
