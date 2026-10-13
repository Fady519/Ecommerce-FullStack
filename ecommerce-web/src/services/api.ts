import type {
  ProductDto,
  BrandDto,
  TypeDto,
  PaginatedResult,
  UserDto,
  CustomerBasketDto,
  DeliveryMethodDto,
  OrderToReturnDto,
  DashboardStatisticsDto
} from '../types/api';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api';

export class ApiError extends Error {
  status: number;
  errors?: Record<string, string[]>;
  title?: string;

  constructor(message: string, status: number, errors?: Record<string, string[]>, title?: string) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.errors = errors;
    this.title = title;
  }
}

/**
 * Universal fetch wrapper that attaches JWT token and handles structured RFC 9457 errors.
 */
async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('token');
  const headers = new Headers(options.headers || {});

  if (!headers.has('Content-Type') && !(options.body instanceof FormData)) {
    headers.set('Content-Type', 'application/json');
  }

  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let errorDetail = 'Request failed';
    let errorsDict: Record<string, string[]> | undefined;
    let title: string | undefined;

    try {
      const errBody = await response.json();
      title = errBody.title;
      errorsDict = errBody.errors;

      if (errBody.detail) {
        errorDetail = errBody.detail;
      } else if (errBody.message) {
        errorDetail = errBody.message;
      } else if (errorsDict && typeof errorsDict === 'object') {
        // Collect first friendly message
        const firstKey = Object.keys(errorsDict)[0];
        if (firstKey && errorsDict[firstKey]?.length > 0) {
          errorDetail = errorsDict[firstKey][0];
        } else {
          errorDetail = title || 'Validation error';
        }
      } else {
        errorDetail = title || `HTTP ${response.status}`;
      }
    } catch {
      errorDetail = `HTTP ${response.status}: ${response.statusText}`;
    }

    throw new ApiError(errorDetail, response.status, errorsDict, title);
  }

  // If status is 204 No Content
  if (response.status === 204) {
    return {} as T;
  }

  return response.json();
}

export const api = {
  // Catalog
  products: {
    getAll: async (params?: { pageIndex?: number; pageSize?: number; brandId?: number; typeId?: number; sort?: string; search?: string }): Promise<PaginatedResult<ProductDto>> => {
      const query = new URLSearchParams();
      if (params?.pageIndex) query.append('pageIndex', params.pageIndex.toString());
      if (params?.pageSize) query.append('pageSize', params.pageSize.toString());
      if (params?.brandId) query.append('brandId', params.brandId.toString());
      if (params?.typeId) query.append('typeId', params.typeId.toString());
      if (params?.sort) query.append('sort', params.sort);
      if (params?.search) query.append('search', params.search);
      const q = query.toString();
      const firstPage = await request<PaginatedResult<ProductDto>>(`/products${q ? `?${q}` : ''}`);

      const requestedSize = params?.pageSize || 0;
      if (requestedSize > 10 && firstPage.count > firstPage.data.length && !params?.pageIndex) {
        const pageSize = firstPage.pageSize || 10;
        const totalPages = Math.ceil(firstPage.count / pageSize);
        const remainingPromises: Promise<PaginatedResult<ProductDto>>[] = [];
        for (let p = 2; p <= totalPages; p++) {
          const nextQuery = new URLSearchParams(query);
          nextQuery.set('pageIndex', p.toString());
          remainingPromises.push(request<PaginatedResult<ProductDto>>(`/products?${nextQuery.toString()}`));
        }
        const results = await Promise.all(remainingPromises);
        let allData = [...firstPage.data];
        for (const res of results) {
          if (res.data) allData = allData.concat(res.data);
        }
        return {
          ...firstPage,
          data: allData.slice(0, requestedSize),
          pageSize: allData.length
        };
      }
      return firstPage;
    },
    getById: (id: number) => request<ProductDto>(`/products/${id}`),
    getBrands: () => request<BrandDto[]>('/products/brands'),
    getTypes: () => request<TypeDto[]>('/products/types'),
  },

  // Basket
  basket: {
    get: (id: string) => request<CustomerBasketDto>(`/basket/${id}`),
    update: (basket: CustomerBasketDto) => request<CustomerBasketDto>('/basket', {
      method: 'POST',
      body: JSON.stringify(basket),
    }),
    delete: (id: string) => request<void>(`/basket/${id}`, { method: 'DELETE' }),
  },

  // Auth
  auth: {
    login: (credentials: { email: string; password: string }) =>
      request<UserDto>('/authentication/login', {
        method: 'POST',
        body: JSON.stringify(credentials),
      }),
    register: (data: { displayName: string; email: string; password: string; phoneNumber?: string }) =>
      request<UserDto>('/authentication/register', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    getCurrentUser: () => request<UserDto>('/Authentication/CurrentUser'),
  },

  // Orders
  orders: {
    create: (order: { basketId: string; deliveryMethodId: number; addressDTO: any; paymentMethod?: string }) =>
      request<OrderToReturnDto>('/order', {
        method: 'POST',
        body: JSON.stringify(order),
      }),
    getAll: () => request<OrderToReturnDto[]>('/order'),
    getById: (id: string) => request<OrderToReturnDto>(`/order/${id}`),
    getDeliveryMethods: () => request<DeliveryMethodDto[]>('/order/DeliveryMethods'),
  },

  // Payments (Stripe)
  payment: {
    createOrUpdateIntent: (basketId: string) =>
      request<CustomerBasketDto>(`/Payment/${basketId}`, {
        method: 'POST',
      }),
  },

  // Admin Dashboard & Management
  admin: {
    getDashboard: () => request<DashboardStatisticsDto>('/admin/dashboard'),
    getAllOrders: () => request<OrderToReturnDto[]>('/admin/orders'),
    updateOrderStatus: (id: string, status: string) =>
      request<OrderToReturnDto>(`/admin/orders/${id}/status`, {
        method: 'PUT',
        body: JSON.stringify(status),
      }),
    createProduct: (formData: FormData) =>
      request<ProductDto>('/admin/products', {
        method: 'POST',
        body: formData,
      }),
    updateProduct: (id: number, formData: FormData) =>
      request<ProductDto>(`/admin/products/${id}`, {
        method: 'PUT',
        body: formData,
      }),
    deleteProduct: (id: number) =>
      request<boolean>(`/admin/products/${id}`, {
        method: 'DELETE',
      }),
    createBrand: (name: string) => {
      const fd = new FormData();
      fd.append('name', name);
      return request<BrandDto>('/admin/catalog/brands', { method: 'POST', body: fd });
    },
    deleteBrand: (id: number) =>
      request<boolean>(`/admin/catalog/brands/${id}`, { method: 'DELETE' }),
    createType: (name: string) => {
      const fd = new FormData();
      fd.append('name', name);
      return request<TypeDto>('/admin/catalog/types', { method: 'POST', body: fd });
    },
    deleteType: (id: number) =>
      request<boolean>(`/admin/catalog/types/${id}`, { method: 'DELETE' }),
  }
};
