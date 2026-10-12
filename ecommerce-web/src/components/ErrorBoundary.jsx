import React from 'react';

export default class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    console.error('React Application Error:', error, errorInfo);
  }

  handleReset = () => {
    localStorage.removeItem('cartItems');
    this.setState({ hasError: false, error: null });
    window.location.href = '/';
  };

  render() {
    if (this.state.hasError) {
      return (
        <div style={{
          minHeight: '80vh',
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '40px 20px',
          textAlign: 'center',
          fontFamily: 'system-ui, sans-serif'
        }}>
          <h2 style={{ fontSize: '24px', color: '#ef4444', marginBottom: '12px' }}>
            Something went wrong while displaying the page
          </h2>
          <p style={{ color: '#64748b', maxWidth: '500px', marginBottom: '24px', lineHeight: 1.6 }}>
            {this.state.error?.message || 'An unexpected rendering error occurred.'}
          </p>
          <button 
            onClick={this.handleReset}
            style={{
              padding: '12px 26px',
              borderRadius: '8px',
              background: '#0090f0',
              color: '#ffffff',
              border: 'none',
              fontWeight: '700',
              cursor: 'pointer'
            }}
          >
            Reload Home Page
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}
