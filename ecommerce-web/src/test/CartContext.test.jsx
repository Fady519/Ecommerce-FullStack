import { describe, it, expect, beforeEach } from 'vitest';
import { render, screen, act } from '@testing-library/react';
import React, { useContext } from 'react';
import CartProvider, { CartContext } from '../components/Context/CartContext';
import { AuthContext } from '../components/Context/AuthContext';

function TestCartConsumer() {
  const { cartItems, favorites, addToCart, removeFromCart, addToFavorite, clearCart } = useContext(CartContext);

  return (
    <div>
      <div data-testid="cart-count">{cartItems.length}</div>
      <div data-testid="fav-count">{favorites.length}</div>
      <button onClick={() => addToCart({ id: 1, name: 'Shirt', price: 100 })}>Add Item</button>
      <button onClick={() => removeFromCart(1)}>Remove Item</button>
      <button onClick={() => addToFavorite({ id: 10, name: 'Watch', price: 250 })}>Add Fav</button>
      <button onClick={clearCart}>Clear Cart</button>
    </div>
  );
}

describe('CartContext & State Isolation', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('initializes empty cart and favorites for a guest user', () => {
    render(
      <AuthContext.Provider value={{ user: null, isAuthenticated: false }}>
        <CartProvider>
          <TestCartConsumer />
        </CartProvider>
      </AuthContext.Provider>
    );

    expect(screen.getByTestId('cart-count').textContent).toBe('0');
    expect(screen.getByTestId('fav-count').textContent).toBe('0');
  });

  it('manages adding, removing, and clearing items in cart for authenticated user', async () => {
    const mockUser = { email: 'fady@demo.com', displayName: 'Fady' };

    render(
      <AuthContext.Provider value={{ user: mockUser, isAuthenticated: true }}>
        <CartProvider>
          <TestCartConsumer />
        </CartProvider>
      </AuthContext.Provider>
    );

    expect(screen.getByTestId('cart-count').textContent).toBe('0');

    // Add item
    await act(async () => {
      screen.getByText('Add Item').click();
    });
    expect(screen.getByTestId('cart-count').textContent).toBe('1');

    // Add favorite
    await act(async () => {
      screen.getByText('Add Fav').click();
    });
    expect(screen.getByTestId('fav-count').textContent).toBe('1');

    // Clear cart
    await act(async () => {
      screen.getByText('Clear Cart').click();
    });
    expect(screen.getByTestId('cart-count').textContent).toBe('0');
    // Favorites remain intact
    expect(screen.getByTestId('fav-count').textContent).toBe('1');
  });
});
