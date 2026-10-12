// TypeScript types matching the ASP.NET Core Backend DTOs

export interface ProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  pictureUrl: string;
  productType: string;
  productTypeId: number;
  productBrand: string;
  productBrandId: number;
  stock: number;
  isActive: boolean;
  createdAt: string;
}

export interface BrandDto {
  id: number;
  name: string;
}

export interface TypeDto {
  id: number;
  name: string;
}

export interface PaginatedResult<T> {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: T[];
}

export interface UserDto {
  displayName: string;
  email: string;
  token: string;
  roles?: string[];
}

export interface AddressDto {
  firstName: string;
  lastName: string;
  street: string;
  city: string;
  country: string;
}

export interface BasketItemDto {
  id: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand?: string;
  type?: string;
}

export interface CustomerBasketDto {
  id: string;
  items: BasketItemDto[];
  deliveryMethodId?: number | null;
  clientSecret?: string | null;
  paymentIntentId?: string | null;
  shippingPrice?: number;
}

export interface DeliveryMethodDto {
  id: number;
  shortName: string;
  deliveryTime: string;
  description: string;
  price: number;
}

export interface OrderItemDto {
  productId?: number;
  id?: number;
  productName: string;
  pictureUrl: string;
  price: number;
  quantity: number;
}

export interface OrderToReturnDto {
  id: string;
  userEmail: string;
  orderDate: string;
  address?: AddressDto;
  shippingAddress?: AddressDto;
  deliveryMethod: string;
  deliveryPrice?: number;
  items?: OrderItemDto[];
  orderItems?: OrderItemDto[];
  subTotal?: number;
  subtotal?: number;
  total: number;
  orderStatus?: string;
  status?: string;
  paymentMethod?: string;
}

export interface DashboardStatisticsDto {
  totalSales: number;
  totalOrders: number;
  lowStockProducts: { id: number; name: string; stock: number }[];
  topSellingProducts: { productId: number; productName: string; totalQuantitySold: number }[];
}
