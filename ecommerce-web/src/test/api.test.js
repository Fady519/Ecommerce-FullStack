import { describe, it, expect, beforeEach } from 'vitest';
import { api } from '../services/api';

describe('API Service Unit Tests', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('provides methods for products, basket, orders, and admin', () => {
    expect(api.products).toBeDefined();
    expect(typeof api.products.getAll).toBe('function');
    expect(typeof api.products.getById).toBe('function');
    expect(typeof api.products.getBrands).toBe('function');
    expect(typeof api.products.getTypes).toBe('function');

    expect(api.basket).toBeDefined();
    expect(typeof api.basket.get).toBe('function');
    expect(typeof api.basket.update).toBe('function');

    expect(api.orders).toBeDefined();
    expect(typeof api.orders.create).toBe('function');
    expect(typeof api.orders.getAll).toBe('function');

    expect(api.admin).toBeDefined();
    expect(typeof api.admin.getDashboard).toBe('function');
    expect(typeof api.admin.createProduct).toBe('function');
  });

  it('stores and retrieves user credentials from localStorage properly', () => {
    const mockUser = {
      email: 'test@example.com',
      displayName: 'Test User',
      token: 'fake-jwt-token-xyz'
    };

    localStorage.setItem('user', JSON.stringify(mockUser));
    const retrieved = JSON.parse(localStorage.getItem('user'));

    expect(retrieved.email).toBe('test@example.com');
    expect(retrieved.token).toBe('fake-jwt-token-xyz');
  });
});
