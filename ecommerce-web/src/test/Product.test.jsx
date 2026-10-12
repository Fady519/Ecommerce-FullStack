import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import Product from '../components/slideProducts/Product';
import { CartContext } from '../components/Context/CartContext';
import { AuthContext } from '../components/Context/AuthContext';

// Mock react-hot-toast
vi.mock('react-hot-toast', () => ({
  default: {
    success: vi.fn(),
    error: vi.fn(),
  },
}));

describe('Product Card Component', () => {
  const mockItem = {
    id: 42,
    name: 'Wireless Noise-Canceling Headphones',
    price: 199.99,
    image: 'http://localhost:5163/images/products/headphones.jpg',
    brand: 'Sony'
  };

  it('renders product information properly', () => {
    render(
      <BrowserRouter>
        <AuthContext.Provider value={{ isAuthenticated: false, user: null }}>
          <CartContext.Provider value={{ cartItems: [], favorites: [], addToCart: vi.fn(), addToFavorites: vi.fn() }}>
            <Product item={mockItem} />
          </CartContext.Provider>
        </AuthContext.Provider>
      </BrowserRouter>
    );

    expect(screen.getByText('Wireless Noise-Canceling Headphones')).toBeInTheDocument();
    expect(screen.getByText('$199.99')).toBeInTheDocument();
    expect(screen.getByText('Sony')).toBeInTheDocument();

    const img = screen.getByAltText('Wireless Noise-Canceling Headphones');
    expect(img).toBeInTheDocument();
    expect(img).toHaveAttribute('loading', 'lazy');
  });

  it('indicates when product is already in cart', () => {
    render(
      <BrowserRouter>
        <AuthContext.Provider value={{ isAuthenticated: true, user: { email: 'user@test.com' } }}>
          <CartContext.Provider value={{
            cartItems: [{ id: 42, name: 'Wireless Noise-Canceling Headphones', price: 199.99 }],
            favorites: [],
            addToCart: vi.fn(),
            addToFavorites: vi.fn()
          }}>
            <Product item={mockItem} />
          </CartContext.Provider>
        </AuthContext.Provider>
      </BrowserRouter>
    );

    expect(screen.getByText(/In Cart/i)).toBeInTheDocument();
  });
});
