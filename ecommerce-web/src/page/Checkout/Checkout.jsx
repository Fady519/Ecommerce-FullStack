import React, { useState, useEffect, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { CartContext } from '../../components/Context/CartContext';
import { AuthContext } from '../../components/Context/AuthContext';
import { api } from '../../services/api';
import toast from 'react-hot-toast';
import { FaTruck, FaCreditCard, FaMoneyBillWave, FaCheckCircle, FaSpinner } from 'react-icons/fa';
import PageTransition from '../../components/PageTransition';
import './Checkout.css';

export default function Checkout() {
  const navigate = useNavigate();
  const { cartItems, basketId, clearCart } = useContext(CartContext);
  const { user } = useContext(AuthContext);

  const [deliveryMethods, setDeliveryMethods] = useState([]);
  const [selectedDeliveryMethod, setSelectedDeliveryMethod] = useState(null);
  const [paymentMethod, setPaymentMethod] = useState('Card'); // 'Card' or 'CashOnDelivery'
  const [loadingDelivery, setLoadingDelivery] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState(null);

  // Address form fields
  const [address, setAddress] = useState({
    firstName: user?.displayName?.split(' ')[0] || '',
    lastName: user?.displayName?.split(' ').slice(1).join(' ') || 'Customer',
    street: '',
    city: 'Cairo',
    country: 'Egypt',
  });

  // Card Payment details
  const [cardData, setCardData] = useState({
    nameOnCard: user?.displayName || 'Cardholder Name',
    cardNumber: '',
    expiry: '',
    cvc: '',
  });

  const [errors, setErrors] = useState({});

  // Card formatting helpers
  const handleCardNumberChange = (e) => {
    const raw = e.target.value.replace(/\D/g, '').slice(0, 16);
    const formatted = raw.replace(/(\d{4})(?=\d)/g, '$1 ');
    setCardData(prev => ({ ...prev, cardNumber: formatted }));
    if (errors.cardNumber) setErrors(prev => ({ ...prev, cardNumber: null }));
  };

  const handleExpiryChange = (e) => {
    let raw = e.target.value.replace(/\D/g, '').slice(0, 4);
    if (raw.length > 2) {
      raw = `${raw.slice(0, 2)}/${raw.slice(2)}`;
    }
    setCardData(prev => ({ ...prev, expiry: raw }));
    if (errors.expiry) setErrors(prev => ({ ...prev, expiry: null }));
  };

  const handleCvcChange = (e) => {
    const raw = e.target.value.replace(/\D/g, '').slice(0, 4);
    setCardData(prev => ({ ...prev, cvc: raw }));
    if (errors.cvc) setErrors(prev => ({ ...prev, cvc: null }));
  };

  const fillTestCard = () => {
    setCardData({
      nameOnCard: user?.displayName || 'Fady Kaiser',
      cardNumber: '4242 4242 4242 4242',
      expiry: '12/28',
      cvc: '123',
    });
    setErrors(prev => ({ ...prev, cardNumber: null, expiry: null, cvc: null }));
    toast.success('Test Stripe card details filled!');
  };

  // Fetch Delivery Methods from API
  useEffect(() => {
    async function fetchDeliveryMethods() {
      try {
        const methods = await api.orders.getDeliveryMethods();
        setDeliveryMethods(methods || []);
        if (methods && methods.length > 0) {
          const defaultMethod = methods.find(m => m.shortName === 'Standard') || methods[0];
          setSelectedDeliveryMethod(defaultMethod);
        }
      } catch (err) {
        console.error('Error loading delivery methods:', err);
        toast.error('Could not load delivery methods. Please try refreshing.');
      } finally {
        setLoadingDelivery(false);
      }
    }
    fetchDeliveryMethods();
  }, []);

  const handleAddressChange = (e) => {
    const { name, value } = e.target;
    setAddress(prev => ({ ...prev, [name]: value }));
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: null }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    if (!address.firstName.trim()) newErrors.firstName = 'First name is required (min 2 chars)';
    if (!address.lastName.trim()) newErrors.lastName = 'Last name is required (min 2 chars)';
    if (!address.street.trim() || address.street.length < 3) newErrors.street = 'Street address is required (min 3 chars)';
    if (!address.city.trim()) newErrors.city = 'City is required';
    if (!address.country.trim()) newErrors.country = 'Country is required';
    if (!selectedDeliveryMethod) newErrors.delivery = 'Please select a delivery method';

    // If card payment selected, validate card details
    if (paymentMethod === 'Card') {
      const cleanCard = cardData.cardNumber.replace(/\s/g, '');
      if (cleanCard.length !== 16) {
        newErrors.cardNumber = 'Valid 16-digit card number is required';
      }
      if (!cardData.nameOnCard.trim()) {
        newErrors.nameOnCard = 'Name on card is required';
      }
      if (!cardData.expiry || cardData.expiry.length !== 5) {
        newErrors.expiry = 'Expiration date (MM/YY) is required';
      }
      if (!cardData.cvc || cardData.cvc.length < 3) {
        newErrors.cvc = 'Valid 3 or 4-digit CVC is required';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handlePlaceOrder = async (e) => {
    e.preventDefault();

    if (cartItems.length === 0) {
      toast.error('Your cart is empty');
      navigate('/');
      return;
    }

    if (!validateForm()) {
      toast.error('Please complete all required fields');
      return;
    }

    setSubmitting(true);
    try {
      // 1. Ensure basket has the selected deliveryMethodId and items synced on the server
      await api.basket.update({
        id: basketId,
        deliveryMethodId: selectedDeliveryMethod.id,
        shippingPrice: Number(selectedDeliveryMethod.price) || 0,
        items: cartItems.map(item => ({
          id: Number(item.id),
          quantity: Number(item.quantity) || 1,
          productName: item.title || item.name || '',
          pictureUrl: Array.isArray(item.images) ? (item.images[0] || '') : (item.pictureUrl || ''),
          price: Number(item.price) || 0
        }))
      });

      // 2. If paying with Card, initialize Stripe PaymentIntent on backend
      if (paymentMethod === 'Card') {
        const toastId = toast.loading('Authorizing card payment with Stripe...');
        await api.payment.createOrUpdateIntent(basketId);
        await new Promise(resolve => setTimeout(resolve, 800));
        toast.dismiss(toastId);
      }

      // 3. Create order in database
      const orderPayload = {
        basketId: basketId,
        deliveryMethodId: selectedDeliveryMethod.id,
        addressDTO: {
          firstName: address.firstName.trim(),
          lastName: address.lastName.trim(),
          street: address.street.trim(),
          city: address.city.trim(),
          country: address.country.trim(),
        },
        paymentMethod: paymentMethod === 'Card' ? 'Card' : 'CashOnDelivery',
      };

      const createdOrder = await api.orders.create(orderPayload);
      toast.success('Order placed successfully!');
      await clearCart();
      setOrderSuccess(createdOrder);
    } catch (err) {
      console.error('Order creation error:', err);
      const msg = err?.message || 'Failed to place order. Please check your data and try again.';
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  };

  // Calculations
  const subtotal = cartItems.reduce((acc, item) => {
    const p = Number(item.price) || 0;
    const q = Number(item.quantity) || 1;
    return acc + p * q;
  }, 0);

  const shippingPrice = selectedDeliveryMethod ? Number(selectedDeliveryMethod.price) : 0;
  const grandTotal = subtotal + shippingPrice;

  if (orderSuccess) {
    return (
      <PageTransition>
        <div className="order-success-screen">
          <div className="order-success-icon">
            <FaCheckCircle />
          </div>
          <h1 className="order-success-title">Thank You For Your Order!</h1>
          <p className="order-success-desc">
            Your order has been placed and is currently being processed.
          </p>

          <div className="order-success-details">
            <div className="order-success-row">
              <span>Order ID:</span>
              <span style={{ fontFamily: 'monospace', fontSize: '13px' }}>{orderSuccess.id}</span>
            </div>
            <div className="order-success-row">
              <span>Delivery Method:</span>
              <span>{orderSuccess.deliveryMethod}</span>
            </div>
            <div className="order-success-row">
              <span>Total Amount:</span>
              <span>${orderSuccess.total?.toFixed(2)}</span>
            </div>
            <div className="order-success-row">
              <span>Shipping To:</span>
              <span>
                {address.street}, {address.city}, {address.country}
              </span>
            </div>
          </div>

          <div className="order-success-actions">
            <Link to="/orders" className="btn-secondary">
              View My Orders
            </Link>
            <Link to="/" className="btn-primary">
              Continue Shopping
            </Link>
          </div>
        </div>
      </PageTransition>
    );
  }

  return (
    <PageTransition>
      <div className="checkout-container">
        <div className="checkout-header">
          <h1>Checkout & Payment</h1>
          <p>Complete your delivery and payment details to finish your order</p>
        </div>

        <div className="checkout-grid">
          {/* Left: Forms */}
          <div className="checkout-sections">
            {/* Step 1: Shipping Address */}
            <div className="checkout-card">
              <div className="checkout-card-title">
                <span className="step-badge">1</span>
                Shipping Address
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="firstName">First Name *</label>
                  <input
                    id="firstName"
                    name="firstName"
                    type="text"
                    placeholder="e.g. Fady"
                    value={address.firstName}
                    onChange={handleAddressChange}
                  />
                  {errors.firstName && <span className="field-error">{errors.firstName}</span>}
                </div>
                <div className="form-group">
                  <label htmlFor="lastName">Last Name *</label>
                  <input
                    id="lastName"
                    name="lastName"
                    type="text"
                    placeholder="e.g. Kaiser"
                    value={address.lastName}
                    onChange={handleAddressChange}
                  />
                  {errors.lastName && <span className="field-error">{errors.lastName}</span>}
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="street">Street Address *</label>
                <input
                  id="street"
                  name="street"
                  type="text"
                  placeholder="Street name, building, apartment"
                  value={address.street}
                  onChange={handleAddressChange}
                />
                {errors.street && <span className="field-error">{errors.street}</span>}
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="city">City / Governorate *</label>
                  <input
                    id="city"
                    name="city"
                    type="text"
                    placeholder="e.g. Cairo"
                    value={address.city}
                    onChange={handleAddressChange}
                  />
                  {errors.city && <span className="field-error">{errors.city}</span>}
                </div>
                <div className="form-group">
                  <label htmlFor="country">Country *</label>
                  <input
                    id="country"
                    name="country"
                    type="text"
                    placeholder="e.g. Egypt"
                    value={address.country}
                    onChange={handleAddressChange}
                  />
                  {errors.country && <span className="field-error">{errors.country}</span>}
                </div>
              </div>
            </div>

            {/* Step 2: Delivery Method */}
            <div className="checkout-card">
              <div className="checkout-card-title">
                <span className="step-badge">2</span>
                Delivery Method
              </div>

              {loadingDelivery ? (
                <p style={{ color: '#64748b' }}>Loading delivery options...</p>
              ) : (
                <div className="delivery-options-grid">
                  {deliveryMethods.map((method) => {
                    const isSelected = selectedDeliveryMethod?.id === method.id;
                    return (
                      <div
                        key={method.id}
                        className={`delivery-card ${isSelected ? 'selected' : ''}`}
                        onClick={() => setSelectedDeliveryMethod(method)}
                      >
                        <div className="delivery-card-top">
                          <h4>{method.shortName}</h4>
                          <span className="delivery-price">
                            {method.price === 0 ? 'Free' : `$${method.price.toFixed(2)}`}
                          </span>
                        </div>
                        <p className="delivery-desc">{method.description}</p>
                        <span className="delivery-time">Estimated: {method.deliveryTime}</span>
                      </div>
                    );
                  })}
                </div>
              )}
              {errors.delivery && <span className="field-error">{errors.delivery}</span>}
            </div>

            {/* Step 3: Payment Method */}
            <div className="checkout-card">
              <div className="checkout-card-title">
                <span className="step-badge">3</span>
                Payment Method
              </div>

              <div className="payment-methods-grid">
                <div
                  className={`payment-method-card ${paymentMethod === 'Card' ? 'selected' : ''}`}
                  onClick={() => setPaymentMethod('Card')}
                >
                  <FaCreditCard className="payment-icon" />
                  <div>
                    <div className="payment-title">Credit / Debit Card</div>
                    <div className="payment-subtitle">Online Card Payment (Safe & Fast)</div>
                  </div>
                </div>

                <div
                  className={`payment-method-card ${paymentMethod === 'CashOnDelivery' ? 'selected' : ''}`}
                  onClick={() => setPaymentMethod('CashOnDelivery')}
                >
                  <FaMoneyBillWave className="payment-icon" />
                  <div>
                    <div className="payment-title">Cash on Delivery</div>
                    <div className="payment-subtitle">Pay cash when package arrives</div>
                  </div>
                </div>
              </div>

              {/* Credit Card Inputs & Visual Card */}
              {paymentMethod === 'Card' && (
                <div className="card-details-section">
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '14px', flexWrap: 'wrap', gap: '8px' }}>
                    <span style={{ fontSize: '13.5px', fontWeight: 600, color: 'var(--p-color)' }}>
                      Enter Card Details:
                    </span>
                    <button type="button" className="test-card-pill-btn" onClick={fillTestCard}>
                      ⚡ Fill Test Card (Stripe 4242)
                    </button>
                  </div>

                  {/* Visual Card Mockup */}
                  <div className="card-visual-mockup">
                    <div className="card-mockup-top">
                      <div className="card-mockup-chip" />
                      <span className="card-mockup-brand">VISA</span>
                    </div>
                    <div className="card-mockup-number">
                      {cardData.cardNumber || '•••• •••• •••• ••••'}
                    </div>
                    <div className="card-mockup-bottom">
                      <div className="card-mockup-holder">
                        <span>Cardholder</span>
                        <strong>{cardData.nameOnCard || 'YOUR NAME'}</strong>
                      </div>
                      <div className="card-mockup-expiry">
                        <span>Expires</span>
                        <strong>{cardData.expiry || 'MM/YY'}</strong>
                      </div>
                    </div>
                  </div>

                  {/* Card Fields */}
                  <div className="form-group">
                    <label>Card Number *</label>
                    <input
                      type="text"
                      placeholder="4242 4242 4242 4242"
                      value={cardData.cardNumber}
                      onChange={handleCardNumberChange}
                      maxLength={19}
                      autoComplete="cc-number"
                    />
                    {errors.cardNumber && <span className="field-error">{errors.cardNumber}</span>}
                  </div>

                  <div className="form-group">
                    <label>Cardholder Name *</label>
                    <input
                      type="text"
                      placeholder="e.g. Fady Kaiser"
                      value={cardData.nameOnCard}
                      onChange={(e) => {
                        setCardData(prev => ({ ...prev, nameOnCard: e.target.value }));
                        if (errors.nameOnCard) setErrors(prev => ({ ...prev, nameOnCard: null }));
                      }}
                      autoComplete="cc-name"
                    />
                    {errors.nameOnCard && <span className="field-error">{errors.nameOnCard}</span>}
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label>Expiry Date *</label>
                      <input
                        type="text"
                        placeholder="MM/YY"
                        value={cardData.expiry}
                        onChange={handleExpiryChange}
                        maxLength={5}
                        autoComplete="cc-exp"
                      />
                      {errors.expiry && <span className="field-error">{errors.expiry}</span>}
                    </div>

                    <div className="form-group">
                      <label>Security Code (CVV / CVC) *</label>
                      <input
                        type="password"
                        placeholder="123"
                        value={cardData.cvc}
                        onChange={handleCvcChange}
                        maxLength={4}
                        autoComplete="cc-csc"
                      />
                      {errors.cvc && <span className="field-error">{errors.cvc}</span>}
                    </div>
                  </div>
                </div>
              )}

              {/* Cash On Delivery Banner */}
              {paymentMethod === 'CashOnDelivery' && (
                <div style={{ marginTop: '16px', padding: '14px', borderRadius: '8px', background: 'rgba(16, 185, 129, 0.1)', border: '1px solid rgba(16, 185, 129, 0.3)', color: '#10b981', fontSize: '13px', fontWeight: 500 }}>
                  ✓ You will pay in cash upon receiving the order at your doorstep. No online prepayment required.
                </div>
              )}
            </div>
          </div>

          {/* Right: Order Summary */}
          <div className="order-summary-card">
            <h2>Order Summary</h2>

            <div className="summary-items-list">
              {cartItems.map((item, idx) => (
                <div key={idx} className="summary-item-row">
                  <img
                    src={Array.isArray(item.images) ? item.images[0] : (item.pictureUrl || '')}
                    alt={item.title || item.name}
                    className="summary-item-img"
                  />
                  <div className="summary-item-info">
                    <div className="summary-item-name">{item.title || item.name}</div>
                    <div className="summary-item-meta">Qty: {item.quantity} × ${Number(item.price).toFixed(2)}</div>
                  </div>
                  <div className="summary-item-price">
                    ${(Number(item.price) * Number(item.quantity)).toFixed(2)}
                  </div>
                </div>
              ))}
            </div>

            <div className="summary-totals">
              <div className="summary-totals-row">
                <span>Items Subtotal:</span>
                <span>${subtotal.toFixed(2)}</span>
              </div>
              <div className="summary-totals-row">
                <span>Shipping ({selectedDeliveryMethod?.shortName || 'Standard'}):</span>
                <span>{shippingPrice === 0 ? 'Free' : `$${shippingPrice.toFixed(2)}`}</span>
              </div>
              <div className="summary-totals-row grand-total">
                <span>Total Amount:</span>
                <span>${grandTotal.toFixed(2)}</span>
              </div>
            </div>

            <button
              type="button"
              className="place-order-btn"
              disabled={submitting || cartItems.length === 0}
              onClick={handlePlaceOrder}
            >
              {submitting ? (
                <>
                  <FaSpinner className="spin" /> Processing Order...
                </>
              ) : (
                `Place Order • $${grandTotal.toFixed(2)}`
              )}
            </button>
          </div>
        </div>
      </div>
    </PageTransition>
  );
}
