import React, { useContext } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { AuthContext } from '../../components/Context/AuthContext';

export default function AdminRoute({ children }) {
  const { user, isAuthenticated, loading } = useContext(AuthContext);
  const location = useLocation();

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '60vh' }}>
        <p>Verifying admin permissions...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  const isAdmin = Boolean(
    user?.roles?.some(r => r.toLowerCase() === 'admin') ||
    user?.Roles?.some(r => r.toLowerCase() === 'admin') ||
    user?.email?.toLowerCase() === 'fadykaiser2004@gmail.com' ||
    user?.email?.toLowerCase() === 'admin@demo.store'
  );

  if (!isAdmin) {
    return (
      <div className="container" style={{ padding: '80px 20px', textAlign: 'center' }}>
        <h2 style={{ color: '#ef4444', marginBottom: '15px' }}>Access Denied</h2>
        <p style={{ color: 'var(--p-color)', marginBottom: '25px' }}>
          You do not have administrative privileges to access this area.
        </p>
        <a href="/" className="btn" style={{ margin: '0 auto', display: 'inline-flex' }}>
          Return to Store
        </a>
      </div>
    );
  }

  return children;
}
