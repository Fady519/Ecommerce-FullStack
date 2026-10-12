import React, { useEffect, useState, useRef, useContext } from 'react';
import { IoMenu } from "react-icons/io5";
import { IoMdArrowDropdown } from "react-icons/io";
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { FaSignInAlt, FaUserPlus, FaSignOutAlt, FaUserCircle, FaTag, FaLayerGroup, FaBoxOpen } from "react-icons/fa";
import { HiOutlineShieldCheck } from "react-icons/hi2";
import { AuthContext } from '../Context/AuthContext';
import { api } from '../../services/api';
import './Header.css';

const NavLinks = [
  { title: "Home", link: "/" },
  { title: "Products", link: "/category/all" },
  { title: "About", link: "/about" },
  { title: "Contact", link: "/contact" },
];

export default function BtmHeader() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isCategoryOpen, setIsCategoryOpen] = useState(false);
  const [types, setTypes] = useState([]);
  const [brands, setBrands] = useState([]);

  const { user, isAuthenticated, logout } = useContext(AuthContext);
  
  const isAdmin = Boolean(
    user?.roles?.some(r => r.toLowerCase() === 'admin') ||
    user?.Roles?.some(r => r.toLowerCase() === 'admin') ||
    user?.email?.toLowerCase() === 'fadykaiser2004@gmail.com' ||
    user?.email?.toLowerCase() === 'admin@demo.store'
  );

  const location = useLocation();
  const navigate = useNavigate();
  const headerRef = useRef(null); 

  // جلب الفئات والماركات من الباك إند الحقيقي
  const loadCategoriesAndBrands = () => {
    Promise.all([
      api.products.getTypes().catch(() => []),
      api.products.getBrands().catch(() => [])
    ]).then(([typesRes, brandsRes]) => {
      const typesList = Array.isArray(typesRes) ? typesRes : (typesRes?.value || typesRes?.data || []);
      const brandsList = Array.isArray(brandsRes) ? brandsRes : (brandsRes?.value || brandsRes?.data || []);
      setTypes(typesList);
      setBrands(brandsList);
    });
  };

  useEffect(() => {
    loadCategoriesAndBrands();
    window.addEventListener('catalog-updated', loadCategoriesAndBrands);
    return () => window.removeEventListener('catalog-updated', loadCategoriesAndBrands);
  }, []);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (headerRef.current && !headerRef.current.contains(event.target)) {
        setIsMenuOpen(false);
        setIsCategoryOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    setIsCategoryOpen(false);
    setIsMenuOpen(false);

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [location]);

  return (
    <div className="btn_header" ref={headerRef}>
      <div className='container'>
        <nav className='nav'>
          {/* قائمة التصنيفات والماركات الاحترافية */}
          <div className='category_nav'>
            <div 
              className={`category_btn ${isCategoryOpen ? "btn_active" : ""}`} 
              onClick={() => setIsCategoryOpen(!isCategoryOpen)}
            >
              <div className="btn_label">
                <IoMenu className="menu_icon" />
                <span>Browse Catalog</span>
              </div>
              <IoMdArrowDropdown className={`arrow_icon ${isCategoryOpen ? "rotate" : ""}`} />
            </div>

            <div className={`category_nav_list ${isCategoryOpen ? "active" : ""}`}>
              <div className="dropdown_section">
                <div className="dropdown_heading">
                  <FaLayerGroup /> Categories / Types
                </div>
                {types.map((type) => (
                  <Link 
                    key={`type-${type.id}`} 
                    to={`/category/${encodeURIComponent(type.name)}`} 
                    onClick={() => setIsCategoryOpen(false)}
                  >
                    <span>{type.name}</span>
                  </Link>
                ))}
              </div>

              {brands.length > 0 && (
                <div className="dropdown_section brands_section">
                  <div className="dropdown_heading">
                    <FaTag /> Top Brands
                  </div>
                  {brands.map((brand) => (
                    <Link 
                      key={`brand-${brand.id}`} 
                      to={`/category/${encodeURIComponent(brand.name)}`} 
                      onClick={() => setIsCategoryOpen(false)}
                    >
                      <span>{brand.name}</span>
                    </Link>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* روابط التصفح */}
          <div className='nav_links'>
            {NavLinks.map((item) => (
              <li key={item.link} className={location.pathname === item.link ? "active" : ""}>
                <Link to={item.link}>{item.title}</Link>
              </li>
            ))}
          </div>
        </nav>

        {/* أزرار الحساب */}
        <div className='sign_reges_icons'>
          {isAuthenticated ? (
            <div className="user_logged_box">
              {isAdmin && (
                <Link to="/admin" className="orders_btn_pill admin_btn_pill" title="Admin Dashboard">
                  <HiOutlineShieldCheck />
                  <span>Admin</span>
                </Link>
              )}
              <Link to="/orders" className="orders_btn_pill" title="My Orders">
                <FaBoxOpen />
                <span>Orders</span>
              </Link>
              <span className="user_badge">
                <FaUserCircle className="user_icon" />
                <span className="user_name">{user?.displayName || "My Account"}</span>
              </span>
              <button 
                onClick={handleLogout} 
                className="logout_btn"
                title="Logout"
                aria-label="Logout"
              >
                <FaSignOutAlt />
              </button>
            </div>
          ) : (
            <div className="auth_guest_links">
              <Link to="/login" className="guest_auth_link" title="Sign In">
                <FaSignInAlt />
                <span className="auth_text">Sign In</span>
              </Link>
              <Link to="/register" className="guest_auth_link register_pill" title="Register">
                <FaUserPlus />
                <span className="auth_text">Register</span>
              </Link>
            </div>
          )}
          
          {/* زر الموبايل */}
          <div className="burger_menu_mobile" onClick={() => setIsMenuOpen(!isMenuOpen)}>
            <IoMenu />
          </div>

          <div className={`mobile_dropdown_menu ${isMenuOpen ? "active" : ""}`}>
            {NavLinks.map((item) => (
              <Link key={item.link} to={item.link} onClick={() => setIsMenuOpen(false)}>
                {item.title}
              </Link>
            ))}
            {isAuthenticated ? (
              <>
                {isAdmin && (
                  <Link to="/admin" onClick={() => setIsMenuOpen(false)} style={{ color: '#f59e0b' }}>
                    Admin Dashboard
                  </Link>
                )}
                <Link to="/orders" onClick={() => setIsMenuOpen(false)}>
                  My Orders
                </Link>
                <button onClick={handleLogout} className="mobile_logout_btn">
                  Logout ({user?.displayName || "Account"})
                </button>
              </>
            ) : (
              <div className="mobile_auth_links">
                <Link to="/login" onClick={() => setIsMenuOpen(false)}>Sign In</Link>
                <Link to="/register" onClick={() => setIsMenuOpen(false)}>Create Account</Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}