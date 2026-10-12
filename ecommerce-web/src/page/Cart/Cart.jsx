import React, { useContext } from 'react';
import { CartContext } from '../../components/Context/CartContext';
import { FaTrash, FaShoppingBag } from 'react-icons/fa';
import { useNavigate, Link } from 'react-router-dom';
import './Cart.css';
import PageTransition from '../../components/PageTransition';

function Cart() {
    const navigate = useNavigate();
    const { cartItems, increaseQuantity, decreaseQuantity, removeFromCart } = useContext(CartContext);

    const total = cartItems.reduce((acc, item) => {
        const price = Number(item.price) || 0;
        const quantity = Number(item.quantity) || 1;
        return acc + price * quantity;
    }, 0);

    return (
        <PageTransition>
            <div className='checkout'>
                <div className='ordersummeary'>
                    <h1>Your Shopping Cart</h1>

                    <div className='items'>
                        {cartItems.length === 0 ? (
                            <div style={{ textAlign: 'center', padding: '50px 20px' }}>
                                <FaShoppingBag style={{ fontSize: '48px', color: '#94a3b8', marginBottom: '16px' }} />
                                <h3 style={{ fontSize: '20px', color: '#334155', marginBottom: '8px' }}>Your cart is empty</h3>
                                <p style={{ color: '#64748b', marginBottom: '24px' }}>Looks like you haven't added anything to your cart yet.</p>
                                <Link
                                    to="/"
                                    style={{
                                        display: 'inline-block',
                                        background: '#0090f0',
                                        color: '#ffffff',
                                        padding: '10px 24px',
                                        borderRadius: '6px',
                                        fontWeight: '600',
                                        textDecoration: 'none'
                                    }}
                                >
                                    Start Shopping
                                </Link>
                            </div>
                        ) : (
                            cartItems.map((item, index) => {
                                const imgSrc = Array.isArray(item.images)
                                    ? item.images[0]
                                    : (item.pictureUrl || '');
                                return (
                                    <div className='item_cart' key={index}>
                                        <div className='image_name'>
                                            <div className='img_item'>
                                                <img src={imgSrc} alt={item.title || item.name} />
                                            </div>

                                            <div className='content'>
                                                <h4>{item.title || item.name}</h4>
                                                <p className='price_item'>${Number(item.price).toFixed(2)}</p>

                                                <div className='quantity_control'>
                                                    <button onClick={() => decreaseQuantity(item.id)}>-</button>
                                                    <span className='quantity'>{item.quantity}</span>
                                                    <button onClick={() => increaseQuantity(item.id)}>+</button>
                                                </div>
                                            </div>
                                        </div>
                                        <button onClick={() => removeFromCart(item.id)} className='deleted_item'>
                                            <FaTrash />
                                        </button>
                                    </div>
                                );
                            })
                        )}
                    </div>

                    {cartItems.length > 0 && (
                        <div className='bottom_summary'>
                            <div className='shop_table'>
                                <p>Subtotal:</p>
                                <span className='total_checkout'>${total.toFixed(2)}</span>
                            </div>

                            <div className='button_div'>
                                <button type='button' onClick={() => navigate('/checkout')}>
                                    Proceed to Checkout
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </PageTransition>
    );
}

export default Cart;


