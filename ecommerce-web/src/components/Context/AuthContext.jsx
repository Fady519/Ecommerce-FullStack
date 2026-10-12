import React, { createContext, useState, useEffect } from 'react';
import { api } from '../../services/api';

export const AuthContext = createContext(null);

export default function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    try {
      const saved = localStorage.getItem('user');
      return saved ? JSON.parse(saved) : null;
    } catch {
      return null;
    }
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const verifyUser = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const userData = await api.auth.getCurrentUser();
          const fullUser = { ...userData, token };
          setUser(fullUser);
          localStorage.setItem('user', JSON.stringify(fullUser));
        } catch (err) {
          // Only logout if token is actually unauthorized (401)
          if (err?.status === 401 || err?.statusCode === 401) {
            logout();
          }
        }
      } else {
        setUser(null);
      }
      setLoading(false);
    };

    verifyUser();
  }, []);

  const login = async (email, password) => {
    const userData = await api.auth.login({ email, password });
    localStorage.setItem('token', userData.token);
    localStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
    return userData;
  };

  const register = async (displayName, email, password, phoneNumber) => {
    const userData = await api.auth.register({ displayName, email, password, phoneNumber });
    localStorage.setItem('token', userData.token);
    localStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
    return userData;
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, isAuthenticated: !!user }}>
      {children}
    </AuthContext.Provider>
  );
}
