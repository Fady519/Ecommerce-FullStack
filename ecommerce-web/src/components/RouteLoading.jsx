import React from 'react';

export default function RouteLoading() {
  return (
    <div style={{
      minHeight: '60vh',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      gap: '16px',
      color: 'var(--color-heading)'
    }}>
      <div style={{
        width: '42px',
        height: '42px',
        border: '3.5px solid rgba(0, 166, 153, 0.2)',
        borderTopColor: 'var(--main-color, #00a699)',
        borderRadius: '50%',
        animation: 'spin 0.75s linear infinite'
      }} />
      <span style={{ fontSize: '14px', fontWeight: 600, color: 'var(--p-color, #888)' }}>
        Loading experience...
      </span>
      <style>{`
        @keyframes spin {
          to { transform: rotate(360deg); }
        }
      `}</style>
    </div>
  );
}
