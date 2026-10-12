import React, { createContext, useEffect, useState, useCallback, useContext, useRef } from 'react';
import { api } from '../../services/api';
import { AuthContext } from './AuthContext';

export const CartContext = createContext();

function generateBasketId(suffix = '') {
  const cleanSuffix = suffix.replace(/[^a-zA-Z0-9]/g, '').substring(0, 8);
  return 'b_' + Math.random().toString(36).substring(2, 8) + '_' + Date.now().toString(36) + (cleanSuffix ? '_' + cleanSuffix : '');
}

export default function CartProvider({ children }) {
  const { user } = useContext(AuthContext);

  // Derive unique storage key per user (or null when logged out as guest)
  const userKey = user?.email
    ? `usr_${user.email.toLowerCase().replace(/[^a-z0-9]/g, '_')}`
    : null;

  // Initialize per-user basket ID (only for authenticated users)
  const [basketId, setBasketId] = useState(() => {
    if (!userKey) return '';
    const saved = localStorage.getItem(`basket_id_${userKey}`);
    if (saved) return saved;
    const newId = generateBasketId(userKey);
    localStorage.setItem(`basket_id_${userKey}`, newId);
    return newId;
  });

  // Initialize per-user cart items (empty for guests or new accounts)
  const [cartItems, setCartItems] = useState(() => {
    if (!userKey) return [];
    try {
      const saved = localStorage.getItem(`cart_${userKey}`);
      return saved ? JSON.parse(saved) : [];
    } catch {
      return [];
    }
  });

  // Initialize per-user favorites (empty for guests or new accounts)
  const [favorites, setFavorites] = useState(() => {
    if (!userKey) return [];
    try {
      const savedFav = localStorage.getItem(`favorites_${userKey}`);
      return savedFav ? JSON.parse(savedFav) : [];
    } catch {
      return [];
    }
  });

  // Track previous user to isolate carts between accounts
  const prevUserKeyRef = useRef(userKey);

  useEffect(() => {
    // One-time cleanup of any legacy guest or global keys from older version
    localStorage.removeItem('cartItems');
    localStorage.removeItem('basket_id');
    localStorage.removeItem('favoritesItems');
    localStorage.removeItem('favorites_guest');
    localStorage.removeItem('cart_guest');
    localStorage.removeItem('basket_id_guest');
  }, []);

  useEffect(() => {
    if (prevUserKeyRef.current !== userKey) {
      prevUserKeyRef.current = userKey;

      if (!userKey) {
        // When logged out, clear cart, basket, and favorites completely
        setBasketId('');
        setCartItems([]);
        setFavorites([]);
        return;
      }

      // 1. Get or create basket ID for newly active user
      let currentBasketId = localStorage.getItem(`basket_id_${userKey}`);
      if (!currentBasketId) {
        currentBasketId = generateBasketId(userKey);
        localStorage.setItem(`basket_id_${userKey}`, currentBasketId);
      }
      setBasketId(currentBasketId);

      // 2. Load cart items for newly active user (empty for new accounts)
      let userItems = [];
      try {
        const savedUserCart = localStorage.getItem(`cart_${userKey}`);
        userItems = savedUserCart ? JSON.parse(savedUserCart) : [];
      } catch {}
      setCartItems(userItems);

      // 3. Load favorites for newly active user
      let userFavs = [];
      try {
        const savedUserFavs = localStorage.getItem(`favorites_${userKey}`);
        userFavs = savedUserFavs ? JSON.parse(savedUserFavs) : [];
      } catch {}
      setFavorites(userFavs);
    }
  }, [userKey]);

  // Sync basket to backend API
  const syncBasketWithBackend = useCallback(async (currentId, items) => {
    try {
      if (!currentId) return;
      const basketPayload = {
        id: currentId,
        items: items.map(item => ({
          id: Number(item.id),
          quantity: Number(item.quantity) || 1,
          productName: item.title || item.name || '',
          pictureUrl: Array.isArray(item.images) ? (item.images[0] || '') : (item.pictureUrl || ''),
          price: Number(item.price) || 0
        }))
      };
      await api.basket.update(basketPayload);
    } catch (err) {
      // Graceful fallback if offline or network hiccup
      console.warn('Basket backend sync warning:', err);
    }
  }, []);

  // Save cartItems to user-specific local storage & sync with backend (only when logged in)
  useEffect(() => {
    if (!userKey) return;
    localStorage.setItem(`cart_${userKey}`, JSON.stringify(cartItems));
    if (basketId && cartItems.length > 0) {
      syncBasketWithBackend(basketId, cartItems);
    }
  }, [cartItems, basketId, userKey, syncBasketWithBackend]);

  // Save favorites to user-specific local storage (only when logged in)
  useEffect(() => {
    if (!userKey) return;
    localStorage.setItem(`favorites_${userKey}`, JSON.stringify(favorites));
  }, [favorites, userKey]);

  const addToFavorite = (item) => {
    setFavorites(prev => {
      if (prev.some(i => i.id === item.id)) return prev;
      return [...prev, item];
    });
  };

  const removeFromFavorites = (id) => {
    setFavorites(prev => prev.filter(i => i.id !== id));
  };

  const addToCart = (item) => {
    setCartItems(prevItems => {
      const existingItem = prevItems.find(i => i.id === item.id);
      if (existingItem) {
        return prevItems.map(i =>
          i.id === item.id ? { ...i, quantity: i.quantity + 1 } : i
        );
      } else {
        return [...prevItems, { ...item, quantity: 1 }];
      }
    });

    // Remove from favorites when added to cart
    setFavorites(prevFavs => prevFavs.filter(fav => fav.id !== item.id));
  };

  const increaseQuantity = (id) => {
    setCartItems(prevItems =>
      prevItems.map(item =>
        item.id === id ? { ...item, quantity: item.quantity + 1 } : item
      )
    );
  };

  const decreaseQuantity = (id) => {
    setCartItems(prevItems =>
      prevItems.map(item =>
        item.id === id && item.quantity > 1
          ? { ...item, quantity: item.quantity - 1 }
          : item
      )
    );
  };

  const removeFromCart = (id) => {
    setCartItems(prevItems => prevItems.filter(item => item.id !== id));
  };

  const clearCart = async () => {
    setCartItems([]);
    localStorage.removeItem(`cart_${userKey}`);
    try {
      if (basketId) {
        await api.basket.delete(basketId);
      }
    } catch (err) {
      console.warn('Error clearing basket on backend:', err);
    }
    // Generate fresh basket ID for this user for future orders
    const newId = generateBasketId(userKey);
    localStorage.setItem(`basket_id_${userKey}`, newId);
    setBasketId(newId);
  };

  return (
    <CartContext.Provider
      value={{
        basketId,
        cartItems,
        addToCart,
        increaseQuantity,
        decreaseQuantity,
        removeFromCart,
        clearCart,
        favorites,
        addToFavorite,
        removeFromFavorites,
      }}
    >
      {children}
    </CartContext.Provider>
  );
}
