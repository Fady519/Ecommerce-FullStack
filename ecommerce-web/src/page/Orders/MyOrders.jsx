import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../../services/api';
import PageTransition from '../../components/PageTransition';
import { FaBoxOpen, FaTruck, FaClock } from 'react-icons/fa';
import './MyOrders.css';

export default function MyOrders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchOrders() {
      try {
        const data = await api.orders.getAll();
        setOrders(data || []);
      } catch (err) {
        console.error('Failed to load orders:', err);
        setError('Failed to fetch your orders. Please check your connection.');
      } finally {
        setLoading(false);
      }
    }
    fetchOrders();
  }, []);

  const getStatusClass = (status) => {
    const s = (status || '').toLowerCase();
    if (s.includes('received') || s.includes('success')) return 'paymentreceived';
    if (s.includes('fail')) return 'paymentfailed';
    return 'pending';
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    try {
      const d = new Date(dateStr);
      return d.toLocaleDateString(undefined, {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      });
    } catch {
      return dateStr;
    }
  };

  return (
    <PageTransition>
      <div className="orders-container">
        <div className="orders-header">
          <h1>My Orders History</h1>
          <p>Review and track your recent purchases and shipments</p>
        </div>

        {loading ? (
          <div className="orders-loading">
            <FaClock style={{ fontSize: '32px', color: '#0090f0', animation: 'spin 1s linear infinite' }} />
            <p style={{ marginTop: '12px', color: '#64748b' }}>Loading your orders...</p>
          </div>
        ) : error ? (
          <div className="orders-empty">
            <p style={{ color: '#ef4444' }}>{error}</p>
          </div>
        ) : orders.length === 0 ? (
          <div className="orders-empty">
            <FaBoxOpen className="orders-empty-icon" />
            <h3>No Orders Placed Yet</h3>
            <p>You haven't placed any orders yet. Discover our latest collections today!</p>
            <Link
              to="/"
              style={{
                display: 'inline-block',
                background: '#0090f0',
                color: '#ffffff',
                padding: '10px 24px',
                borderRadius: '6px',
                fontWeight: '600',
                textDecoration: 'none',
              }}
            >
              Start Shopping
            </Link>
          </div>
        ) : (
          <div className="orders-list">
            {orders.map((order) => {
              const items = order.items || order.orderItems || [];
              const address = order.address || order.shippingAddress;
              return (
                <div key={order.id} className="order-card">
                  <div className="order-card-header">
                    <div className="order-meta">
                      <div className="order-meta-col">
                        <span className="label">Order Placed</span>
                        <span className="val">{formatDate(order.orderDate)}</span>
                      </div>
                      <div className="order-meta-col">
                        <span className="label">Total Amount</span>
                        <span className="val">${Number(order.total).toFixed(2)}</span>
                      </div>
                      <div className="order-meta-col">
                        <span className="label">Order ID</span>
                        <span className="val" style={{ fontFamily: 'monospace' }}>
                          {order.id?.substring(0, 13)}...
                        </span>
                      </div>
                    </div>

                    <div>
                      <span className={`order-status-badge ${getStatusClass(order.orderStatus || order.status)}`}>
                        {order.orderStatus || order.status || 'Pending'}
                      </span>
                    </div>
                  </div>

                  <div className="order-card-body">
                    <div className="order-items-grid">
                      {items.map((item, idx) => (
                        <div key={idx} className="order-item-line">
                          <img
                            src={item.pictureUrl || ''}
                            alt={item.productName}
                            onError={(e) => { e.currentTarget.src = '/vite.svg'; }}
                          />
                          <div className="order-item-desc">
                            <h5>{item.productName}</h5>
                            <span>
                              Qty: {item.quantity} × ${Number(item.price).toFixed(2)}
                            </span>
                          </div>
                          <div className="order-item-price">
                            ${(Number(item.price) * Number(item.quantity)).toFixed(2)}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="order-card-footer">
                    <div>
                      <FaTruck style={{ marginRight: '6px', color: '#0090f0' }} />
                      Delivery Method: <strong>{order.deliveryMethod}</strong>
                    </div>
                    {address && (
                      <div>
                        Shipping to: <strong>{address.city}, {address.country}</strong>
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </PageTransition>
  );
}
