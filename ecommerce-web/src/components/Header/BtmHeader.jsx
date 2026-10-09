import React, { useEffect, useState, useRef } from 'react';
import { IoMenu } from "react-icons/io5";
import { IoMdArrowDropdown } from "react-icons/io";
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { FaSignInAlt, FaUserPlus, FaSignOutAlt } from "react-icons/fa"; // ضفنا أيقونة الخروج
import './Header.css';

const NavLinks = [
  { title: "Home", link: "/" },
  { title: "About", link: "/about" },
  { title: "Accessories", link: "/accessories" },
  { title: "Blog", link: "/blog" },
  { title: "Contact", link: "/contact" },
];

export default function BtmHeader() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isCategoryOpen, setIsCategoryOpen] = useState(false);
  const [categories, setCategories] = useState([]);
  const [scrollDirection, setScrollDirection] = useState("show");
  const [prevOffset, setPrevOffset] = useState(0);
  
  const location = useLocation();
  const navigate = useNavigate();
  const headerRef = useRef(null); 

  // بنشيك لو فيه مستخدم مسجل دخول
  const isAuthenticated = localStorage.getItem("user");

  // جلب التصنيفات
  useEffect(() => {
    fetch('https://dummyjson.com/products/categories')
      .then((res) => res.json())
      .then((data) => setCategories(data));
  }, []);

  // وظيفة تسجيل الخروج
  const handleLogout = () => {
    localStorage.removeItem("user");
    navigate("/login");
    window.location.reload(); // ريفرش عشان السيستم يحس بالخروج
  };

  // مراقبة الضغط الخارجي وقفل القوائم عند تغيير الصفحة
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (headerRef.current && !headerRef.current.contains(event.target)) {
        setIsMenuOpen(false);
        setIsCategoryOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    
    // قفل القوائم أوتوماتيك عند الانتقال لصفحة جديدة
    setIsCategoryOpen(false);
    setIsMenuOpen(false);

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [location]);

  // منطق الـ Sticky Header (يختفي عند النزول ويظهر عند الطلوع)
  useEffect(() => {
    const handleScroll = () => {
      const currentOffset = window.scrollY;
      if (currentOffset > 150) {
        setScrollDirection(currentOffset > prevOffset ? "hide" : "show");
      } else {
        setScrollDirection("show");
      }
      setPrevOffset(currentOffset);
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, [prevOffset]);

  return (
    <div 
      className={`btn_header ${scrollDirection === "hide" ? "header_hidden" : ""}`} 
      ref={headerRef}
    >
      <div className='container'>
        <nav className='nav'>
          {/* قسم التصنيفات */}
          <div className='category_nav'>
            <div className='category_btn' onClick={() => setIsCategoryOpen(!isCategoryOpen)}>
              <IoMenu />
              <p> Browse Category </p>
              <IoMdArrowDropdown />
            </div>

            <div className={`category_nav_list ${isCategoryOpen ? "active" : ""}`}>
              {categories.map((category) => (
                <Link key={category.slug} to={`category/${category.slug}`} onClick={() => setIsCategoryOpen(false)}>
                  {category.name}
                </Link>
              ))}
            </div>
          </div>

          {/* لينكات الناف بار */}
          <div className='nav_links'>
            {NavLinks.map((item) => (
              <li key={item.link} className={location.pathname === item.link ? "active" : ""}>
                <Link to={item.link}>{item.title}</Link>
              </li>
            ))}
          </div>
        </nav>

        {/* أيقونات اليمين (Login/Register أو Logout) */}
        <div className='sign_reges_icons'>
          {isAuthenticated ? (
            /* لو مسجل دخول يظهر زرار الخروج */
            <button 
              onClick={handleLogout} 
              className="logout_btn"
              title="Logout"
            >
              <FaSignOutAlt />
            </button>
          ) : (
            /* لو مش مسجل يظهر أيقونات الدخول والتسجيل */
            <>
              <Link to="/login" title="Sign In"><FaSignInAlt /></Link>
              <Link to="/register" title="Register"><FaUserPlus /></Link>
            </>
          )}
          
          {/* زرار المنيو موبايل */}
          <div className="burger_menu_mobile" onClick={() => setIsMenuOpen(!isMenuOpen)}>
            <IoMenu />
          </div>

          {/* القائمة المنسدلة للموبايل */}
          <div className={`mobile_dropdown_menu ${isMenuOpen ? "active" : ""}`}>
            {NavLinks.map((item) => (
              <Link key={item.link} to={item.link} onClick={() => setIsMenuOpen(false)}>
                {item.title}
              </Link>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}