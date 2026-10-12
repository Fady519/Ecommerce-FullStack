import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { 
  HiOutlineHome, 
  HiOutlineShoppingBag, 
  HiOutlineExclamationTriangle,
  HiOutlineChatBubbleBottomCenterText
} from 'react-icons/hi2';
import './NotFound.css';

export default function NotFound() {
  useEffect(() => {
    document.title = 'LuxeStore | 404 - Page Not Found';
  }, []);

  return (
    <div className="notfound_container">
      <div className="notfound_content">
        <div className="notfound_badge">
          <HiOutlineExclamationTriangle />
          <span>Error 404</span>
        </div>

        <div className="notfound_glitch_wrapper">
          <div className="notfound_code">404</div>
        </div>

        <h1 className="notfound_title">Lost in Space? Page Not Found</h1>
        
        <p className="notfound_desc">
          The page you are looking for might have been removed, had its name changed,
          or is temporarily unavailable. Let's get you back on track!
        </p>

        <div className="notfound_actions">
          <Link to="/" className="notfound_btn_primary">
            <HiOutlineHome size={20} />
            <span>Back to Home</span>
          </Link>
          <Link to="/category" className="notfound_btn_secondary">
            <HiOutlineShoppingBag size={20} />
            <span>Browse Products</span>
          </Link>
          <Link to="/contact" className="notfound_btn_secondary">
            <HiOutlineChatBubbleBottomCenterText size={20} />
            <span>Contact Support</span>
          </Link>
        </div>

        <div className="notfound_quicklinks">
          <div className="notfound_quicklinks_title">Popular Destinations</div>
          <div className="notfound_links_pills">
            <Link to="/category">All Products</Link>
            <Link to="/blog">Our Journal</Link>
            <Link to="/about">About Us</Link>
            <Link to="/orders">My Orders</Link>
            <Link to="/cart">Cart</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
